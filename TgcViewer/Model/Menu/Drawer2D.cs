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
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using Microsoft.DirectX;
using System.Drawing;
using TraficoFauna.Examples;

namespace Examples.TraficoFauna.Menu
{
    /// <summary>
    /// Draws sprites and primitives.
    /// </summary>
    public class Drawer2D
    {
        Device d3dDevice;
        Sprite DxSprite;
        Line line;

        CustomVertex.PositionColoredTextured[] LineVertexData = new CustomVertex.PositionColoredTextured[2];

        public Drawer2D(Device device)
        {
            DxSprite = new Sprite(device);
            line = new Line(device);
        }

        /// <summary>
        /// Call this method before drawing.
        /// </summary>
        public void BeginDrawSprite()
        {
            DxSprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthFrontToBack);
        }

        /// <summary>
        /// Call this method after drawing.
        /// </summary>
        public void EndDrawSprite()
        {
            DxSprite.End();
        }

        /// <summary>
        /// Draws a sprite on the screen.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        public void DrawSprite(GameSprite sprite)
        {
            if (sprite.Enabled)
            {
                DxSprite.Transform = sprite.TransformationMatrix;
                DxSprite.Draw(sprite.Texture.D3dTexture, sprite.SrcRect, Vector3.Empty, Vector3.Empty, sprite.Color);
            }
        }

        /// <summary>
        /// Dibujar un sprite animado
        /// </summary>
        /// <param name="animatedSprite">El sprite a dibujar</param>
        public void DrawAnimatedSprite(AnimatedSprite animatedSprite)
        {
            if (animatedSprite.Enabled)
            {
                animatedSprite.update(GameModel.instance.ElapsedTime);
                DrawSprite(animatedSprite.Sprite);
            }
        }

        /// <summary>
        /// Draws a sprite on the screen.
        /// With an atomic Begin and End Sprite
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        public void DrawSpriteAtomic(GameSprite sprite)
        {
            BeginDrawSprite();
            DrawSprite(sprite);
            EndDrawSprite();
        }

        /// <summary>
        /// Draws a colored point.
        /// </summary>
        /// <param name="position">The location.</param>
        /// <param name="color">The color.</param>
        public void DrawPoint(Vector2 position, Color color)
        {
            LineVertexData[0].X = position.X;
            LineVertexData[0].Y = position.Y;
            LineVertexData[0].Color = color.ToArgb();

            d3dDevice.VertexFormat = CustomVertex.PositionColoredTextured.Format;

            d3dDevice.DrawUserPrimitives(PrimitiveType.PointList, 1, LineVertexData);
        }

        /// <summary>
        /// Draws a line segment between two points.
        /// </summary>
        /// <param name="position1">The first point.</param>
        /// <param name="position2">The second point.</param>
        /// <param name="color">The color.</param>
        /// <param name="width">The width</param>
        /// <param name="antiAlias">Anti-alias enabled.</param>
        public void DrawLine(Vector2 position1, Vector2 position2, Color color, int width, bool antiAlias)
        {
            Vector2[] positionList = new Vector2[2] { position1, position2 };
            DrawPolyline(positionList, color, width, antiAlias);
        }

        /// <summary>
        /// Draws a polyline.
        /// </summary>
        /// <param name="positionList">The list of lines.</param>
        /// <param name="color">The color</param>
        /// <param name="width">The width</param>
        /// <param name="antiAlias">Anti-alias enabled.</param>
        public void DrawPolyline(Vector2[] positionList, Color color, int width, bool antiAlias)
        {

            line.Antialias = antiAlias;
            line.Width = width;
            line.Draw(positionList, color);

        }

    }

}
