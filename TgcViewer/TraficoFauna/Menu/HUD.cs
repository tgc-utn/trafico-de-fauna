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
using Examples.TraficoFauna.ItemsAnimal;
using TgcViewer.Utils._2D;
using System.Drawing;
using TraficoFauna.Examples;
using TgcViewer;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using Examples.TraficoFauna.Menu;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using System.Drawing.Text;
using Examples.TraficoFauna.Interpolators;

namespace Examples.TraficoFauna
{
    /// <summary>
    /// Controles visuales del juego
    /// </summary>
    public class HUD
    {
        static readonly Font FONT_COMIC_SANS_24 = new Font("Comic Sans MS", 24, FontStyle.Bold, GraphicsUnit.Pixel);
        public readonly Font GAME_FONT;
        readonly Vector2 SCREEN_CENTRAL_POS;

        TgcText2d textoContadorBananas;
        TgcText2d textoContadorItemsAnimal;
        TgcText2d textoContadorVidas;
        GameSprite spriteLlaveRoja;
        GameSprite spriteLlaveVerde;
        GameSprite spriteLlaveAmarilla;
        GameSprite spriteContadorBananas;
        GameSprite spriteBananaAgarrada;
        GameSprite spriteVariasBananasAgarradas;
        GameSprite spriteLife;
        GameSprite spriteLifeAgarrada;
        GameSprite spriteItemAnimal;
        GameSprite spriteItemAnimalLiberado;
        Drawer2D drawer2D;
        PrivateFontCollection privateFontCollection;
        Position2dInterpolator interpoladorBananaAgarrada;
        Position2dInterpolator interpoladorVariasBananasAgarradas;
        Position2dInterpolator interpoladorLifeAgarrada;
        Position2dInterpolator interpoladorItemAnimalLiberado;
        TgcText2d textoCurrentSectorName;


