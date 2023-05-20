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

namespace TgcViewer.Utils.TgcSceneLoader
{
    public class TgcSceneData
    {
        public string name;
        public string texturesDir;
        
        //Info de LightMaps
        public string lightmapsDir;
        public bool lightmapsEnabled;

        public TgcMeshData[] meshesData;
        public TgcMaterialData[] materialsData;

        //BoundingBox de la escena
        public float[] pMin;
        public float[] pMax;

    }
}
