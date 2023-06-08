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
using TraficoFauna.Examples;
using Examples.TraficoFauna.Meshes;
using Examples.TraficoFauna.ItemsAnimal;
using TGC.Core.Sound;
using TGC.Core.SceneLoader;
using TGC.Core.Geometry;
using TGC.Core.BoundingVolumes;

namespace Examples.TraficoFauna.Bananas
{
    /// <summary>
    /// Item de Banana en un sector
    /// </summary>
    public class Banana
    {
        const float BSPHERE_RADIUS = 15;

        Vector3 center;
        TgcMeshShader mesh;
        RotadorDeItems rotador;


        string name;
        public string Name
        {
            get { return name; }
        }

        bool enable;
        /// <summary>
        /// Indica si la banana ya fue tomada
        /// </summary>
        public bool Enabled
        {
            get { return enable; }
            set { enable = value; }
        }

        TgcBoundingSphere boundingSphere;
        /// <summary>
        /// BoundingSphere para agarrar la banana
        /// </summary>
        public TgcBoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
        }

        public Banana(TgcMeshData meshData)
        {
            name = meshData.name;
            enable = true;
            TgcBoundingAxisAlignBox boundingBox = GameUtils.createBBFromMeshData(meshData);
            center = boundingBox.calculateBoxCenter();

            boundingSphere = new TgcBoundingSphere(new Vector3(center.X, center.Y + BSPHERE_RADIUS / 2, center.Z), BSPHERE_RADIUS);

            rotador = new RotadorDeItems();
            rotador.YSpeed = 13f;
            rotador.LimitY = 5;
            rotador.YawSpeed = 3f;
        }

        public void init()
        {
            if (!enable)
            {
                return;
            }

            mesh = GameModel.instance.bananasPool.getAvaliableMesh();
            mesh.Position = center;
        }

        public void render()
        {
            if (enable)
            {
                //Dibujar si esta en rango visible
                if (GameModel.instance.mainCharacter.isInProximityRange(mesh.Position))
                {
                    rotador.rotate(GameModel.instance.ElapsedTime, this.mesh);
                    GameModel.instance.shaders.setCorrectTechnique(mesh);
                    this.mesh.render();
                }

                //Debug
                if (GameModel.instance.debugMode.Enabled && GameModel.instance.debugMode.ShowBoundingVolumes)
                {
                    boundingSphere.render();
                }

            }
        }



        public void hide()
        {
            this.mesh = null;
        }

        public void dispose()
        {
            mesh = null;
            boundingSphere.dispose();
            boundingSphere = null;
        }

        /// <summary>
        /// Agarra la banana por el personaje principal, si esta activada y colisiona
        /// </summary>
        public void agarrar(MainCharacter mainCharacter)
        {
            if (!enable)
            {
                return;
            }

            //Colision
            if (mainCharacter.collideWithBSphere(boundingSphere))
            {
                mainCharacter.agregarBananas(1);
                TgcStaticSound sound = GameModel.instance.sonidos.banana_agarrar;
                sound.stop();
                sound.play();
                GameModel.instance.hud.efectoAgarrarBanana();
                this.enable = false;
            }
        }
    }
}
