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
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.Interpolators;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model.CustomEvents.Piedras
{
    /// <summary>
    ///     Lanza piedras en forma intermitente desde un punto del BoundingBox hacia el otro.
    ///     Chocan contra el personaje principal.
    /// </summary>
    public class LanzadorPiedras : CustomEvent
    {
        private readonly float ROTACION_VELOCIDAD = FastMath.PI;
        private float acumTime;
        private bool axisX;
        private TgcBoundingAxisAlignBox boundingBox;
        private float distanceToTravel;
        private float frecuencia;
        private bool lanzando;

        private TgcMeshShader mesh;
        private float meshOriginalPosY;
        private TGCVector3 movementDir;
        private float origDistanceToTravel;
        private TGCVector3 posFinal;
        private TGCVector3 posOrigen;
        private float rotAngle;
        private InterpoladorVaiven vaiven;
        private float velocidad;

        public override void load(TgcMeshData meshData, string texturePath, TgcSceneData originalSceneData)
        {
            //Mesh
            var loader = new TgcSceneLoader();
            loader.MeshFactory = new TgcMeshShaderFactory();
            var meshPath = GameModel.instance.gameConfig.basePath + "Piedras\\PiedraNormal\\PiedraNormal-TgcScene.xml";
            var scene = loader.loadSceneFromFile(meshPath);
            mesh = (TgcMeshShader)scene.Meshes[0];
            mesh.AutoTransformEnable = true;

            //Extremos y direccion de movimiento
            axisX = GameUtils.getBBExtremeCentersXZ(meshData, out posOrigen, out posFinal);
            boundingBox = GameUtils.createBBFromMeshData(meshData);
            var axisRadius = mesh.BoundingBox.calculateAxisRadius();
            meshOriginalPosY = axisRadius.Y;
            posOrigen.Y = meshOriginalPosY;
            posFinal.Y = meshOriginalPosY;
            movementDir = posFinal - posOrigen;
            movementDir.Y = meshOriginalPosY;
            origDistanceToTravel = movementDir.Length();
            movementDir.Normalize();

            //Rotacion segun orientacion
            if (axisX)
            {
                rotAngle = posOrigen.X > posFinal.X ? ROTACION_VELOCIDAD : -ROTACION_VELOCIDAD;
            }
            else
            {
                rotAngle = posOrigen.Z > posFinal.Z ? -ROTACION_VELOCIDAD : ROTACION_VELOCIDAD;
            }

            //Efecto rebote
            vaiven = new InterpoladorVaiven(0, 20, 100);

            //Propiedad: frecuencia=4 (default=3) es en segundos
            if (GameUtils.containsUserProp(meshData.userProperties, "frecuencia"))
            {
                if (!float.TryParse(meshData.userProperties["frecuencia"], out frecuencia))
                {
                    throw new Exception("Frecuencia de LanzadorPiedras no es numerica: " + meshData.name);
                }
            }
            else
            {
                frecuencia = 3;
            }

            //Propiedad: velocidad=100 (default=300) es en segundos
            if (GameUtils.containsUserProp(meshData.userProperties, "velocidad"))
            {
                if (!float.TryParse(meshData.userProperties["velocidad"], out velocidad))
                {
                    throw new Exception("Velocidad de LanzadorPiedras no es numerica: " + meshData.name);
                }
            }
            else
            {
                velocidad = 300;
            }
        }

        public override void init()
        {
            lanzando = false;
            acumTime = 0;
            mesh.Enabled = false;
        }

        public override void end()
        {
            GameModel.instance.sonidos.piedra_rodando.Stop();
        }

        public override void update(float elapsedTime)
        {
            //Mover piedra actual
            if (lanzando)
            {
                var movement = velocidad * elapsedTime;
                distanceToTravel -= movement;
                if (distanceToTravel < 0)
                {
                    //Llego a su destino
                    lanzando = false;
                    mesh.Enabled = false;

                    //Apagar sonido de girando
                    GameModel.instance.sonidos.piedra_rodando.Stop();
                }
                else
                {
                    //Rotar segun orientacion
                    if (axisX)
                    {
                        mesh.RotateZ(rotAngle * elapsedTime);
                    }
                    else
                    {
                        mesh.RotateX(rotAngle * elapsedTime);
                    }

                    //Desplazar en XZ y mover en Y con vaiven 
                    var move = TGCVector3.Scale(movementDir, movement);
                    var newPos = mesh.Position + move;
                    newPos.Y = meshOriginalPosY + vaiven.update(elapsedTime);
                    mesh.Position = newPos;
                }

                //Ver si la piedra colisiona contra el personaje
                if (GameModel.instance.mainCharacter.collideWithAABB(mesh.BoundingBox))
                {
                    //Sonido de golpe de piedra
                    GameModel.instance.sonidos.piedra_golpear_personaje.Play(false);

                    //Quitar vida
                    GameModel.instance.currentLevel.removeLifeToPlayer();
                }
            }
            //Ver si paso el tiempo para lanzar una nueva piedra
            else
            {
                acumTime += elapsedTime;

                //Lanzar nueva piedra
                if (acumTime > frecuencia)
                {
                    lanzando = true;
                    acumTime = 0;
                    mesh.Enabled = true;
                    mesh.Position = posOrigen;
                    distanceToTravel = origDistanceToTravel;
                    mesh.Rotation = new TGCVector3(0, 0, 0);

                    //Activar sonido de rodando
                    GameModel.instance.sonidos.piedra_rodando.Play(true);
                }
            }
        }

        public override void render()
        {
            //Dibujar si esta en rango visible
            if (GameModel.instance.mainCharacter.isInProximityRange(posOrigen))
            {
                GameModel.instance.shaders.setCorrectTechnique(mesh);
                mesh.render();
            }
        }

        public override void renderDebug()
        {
            boundingBox.Render();
        }

        public override ColliderObject getColliderObject()
        {
            return null;
        }

        public override void dispose()
        {
            mesh.Dispose();
            mesh = null;
            boundingBox.Dispose();
            boundingBox = null;
        }
    }
}
