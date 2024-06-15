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
using Microsoft.DirectX.DirectInput;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Core.Textures;
using Trafico.De.Fauna.Model.Interpolators;
using Trafico.De.Fauna.Model.ItemsAnimal;
using Device = Microsoft.DirectX.Direct3D.Device;
using SystemFont = System.Drawing.Font;

namespace Trafico.De.Fauna.Model.Menu
{
    /// <summary>
    ///     Controles visuales del juego
    /// </summary>
    public class HUD
    {
        private readonly Drawer2D drawer2D;
        private readonly Position2dInterpolator interpoladorBananaAgarrada;
        private readonly Position2dInterpolator interpoladorItemAnimalLiberado;
        private readonly Position2dInterpolator interpoladorLifeAgarrada;
        private readonly Position2dInterpolator interpoladorVariasBananasAgarradas;
        private readonly TGCVector2 SCREEN_CENTRAL_POS;
        private readonly GameSprite spriteBananaAgarrada;
        private readonly GameSprite spriteContadorBananas;
        private readonly GameSprite spriteItemAnimal;
        private readonly GameSprite spriteItemAnimalLiberado;
        private readonly GameSprite spriteLife;
        private readonly GameSprite spriteLifeAgarrada;
        private readonly GameSprite spriteLlaveAmarilla;
        private readonly GameSprite spriteLlaveRoja;
        private readonly GameSprite spriteLlaveVerde;
        private readonly GameSprite spriteVariasBananasAgarradas;

        private readonly TGCText2D textoContadorBananas;
        private readonly TGCText2D textoContadorItemsAnimal;
        private readonly TGCText2D textoContadorVidas;
        private readonly TGCText2D textoCurrentSectorName;

