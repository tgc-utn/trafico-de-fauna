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
    ///     Objeto con el que se puede colisionar
    /// </summary>
    public class ColliderObject
    {
        public ColliderObject(TgcMeshShader mesh)
        {
            this.Mesh = mesh;
            BoundingBox = mesh.BoundingBox;
            Enabled = true;
        }

        public ColliderObject(TgcBoundingAxisAlignBox boundingBox)
        {
            Mesh = null;
            this.BoundingBox = boundingBox;
            Enabled = true;
        }

        /// <summary>
        ///     AABB
        /// </summary>
        public TgcBoundingAxisAlignBox BoundingBox { get; set; }

        /// <summary>
        ///     Habilitado
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        ///     Mesh
        /// </summary>
        public TgcMeshShader Mesh { get; set; }

        public override string ToString()
        {
            if (Mesh != null)
            {
                return "ColliderObject: " + Mesh.Name;
            }

            return "ColliderObject: BoundingBox";
        }

        public void dispose()
        {
            if (BoundingBox != null)
            {
                BoundingBox.Dispose();
                BoundingBox = null;
            }

            // TODO in TGC.Core there are some missing validations (like was in Trafico de fauna TgcMesh) if attributes are null.
            if (Mesh != null && Mesh.D3dMesh != null)
            {
                Mesh.Dispose();
                Mesh = null;
            }
        }
    }
}
