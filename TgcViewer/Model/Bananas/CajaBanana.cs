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
using Microsoft.DirectX;
using TraficoFauna.Examples;
using Examples.TraficoFauna.Meshes;
using TGC.Core.Geometry;
using TGC.Core.KeyFrameLoader;
using TGC.Core.SceneLoader;

namespace Examples.TraficoFauna.Bananas
{
    /// <summary>
    /// Item de caja con bananas de un sector
    /// </summary>
    public class CajaBanana
    {
        const float RADIO_SPHERE = 28f;

        TgcBoundingBox boundingBox;
        TgcBoundingSphere boundingSphere;
        Vector3 meshPos;
        TgcKeyFrameMesh mesh;
        int cantidadBananas;
        int golpes;

        string name;
        public string Name
        {
            get { return name; }
        }

        bool enable;
        /// <summary>
        /// Indica si la banana ya fue tomada
        /// </summary>
        public bool Enabled
        {
            get { return enable; }
            set { enable = value; }
        }

        ColliderObject colliderObject;
        /// <summary>
        /// Collider
        /// </summary>
        public ColliderObject ColliderObject
        {
            get { return colliderObject; }
        }

        public CajaBanana(TgcMeshData meshData)
        {
            name = meshData.name;
            enable = true;
            boundingBox = GameUtils.createBBFromMeshData(meshData);
            Vector3 center = boundingBox.calculateBoxCenter();
            meshPos = new Vector3(center.X, 0, center.Z);
            boundingSphere = new TgcBoundingSphere(center, RADIO_SPHERE);

            //Collider
            colliderObject = new ColliderObject(boundingBox);
            

            //Propiedad: bananas=5 (default 0)
            if (GameUtils.containsUserProp(meshData.userProperties, "bananas"))
            {
                if (!int.TryParse(meshData.userProperties["bananas"], out cantidadBananas))
                {
                    throw new Exception("La cantidad de bananas de la Caja de Bananas no es un numero:" + meshData.name);
                }
            }
            else
            {
                cantidadBananas = 0;
            }
            //Propiedad: golpes=3 (default 1)
            if (GameUtils.containsUserProp(meshData.userProperties, "golpes"))
            {
                if (!int.TryParse(meshData.userProperties["golpes"], out golpes))
                {
                    throw new Exception("La cantidad de golpes de la Caja de Bananas no es un numero:" + meshData.name);
                }
            }
            else
            {
                golpes = 1;
            }
        }

        public void init()
        {
            if (!enable)
            {
                return;
            }

            if (cantidadBananas == 0)
            {
                mesh = GameModel.instance.cajasBananasPool.getAvaliableMeshSinBanana();
            }
            else
            {
                mesh = GameModel.instance.cajasBananasPool.getAvaliableMeshConBanana();
            }

            mesh.playAnimation("StandBy", true);
            mesh.Position = meshPos;
            mesh.AnimationEnds += new TgcKeyFrameMesh.AnimationEndsHandler(mesh_AnimationEnds);
        }

        public void render(float elapsedTime)
        {
            if (enable)
            {
                //Dibujar si esta en rango visible
                if (GameModel.instance.mainCharacter.isInProximityRange(mesh.Position))
                {
                    /*
                    //Solo animar si no esta en la animacion de quieta
                    if (mesh.CurrentAnimation.Data.name == "StandBy")
                    {
                        mesh.render();
                    }
                    else
                    {
                        this.mesh.animateAndRender();
                    }
                    */

                    this.mesh.animateAndRender(elapsedTime);

                    if (GameModel.instance.debugMode.Enabled && GameModel.instance.debugMode.ShowBoundingVolumes)
                    {
                        boundingSphere.render();
                    }
                }
            }
        }

        /// <summary>
        /// Cuando finaliza una animacion
        /// </summary>
        void mesh_AnimationEnds(TgcKeyFrameMesh mesh)
        {
            //Termino la explosion
            if (mesh.CurrentAnimation.Data.name == "Explosion")
            {
                //Desactivar caja
                this.enable = false;
                //Quitar como colision
                colliderObject.Enabled = false;
            }
            //Termino un golpe
            else if (mesh.CurrentAnimation.Data.name == "Explosion")
            {
                mesh.playAnimation("StandBy", true);
            }
        }


        /// <summary>
        /// Liberar mesh
        /// </summary>
        public void hide()
        {
            if (this.mesh != null)
            {
                this.mesh.AnimationEnds -= mesh_AnimationEnds;
                this.mesh = null;
            }
        }

        public void dispose()
        {
            mesh = null;
            boundingBox.dispose();
            boundingBox = null;
            boundingSphere.dispose();
            boundingSphere = null;
        }

        /// <summary>
        /// Le da un golpe a la caja, si esta activada y si el personaje colisiona.
        /// Si la rompe le otorga las bananas (si es que tiene para dar)
        /// </summary>
        public void golpear(MainCharacter mainCharacter)
        {
            if (!enable)
            {
                return;
            }
            
            //Si la animacion actual es explosion, significa que ya fue rota (pero aun esta explotando)
            if (mesh.CurrentAnimation.Data.name == "Explosion")
            {
                return;
            }

            //Colision
            if (mainCharacter.collideWithBSphere(boundingSphere))
            {
                //Solo rompe si el personaje esta mirando en un angulo correcto
                Vector3 currentVecRot = mainCharacter.getCurrentVecRot();
                Vector3 dir = Vector3.Subtract(meshPos, mainCharacter.CurrentPos);
                float dotAngle = Vector3.Dot(currentVecRot, dir);
                if (dotAngle > GameModel.instance.gameConfig.fisica.anguloGolpeDotValue)
                {
                    golpes--;
                    //Se rompe la caja, otorgar bananas
                    if (golpes < 1)
                    {
                        //Si tiene bananas, otorgar
                        if (cantidadBananas > 0)
                        {
                            mainCharacter.agregarBananas(cantidadBananas);
                            GameModel.instance.hud.efectoAgarrarVariasBananas(cantidadBananas);
                            GameModel.instance.sonidos.banana_agarrar_varias.play();
                        }

                        //Romper
                        mesh.playAnimation("Explosion", false);
                        GameModel.instance.sonidos.caja_bananas_romper.play();
                    }
                    //Golpear caja
                    else
                    {
                        mesh.playAnimation("Golpe", false);
                        GameModel.instance.sonidos.caja_bananas_golpear.play();
                    }
                }
            }
        }
    }
}
