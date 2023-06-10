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

using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.MeshFactory;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using Trafico.De.Fauna.Utils.Render;

namespace Trafico.De.Fauna.Model.Meshes
{
    /// <summary>
    /// Extendemos de TgcMesh para poder redefinir el método executeRender() y agregar renderizado de Shaders. 
    /// 
    /// </summary>
    public class TgcMeshShader : TgcMesh, IRenderQueueElement, IAlphaObject
    {

        string technique;
        /// <summary>
        /// Technique a utilizar en el renderizado
        /// </summary>
        public string Technique
        {
            get { return technique; }
            set { technique = value; }
        }

        /// <summary>
        /// Se llama antes de iniciar el shader.
        /// </summary>
        /// <param name="mesh">Mesh</param>
        public delegate void ShaderBeginHandler(TgcMeshShader mesh);

        /// <summary>
        /// Evento que se llama antes de iniciar el shader.
        /// </summary>
        public event ShaderBeginHandler ShaderBegin;

        /// <summary>
        /// Se llama antes de iniciar una pasada del shader.
        /// </summary>
        /// <param name="mesh">Mesh</param>
        /// <param name="pass">Número de pasada del shader</param>
        public delegate void ShaderPassBeginHandler(TgcMeshShader mesh, int pass);

        /// <summary>
        /// Evento que se llama antes de iniciar una pasada del shader.
        /// </summary>
        public event ShaderPassBeginHandler ShaderPassBegin;


        public TgcMeshShader(Mesh mesh, string name, MeshRenderType renderType)
            : base(mesh, name, renderType)
        {
        }

        public TgcMeshShader(string name, TgcMesh parentInstance, TGCVector3 translation, TGCVector3 rotation, TGCVector3 scale)
            : base(name, parentInstance, translation, rotation, scale)
        {
        }

        /// <summary>
        /// Se redefine este método.
        /// Si tiene Alpha habilitado no renderiza directo, sino que lo manda a una lista para renderizar
        /// luego de todos los objetos opacos.
        /// </summary>
        public new void render()
        {
            if (!enabled)
                return;

            if (AlphaBlendEnable)
            {
                GameModel.instance.currentLevel.AlphaObjects.Add(this);
            }
            else
            {
                executeRender();
            }
        }

        /// <summary>
        /// Render directo
        /// </summary>
        public void renderWithAlpha()
        {
            executeRender();
        }

        /// <summary>
        /// Se redefine este método para agregar shaders.
        /// Es el mismo código del executeRender() pero con la sección de "MeshRenderType.DIFFUSE_MAP" ampliada
        /// para Shaders.
        /// </summary>
        public new void executeRender()
        {
            var device = D3DDevice.Instance.Device;
            Effect effect = GameModel.instance.shaders.effect;

            //Cargar technique
            effect.Technique = this.technique;

            //Aplicar transformacion de malla
            if (AutoTransformEnable)
            {
                this.transform = TGCMatrix.Identity
                    * TGCMatrix.Scaling(scale)
                    * TGCMatrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z)
                    * TGCMatrix.Translation(translation);
            }
            device.Transform.World = this.transform;

            //Cargar VertexDeclaration
            device.VertexDeclaration = vertexDeclaration;


            //Cargar valores de shader de matrices que dependen de la posición del mesh
            int numPasses;
            TGCMatrix matWorldView = this.transform * TGCMatrix.FromMatrix(device.Transform.View);
            TGCMatrix matWorldViewProj = matWorldView * TGCMatrix.FromMatrix(device.Transform.Projection);
            effect.SetValue("matWorld", this.transform);
            effect.SetValue("matWorldView", matWorldView);
            effect.SetValue("matWorldViewProj", matWorldViewProj);


