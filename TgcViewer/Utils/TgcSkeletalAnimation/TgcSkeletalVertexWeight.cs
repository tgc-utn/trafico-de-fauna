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

namespace TgcViewer.Utils.TgcSkeletalAnimation
{
    /// <summary>
    /// Influencias de huesos sobre un vertice.
    /// Un vertice puede estar influenciado por mas de un hueso
    /// </summary>
    public class TgcSkeletalVertexWeight
    {
        public TgcSkeletalVertexWeight()
        {
            this.weights = new List<BoneWeight>();
        }

        private List<BoneWeight> weights;
        /// <summary>
        /// Influencias del vertice
        /// </summary>
        public List<BoneWeight> Weights
        {
            get { return weights; }
        }


        /// <summary>
        /// Influencia de un hueso sobre un vertice
        /// </summary>
        public class BoneWeight
        {
            public BoneWeight(TgcSkeletalBone bone, float weight)
            {
                this.bone = bone;
                this.weight = weight;
            }
            
            
            private TgcSkeletalBone bone;
            /// <summary>
            /// Hueso que influye
            /// </summary>
            public TgcSkeletalBone Bone
            {
                get { return bone; }
                set { bone = value; }
            }
            
            private float weight;
            /// <summary>
            /// Influencia del hueso sobre el vertice. Valor normalizado entre 0 y 1
            /// </summary>
            public float Weight
            {
                get { return weight; }
                set { weight = value; }
            }
        }


    }
}
