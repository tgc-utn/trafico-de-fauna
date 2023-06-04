/*
Copyright 2012 Educ.ar S.E.

This file is part of "Trafico de Fauna".

"Trafico de Fauna" is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

"Trafico de Fauna" is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with "Trafico de Fauna".  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using System.Drawing;
using TraficoFauna.Examples;
using Examples.TraficoFauna.Interpolators;
using TGC.Core.Textures;
using TGC.Core.Direct3D;

namespace Examples.TraficoFauna.PostProcess
{
    /// <summary>
    /// Efectos de post procesado
    /// </summary>
    public class PostProcessEffects
    {
        VertexBuffer screenQuadVB;
        Texture render_target2D;
        Surface pOldRT;
        Effect effect;
        InterpoladorLineal intLinealPlayerDead;
        Temporizador temporizadorPlayerDead;
        TgcTexture alarmTexture;
        InterpoladorVaiven intVaivenAlarm;
        bool alarmMode;


        public PostProcessEffects(Device d3dDevice)
        {
            //Se crean 2 triangulos con las dimensiones de la pantalla con sus posiciones ya transformadas
            // x = -1 es el extremo izquiedo de la pantalla, x=1 es el extremo derecho
            // Lo mismo para la Y con arriba y abajo
            // la Z en 1 simpre
            CustomVertex.PositionTextured[] vertices = new CustomVertex.PositionTextured[]
            {
                new CustomVertex.PositionTextured( -1, 1, 1, 0,0),
                new CustomVertex.PositionTextured(1,  1, 1, 1,0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0,1),
                new CustomVertex.PositionTextured(1,-1, 1, 1,1)
            };
            //vertex buffer de los triangulos
            screenQuadVB = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                    4, d3dDevice, Usage.Dynamic | Usage.WriteOnly,
                        CustomVertex.PositionTextured.Format, Pool.Default);
            screenQuadVB.SetData(vertices, 0, LockFlags.None);

            // inicializo el render target
            render_target2D = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);


            //Cargar Shader de StaticMesh
            string compilationErrors;
            effect = Effect.FromFile(d3dDevice, GameModel.instance.gameConfig.shadersPath + "PostProcess.fx",
                null, null, ShaderFlags.None, null, out compilationErrors);
            if (effect == null)
            {
                throw new Exception("Error al cargar shader. Errores: " + compilationErrors);
            }
            //Configurar Technique
            //effect.Technique = "DefaultTechnique";
            effect.Technique = "AlarmaTechnique";

            //Player dead
            intLinealPlayerDead = new InterpoladorLineal();
            temporizadorPlayerDead = new Temporizador();

            //Modo alarma
            alarmTexture = TgcTexture.createTexture(d3dDevice, GameModel.instance.gameConfig.basePath + "Shaders\\efecto_alarma.png");
            intVaivenAlarm = new InterpoladorVaiven(0, 1, 5f);
            alarmMode = false;
        }

        /// <summary>
        /// Activar modo alarma
        /// </summary>
        public void setAlarmModeEnable(bool enable)
        {
            if (enable)
            {
                effect.Technique = "AlarmaTechnique";
            }
            else
            {
                effect.Technique = "DefaultTechnique";
            }
            alarmMode = enable;
        }

        public void set2DRenderTarget()
        {
            Device device = D3DDevice.Instance.Device;

            pOldRT = device.GetRenderTarget(0);
            Surface pSurf = render_target2D.GetSurfaceLevel(0);
            device.SetRenderTarget(0, pSurf);
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            device.BeginScene();
        }

        public void renderAndRestoreTarget()
        {
            Device device = D3DDevice.Instance.Device;

            device.EndScene();

            //TextureLoader.Save("render_target.bmp", ImageFileFormat.Bmp, render_target2D);

            device.SetRenderTarget(0, pOldRT);
            device.BeginScene();

            device.VertexFormat = CustomVertex.PositionTextured.Format;
            device.SetStreamSource(0, screenQuadVB, 0);
            effect.SetValue("render_target2D", render_target2D);

            //Parametros de modo alarma
            if (alarmMode)
            {
                effect.SetValue("textura_alarma", alarmTexture.D3dTexture);
                effect.SetValue("alarmaScaleFactor", intVaivenAlarm.update(GameModel.instance.ElapsedTime));
            }


            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            effect.Begin(FX.None);
            effect.BeginPass(0);
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
            effect.End();

            device.EndScene();
        }

        public void setPlayerDeadEffect()
        {
            temporizadorPlayerDead.setValues(3);
            intLinealPlayerDead.setValues(0, 1, 1);
            effect.Technique = "OscurecerTechnique";
        }

        /// <summary>
        /// Dibujar siempre el ultimo cuadro de animacion que hubo con efecto de oscurecimiento
        /// </summary>
        public void renderOldFrame()
        {
            var device = D3DDevice.Instance.Device;

            float scaleFactor = intLinealPlayerDead.update(GameModel.instance.ElapsedTime);
            if (temporizadorPlayerDead.update(GameModel.instance.ElapsedTime))
            {
                //Arrancar nuevamente en el ultimo sector
                effect.Technique = "DefaultTechnique";
                GameModel.instance.currentLevel.restoreFromLoseLife();
            }
            else
            {
                device.BeginScene();

                device.VertexFormat = CustomVertex.PositionTextured.Format;
                device.SetStreamSource(0, screenQuadVB, 0);
                effect.SetValue("render_target2D", render_target2D);
                effect.SetValue("scaleFactor", scaleFactor);

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                effect.Begin(FX.None);
                effect.BeginPass(0);
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                effect.EndPass();
                effect.End();

                device.EndScene();
            }
        }

    }
}
