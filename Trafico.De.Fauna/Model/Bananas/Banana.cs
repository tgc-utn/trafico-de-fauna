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
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.ItemsAnimal;
using Trafico.De.Fauna.Model.MainCharacters;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model.Bananas
{
    /// <summary>
    ///     Item de Banana en un sector
    /// </summary>
    public class Banana
    {
        private const float BSPHERE_RADIUS = 15;

        private readonly TGCVector3 center;

        private TgcMeshShader mesh;


        private readonly RotadorDeItems rotador;

        public Banana(TgcMeshData meshData)
        {
            Name = meshData.name;
            Enabled = true;
            var boundingBox = GameUtils.createBBFromMeshData(meshData);
            center = boundingBox.calculateBoxCenter();

            BoundingSphere = new TgcBoundingSphere(new TGCVector3(center.X, center.Y + BSPHERE_RADIUS / 2, center.Z),
                BSPHERE_RADIUS);

            rotador = new RotadorDeItems();
            rotador.YSpeed = 13f;
            rotador.LimitY = 5;
            rotador.YawSpeed = 3f;
        }

        public string Name { get; }

        /// <summary>
        ///     Indica si la banana ya fue tomada
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        ///     BoundingSphere para agarrar la banana
        /// </summary>
        public TgcBoundingSphere BoundingSphere { get; private set; }

        public void init()
        {
            if (!Enabled)
            {
                return;
            }

            mesh = GameModel.instance.bananasPool.getAvaliableMesh();
            mesh.Position = center;
        }

        public void render()
        {
            if (Enabled)
            {
                //Dibujar si esta en rango visible
                if (GameModel.instance.mainCharacter.isInProximityRange(mesh.Position))
                {
                    rotador.rotate(GameModel.instance.ElapsedTime, mesh);
                    GameModel.instance.shaders.setCorrectTechnique(mesh);
                    mesh.render();
                }

                //Debug
                if (GameModel.instance.debugMode.Enabled && GameModel.instance.debugMode.ShowBoundingVolumes)
                {
                    BoundingSphere.Render();
                }
            }
        }


        public void hide()
        {
            mesh = null;
        }

        public void dispose()
        {
            mesh = null;
            BoundingSphere.Dispose();
            BoundingSphere = null;
        }

        /// <summary>
        ///     Agarra la banana por el personaje principal, si esta activada y colisiona
        /// </summary>
        public void agarrar(MainCharacter mainCharacter)
        {
            if (!Enabled)
            {
                return;
            }

            //Colision
            if (mainCharacter.collideWithBSphere(BoundingSphere))
            {
                mainCharacter.agregarBananas(1);
                var sound = GameModel.instance.sonidos.banana_agarrar;
                sound.Stop();
                sound.Play();
                GameModel.instance.hud.efectoAgarrarBanana();
                Enabled = false;
            }
        }
    }
}
