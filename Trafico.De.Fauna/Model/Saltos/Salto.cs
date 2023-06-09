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
using Trafico.De.Fauna.Model.MainCharacters;

namespace Trafico.De.Fauna.Model.Saltos
{
    /// <summary>
    /// Salto entre dos plataformas
    /// </summary>
    public class Salto
    {
        Plataforma[] plataformas;

        public Salto(TgcBoundingAxisAlignBox bbPlataforma1, TgcBoundingAxisAlignBox bbPlataforma2)
        {
            plataformas = new Plataforma[2];

            Plataforma p1 = new Plataforma(bbPlataforma1, this, 0);
            Plataforma p2 = new Plataforma(bbPlataforma2, this, 1);

            p1.NextPlatform = p2;
            p2.NextPlatform = p1;

            plataformas[0] = p1;
            plataformas[1] = p2;
        }

        /// <summary>
        /// Ver si colisiona alguna plataforma
        /// </summary>
        public Plataforma checkCollision(MainCharacter mainCharacter)
        {
            if (checkCollisionAndAngle(mainCharacter, plataformas[0], plataformas[1]))
            {
                return plataformas[0];
            }
            if (checkCollisionAndAngle(mainCharacter, plataformas[1], plataformas[0]))
            {
                return plataformas[1];
            }
            return null;
        }

        /// <summary>
        /// Colision con BoundingBox de plataforma y consideracion de angulo de vision del personaje
        /// </summary>
        private bool checkCollisionAndAngle(MainCharacter mainCharacter, Plataforma p1, Plataforma p2)
        {
            //Colision con BoundingBox
            if (mainCharacter.collideWithAABB(p1.BoundingBox))
            {
                //Ver si el personaje esa mirando en un angulo tolerable hacia la prox plataforma
                TGCVector3 currentVecRot = mainCharacter.getCurrentVecRot();
                TGCVector3 plataformaDir = TGCVector3.Normalize(TGCVector3.Subtract(p2.Position, p1.Position));
                float dotAngle = TGCVector3.Dot(currentVecRot, plataformaDir);
                if (dotAngle > GameModel.instance.gameConfig.fisica.anguloSaltoDotValue)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public void dispose()
        {
            plataformas[0].dispose();
            plataformas[1].dispose();
            plataformas = null;
        }

        public void renderDebug()
        {
            plataformas[0].BoundingBox.Render();
            plataformas[1].BoundingBox.Render();
        }

        /// <summary>
        /// Plataforma individual de salto
        /// </summary>
        public class Plataforma
        {
            public Plataforma(TgcBoundingAxisAlignBox boundingBox, Salto salto, int index)
            {
                this.boundingBox = boundingBox;
                this.salto = salto;
                this.index = index;

                this.position = boundingBox.calculateBoxCenter();
                this.position.Y = boundingBox.PMin.Y;
            }

            TGCVector3 position;
            /// <summary>
            /// Posicion central en el piso de la plataforma
            /// </summary>
            public TGCVector3 Position
            {
                get { return position; }
            }

            TgcBoundingAxisAlignBox boundingBox;
            public TgcBoundingAxisAlignBox BoundingBox
            {
                get { return boundingBox; }
            }

            Salto salto;
            public Salto Salto
            {
                get { return salto; }
            }

            int index;
            public int Index
            {
                get { return index; }
            }

            Plataforma nextPlatform;
            /// <summary>
            /// La otra plataforma
            /// </summary>
            public Plataforma NextPlatform
            {
                get { return nextPlatform; }
                set { nextPlatform = value; }
            }

            public void dispose()
            {
                if (boundingBox != null)
                {
                    boundingBox.Dispose();
                    boundingBox = null;
                }

                salto = null;
                nextPlatform = null;
            }
        }

    }
}
