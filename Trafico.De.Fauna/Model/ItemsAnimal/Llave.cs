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
    ///     Llave para abrir una celda de item animal
    /// </summary>
    public class Llave
    {
        /// <summary>
        ///     Color de la llave y celda
        /// </summary>
        public enum ColorType
        {
            VERDE,
            ROJA,
            AMARILLA
        }

        /// <summary>
        ///     Altura a la que aparecen las llaves
        /// </summary>
        private const float ALTURA_LLAVE = 10f;

        /// <summary>
        ///     Radio colision
        /// </summary>
        private const float BSPHERE_RADIUS = 28f;


        private TgcMeshShader mesh;
        private readonly RotadorDeItems rotador;


        public Llave(TgcMeshData meshDataNode)
        {
            Agarrada = false;

            //Nombre: VERDE o ROJA o AMARILLA
            var name = meshDataNode.name;
            name.ToUpper();


            //Determinar color
            Color = getColorType(name);

            //Cargar Mesh
            var nodeCenter = GameUtils.getBBLowerCenterFromMeshData(meshDataNode);
            var loader = new TgcSceneLoader();
            loader.MeshFactory = new TgcMeshShaderFactory();
            var meshPath = GameModel.instance.gameConfig.basePath + "Llaves\\" + Color + "\\" + Color + "-TgcScene.xml";
            var scene = loader.loadSceneFromFile(meshPath);
            mesh = (TgcMeshShader)scene.Meshes[0];
            mesh.AutoTransformEnable = true;
            mesh.Position = new TGCVector3(nodeCenter.X, ALTURA_LLAVE, nodeCenter.Z);

            var sphereCenter = new TGCVector3(nodeCenter.X, ALTURA_LLAVE + BSPHERE_RADIUS / 2, nodeCenter.Z);
            BoundingSphere = new TgcBoundingSphere(sphereCenter, BSPHERE_RADIUS);

            rotador = new RotadorDeItems();
            rotador.YSpeed = 13f;
            rotador.LimitY = 0;
            rotador.YawSpeed = 3f;
        }

        /// <summary>
        ///     Color de la llave
        /// </summary>
        public ColorType Color { get; }

        public bool Agarrada { get; private set; }

        /// <summary>
        ///     BoundingSphere para agarrar la llave
        /// </summary>
        public TgcBoundingSphere BoundingSphere { get; private set; }

        public void render()
        {
            if (!Agarrada)
            {
                //Dibujar si esta en rango visible
                if (GameModel.instance.mainCharacter.isInProximityRange(mesh.Position))
                {
                    rotador.rotate(GameModel.instance.ElapsedTime, mesh);
                    GameModel.instance.shaders.setCorrectTechnique(mesh);
                    mesh.render();
                }

                //Debug render
                if (GameModel.instance.debugMode.Enabled && GameModel.instance.debugMode.ShowBoundingVolumes)
                {
                    BoundingSphere.Render();
                }
            }
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(ColorType), Color);
        }


        public static ColorType getColorType(string colorName)
        {
            switch (colorName)
            {
                case "VERDE":
                    return ColorType.VERDE;
                case "ROJA":
                    return ColorType.ROJA;
                case "AMARILLA":
                    return ColorType.AMARILLA;
                default:
                    throw new Exception("Color de llave incorrecto: " + colorName);
            }
        }

        /// <summary>
        ///     Marca la llave como agarrada
        /// </summary>
        public void agarrar()
        {
            Agarrada = true;
            GameModel.instance.sonidos.llave_agarrar.Play();
        }

        public void dispose()
        {
            mesh.Dispose();
            mesh = null;
            BoundingSphere.Dispose();
            BoundingSphere = null;
        }
    }
}
