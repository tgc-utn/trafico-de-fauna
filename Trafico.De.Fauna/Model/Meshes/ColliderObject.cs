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

using TGC.Core.BoundingVolumes;

namespace Trafico.De.Fauna.Model.Meshes
{
    /// <summary>
    /// Objeto con el que se puede colisionar
    /// </summary>
    public class ColliderObject
    {
        public ColliderObject(TgcMeshShader mesh)
        {
            this.mesh = mesh;
            this.boundingBox = mesh.BoundingBox;
            this.enable = true;
        }

        public ColliderObject(TgcBoundingAxisAlignBox boundingBox)
        {
            this.mesh = null;
            this.boundingBox = boundingBox;
            this.enable = true;
        }

        TgcBoundingAxisAlignBox boundingBox;
        /// <summary>
        /// AABB
        /// </summary>
        public TgcBoundingAxisAlignBox BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }

        bool enable;
        /// <summary>
        /// Habilitado
        /// </summary>
        public bool Enabled
        {
            get { return enable; }
            set { enable = value; }
        }

        TgcMeshShader mesh;
        /// <summary>
        /// Mesh
        /// </summary>
        public TgcMeshShader Mesh
        {
            get { return mesh; }
            set { mesh = value; }
        }

        public override string ToString()
        {
            if (mesh != null)
            {
                return "ColliderObject: " + mesh.Name;
            }
            return "ColliderObject: BoundingBox";
        }

        public void dispose()
        {
            if (boundingBox != null)
            {
                boundingBox.Dispose();
                boundingBox = null;
            }
            if (mesh != null)
            {
                mesh.Dispose();
                mesh = null;
            }
        }
    }
}
