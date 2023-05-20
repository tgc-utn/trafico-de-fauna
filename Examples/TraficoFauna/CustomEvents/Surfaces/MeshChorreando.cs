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
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TraficoFauna.Examples;
using Examples.TraficoFauna.Dialogs;
using TgcViewer;
using Microsoft.DirectX.DirectInput;
using Examples.TraficoFauna.Billboards;
using System.Drawing;
using Examples.TraficoFauna.Interpolators;
using Examples.TraficoFauna.Meshes;

namespace Examples.TraficoFauna.CustomEvents.Surfaces
{

    /// <summary>
    /// Efecto de mesh que le chorrea la textura para abajo
    /// </summary>
    public class MeshChorreando: CustomEvent
    {
        MeshAnimatedTexture mesh;
        InterpoladorVaiven vaiven;
        ColliderObject collider;
        float speed;

        public override void load(TgcMeshData meshData, string texturePath, TgcSceneData originalSceneData)
        {
            //Cargar MeshAnimatedTexture
            TgcSceneLoader loader = new TgcSceneLoader();
            loader.MeshFactory = new MeshAnimatedTexture.MeshAnimatedTextureFactory();
            TgcSceneData sceneData = GameUtils.createSceneDataForMeshData(originalSceneData, meshData);
            TgcScene scene = loader.loadScene(sceneData, texturePath);
            mesh = (MeshAnimatedTexture)scene.Meshes[0];

            vaiven = new InterpoladorVaiven(-0.2f, 0.2f, 0.1f);

            //Propiedad: colision=true (default=false, indica si es decorativo o participa en la deteccion de colisiones) 
            if (GameUtils.containsUserProp(meshData.userProperties, "collision"))
            {
                bool collision;
                if (!bool.TryParse(meshData.userProperties["collision"], out collision))
                {
                    throw new Exception("El campo collision no es boolean: " + meshData.name);
                }
                else
                {
                    collider = new ColliderObject(mesh);
                }
            }
            //Propiedad: speed=3 (default=1, velocidad vertical) 
            if (GameUtils.containsUserProp(meshData.userProperties, "speed"))
            {
                if (!float.TryParse(meshData.userProperties["speed"], out speed))
                {
                    throw new Exception("El campo speed no es un float: " + meshData.name);
                }
            }
            else
            {
                speed = 1;
            }
        }

        public override void init()
        {
            mesh.Playing = true;
        }

        public override void end()
        {
            mesh.Playing = false;
        }

        public override void update()
        {
            mesh.Speed = new Vector2(vaiven.update(), speed);
        }

        public override void render()
        {
            //Dibujar si esta en rango visible
            if (Game.instance.mainCharacter.isInProximityRange(mesh.BoundingBox.PMin))
            {
                Game.instance.currentLevel.AlphaObjects.Add(mesh);
            }
        }

        public override void renderDebug()
        {
            mesh.BoundingBox.render();
        }

        public override ColliderObject getColliderObject()
        {
            return collider;
        }

        public override void dispose()
        {
            mesh.dispose();
        }


        

    }
}