        public HUD()
        {
            int width = GuiController.Instance.Panel3d.Width;
            int height = GuiController.Instance.Panel3d.Height;
            SCREEN_CENTRAL_POS = new Vector2(width / 2, height / 2);

            string hudPath = Game.instance.gameConfig.menuPath + "HUD\\";
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            drawer2D = new Drawer2D();

            //Cargar fuentes personalizadas
            privateFontCollection = new PrivateFontCollection();
            privateFontCollection.AddFontFile(Game.instance.gameConfig.basePath + "Fonts\\JUNGLEFE.TTF");
            FontFamily[] families = privateFontCollection.Families;
            GAME_FONT = new Font(families[0], 24, FontStyle.Regular, GraphicsUnit.Pixel);


            //Sprite para contador de bananas
            spriteContadorBananas = new GameSprite();
            spriteContadorBananas.Texture = TgcTexture.createTexture(d3dDevice, hudPath + "hud_bananas.png");
            spriteContadorBananas.Position = new Vector2(2, 2);

            //Contador de bananas
            textoContadorBananas = new TgcText2d();
            textoContadorBananas.Color = Color.White;
            textoContadorBananas.Align = TgcText2d.TextAlign.LEFT;
            textoContadorBananas.Position = new Point(64 + 5, 20);
            textoContadorBananas.Size = new Size(200, 100);
            textoContadorBananas.Text = ": 0";
            textoContadorBananas.changeFont(GAME_FONT);

            //Sprite items animal liberados
            spriteItemAnimal = new GameSprite();
            spriteItemAnimal.Texture = TgcTexture.createTexture(d3dDevice, hudPath + "hud_items_animal.png");
            spriteItemAnimal.Position = new Vector2(width - 64 - 180, 2);

            //Contador de items animal liberados
            textoContadorItemsAnimal = new TgcText2d();
            textoContadorItemsAnimal.Color = Color.White;
            textoContadorItemsAnimal.Align = TgcText2d.TextAlign.LEFT;
            textoContadorItemsAnimal.Position = new Point(width - 160, 20);
            textoContadorItemsAnimal.Size = new Size(150, 100);
            textoContadorItemsAnimal.Text = ": 0";
            textoContadorItemsAnimal.changeFont(GAME_FONT);

            //Sprite para vidas
            spriteLife = new GameSprite();
            spriteLife.Texture = TgcTexture.createTexture(d3dDevice, hudPath + "hud_life.png");
            spriteLife.Position = new Vector2(width / 2 - 32, 2);

            //Contador de vidas del personaje
            textoContadorVidas = new TgcText2d();
            textoContadorVidas.Color = Color.White;
            textoContadorVidas.Align = TgcText2d.TextAlign.LEFT;
            textoContadorVidas.Position = new Point(width / 2 + 36, 20);
            textoContadorVidas.Size = new Size(100, 100);
            textoContadorVidas.Text = ": 0";
            textoContadorVidas.changeFont(GAME_FONT);



            //Sprite para cuando se agarra una banana
            spriteBananaAgarrada = new GameSprite();
            spriteBananaAgarrada.Texture = TgcTexture.createTexture(d3dDevice, hudPath + "hud_banana_agarrada.png");
            spriteBananaAgarrada.Position = spriteContadorBananas.Position;
            spriteBananaAgarrada.Enabled = false;
            spriteVariasBananasAgarradas = new GameSprite();
            spriteVariasBananasAgarradas.Texture = TgcTexture.createTexture(d3dDevice, hudPath + "hud_varias_bananas_agarradas.png");
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
            spriteLifeAgarrada.Texture = TgcTexture.createTexture(d3dDevice, hudPath + "hud_life.png");
            spriteLifeAgarrada.Position = spriteLife.Position;
            spriteLifeAgarrada.Enabled = false;

            //Interpolador vida agarrada
            interpoladorLifeAgarrada = new Position2dInterpolator();
            interpoladorLifeAgarrada.End = spriteLife.Position;
            interpoladorLifeAgarrada.Speed = 400f;


            //Sprite para cuando se libera un item animal
            spriteItemAnimalLiberado = new GameSprite();
            spriteItemAnimalLiberado.Texture = TgcTexture.createTexture(d3dDevice, hudPath + "hud_items_animal.png");
            spriteItemAnimalLiberado.Position = spriteLife.Position;
            spriteItemAnimalLiberado.Enabled = false;

            //Interpolador vida agarrada
            interpoladorItemAnimalLiberado = new Position2dInterpolator();
            interpoladorItemAnimalLiberado.End = spriteItemAnimal.Position;
            interpoladorItemAnimalLiberado.Speed = 1000f;


            //Sprite de llave roja
            spriteLlaveRoja = new GameSprite();
            spriteLlaveRoja.Texture = TgcTexture.createTexture(d3dDevice, hudPath + "hud_llave_roja.png");
            spriteLlaveRoja.Position = new Vector2(width - 70, 100);

            //Sprite de llave verde
            spriteLlaveVerde = new GameSprite();
            spriteLlaveVerde.Texture = TgcTexture.createTexture(d3dDevice, hudPath + "hud_llave_verde.png");
            spriteLlaveVerde.Position = new Vector2(width - 70, 140);

            //Sprite de llave amarilla
            spriteLlaveAmarilla = new GameSprite();
            spriteLlaveAmarilla.Texture = TgcTexture.createTexture(d3dDevice, hudPath + "hud_llave_amarilla.png");
            spriteLlaveAmarilla.Position = new Vector2(width - 70, 180);



            //Texto con el nombre del sector actual
            textoCurrentSectorName = new TgcText2d();
            textoCurrentSectorName.Color = Color.White;
            textoCurrentSectorName.Align = TgcText2d.TextAlign.LEFT;
            textoCurrentSectorName.Position = new Point(50, height - 50);
            textoCurrentSectorName.Size = new Size(500, 50);
            textoCurrentSectorName.Text = ": 0";
            textoCurrentSectorName.changeFont(GAME_FONT);
        }

