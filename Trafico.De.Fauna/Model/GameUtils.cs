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

using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model
{
    /// <summary>
    ///     Utilidades generales del juego
    /// </summary>
    public class GameUtils
    {
        /// <summary>
        ///     Orientaciones basicas en XZ
        /// </summary>
        public enum Orientacion
        {
            Norte,
            Sur,
            Este,
            Oeste
        }

        public static TgcBoundingAxisAlignBox createBBFromMeshData(TgcMeshData meshData)
        {
            return new TgcBoundingAxisAlignBox(
                TGCVector3.Float3ArrayToTGCVector3(meshData.pMin),
                TGCVector3.Float3ArrayToTGCVector3(meshData.pMax)
            );
        }

        public static TGCVector3 getBBCenterFromMeshData(TgcMeshData meshData)
        {
            var aabb = new TgcBoundingAxisAlignBox(
                TGCVector3.Float3ArrayToTGCVector3(meshData.pMin),
                TGCVector3.Float3ArrayToTGCVector3(meshData.pMax)
            );
            return aabb.calculateBoxCenter();
        }

        public static TGCVector3 getBBLowerCenterFromMeshData(TgcMeshData meshData)
        {
            var aabb = new TgcBoundingAxisAlignBox(
                TGCVector3.Float3ArrayToTGCVector3(meshData.pMin),
                TGCVector3.Float3ArrayToTGCVector3(meshData.pMax)
            );
            var center = aabb.calculateBoxCenter();
            center.Y = aabb.PMin.Y;
            return center;
        }

        public static bool containsUserProp(Dictionary<string, string> userProperties, string key)
        {
            return userProperties != null && userProperties.ContainsKey(key);
        }

        /// <summary>
        ///     Crear un TgcSceneData reducido en base a solo lo necesario por un  TgcMeshData
        /// </summary>
        public static TgcSceneData createSceneDataForMeshData(TgcSceneData origSceneData, TgcMeshData meshData)
        {
            //Duplicar parte base
            var newSceneData = new TgcSceneData();
            newSceneData.name = origSceneData.name;
            newSceneData.pMin = origSceneData.pMin;
            newSceneData.pMax = origSceneData.pMax;
            newSceneData.texturesDir = origSceneData.texturesDir;
            newSceneData.lightmapsDir = origSceneData.lightmapsDir;
            newSceneData.lightmapsEnabled = origSceneData.lightmapsEnabled;

            //Agregar unico meshData
            newSceneData.meshesData = new[] { meshData };

            //Agarrar solo los materials necesarios por este meshData
            if (meshData.materialId == -1)
            {
                newSceneData.materialsData = new TgcMaterialData[] { };
            }
            else
            {
                newSceneData.materialsData = new[] { origSceneData.materialsData[meshData.materialId] };
                meshData.materialId = 0;
            }

            return newSceneData;
        }

        /// <summary>
        ///     Carga un unico Mesh en base en base a su MeshData y a las texturas de toda la escena
        /// </summary>
        public static TgcMeshShader loadMeshShaderFromSceneData(TgcMeshData meshDataNode, string texturePath,
            TgcSceneData originalSceneData)
        {
            var loader = new TgcSceneLoader();
            loader.MeshFactory = new TgcMeshShaderFactory();
            var sceneData = createSceneDataForMeshData(originalSceneData, meshDataNode);
            var scene = loader.loadScene(sceneData, texturePath);
            var mesh = (TgcMeshShader)scene.Meshes[0];
            mesh.AutoTransformEnable = true;
            return mesh;
        }


        public static void findExtremeVertices(TgcMeshData meshData, out TGCVector3 extreme1, out TGCVector3 extreme2)
        {
            extreme1 = new TGCVector3();
            extreme2 = new TGCVector3();
        }


        /// <summary>
        ///     Devuelve los extremos en X o en Z del Bounding, dependiendo que lado es mas largo (X o Z).
        ///     Devuelve true si el lado mas largo es el X
        /// </summary>
        public static bool getBBExtremeCentersXZ(TgcMeshData meshData, out TGCVector3 extreme1, out TGCVector3 extreme2)
        {
            var bb = createBBFromMeshData(meshData);
            var size = bb.calculateSize();
            var center = bb.calculateBoxCenter();

            if (size.X > size.Z)
            {
                extreme1 = new TGCVector3(bb.PMin.X, bb.PMin.Y, center.Z);
                extreme2 = new TGCVector3(bb.PMax.X, bb.PMin.Y, center.Z);
                return true;
            }

            extreme1 = new TGCVector3(center.X, bb.PMin.Y, bb.PMin.Z);
            extreme2 = new TGCVector3(center.X, bb.PMin.Y, bb.PMax.Z);
            return false;
        }
    }
}
