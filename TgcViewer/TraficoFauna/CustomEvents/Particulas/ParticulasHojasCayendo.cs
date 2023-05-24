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
using Examples.TraficoFauna.Particles;
using TraficoFauna.Examples;
using System.Drawing;
using Microsoft.DirectX;
using Examples.TraficoFauna.Meshes;

namespace Examples.TraficoFauna.CustomEvents.Particulas
{
    /// <summary>
    /// Efecto de hojas de arbol cayendo de arriba
    /// </summary>
    public class ParticulasHojasCayendo : CustomEvent, IAlphaObject
    {
        ParticleEmitter emitter;

        public override void load(TgcMeshData meshData, string texturePath, TgcSceneData originalSceneData)
        {
            emitter = new ParticleEmitter(GameModel.instance.gameConfig.shadersPath + "Particles\\leaf.png", 5);
            emitter.Position = GameUtils.getBBLowerCenterFromMeshData(meshData);
            emitter.Dispersion = 500;
            emitter.ParticleTimeToLive = 15f;
            emitter.CreationFrecuency = 1f;
            emitter.Speed = new Vector3(5f, -5f, 5f);
            emitter.MinSizeParticle = 0.2f;
            emitter.MaxSizeParticle = 0.1f;
        }

        public override void init()
        {
            emitter.Playing = true;
        }

        public override void end()
        {
            emitter.Playing = false;
        }

        public override void update()
        {
        }

        public override void render()
        {
            //Dibujar si esta en rango visible
            if (GameModel.instance.mainCharacter.isInProximityRange(emitter.Position))
            {
                GameModel.instance.currentLevel.AlphaObjects.Add(this);
            }
        }

        public void renderWithAlpha()
        {
            emitter.render();
        }

        public override void renderDebug()
        {
        }

        public override ColliderObject getColliderObject()
        {
            return null;
        }

        public override void dispose()
        {
            emitter.dispose();
        }
    }
}
