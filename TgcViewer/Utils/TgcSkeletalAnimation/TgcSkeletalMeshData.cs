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
using TgcViewer.Utils.TgcSceneLoader;

namespace TgcViewer.Utils.TgcSkeletalAnimation
{
    /// <summary>
    /// Información de la Malla de un modelo animado por Animación Esquelética
    /// </summary>
    public class TgcSkeletalMeshData
    {
        public string name;
        public string texturesDir;

        //Valores por triangulo
        public int[] coordinatesIndices;
        public int[] texCoordinatesIndices;
        public int[] colorIndices;

        //Color general, por si no tiene Material
        public float[] color;

        //SubMaterials para cada triangulo
        public int materialId;
        public int[] materialsIds;

        //Valores por vertice
        public float[] verticesCoordinates;
        public float[] textureCoordinates;
        public int[] verticesColors;

        //Informacion de Texturas y Materials
        public TgcMaterialData[] materialsData;

        //Huesos del esqueleto
        public TgcSkeletalBoneData[] bones;

        //Tomar de a 3: VertesIdx, BoneIdx, Weight
        public float[] verticesWeights;

        //BoundingBox
        public float[] pMin;
        public float[] pMax;

    }
}
