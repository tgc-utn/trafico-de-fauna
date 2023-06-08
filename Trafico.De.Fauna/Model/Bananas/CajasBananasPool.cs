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
using TGC.Core.KeyFrameLoader;

namespace TgcViewer.Model.Bananas
{
    /// <summary>
    /// Pool con modelos de cajas de bananas para reutilizar
    /// </summary>
    public class CajasBananasPool
    {
        const int MAX_CAJAS_CON_BANANA = 20;
        const int MAX_CAJAS_SIN_BANANA = 20;

        List<TgcKeyFrameMesh> meshesConBanana;
        List<TgcKeyFrameMesh> meshesSinBanana;

        public CajasBananasPool()
        {
            meshesConBanana = new List<TgcKeyFrameMesh>();
            meshesSinBanana = new List<TgcKeyFrameMesh>();
        }

        public void loadModels()
        {
            //Parsear modelo sin bananas una vez
            string mediaPath = GameModel.instance.gameConfig.basePath + "Bananas\\CajaBananas\\";
            string meshPath = mediaPath + "CajaSinBananas-TgcKeyFrameMesh.xml";
            TgcKeyFrameLoader loader = new TgcKeyFrameLoader();
            TgcKeyFrameParser parser = new TgcKeyFrameParser();
            TgcKeyFrameMeshData meshDataSinBananas = parser.parseMeshFromString(File.ReadAllText(meshPath));
            meshDataSinBananas.materialsData[0].fileName = "CajaSinBananas.jpg";
            TgcKeyFrameAnimationData[] animData = new TgcKeyFrameAnimationData[]
            {
                parser.parseAnimationFromString(File.ReadAllText(mediaPath + "StandBy-TgcKeyFrameAnim.xml")),
                parser.parseAnimationFromString(File.ReadAllText(mediaPath + "Golpe-TgcKeyFrameAnim.xml")),
                parser.parseAnimationFromString(File.ReadAllText(mediaPath + "Explosion-TgcKeyFrameAnim.xml"))
            };

            //Modelo con bananas (solo cambia la textura)
            TgcKeyFrameMeshData meshDataConBananas = parser.parseMeshFromString(File.ReadAllText(meshPath));
            meshDataConBananas.materialsData[0].fileName = "CajaConBananas.jpg";

            //Cargar varios modelos con bananas para el pool
            for (int i = 0; i < MAX_CAJAS_SIN_BANANA; i++)
            {
                TgcKeyFrameMesh mesh = loader.loadMesh(meshDataConBananas, mediaPath);
                mesh.AutoTransformEnable = true;
                for (int j = 0; j < animData.Length; j++)
                {
                    loader.loadAnimation(mesh, animData[j]);
                }
                meshesConBanana.Add(mesh);
            }

            //Cargar varios modelos sin bananas para el pool
            for (int i = 0; i < MAX_CAJAS_SIN_BANANA; i++)
            {
                TgcKeyFrameMesh mesh = loader.loadMesh(meshDataSinBananas, mediaPath);
                mesh.AutoTransformEnable = true;
                for (int j = 0; j < animData.Length; j++)
                {
                    loader.loadAnimation(mesh, animData[j]);
                }
                meshesSinBanana.Add(mesh);
            }

            freeAllMeshes();
        }

        
        /// <summary>
        /// Obtener un mesh con banana libre y tomarlo
        /// </summary>
        public TgcKeyFrameMesh getAvaliableMeshConBanana()
        {
            foreach (TgcKeyFrameMesh m in meshesConBanana)
            {
                if (!m.Enabled)
                {
                    m.Enabled = true;
                    return m;
                }
            }
            throw new Exception("No hay mas Cajas con Bananas disponibles para utilizar. Cantidad actual: " + meshesConBanana.Count);
        }

        /// <summary>
        /// Obtener un mesh sin banana libre y tomarlo
        /// </summary>
        public TgcKeyFrameMesh getAvaliableMeshSinBanana()
        {
            foreach (TgcKeyFrameMesh m in meshesSinBanana)
            {
                if (!m.Enabled)
                {
                    m.Enabled = true;
                    return m;
                }
            }
            throw new Exception("No hay mas Cajas sin Bananas disponibles para utilizar. Cantidad actual: " + meshesSinBanana.Count);
        }
        

        /// <summary>
        /// Liberar todos los meshes
        /// </summary>
        public void freeAllMeshes()
        {
            foreach (TgcKeyFrameMesh m in meshesConBanana)
            {
                m.Enabled = false;
            }
            foreach (TgcKeyFrameMesh m in meshesSinBanana)
            {
                m.Enabled = false;
            }
        }

    }
}
