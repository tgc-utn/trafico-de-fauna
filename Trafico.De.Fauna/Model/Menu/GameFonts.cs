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
using System.Drawing.Text;

namespace Trafico.De.Fauna.Model.Menu
{
    /// <summary>
    ///     Carga las fuentes del juego (JungleFever y Spatha) desde Media\Fonts y crea, en el tamaño pedido, las
    ///     fuentes con las que se dibujan los textos del HUD y de los dialogos.
    ///     Las colecciones cargadas se mantienen en campos estaticos durante toda la ejecucion: Direct3D crea sus
    ///     fuentes por nombre a traves de GDI y, desde .NET Framework 4.7.2, una PrivateFontCollection liberada por
    ///     el GC des-registra la fuente de GDI, con lo cual el texto pasa a dibujarse con una fuente generica.
    /// </summary>
    public static class GameFonts
    {
        private static PrivateFontCollection jungleFeverCollection;
        private static PrivateFontCollection spathaCollection;

        /// <summary>
        ///     Fuente JungleFever, usada en el HUD y en los mensajes simples
        /// </summary>
        public static Font JungleFever(float size)
        {
            if (jungleFeverCollection == null)
            {
                jungleFeverCollection = Load("Fonts\\JUNGLEFE.TTF");
            }
            return Create(jungleFeverCollection, size);
        }

        /// <summary>
        ///     Fuente Spatha, usada en los dialogos de texto
        /// </summary>
        public static Font Spatha(float size)
        {
            if (spathaCollection == null)
            {
                spathaCollection = Load("Fonts\\spathasans.ttf");
            }
            return Create(spathaCollection, size);
        }

        private static PrivateFontCollection Load(string relativePath)
        {
            var collection = new PrivateFontCollection();
            collection.AddFontFile(GameModel.instance.gameConfig.basePath + relativePath);
            return collection;
        }

        private static Font Create(PrivateFontCollection collection, float size)
        {
            return new Font(collection.Families[0], size, FontStyle.Regular, GraphicsUnit.Pixel);
        }
    }
}