        public HUD(Device device)
        {
            var width = device.Viewport.Width;
            var height = device.Viewport.Height;
            SCREEN_CENTRAL_POS = new TGCVector2(width / 2, height / 2);

            var hudPath = GameModel.instance.gameConfig.menuPath + "HUD\\";
            drawer2D = new Drawer2D(device);

            //Cargar fuente
            var font = new SystemFont("JungleFever", 20, FontStyle.Regular, GraphicsUnit.Pixel);

            //Sprite para contador de bananas
            spriteContadorBananas = new GameSprite();
            spriteContadorBananas.Texture = TGCTexture.CreateTexture(device, hudPath + "hud_bananas.png");
            spriteContadorBananas.Position = new TGCVector2(2, 2);

            //Contador de bananas
            textoContadorBananas = new TGCText2D();
            textoContadorBananas.Color = Color.White;
            textoContadorBananas.Align = TGCText2D.TextAlign.LEFT;
            textoContadorBananas.Position = new Point(64 + 5, 20);
            textoContadorBananas.Size = new Size(200, 100);
            textoContadorBananas.Text = ": 0";
            textoContadorBananas.changeFont(font);

            //Sprite items animal liberados
            spriteItemAnimal = new GameSprite();
            spriteItemAnimal.Texture = TGCTexture.CreateTexture(device, hudPath + "hud_items_animal.png");
            spriteItemAnimal.Position = new TGCVector2(width - 64 - 180, 2);

            //Contador de items animal liberados
            textoContadorItemsAnimal = new TGCText2D();
            textoContadorItemsAnimal.Color = Color.White;
            textoContadorItemsAnimal.Align = TGCText2D.TextAlign.LEFT;
            textoContadorItemsAnimal.Position = new Point(width - 160, 20);
            textoContadorItemsAnimal.Size = new Size(150, 100);
            textoContadorItemsAnimal.Text = ": 0";
            textoContadorItemsAnimal.changeFont(font);

            //Sprite para vidas
            spriteLife = new GameSprite();
            spriteLife.Texture = TGCTexture.CreateTexture(device, hudPath + "hud_life.png");
            spriteLife.Position = new TGCVector2(width / 2 - 32, 2);

            //Contador de vidas del personaje
            textoContadorVidas = new TGCText2D();
            textoContadorVidas.Color = Color.White;
            textoContadorVidas.Align = TGCText2D.TextAlign.LEFT;
            textoContadorVidas.Position = new Point(width / 2 + 36, 20);
            textoContadorVidas.Size = new Size(100, 100);
            textoContadorVidas.Text = ": 0";
            textoContadorVidas.changeFont(font);

            //Sprite para cuando se agarra una banana
            spriteBananaAgarrada = new GameSprite();
            spriteBananaAgarrada.Texture = TGCTexture.CreateTexture(device, hudPath + "hud_banana_agarrada.png");
            spriteBananaAgarrada.Position = spriteContadorBananas.Position;
            spriteBananaAgarrada.Enabled = false;
            spriteVariasBananasAgarradas = new GameSprite();
            spriteVariasBananasAgarradas.Texture =
                TGCTexture.CreateTexture(device, hudPath + "hud_varias_bananas_agarradas.png");
            spriteVariasBananasAgarradas.Position = spriteContadorBananas.Position;
            spriteVariasBananasAgarradas.Enabled = false;

            //Interpolador banana agarrada
            interpoladorBananaAgarrada = new Position2dInterpolator();
            interpoladorBananaAgarrada.End = spriteContadorBananas.Position;
            interpoladorBananaAgarrada.Speed = 1500f;
            interpoladorVariasBananasAgarradas = new Position2dInterpolator();
            interpoladorVariasBananasAgarradas.End = spriteContadorBananas.Position;
            interpoladorVariasBananasAgarradas.Speed = 1500f;

            //Sprite para cuando se agarra una vida
            spriteLifeAgarrada = new GameSprite();
            spriteLifeAgarrada.Texture = TGCTexture.CreateTexture(device, hudPath + "hud_life.png");
            spriteLifeAgarrada.Position = spriteLife.Position;
            spriteLifeAgarrada.Enabled = false;

            //Interpolador vida agarrada
            interpoladorLifeAgarrada = new Position2dInterpolator();
            interpoladorLifeAgarrada.End = spriteLife.Position;
            interpoladorLifeAgarrada.Speed = 400f;

            //Sprite para cuando se libera un item animal
            spriteItemAnimalLiberado = new GameSprite();
            spriteItemAnimalLiberado.Texture = TGCTexture.CreateTexture(device, hudPath + "hud_items_animal.png");
            spriteItemAnimalLiberado.Position = spriteLife.Position;
            spriteItemAnimalLiberado.Enabled = false;

            //Interpolador vida agarrada
            interpoladorItemAnimalLiberado = new Position2dInterpolator();
            interpoladorItemAnimalLiberado.End = spriteItemAnimal.Position;
            interpoladorItemAnimalLiberado.Speed = 1000f;

            //Sprite de llave roja
            spriteLlaveRoja = new GameSprite();
            spriteLlaveRoja.Texture = TGCTexture.CreateTexture(device, hudPath + "hud_llave_roja.png");
            spriteLlaveRoja.Position = new TGCVector2(width - 70, 100);

            //Sprite de llave verde
            spriteLlaveVerde = new GameSprite();
            spriteLlaveVerde.Texture = TGCTexture.CreateTexture(device, hudPath + "hud_llave_verde.png");
            spriteLlaveVerde.Position = new TGCVector2(width - 70, 140);

            //Sprite de llave amarilla
            spriteLlaveAmarilla = new GameSprite();
            spriteLlaveAmarilla.Texture = TGCTexture.CreateTexture(device, hudPath + "hud_llave_amarilla.png");
            spriteLlaveAmarilla.Position = new TGCVector2(width - 70, 180);

            //Texto con el nombre del sector actual
            textoCurrentSectorName = new TGCText2D();
            textoCurrentSectorName.Color = Color.White;
            textoCurrentSectorName.Align = TGCText2D.TextAlign.LEFT;
            textoCurrentSectorName.Position = new Point(50, height - 50);
            textoCurrentSectorName.Size = new Size(500, 50);
            textoCurrentSectorName.Text = ": 0";
            textoCurrentSectorName.changeFont(font);
        }

