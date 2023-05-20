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
using Examples.TraficoFauna.Billboards;
using System.Drawing;
using Examples.TraficoFauna.Interpolators;
using Examples.TraficoFauna.Meshes;

namespace Examples.TraficoFauna.CustomEvents.Surfaces
{

    /// <summary>
    /// Efecto de agua animada en el piso
    /// </summary>
    public class WaterSurface : CustomEvent, IAlphaObject
    {
        HorizontalDisplacementBillboard billboard;
        InterpoladorVaiven vaiven;
        TgcBoundingBox aabb;

        public override void load(TgcMeshData meshData, string texturePath, TgcSceneData originalSceneData)
        {
            aabb = GameUtils.createBBFromMeshData(meshData);
            billboard = new HorizontalDisplacementBillboard(Game.instance.gameConfig.basePath + "Billboards\\WaterSurface.png", aabb.PMin, aabb.PMax);
            billboard.Speed = new Vector2(0, 0.3f);

            vaiven = new InterpoladorVaiven(-0.2f, 0.2f, 0.1f);
        }

        public override void init()
        {
            billboard.Playing = true;
        }

        public override void end()
        {
            billboard.Playing = false;
        }

        public override void update()
        {
            billboard.Speed = new Vector2(vaiven.update(), 0.3f);
        }

        public override void render()
        {
            //Dibujar si esta en rango visible
            if (Game.instance.mainCharacter.isInProximityRange(billboard.PMin))
            {
                Game.instance.currentLevel.AlphaObjects.Add(this);
            }
        }

        public void renderWithAlpha()
        {
            billboard.render();
        }

        public override void renderDebug()
        {
            aabb.render();
        }

        public override ColliderObject getColliderObject()
        {
            return null;
        }

        public override void dispose()
        {
            billboard.dispose();
            aabb.dispose();
        }


        

    }
}
