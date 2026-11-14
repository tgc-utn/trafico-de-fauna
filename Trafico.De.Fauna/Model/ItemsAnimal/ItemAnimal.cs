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
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model.ItemsAnimal
{
    /// <summary>
    ///     Item con animal a rescatar
    /// </summary>
    public class ItemAnimal
    {
        private TgcMeshShader mesh;


        public ItemAnimal(TgcMeshData meshDataNode)
        {
            Liberado = false;

            //Nombre => VERDE_Animal1 o ROJA_Animal1 o AMARILLA_Animal1
            var name = meshDataNode.name;
            name.ToUpper();
            var split = name.Split('_');

            //Determinar color
            var colorName = split[0];
            Color = Llave.getColorType(colorName);

            //Cargar mesh
            var meshName = split[1];
            var nodeCenter = GameUtils.getBBLowerCenterFromMeshData(meshDataNode);
            var loader = new TgcSceneLoader();
            loader.MeshFactory = new TgcMeshShaderFactory();
            var meshPath = GameModel.instance.gameConfig.basePath + "ItemsAnimal\\" + colorName + "\\" + colorName +
                           "-TgcScene.xml";
            var scene = loader.loadSceneFromFile(meshPath);
            mesh = (TgcMeshShader)scene.Meshes[0];
            mesh.AutoTransformEnable = true;
            mesh.Position = new TGCVector3(nodeCenter.X, 0, nodeCenter.Z);

            //bSphereProximidad de proximidad
            BSphereProximidad = new TgcBoundingSphere(mesh.BoundingBox.calculateBoxCenter(),
                mesh.BoundingBox.calculateBoxRadius());

            //Collider
            ColliderObject = new ColliderObject(mesh);

            //Propiedad: orientacion=norte (default norte) => norte = +Y, sur = -Y, este = X, oeste = -X
            GameUtils.Orientacion orientacion;
            if (GameUtils.containsUserProp(meshDataNode.userProperties, "orientacion"))
            {
                var orientacionStr = meshDataNode.userProperties["orientacion"].ToUpper();
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

        /// <summary>
        ///     Color de la llave
        /// </summary>
        public Llave.ColorType Color { get; }

        public bool Liberado { get; private set; }

        public TgcBoundingSphere BSphereProximidad { get; private set; }

        /// <summary>
        ///     Collider
        /// </summary>
        public ColliderObject ColliderObject { get; private set; }

        public void render()
        {
            if (!Liberado)
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
                    BSphereProximidad.Render();
                }
            }
        }

        /// <summary>
        ///     Intentar liberar el item si ya se encontro la llave indicada.
        ///     Devuelve true si se pudo liberar
        /// </summary>
        public bool intentarLiberar()
        {
            var llave = GameModel.instance.currentLevel.getLlave(Color);
            if (llave.Agarrada)
            {
                //Liberar
                Liberado = true;
                GameModel.instance.sonidos.itemAnimal_liberar.Play();
                GameModel.instance.hud.efectoLiberarItem(this);
                GameModel.instance.mainCharacter.agregarItemAnimalLiberado(this);

                //Quitar como colision
                ColliderObject.Enabled = false;
            }
            else
            {
                //Aun no fue agarrada la llave
                GameModel.instance.sonidos.itemAnimal_fallo_liberar.Play();
                GameModel.instance.hud.efectoFaltaLlaveParaLiberarItem(this);
            }

            return Liberado;
        }

        public void dispose()
        {
            mesh.Dispose();
            mesh = null;
            ColliderObject = null;
            BSphereProximidad.Dispose();
            BSphereProximidad = null;
        }
    }
}
