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
using Examples.TraficoFauna.Meshes;
using TraficoFauna.Examples;
using System.IO;
using TGC.Core.SceneLoader;

namespace Examples.TraficoFauna.Bananas
{
    /// <summary>
    /// Pool con modelos de bananas para reutilizar
    /// </summary>
    public class BananasPool
    {
        const int MAX_BANANAS = 50;

        List<TgcMeshShader> meshes;

        public BananasPool()
        {
            meshes = new List<TgcMeshShader>();
        }

        public void loadModels()
        {
            //Parsear modelo una vez
            string mediaPath = GameModel.instance.gameConfig.basePath + "Bananas\\Banana\\";
            string meshPath = mediaPath + "Banana-TgcScene.xml";
            TgcSceneLoader loader = new TgcSceneLoader();
            loader.MeshFactory = new TgcMeshShaderFactory();
            TgcSceneParser parser = new TgcSceneParser();
            TgcSceneData sceneData = parser.parseSceneFromString(File.ReadAllText(meshPath));

            //Cargar varios para el pool
            for (int i = 0; i < MAX_BANANAS; i++)
            {
                TgcMeshShader mesh = (TgcMeshShader)loader.loadScene(sceneData, mediaPath).Meshes[0];
                mesh.AutoTransformEnable = true;
                meshes.Add(mesh);
            }

            freeAllMeshes();
        }

        /// <summary>
        /// Obtener un mesh libre y tomarlo
        /// </summary>
        public TgcMeshShader getAvaliableMesh()
        {
            foreach (TgcMeshShader m in meshes)
            {
                if (!m.Enabled)
                {
                    m.Enabled = true;
                    return m;
                }
            }
            throw new Exception("No hay mas Bananas disponibles para utilizar. Cantidad actual: " + meshes.Count);
        }

        /// <summary>
        /// Liberar todos los meshes
        /// </summary>
        public void freeAllMeshes()
        {
            foreach (TgcMeshShader m in meshes)
            {
                m.Enabled = false;
            }
        }

    }
}
