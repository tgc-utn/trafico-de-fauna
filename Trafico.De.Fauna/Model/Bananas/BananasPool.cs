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
using System.IO;
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model.Bananas
{
    /// <summary>
    ///     Pool con modelos de bananas para reutilizar
    /// </summary>
    public class BananasPool
    {
        private const int MAX_BANANAS = 50;

        private readonly List<TgcMeshShader> meshes;

        public BananasPool()
        {
            meshes = new List<TgcMeshShader>();
        }

        public void loadModels()
        {
            //Parsear modelo una vez
            var mediaPath = GameModel.instance.gameConfig.basePath + "Bananas\\Banana\\";
            var meshPath = mediaPath + "Banana-TgcScene.xml";
            var loader = new TgcSceneLoader();
            loader.MeshFactory = new TgcMeshShaderFactory();
            var parser = new TgcSceneParser();
            var sceneData = parser.parseSceneFromString(File.ReadAllText(meshPath));

            //Cargar varios para el pool
            for (var i = 0; i < MAX_BANANAS; i++)
            {
                var mesh = (TgcMeshShader)loader.loadScene(sceneData, mediaPath).Meshes[0];
                mesh.AutoTransformEnable = true;
                meshes.Add(mesh);
            }

            freeAllMeshes();
        }

        /// <summary>
        ///     Obtener un mesh libre y tomarlo
        /// </summary>
        public TgcMeshShader getAvaliableMesh()
        {
            foreach (var m in meshes)
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
        ///     Liberar todos los meshes
        /// </summary>
        public void freeAllMeshes()
        {
            foreach (var m in meshes)
            {
                m.Enabled = false;
            }
        }
    }
}
