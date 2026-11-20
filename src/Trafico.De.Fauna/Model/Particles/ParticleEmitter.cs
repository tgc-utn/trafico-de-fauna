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
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Textures;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model.Particles
{
    /// <summary>
    ///     Emisor de particulas para generar efectos de particulas
    /// </summary>
    public class ParticleEmitter : IAlphaObject
    {
        private Particle[] particles;
        private ColaDeParticulas particlesAlive;
        private int particlesCount;
        private Stack<Particle> particlesDead;

        private readonly Particle.ParticleVertex[] particleVertexArray;

        private readonly Random random;

        private TGCTexture texture;
        private float tiempoAcumulado;
        private readonly VertexDeclaration vertexDeclaration;

        /// <summary>
        ///     Crear un emisor de particulas
        /// </summary>
        /// <param name="texturePath">textura a utilizar</param>
        /// <param name="particlesCount">cantidad maxima de particlas a generar</param>
        public ParticleEmitter(string texturePath, int particlesCount)
        {
            var device = D3DDevice.Instance.Device;

            //valores default
            Enabled = true;
            Playing = true;
            random = new Random(0);
            CreationFrecuency = 1.0f;
            ParticleTimeToLive = 5.0f;
            MinSizeParticle = 0.25f;
            MaxSizeParticle = 0.5f;
            Dispersion = 100;
            Speed = new TGCVector3(1, 1, 1);
            particleVertexArray = new Particle.ParticleVertex[1];
            vertexDeclaration = new VertexDeclaration(device, Particle.ParticleVertexElements);

            this.particlesCount = particlesCount;
            particles = new Particle[particlesCount];

            particlesAlive = new ColaDeParticulas(particlesCount);
            particlesDead = new Stack<Particle>(particlesCount);

            //Creo todas las particulas. Inicialmente estan todas muertas.
            for (var i = 0; i < particlesCount; i++)
            {
                particles[i] = new Particle();
                particlesDead.Push(particles[i]);
            }

            //Cargo la textura que tendra cada particula
            texture = TGCTexture.CreateTexture(device, texturePath);
        }

        /// <summary>
        ///     Habilita o deshabilita el emisor
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        ///     Iniciar o parar la generacion de nuevas particulas
        /// </summary>
        public bool Playing { get; set; }

        /// <summary>
        ///     Posicion del emisor de particulas en la escena.
        ///     Todas las particulas se generan en esta posicion.
        /// </summary>
        public TGCVector3 Position { get; set; }

        /// <summary>
        ///     Tiempo de frecuencia de creacion de particulas.
        ///     Cuanto mas chico mas rapido.
        /// </summary>
        public float CreationFrecuency { get; set; }

        /// <summary>
        ///     Tiempo de vida de las particulas.
        /// </summary>
        public float ParticleTimeToLive { get; set; }

        /// <summary>
        ///     Minimo tamaño que puede tener una particula.
        /// </summary>
        public float MinSizeParticle { get; set; }

        /// <summary>
        ///     Maximo tamaño que puede tener una particula.
        /// </summary>
        public float MaxSizeParticle { get; set; }

        /// <summary>
        ///     Valor que representa el grado de dispersion de las particulas.
        /// </summary>
        public int Dispersion { get; set; }

        /// <summary>
        ///     Vector que se le multiplica a la velocidad base (Por defecto es (1,1,1)).
        ///     Si Y es negativo las particulas descienden.
        /// </summary>
        public TGCVector3 Speed { get; set; }

        /// <summary>
        ///     Con Alpha
        /// </summary>
        public void renderWithAlpha()
        {
            if (!Enabled)
            {
                return;
            }

            render(GameModel.instance.ElapsedTime);
        }

        /// <summary>
        ///     Cambiar la textura de las particulas
        /// </summary>
        public void changeTexture(string texturePath)
        {
            texture.Dispose();
            texture = TGCTexture.CreateTexture(D3DDevice.Instance.Device, texturePath);
        }

        /// <summary>
        ///     Liberar recursos
        /// </summary>
        public void dispose()
        {
            texture.Dispose();
            particlesAlive = null;
            particlesDead = null;
            particles = null;
            vertexDeclaration.Dispose();
        }

        /// <summary>
        ///     Dibujar particulas.
        ///     Emite particulas a medida que avanza el tiempo.
        /// </summary>
        public void render(float elapsedTime)
        {
            if (!Enabled)
            {
                return;
            }

            //Ver si hay que generar nuevas particulas
            tiempoAcumulado += elapsedTime;
            if (tiempoAcumulado >= CreationFrecuency && Playing)
            {
                tiempoAcumulado = 0.0f;

                //Inicializa y agrega una particula a la lista de particulas vivas.
                createParticle();
            }

            //Dibujar particulas existentes
            if (particlesAlive.Count > 0)
            {
                var device = D3DDevice.Instance.Device;

                //Cargar VertexDeclaration
                device.VertexDeclaration = vertexDeclaration;

                //Fijo la textura actual de la particula.
                TexturesManager.Instance.clear(0);
                TexturesManager.Instance.clear(1);
                TexturesManager.Instance.set(0, texture);
                device.Material = D3DDevice.DEFAULT_MATERIAL;
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.ZBufferWriteEnable = false;
                device.Transform.World = TGCMatrix.Identity;

                // Va recorriendo la lista de particulas vivas,
                // actualizando el tiempo de vida restante, y dibujando.
                var p = particlesAlive.peek();
                while (p != null)
                {
                    p.TimeToLive -= elapsedTime;

                    if (p.TimeToLive <= 0)
                    {
                        //Saco la particula de la lista de particulas vivas.
                        particlesAlive.dequeue(out p);

                        //Inserto la particula en la lista de particulas muertas.
                        particlesDead.Push(p);
                    }
                    else
                    {
                        //Actualizo y Dibujo la partícula
                        updateExistingParticle(elapsedTime, p);
                        renderParticle(p);
                    }

                    p = particlesAlive.peekNext();
                }

                //Restaurar valores de RenderState
                device.RenderState.AlphaBlendEnable = false;
                device.RenderState.ZBufferWriteEnable = true;
            }
        }

        /// <summary>
        ///     Crear una nueva particula
        /// </summary>
        private void createParticle()
        {
            //Saco una particula de la lista de particulas muertas.
            if (particlesDead.Count > 0)
            {
                var p = particlesDead.Pop();
                //Agrego la partícula a la lista de partículas vivas.
                particlesAlive.enqueue(p);

                //Seteo valores iniciales de la partícula.
                p.TotalTimeToLive = ParticleTimeToLive;
                p.TimeToLive = ParticleTimeToLive;
                p.Position = Position;
                p.Color = Particle.DEFAULT_COLOR.ToArgb();

                float faux;
                var pSpeed = TGCVector3.Empty;

                // Según la dispersion asigno una velocidad inicial. 
                //(Si la dispersion es 0 la velocidad inicial sera (0,1,0)).
                faux = random.Next(Dispersion) / 1000.0f;
                faux *= faux * 1000 % 2 == 0 ? 1.0f : -1.0f;
                pSpeed.X = faux * 2.0f;

                faux = 1.0f - 2.0f * random.Next(Dispersion) / 1000.0f;
                pSpeed.Y = faux * 2.0f;

                faux = random.Next(Dispersion) / 1000.0f;
                faux *= faux * 1000 % 2 == 0 ? 1.0f : -1.0f;
                pSpeed.Z = faux * 2.0f;

                p.Speed = pSpeed;

                //Modifico el tamaño de manera aleatoria.
                var size = (float)random.NextDouble() * MaxSizeParticle;
                if (size < MinSizeParticle)
                {
                    size = MinSizeParticle;
                }

                p.PointSize = size;
            }
        }

        /// <summary>
        ///     Actualizar el estado de una particula existente
        /// </summary>
        private void updateExistingParticle(float elapsedTime, Particle p)
        {
            //Actulizo posicion de la particula.
            var scaleVec = TGCVector3.Scale(Speed, elapsedTime);
            scaleVec.X *= p.Speed.X;
            scaleVec.Y *= p.Speed.Y;
            scaleVec.Z *= p.Speed.Z;
            p.Position += scaleVec;
        }

        /// <summary>
        ///     Dibujar particula
        /// </summary>
        private void renderParticle(Particle p)
        {
            var device = D3DDevice.Instance.Device;

            //Variamos el canal Alpha de la particula con efecto Fade-in y Fade-out (hasta 20% Alpha - Normal - desde 60% alpha)
            var currentProgress = 1 - p.TimeToLive / p.TotalTimeToLive;
            var alphaComp = 255;
            if (currentProgress < 0.2)
            {
                alphaComp = (int)(currentProgress / 0.2f * 255f);
            }
            else if (currentProgress > 0.6)
            {
                alphaComp = (int)((1 - (currentProgress - 0.6f) / 0.4f) * 255);
            }

            //Crear nuevo color con Alpha interpolado
            var origColor = Particle.DEFAULT_COLOR;
            p.Color = Color.FromArgb(alphaComp, origColor.R, origColor.G, origColor.B).ToArgb();

            //Render con Moduliacion de color Alpha
            var color = device.RenderState.TextureFactor;

            device.RenderState.TextureFactor = p.Color;
            device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
            device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
            device.TextureState[0].AlphaArgument2 = TextureArgument.TFactor;

            particleVertexArray[0] = p.PointSprite;
            device.DrawUserPrimitives(PrimitiveType.PointList, 1, particleVertexArray);

            //Restaurar valor original
            device.RenderState.TextureFactor = color;
        }
    }
}
