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
using TGC.Core.SkeletalAnimation;
using TraficoFauna.Examples;

namespace Examples.TraficoFauna.Enemies
{
    /// <summary>
    /// Pool con modelos de guardias para reutilizar
    /// </summary>
    public class GuardiasPool
    {
        const int MAX_GUARDIAS = 3;

        List<TgcSkeletalMesh> meshes;

        public GuardiasPool()
        {
            meshes = new List<TgcSkeletalMesh>();
        }


        public void loadModels()
        {
            string meshPath = GameModel.instance.gameConfig.basePath + "Guardia\\";
            TgcSkeletalLoader loader = new TgcSkeletalLoader();

            for (int i = 0; i < MAX_GUARDIAS; i++)
            {
                TgcSkeletalMesh mesh = loader.loadMeshAndAnimationsFromFile(
                meshPath + "Guardia-TgcSkeletalMesh.xml",
                meshPath,
                new string[] { 
                    meshPath + "Walk-TgcSkeletalAnim.xml",
                    meshPath+ "StandBy-TgcSkeletalAnim.xml",
                    meshPath+ "Run-TgcSkeletalAnim.xml"
                });
                mesh.AutoTransformEnable = true;
                meshes.Add(mesh);
            }

            freeAllMeshes();
        }

        /// <summary>
        /// Obtener un mesh libre y tomarlo
        /// </summary>
        public TgcSkeletalMesh getAvaliableMesh()
        {
            foreach (TgcSkeletalMesh m in meshes)
            {
                if (!m.Enabled)
                {
                    m.Enabled = true;
                    return m;
                }
            }
            throw new Exception("No hay mas Guardias disponibles para utilizar. Cantidad actual: " + meshes.Count);
        }

        /// <summary>
        /// Liberar todos los meshes
        /// </summary>
        public void freeAllMeshes()
        {
            foreach (TgcSkeletalMesh m in meshes)
            {
                m.Enabled = false;
            }
        }

    }
}
