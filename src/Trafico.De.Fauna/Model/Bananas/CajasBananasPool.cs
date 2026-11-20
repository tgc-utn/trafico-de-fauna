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

namespace Trafico.De.Fauna.Model.Bananas
{
    /// <summary>
    ///     Pool con modelos de cajas de bananas para reutilizar
    /// </summary>
    public class CajasBananasPool
    {
        private const int MAX_CAJAS_CON_BANANA = 20;
        private const int MAX_CAJAS_SIN_BANANA = 20;

        private readonly List<TgcKeyFrameMesh> meshesConBanana;
        private readonly List<TgcKeyFrameMesh> meshesSinBanana;

        public CajasBananasPool()
        {
            meshesConBanana = new List<TgcKeyFrameMesh>();
            meshesSinBanana = new List<TgcKeyFrameMesh>();
        }

        public void loadModels()
        {
            //Parsear modelo sin bananas una vez
            var mediaPath = GameModel.instance.gameConfig.basePath + "Bananas\\CajaBananas\\";
            var meshPath = mediaPath + "CajaSinBananas-TgcKeyFrameMesh.xml";
            var loader = new TgcKeyFrameLoader();
            var parser = new TgcKeyFrameParser();
            var meshDataSinBananas = parser.parseMeshFromString(File.ReadAllText(meshPath));
            meshDataSinBananas.materialsData[0].fileName = "CajaSinBananas.jpg";
            TgcKeyFrameAnimationData[] animData =
            {
                parser.parseAnimationFromString(File.ReadAllText(mediaPath + "StandBy-TgcKeyFrameAnim.xml")),
                parser.parseAnimationFromString(File.ReadAllText(mediaPath + "Golpe-TgcKeyFrameAnim.xml")),
                parser.parseAnimationFromString(File.ReadAllText(mediaPath + "Explosion-TgcKeyFrameAnim.xml"))
            };

            //Modelo con bananas (solo cambia la textura)
            var meshDataConBananas = parser.parseMeshFromString(File.ReadAllText(meshPath));
            meshDataConBananas.materialsData[0].fileName = "CajaConBananas.jpg";

            //Cargar varios modelos con bananas para el pool
            for (var i = 0; i < MAX_CAJAS_SIN_BANANA; i++)
            {
                var mesh = loader.loadMesh(meshDataConBananas, mediaPath);
                mesh.AutoTransformEnable = true;
                for (var j = 0; j < animData.Length; j++)
                {
                    loader.loadAnimation(mesh, animData[j]);
                }

                meshesConBanana.Add(mesh);
            }

            //Cargar varios modelos sin bananas para el pool
            for (var i = 0; i < MAX_CAJAS_SIN_BANANA; i++)
            {
                var mesh = loader.loadMesh(meshDataSinBananas, mediaPath);
                mesh.AutoTransformEnable = true;
                for (var j = 0; j < animData.Length; j++)
                {
                    loader.loadAnimation(mesh, animData[j]);
                }

                meshesSinBanana.Add(mesh);
            }

            freeAllMeshes();
        }


        /// <summary>
        ///     Obtener un mesh con banana libre y tomarlo
        /// </summary>
        public TgcKeyFrameMesh getAvaliableMeshConBanana()
        {
            foreach (var m in meshesConBanana)
            {
                if (!m.Enabled)
                {
                    m.Enabled = true;
                    return m;
                }
            }

            throw new Exception("No hay mas Cajas con Bananas disponibles para utilizar. Cantidad actual: " +
                                meshesConBanana.Count);
        }

        /// <summary>
        ///     Obtener un mesh sin banana libre y tomarlo
        /// </summary>
        public TgcKeyFrameMesh getAvaliableMeshSinBanana()
        {
            foreach (var m in meshesSinBanana)
            {
                if (!m.Enabled)
                {
                    m.Enabled = true;
                    return m;
                }
            }

            throw new Exception("No hay mas Cajas sin Bananas disponibles para utilizar. Cantidad actual: " +
                                meshesSinBanana.Count);
        }


        /// <summary>
        ///     Liberar todos los meshes
        /// </summary>
        public void freeAllMeshes()
        {
            foreach (var m in meshesConBanana)
            {
                m.Enabled = false;
            }

            foreach (var m in meshesSinBanana)
            {
                m.Enabled = false;
            }
        }
    }
}
