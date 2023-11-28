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
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model.CustomEvents.ZonasEspeciales
{

    /// <summary>
    /// Si el personaje colisiona con el BoundingBox pierde una vida
    /// </summary>
    public class ZonaDeMuerte : CustomEvent
    {

        TgcBoundingAxisAlignBox boundingBox;

        public override void load(TgcMeshData meshData, string texturePath, TgcSceneData originalSceneData)
        {
            boundingBox = GameUtils.createBBFromMeshData(meshData);
        }

        public override void init()
        {
            
        }

        public override void end()
        {
            
        }

        public override void update(float elapsedTime)
        {
        }

        public override void render()
        {
            if (GameModel.instance.mainCharacter.collideWithAABB(boundingBox))
            {
                GameModel.instance.currentLevel.removeLifeToPlayer();
            }
        }

        public override void renderDebug()
        {
            boundingBox.Render();
        }

        public override ColliderObject getColliderObject()
        {
            return null;
        }

        public override void dispose()
        {
            boundingBox.Dispose();
            boundingBox = null;
        }
    }
}
