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
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;
using TgcViewer.Model.Meshes;

namespace TgcViewer.Model.ItemsAnimal
{
    /// <summary>
    /// Item con animal a rescatar
    /// </summary>
    public class ItemAnimal
    {

        Llave.ColorType color;
        /// <summary>
        /// Color de la llave
        /// </summary>
        public Llave.ColorType Color
        {
            get { return color; }
        }

        bool liberado;
        public bool Liberado
        {
            get { return liberado; }
        }

        TgcBoundingSphere bSphereProximidad;
        public TgcBoundingSphere BSphereProximidad
        {
            get { return bSphereProximidad; }
        }

        ColliderObject colliderObject;
        /// <summary>
        /// Collider
        /// </summary>
        public ColliderObject ColliderObject
        {
            get { return colliderObject; }
        }

        TgcMeshShader mesh;

        

        public ItemAnimal(TgcMeshData meshDataNode)
        {
            liberado = false;

            //Nombre => VERDE_Animal1 o ROJA_Animal1 o AMARILLA_Animal1
            string name = meshDataNode.name;
            name.ToUpper();
            string[] split = name.Split('_');

            //Determinar color
            string colorName = split[0];
            this.color = Llave.getColorType(colorName);

            //Cargar mesh
            string meshName = split[1];
            TGCVector3 nodeCenter = GameUtils.getBBLowerCenterFromMeshData(meshDataNode);
            TgcSceneLoader loader = new TgcSceneLoader();
            loader.MeshFactory = new TgcMeshShaderFactory();
            string meshPath = GameModel.instance.gameConfig.basePath + "ItemsAnimal\\" + colorName + "\\" + colorName + "-TgcScene.xml";
            TgcScene scene = loader.loadSceneFromFile(meshPath);
            mesh = (TgcMeshShader)scene.Meshes[0];
            mesh.AutoTransform = true;
            mesh.Position = new TGCVector3(nodeCenter.X, 0, nodeCenter.Z);

            //bSphereProximidad de proximidad
            bSphereProximidad = new TgcBoundingSphere(mesh.BoundingBox.calculateBoxCenter(), mesh.BoundingBox.calculateBoxRadius());

            //Collider
            colliderObject = new ColliderObject(this.mesh);

            //Propiedad: orientacion=norte (default norte) => norte = +Y, sur = -Y, este = X, oeste = -X
            GameUtils.Orientacion orientacion;
            if (GameUtils.containsUserProp(meshDataNode.userProperties, "orientacion"))
            {
                string orientacionStr = meshDataNode.userProperties["orientacion"].ToUpper();
                switch (orientacionStr)
                {
                    case "NORTE":
                        orientacion = GameUtils.Orientacion.Norte;
                        break;
                    case "SUR":
                        orientacion = GameUtils.Orientacion.Sur;
                        break;
                    case "ESTE":
                        orientacion = GameUtils.Orientacion.Este;
                        break;
                    case "OESTE":
                        orientacion = GameUtils.Orientacion.Oeste;
                        break;
                    default:
                        throw new Exception("Orientacion incorrecta: " + orientacionStr + ", en ItemAnimal: " + name);
                }
            }
            else
            {
                orientacion = GameUtils.Orientacion.Norte;
            }

            //Rotar segun orientacion
            float angleRot = 0;
            switch (orientacion)
            {
                case GameUtils.Orientacion.Norte:
                    angleRot = FastMath.PI;
                    break;
                case GameUtils.Orientacion.Sur:
                    angleRot = 0;
                    break;
                case GameUtils.Orientacion.Este:
                    angleRot = -FastMath.PI_HALF;
                    break;
                case GameUtils.Orientacion.Oeste:
                    angleRot = FastMath.PI_HALF;
                    break;
            }
            mesh.RotateY(angleRot);

        }

        public void render()
        {
            if (!liberado)
            {
                //Dibujar si esta en rango visible
                if (GameModel.instance.mainCharacter.isInProximityRange(mesh.Position))
                {
                    GameModel.instance.shaders.setCorrectTechnique(mesh);
                    mesh.render();
                }

                //Debug render
                if (GameModel.instance.debugMode.Enabled && GameModel.instance.debugMode.ShowBoundingVolumes)
                {
                    bSphereProximidad.Render();
                }
            }
        }

        /// <summary>
        /// Intentar liberar el item si ya se encontro la llave indicada.
        /// Devuelve true si se pudo liberar
        /// </summary>
        public bool intentarLiberar()
        {
            Llave llave = GameModel.instance.currentLevel.getLlave(this.color);
            if (llave.Agarrada)
            {
                //Liberar
                this.liberado = true;
                GameModel.instance.sonidos.itemAnimal_liberar.play();
                GameModel.instance.hud.efectoLiberarItem(this);
                GameModel.instance.mainCharacter.agregarItemAnimalLiberado(this);

                //Quitar como colision
                colliderObject.Enabled = false;
            }
            else
            {
                //Aun no fue agarrada la llave
                GameModel.instance.sonidos.itemAnimal_fallo_liberar.play();
                GameModel.instance.hud.efectoFaltaLlaveParaLiberarItem(this);
            }

            return this.liberado;
        }

        public void dispose()
        {
            mesh.Dispose();
            mesh = null;
            colliderObject = null;
            bSphereProximidad.Dispose();
            bSphereProximidad = null;
        }
    }
}