            //Renderizar segun el tipo de render de la malla
            switch (renderType)
            {
                case MeshRenderType.VERTEX_COLOR:

                    //Reset de texturas
                    TexturesManager.Instance.clear(0);
                    TexturesManager.Instance.clear(1);
                    device.Material = D3DDevice.DEFAULT_MATERIAL;

                    //Llamar evento para configurar inicio del shader
                    if (ShaderBegin != null)
                    {
                        ShaderBegin.Invoke(this);
                    }

                    //Iniciar Shader e iterar sobre sus Render Passes
                    numPasses = effect.Begin(0);
                    for (int n = 0; n < numPasses; n++)
                    {
                        //Llamar evento para configurar inicio de la pasada del shader
                        if (ShaderPassBegin != null)
                        {
                            ShaderPassBegin.Invoke(this, n);
                        }

                        //Dibujar cada el unico subset que hay
                        effect.BeginPass(n);
                        d3dMesh.DrawSubset(0);
                        effect.EndPass();
                    }

                    //Finalizar shader
                    effect.End();

                    /*
                    //Hacer reset de texturas y materiales
                    texturesManager.clear(0);
                    texturesManager.clear(1);
                    device.Material = TgcD3dDevice.DEFAULT_MATERIAL;

                    //Dibujar mesh
                    d3dMesh.DrawSubset(0);
                    */
                    break;

                case MeshRenderType.DIFFUSE_MAP:

                    //Hacer reset de Lightmap
                    TexturesManager.Instance.clear(1);
                    
                    //Activar AlphaBlending
                    if (this.AlphaBlendEnable)
                    {
                        device.RenderState.AlphaBlendEnable = true;
                        device.RenderState.AlphaTestEnable = true;
                    }
                    
                    
                    //Llamar evento para configurar inicio del shader
                    if (ShaderBegin != null)
                    {
                        ShaderBegin.Invoke(this);
                    }

                    //Iniciar Shader e iterar sobre sus Render Passes
                    numPasses = effect.Begin(0);
                    for (int n = 0; n < numPasses; n++)
                    {
                        //Llamar evento para configurar inicio de la pasada del shader
                        if (ShaderPassBegin != null)
                        {
                            ShaderPassBegin.Invoke(this, n);
                        }

                        

                        //Dibujar cada subset con su Material y DiffuseMap correspondiente
                        for (int i = 0; i < materials.Length; i++)
                        {
                            device.Material = materials[i];

                            //Setear textura en shader
                            TexturesManager.Instance.shaderSet(effect, "diffuseMap_Tex", diffuseMaps[i]);

                            //Iniciar pasada de shader
                            // guarda: Todos los SetValue tienen que ir ANTES del beginPass.
                            // si no hay que llamar effect.CommitChanges para que tome el dato!
                            effect.BeginPass(n);
                            d3dMesh.DrawSubset(i);
                            effect.EndPass();
                        }
                    }

                    //Finalizar shader
                    effect.End();

                    //Desactivar AlphaBlending
                    device.RenderState.AlphaBlendEnable = false;
                    device.RenderState.AlphaTestEnable = false;

                    break;

                case MeshRenderType.DIFFUSE_MAP_AND_LIGHTMAP:


                    //Activar AlphaBlending
                    if (this.AlphaBlendEnable)
                    {
                        device.RenderState.AlphaBlendEnable = true;
                        device.RenderState.AlphaTestEnable = true;
                    }


                    //Llamar evento para configurar inicio del shader
                    if (ShaderBegin != null)
                    {
                        ShaderBegin.Invoke(this);
                    }

                    //Iniciar Shader e iterar sobre sus Render Passes
                    numPasses = effect.Begin(0);
                    for (int n = 0; n < numPasses; n++)
                    {
                        //Llamar evento para configurar inicio de la pasada del shader
                        if (ShaderPassBegin != null)
                        {
                            ShaderPassBegin.Invoke(this, n);
                        }

                        //Cargar lightmap
                        TexturesManager.Instance.shaderSet(effect, "lightMap_Tex", lightMap);

                        //Dibujar cada subset con su Material y su DiffuseMap correspondiente
                        for (int i = 0; i < materials.Length; i++)
                        {
                            device.Material = materials[i];

                            //Setear textura en shader
                            TexturesManager.Instance.shaderSet(effect, "diffuseMap_Tex", diffuseMaps[i]);

                            //Iniciar pasada de shader
                            // guarda: Todos los SetValue tienen que ir ANTES del beginPass.
                            // si no hay que llamar effect.CommitChanges para que tome el dato!
                            effect.BeginPass(n);
                            d3dMesh.DrawSubset(i);
                            effect.EndPass();
                        }
                    }

                    //Finalizar shader
                    effect.End();

                    //Desactivar AlphaBlending
                    device.RenderState.AlphaBlendEnable = false;
                    device.RenderState.AlphaTestEnable = false;

                    break;
            }
        }

    }

    /// <summary>
    /// Factory customizado para poder crear clase TgcMeshShader
    /// </summary>
    public class TgcMeshShaderFactory : IMeshFactory
    {
        public TgcMesh createNewMesh(Mesh d3dMesh, string meshName, TgcMesh.MeshRenderType renderType)
        {
            return new TgcMeshShader(d3dMesh, meshName, renderType);
        }

        public TgcMesh createNewMeshInstance(string meshName, TgcMesh originalMesh, TGCVector3 translation, TGCVector3 rotation, TGCVector3 scale)
        {
            return new TgcMeshShader(meshName, originalMesh, translation, rotation, scale);
        }
    }
}
