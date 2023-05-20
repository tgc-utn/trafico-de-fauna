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

namespace TgcViewer.Utils.TgcSceneLoader
{
    public class TgcMeshData
    {
        //Tipos de Mesh Instancias
        public static readonly string ORIGINAL = "Original";
        public static readonly string INSTANCE = "Instance";

        public string name;
        public string layerName;

        //UserProperties
        public Dictionary<string, string> userProperties;

        //Material global
        public int materialId;

        //Filename del LightMap
        public string lightmap;
        public bool lightmapEnabled;

        //Color general, por si no tiene Material
        public float[] color;

        //Valores por triangulo
        public int[] coordinatesIndices;
        public int[] texCoordinatesIndices;
        public int[] colorIndices;
        public int[] texCoordinatesIndicesLightMap;

        //SubMaterials para cada triangulo
        public int[] materialsIds;

        //Valores por vertice
        public float[] verticesCoordinates;
        public float[] textureCoordinates;
        public float[] verticesNormals;
        public int[] verticesColors;
        public float[] textureCoordinatesLightMap;

        //BoundingBox
        public float[] pMin;
        public float[] pMax;

        //Tipo de instancia
        public string instanceType;

        //Indice de la malla original
        public int originalMesh;

        //Datos de transformacion para instancia
        public float[] position;
        public float[] rotation;
        public float[] scale;

        //AlphaBlending activado
        public bool alphaBlending;
    }
}