        /// <summary>
        /// Actualizar estado de HUD
        /// </summary>
        public void update()
        {
            //Salir al menu
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            if (d3dInput.keyPressed(Key.Escape))
            {
                Game.instance.currentGameMode = Game.GameMode.Menu;
            }


            MainCharacter mainCharacter = Game.instance.mainCharacter;

            //Contador de bananas
            textoContadorBananas.Text = ": " + mainCharacter.BananasAgarradas;

            //Bananas agarradas
            if (spriteBananaAgarrada.Enabled)
            {
                spriteBananaAgarrada.Position = interpoladorBananaAgarrada.update();
                if (interpoladorBananaAgarrada.isEnd())
                {
                    spriteBananaAgarrada.Enabled = false;
                }
            }
            if (spriteVariasBananasAgarradas.Enabled)
            {
                spriteVariasBananasAgarradas.Position = interpoladorVariasBananasAgarradas.update();
                if (interpoladorVariasBananasAgarradas.isEnd())
                {
                    spriteVariasBananasAgarradas.Enabled = false;
                }
            }

            //Vida agarrada
            if (spriteLifeAgarrada.Enabled)
            {
                spriteLifeAgarrada.Position = interpoladorLifeAgarrada.update();
                if (interpoladorLifeAgarrada.isEnd())
                {
                    spriteLifeAgarrada.Enabled = false;
                }
            }
            //Item animal liberado
            if (spriteItemAnimalLiberado.Enabled)
            {
                spriteItemAnimalLiberado.Position = interpoladorItemAnimalLiberado.update();
                if (interpoladorItemAnimalLiberado.isEnd())
                {
                    spriteItemAnimalLiberado.Enabled = false;
                }
            }


            //Contador de items animal liberados
            textoContadorItemsAnimal.Text = ": " + mainCharacter.ItemsAnimalLiberados + "/" + Game.instance.currentLevel.TotalItemsAnimal;

            //Vidas
            textoContadorVidas.Text = ": " + mainCharacter.Vidas;

            //Nombre de sector
            textoCurrentSectorName.Text = Game.instance.currentLevel.CurrentSector.DisplaySectorName;
        }

        /// <summary>
        /// Dibujar HUD
        /// </summary>
        public void render()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            //textos
            textoContadorBananas.render();
            textoContadorItemsAnimal.render();
            textoContadorVidas.render();

            //Sprites
            drawer2D.BeginDrawSprite();

            //Dibujar las llaves agarradas
            Level level = Game.instance.currentLevel;
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
            textoCurrentSectorName.render();

            drawer2D.EndDrawSprite();
        }


        /// <summary>
        /// Cuando se intenta liberar un Item Animal sin tener su llave aun
        /// </summary>
        public void efectoFaltaLlaveParaLiberarItem(ItemAnimal item)
        {

        }

        /// <summary>
        /// Cuando se libera satisfactoriamente un Item Animal
        /// </summary>
        public void efectoLiberarItem(ItemAnimal item)
        {
            spriteItemAnimalLiberado.Enabled = true;
            interpoladorItemAnimalLiberado.Init = SCREEN_CENTRAL_POS;
            interpoladorItemAnimalLiberado.reset();
        }

        /// <summary>
        /// Efecto de agarrar una banana
        /// </summary>
        public void efectoAgarrarBanana()
        {
            spriteBananaAgarrada.Enabled = true;
            interpoladorBananaAgarrada.Init = SCREEN_CENTRAL_POS;
            interpoladorBananaAgarrada.reset();
        }

        /// <summary>
        /// Efecto de agarrar muchas bananas juntas de una caja de bananas
        /// </summary>
        public void efectoAgarrarVariasBananas(int cantidadBananas)
        {
            spriteVariasBananasAgarradas.Enabled = true;
            interpoladorVariasBananasAgarradas.Init = SCREEN_CENTRAL_POS;
            interpoladorVariasBananasAgarradas.reset();
        }

        /// <summary>
        /// Efecto de ganar una vida
        /// </summary>
        public void efectoGanarVida()
        {
            spriteLifeAgarrada.Enabled = true;
            interpoladorLifeAgarrada.Init = SCREEN_CENTRAL_POS;
            interpoladorLifeAgarrada.reset();
        }
    }
}
