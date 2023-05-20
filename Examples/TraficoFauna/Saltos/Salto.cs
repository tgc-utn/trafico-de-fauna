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
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using TraficoFauna.Examples;

namespace Examples.TraficoFauna.Saltos
{
    /// <summary>
    /// Salto entre dos plataformas
    /// </summary>
    public class Salto
    {
        Plataforma[] plataformas;

        public Salto(TgcBoundingBox bbPlataforma1, TgcBoundingBox bbPlataforma2)
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
                Vector3 currentVecRot = mainCharacter.getCurrentVecRot();
                Vector3 plataformaDir = Vector3.Normalize(Vector3.Subtract(p2.Position, p1.Position));
                float dotAngle = Vector3.Dot(currentVecRot, plataformaDir);
                if (dotAngle > Game.instance.gameConfig.fisica.anguloSaltoDotValue)
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
            plataformas[0].BoundingBox.render();
            plataformas[1].BoundingBox.render();
        }

        /// <summary>
        /// Plataforma individual de salto
        /// </summary>
        public class Plataforma
        {
            public Plataforma(TgcBoundingBox boundingBox, Salto salto, int index)
            {
                this.boundingBox = boundingBox;
                this.salto = salto;
                this.index = index;

                this.position = boundingBox.calculateBoxCenter();
                this.position.Y = boundingBox.PMin.Y;
            }

            Vector3 position;
            /// <summary>
            /// Posicion central en el piso de la plataforma
            /// </summary>
            public Vector3 Position
            {
                get { return position; }
            }

            TgcBoundingBox boundingBox;
            public TgcBoundingBox BoundingBox
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
                    boundingBox.dispose();
                    boundingBox = null;
                }

                salto = null;
                nextPlatform = null;
            }
        }

    }
}
