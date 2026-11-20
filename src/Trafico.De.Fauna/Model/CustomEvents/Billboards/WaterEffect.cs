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

using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.Billboards;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model.CustomEvents.Billboards
{
    /// <summary>
    ///     Efecto de agua animada en el piso
    /// </summary>
    public class WaterEffect : CustomEvent
    {
        private TgcBoundingAxisAlignBox aabb;
        private HorizontalAnimatedBillboard billboard;

        public override void load(TgcMeshData meshData, string texturePath, TgcSceneData originalSceneData)
        {
            var device = D3DDevice.Instance.Device;
            billboard = new HorizontalAnimatedBillboard(device,
                GameModel.instance.gameConfig.basePath + "Billboards\\Water.png", new Size(128, 128), 4, 7);
            aabb = GameUtils.createBBFromMeshData(meshData);
            billboard.PMin = aabb.PMin;
            billboard.PMax = aabb.PMax;
        }

        public override void init()
        {
            billboard.Playing = true;
        }

        public override void end()
        {
            billboard.Playing = false;
        }

        public override void update(float elapsedTime)
        {
            //Actualizar valores de animacion si esta en rango visible
            if (GameModel.instance.mainCharacter.isInProximityRange(billboard.PMin))
            {
                billboard.updateValues(elapsedTime);
            }
        }

        public override void render()
        {
            //Dibujar si esta en rango visible
            if (GameModel.instance.mainCharacter.isInProximityRange(billboard.PMin))
            {
                billboard.Render();
            }
        }

        public override void renderDebug()
        {
            aabb.Render();
        }

        public override ColliderObject getColliderObject()
        {
            return null;
        }

        public override void dispose()
        {
            billboard.Dispose();
            aabb.Dispose();
        }
    }
}