        /// <summary>
        ///     Actualizar estado de HUD
        /// </summary>
        public void update(float elapsedTime)
        {
            //Salir al menu
            var d3dInput = GameModel.instance.Input;
            if (d3dInput.keyPressed(Key.Escape))
            {
                GameModel.instance.currentGameMode = GameMode.Menu;
            }

            var mainCharacter = GameModel.instance.mainCharacter;

            //Contador de bananas
            textoContadorBananas.Text = ": " + mainCharacter.BananasAgarradas;

            //Bananas agarradas
            if (spriteBananaAgarrada.Enabled)
            {
                spriteBananaAgarrada.Position = interpoladorBananaAgarrada.update(elapsedTime);
                if (interpoladorBananaAgarrada.isEnd())
                {
                    spriteBananaAgarrada.Enabled = false;
                }
            }

            if (spriteVariasBananasAgarradas.Enabled)
            {
                spriteVariasBananasAgarradas.Position = interpoladorVariasBananasAgarradas.update(elapsedTime);
                if (interpoladorVariasBananasAgarradas.isEnd())
                {
                    spriteVariasBananasAgarradas.Enabled = false;
                }
            }

            //Vida agarrada
            if (spriteLifeAgarrada.Enabled)
            {
                spriteLifeAgarrada.Position = interpoladorLifeAgarrada.update(elapsedTime);
                if (interpoladorLifeAgarrada.isEnd())
                {
                    spriteLifeAgarrada.Enabled = false;
                }
            }

            //Item animal liberado
            if (spriteItemAnimalLiberado.Enabled)
            {
                spriteItemAnimalLiberado.Position = interpoladorItemAnimalLiberado.update(elapsedTime);
                if (interpoladorItemAnimalLiberado.isEnd())
                {
                    spriteItemAnimalLiberado.Enabled = false;
                }
            }

            //Contador de items animal liberados
            textoContadorItemsAnimal.Text = ": " + mainCharacter.ItemsAnimalLiberados + "/" +
                                            GameModel.instance.currentLevel.TotalItemsAnimal;

            //Vidas
            textoContadorVidas.Text = ": " + mainCharacter.Vidas;

            //Nombre de sector
            textoCurrentSectorName.Text = GameModel.instance.currentLevel.CurrentSector.DisplaySectorName;
        }

        /// <summary>
        ///     Dibujar HUD
        /// </summary>
        public void render(Device device)
        {
            //textos
            textoContadorBananas.Render();
            textoContadorItemsAnimal.Render();
            textoContadorVidas.Render();

            //Sprites
            drawer2D.BeginDrawSprite();

            //Dibujar las llaves agarradas
            var level = GameModel.instance.currentLevel;
            Llave llave;

            llave = level.getLlave(Llave.ColorType.ROJA);
            if (llave != null && llave.Agarrada)
            {
                drawer2D.DrawSprite(spriteLlaveRoja);
            }

            llave = level.getLlave(Llave.ColorType.AMARILLA);
            if (llave != null && llave.Agarrada)
            {
                drawer2D.DrawSprite(spriteLlaveAmarilla);
            }

            llave = level.getLlave(Llave.ColorType.VERDE);
            if (llave != null && llave.Agarrada)
            {
                drawer2D.DrawSprite(spriteLlaveVerde);
            }

            //Contadores de Bananas, vidas e items animal
            drawer2D.DrawSprite(spriteContadorBananas);
            drawer2D.DrawSprite(spriteLife);
            drawer2D.DrawSprite(spriteItemAnimal);

            //animacion de bananas agarradas
            drawer2D.DrawSprite(spriteBananaAgarrada);
            drawer2D.DrawSprite(spriteVariasBananasAgarradas);

            //animacion de vidas agarradas
            drawer2D.DrawSprite(spriteLifeAgarrada);

            //animacion de items animal liberados
            drawer2D.DrawSprite(spriteItemAnimalLiberado);

            //Nombre de sector actual
            textoCurrentSectorName.Render();

            drawer2D.EndDrawSprite();
        }

        /// <summary>
        ///     Cuando se intenta liberar un Item Animal sin tener su llave aun
        /// </summary>
        public void efectoFaltaLlaveParaLiberarItem(ItemAnimal item)
        {
        }

        /// <summary>
        ///     Cuando se libera satisfactoriamente un Item Animal
        /// </summary>
        public void efectoLiberarItem(ItemAnimal item)
        {
            spriteItemAnimalLiberado.Enabled = true;
            interpoladorItemAnimalLiberado.Init = SCREEN_CENTRAL_POS;
            interpoladorItemAnimalLiberado.reset();
        }

        /// <summary>
        ///     Efecto de agarrar una banana
        /// </summary>
        public void efectoAgarrarBanana()
        {
            spriteBananaAgarrada.Enabled = true;
            interpoladorBananaAgarrada.Init = SCREEN_CENTRAL_POS;
            interpoladorBananaAgarrada.reset();
        }

        /// <summary>
        ///     Efecto de agarrar muchas bananas juntas de una caja de bananas
        /// </summary>
        public void efectoAgarrarVariasBananas(int cantidadBananas)
        {
            spriteVariasBananasAgarradas.Enabled = true;
            interpoladorVariasBananasAgarradas.Init = SCREEN_CENTRAL_POS;
            interpoladorVariasBananasAgarradas.reset();
        }

        /// <summary>
        ///     Efecto de ganar una vida
        /// </summary>
        public void efectoGanarVida()
        {
            spriteLifeAgarrada.Enabled = true;
            interpoladorLifeAgarrada.Init = SCREEN_CENTRAL_POS;
            interpoladorLifeAgarrada.reset();
        }
    }
}
