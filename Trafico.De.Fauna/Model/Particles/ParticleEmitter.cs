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
    /// Emisor de particulas para generar efectos de particulas
    /// </summary>
    public class ParticleEmitter : IAlphaObject
    {
        private int particlesCount;
        private Particle[] particles;
        private ColaDeParticulas particlesAlive;
        private Stack<Particle> particlesDead;
        private TgcTexture texture;
        private float tiempoAcumulado = 0;
        private Random random;
        private VertexDeclaration vertexDeclaration;
        private Particle.ParticleVertex[] particleVertexArray;

        private bool enabled;
        /// <summary>
        /// Habilita o deshabilita el emisor
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private bool playing;
        /// <summary>
        /// Iniciar o parar la generacion de nuevas particulas
        /// </summary>
        public bool Playing
        {
            get { return playing; }
            set { playing = value; }
        }

        private TGCVector3 position;
        /// <summary>
        /// Posicion del emisor de particulas en la escena.
        /// Todas las particulas se generan en esta posicion.
        /// </summary>
        public TGCVector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        private float creationFrecuency;
        /// <summary>
        /// Tiempo de frecuencia de creacion de particulas.
        /// Cuanto mas chico mas rapido.
        /// </summary>
        public float CreationFrecuency
        {
            get { return this.creationFrecuency; }
            set { this.creationFrecuency = value; }
        }

        private float particleTimeToLive;
        /// <summary>
        /// Tiempo de vida de las particulas.
        /// </summary>
        public float ParticleTimeToLive
        {
            get { return this.particleTimeToLive; }
            set { this.particleTimeToLive = value; }
        }

        private float minSizeParticle;
        /// <summary>
        /// Minimo tamaño que puede tener una particula.
        /// </summary>
        public float MinSizeParticle
        {
            get { return this.minSizeParticle; }
            set { this.minSizeParticle = value; }
        }

        private float maxSizeParticle;
        /// <summary>
        /// Maximo tamaño que puede tener una particula.
        /// </summary>
        public float MaxSizeParticle
        {
            get { return this.maxSizeParticle; }
            set { this.maxSizeParticle = value; }
        }

        private int dispersion;
        /// <summary>
        /// Valor que representa el grado de dispersion de las particulas.
        /// </summary>
        public int Dispersion
        {
            get { return this.dispersion; }
            set { this.dispersion = value;}
        }

        private TGCVector3 speed;
        /// <summary>
        /// Vector que se le multiplica a la velocidad base (Por defecto es (1,1,1)).
        /// Si Y es negativo las particulas descienden.
        /// </summary>
        public TGCVector3 Speed
        {
            get { return this.speed; }
            set { this.speed = value; }
        }

        /// <summary>
        /// Crear un emisor de particulas
        /// </summary>
        /// <param name="texturePath">textura a utilizar</param>
        /// <param name="particlesCount">cantidad maxima de particlas a generar</param>
        public ParticleEmitter(string texturePath, int particlesCount)
        {
            Device device = D3DDevice.Instance.Device;

            //valores default
            enabled = true;
            playing = true;
            random = new Random(0);
            creationFrecuency = 1.0f;
            particleTimeToLive = 5.0f;
            minSizeParticle = 0.25f;
            maxSizeParticle = 0.5f;
            dispersion = 100;
            speed = new TGCVector3(1, 1, 1);
            particleVertexArray = new Particle.ParticleVertex[1];
            vertexDeclaration = new VertexDeclaration(device, Particle.ParticleVertexElements);

            this.particlesCount = particlesCount;
            this.particles = new Particle[particlesCount];

            this.particlesAlive = new ColaDeParticulas(particlesCount);
            this.particlesDead = new Stack<Particle>(particlesCount);

            //Creo todas las particulas. Inicialmente estan todas muertas.
            for (int i = 0; i < particlesCount; i++)
            {
                this.particles[i] = new Particle();
                this.particlesDead.Push(this.particles[i]);
            }

            //Cargo la textura que tendra cada particula
            this.texture = TgcTexture.createTexture(device, texturePath);
        }

        /// <summary>
        /// Cambiar la textura de las particulas
        /// </summary>
        public void changeTexture(string texturePath)
        {
            this.texture.dispose();
            this.texture = TgcTexture.createTexture(D3DDevice.Instance.Device, texturePath);
        }

        /// <summary>
        /// Liberar recursos
        /// </summary>
        public void dispose()
        {
            texture.dispose();
            particlesAlive = null;
            particlesDead = null;
            particles = null;
            vertexDeclaration.Dispose();
        }

        /// <summary>
        /// Con Alpha
        /// </summary>
        public void renderWithAlpha()
        {
            if (!enabled)
            {
                return;
            }

            render(GameModel.instance.ElapsedTime);
        }

        /// <summary>
        /// Dibujar particulas.
        /// Emite particulas a medida que avanza el tiempo.
        /// </summary>
        public void render(float elapsedTime)
        {
            if (!enabled)
            {
                return;
            }

            //Ver si hay que generar nuevas particulas
            tiempoAcumulado += elapsedTime;
            if (tiempoAcumulado >= this.creationFrecuency && playing)
            {
                tiempoAcumulado = 0.0f;

                //Inicializa y agrega una particula a la lista de particulas vivas.
                this.createParticle();
            }

            //Dibujar particulas existentes
            if (this.particlesAlive.Count > 0)
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
                Particle p = this.particlesAlive.peek();
                while (p != null)
                {
                    p.TimeToLive -= elapsedTime;

                    if (p.TimeToLive <= 0)
                    {
                        //Saco la particula de la lista de particulas vivas.
                        this.particlesAlive.dequeue(out p);

                        //Inserto la particula en la lista de particulas muertas.
                        this.particlesDead.Push(p);
                    }
                    else
                    {
                        //Actualizo y Dibujo la partícula
                        this.updateExistingParticle(elapsedTime, p);
                        this.renderParticle(p);
                    }

                    p = this.particlesAlive.peekNext();
                }

                //Restaurar valores de RenderState
                device.RenderState.AlphaBlendEnable = false;
                device.RenderState.ZBufferWriteEnable = true;
            }
        }

        /// <summary>
        /// Crear una nueva particula
        /// </summary>
        private void createParticle()
        {
            //Saco una particula de la lista de particulas muertas.
            if (particlesDead.Count > 0)
            {
                Particle p = particlesDead.Pop();
                //Agrego la partícula a la lista de partículas vivas.
                this.particlesAlive.enqueue(p);

                //Seteo valores iniciales de la partícula.
                p.TotalTimeToLive = this.particleTimeToLive;
                p.TimeToLive = this.particleTimeToLive;
                p.Position = position;
                p.Color = Particle.DEFAULT_COLOR.ToArgb();

                float faux;
                TGCVector3 pSpeed = TGCVector3.Empty;

                // Según la dispersion asigno una velocidad inicial. 
                //(Si la dispersion es 0 la velocidad inicial sera (0,1,0)).
                faux = random.Next(this.dispersion) / 1000.0f;
                faux *= (faux * 1000 % 2 == 0 ? 1.0f : -1.0f);
                pSpeed.X = faux * 2.0f;

                faux = 1.0f - (2.0f * random.Next(this.dispersion) / 1000.0f);
                pSpeed.Y = faux * 2.0f;

                faux = random.Next(this.dispersion) / 1000.0f;
                faux *= (faux * 1000 % 2 == 0 ? 1.0f : -1.0f);
                pSpeed.Z = faux * 2.0f;

                p.Speed = pSpeed;

                //Modifico el tamaño de manera aleatoria.
                float size = (float)random.NextDouble() * this.maxSizeParticle;
                if (size < this.minSizeParticle) size = this.minSizeParticle;
                p.PointSize = size;
            }
        }

        /// <summary>
        /// Actualizar el estado de una particula existente
        /// </summary>
        private void updateExistingParticle(float elapsedTime, Particle p)
        {
            //Actulizo posicion de la particula.
            TGCVector3 scaleVec = TGCVector3.Scale(this.speed, elapsedTime);
            scaleVec.X *= p.Speed.X;
            scaleVec.Y *= p.Speed.Y;
            scaleVec.Z *= p.Speed.Z;
            p.Position += scaleVec;
        }

        /// <summary>
        /// Dibujar particula
        /// </summary>
        private void renderParticle(Particle p)
        {
            var device = D3DDevice.Instance.Device;

            //Variamos el canal Alpha de la particula con efecto Fade-in y Fade-out (hasta 20% Alpha - Normal - desde 60% alpha)
            float currentProgress = 1 - (p.TimeToLive / p.TotalTimeToLive);
            int alphaComp = 255;
            if (currentProgress < 0.2)
            {
                alphaComp = (int)((currentProgress / 0.2f) * 255f);
            }
            else if (currentProgress > 0.6)
            {
                alphaComp = (int)((1 - ((currentProgress - 0.6f) / 0.4f)) * 255);
            }
            
            //Crear nuevo color con Alpha interpolado
            Color origColor = Particle.DEFAULT_COLOR;
            p.Color = Color.FromArgb(alphaComp, origColor.R, origColor.G, origColor.B).ToArgb();

            //Render con Moduliacion de color Alpha
            int color = device.RenderState.TextureFactor;

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
