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
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TraficoFauna.Examples;
using Examples.TraficoFauna.Dialogs;
using TgcViewer;
using Microsoft.DirectX.DirectInput;
using Examples.TraficoFauna.Meshes;

namespace Examples.TraficoFauna.CustomEvents.ZonasEspeciales
{

    /// <summary>
    /// Si el personaje colisiona con el BoundingBox pierde una vida
    /// </summary>
    public class ZonaDeMuerte : CustomEvent
    {

        TgcBoundingBox boundingBox;

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

        public override void update()
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
            boundingBox.render();
        }

        public override ColliderObject getColliderObject()
        {
            return null;
        }

        public override void dispose()
        {
            boundingBox.dispose();
            boundingBox = null;
        }
    }
}
