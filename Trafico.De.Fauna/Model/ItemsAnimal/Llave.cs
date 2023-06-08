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
using TGC.Core.SceneLoader;
using TgcViewer.Model.Meshes;

namespace TgcViewer.Model.ItemsAnimal
{
    /// <summary>
    /// Llave para abrir una celda de item animal
    /// </summary>
    public class Llave
    {
        /// <summary>
        /// Altura a la que aparecen las llaves
        /// </summary>
        const float ALTURA_LLAVE = 10f;

        /// <summary>
        /// Radio colision
        /// </summary>
        const float BSPHERE_RADIUS = 28f;

        /// <summary>
        /// Color de la llave y celda
        /// </summary>
        public enum ColorType
        {
            VERDE,
            ROJA,
            AMARILLA,
        }


        ColorType color;
        /// <summary>
        /// Color de la llave
        /// </summary>
        public ColorType Color
        {
            get { return color; }
        }

        bool agarrada;
        public bool Agarrada
        {
            get { return agarrada; }
        }

        TgcBoundingSphere boundingSphere;
        /// <summary>
        /// BoundingSphere para agarrar la llave
        /// </summary>
        public TgcBoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
        }

        TgcMeshShader mesh;
        RotadorDeItems rotador;
        


        public Llave(TgcMeshData meshDataNode)
        {
            agarrada = false;
            
            //Nombre: VERDE o ROJA o AMARILLA
            string name = meshDataNode.name;
            name.ToUpper();
            

            //Determinar color
            this.color = getColorType(name);

            //Cargar Mesh
            Vector3 nodeCenter = GameUtils.getBBLowerCenterFromMeshData(meshDataNode);
            TgcSceneLoader loader = new TgcSceneLoader();
            loader.MeshFactory = new TgcMeshShaderFactory();
            string meshPath = GameModel.instance.gameConfig.basePath + "Llaves\\" + color + "\\" + color + "-TgcScene.xml";
            TgcScene scene = loader.loadSceneFromFile(meshPath);
            mesh = (TgcMeshShader)scene.Meshes[0];
            mesh.AutoTransformEnable = true;
            mesh.Position = new Vector3(nodeCenter.X, ALTURA_LLAVE, nodeCenter.Z);

            Vector3 sphereCenter = new Vector3(nodeCenter.X, ALTURA_LLAVE + BSPHERE_RADIUS / 2, nodeCenter.Z);
            boundingSphere = new TgcBoundingSphere(sphereCenter, BSPHERE_RADIUS);

            rotador = new RotadorDeItems();
            rotador.YSpeed = 13f;
            rotador.LimitY = 0;
            rotador.YawSpeed = 3f;
        }

        public void render()
        {
            if (!agarrada)
            {
                //Dibujar si esta en rango visible
                if (GameModel.instance.mainCharacter.isInProximityRange(mesh.Position))
                {
                    rotador.rotate(GameModel.instance.ElapsedTime, this.mesh);
                    GameModel.instance.shaders.setCorrectTechnique(mesh);
                    this.mesh.render();
                }

                //Debug render
                if (GameModel.instance.debugMode.Enabled && GameModel.instance.debugMode.ShowBoundingVolumes)
                {
                    boundingSphere.render();
                }
            }
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(ColorType), this.color);
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
        /// Marca la llave como agarrada
        /// </summary>
        public void agarrar()
        {
            agarrada = true;
            GameModel.instance.sonidos.llave_agarrar.play();
        }

        public void dispose()
        {
            mesh.dispose();
            mesh = null;
            boundingSphere.dispose();
            boundingSphere = null;
            
        }
    }
}
