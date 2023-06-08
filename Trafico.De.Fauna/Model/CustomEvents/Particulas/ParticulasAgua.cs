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

using Microsoft.DirectX;
using TGC.Core.SceneLoader;
using TgcViewer.Model.Meshes;
using TgcViewer.Model.Particles;

namespace TgcViewer.Model.CustomEvents.Particulas
{
    /// <summary>
    /// Efecto de agua salpicando
    /// </summary>
    public class ParticulasAgua : CustomEvent, IAlphaObject
    {
        ParticleEmitter emitter;

        public override void load(TgcMeshData meshData, string texturePath, TgcSceneData originalSceneData)
        {
            emitter = new ParticleEmitter(GameModel.instance.gameConfig.shadersPath + "Particles\\particula_agua.png", 5);
            emitter.Position = GameUtils.getBBLowerCenterFromMeshData(meshData);
            emitter.Dispersion = 100;
            emitter.ParticleTimeToLive = 3f;
            emitter.CreationFrecuency = 0.4f;
            emitter.Speed = new Vector3(5f, 5f, 5f);
            emitter.MinSizeParticle = 8f;
            emitter.MaxSizeParticle = 10f;
        }

        public override void init()
        {
            emitter.Playing = true;
        }

        public override void end()
        {
            emitter.Playing = false;
        }

        public override void update(float elapsedTime)
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
            emitter.render(GameModel.instance.ElapsedTime);
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
