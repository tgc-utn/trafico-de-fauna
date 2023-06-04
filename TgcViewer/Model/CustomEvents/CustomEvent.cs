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
using Examples.TraficoFauna.Meshes;
using Microsoft.DirectX.Direct3D;
using TGC.Core.SceneLoader;

namespace Examples.TraficoFauna.CustomEvents
{
    /// <summary>
    /// Evento personalizado que se ejecuta en un sector bajo sus propias condiciones.
    /// Se posiciona en el sector. Puede tener sus propias propiedades.
    /// </summary>
    public abstract class CustomEvent
    {
        bool enabled = true;
        /// <summary>
        /// Habilitar evento
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Creacion de instancia del evento
        /// </summary>
        public abstract void load(TgcMeshData meshData, string texturePath, TgcSceneData originalSceneData);

        /// <summary>
        /// Iniciar evento en sector
        /// </summary>
        public abstract void init();

        /// <summary>
        /// Finalizar evento al salir del sector
        /// </summary>
        public abstract void end();

        /// <summary>
        /// Actualizar estado del evento en cada cuadro
        /// </summary>
        public abstract void update(float elapsedTime);

        /// <summary>
        /// Dibujar evento. Se llama luego de update.
        /// </summary>
        public abstract void render();

        /// <summary>
        /// Dibujar en modo debug.
        /// </summary>
        public abstract void renderDebug();

        /// <summary>
        /// Devolver el objeto colisionable del CustomMesh para que 
        /// participe en la deteccion de colisiones del personaje.
        /// Si no necesita, devolver null.
        /// </summary>
        public abstract ColliderObject getColliderObject();

        /// <summary>
        /// Liberar todos los recursos
        /// </summary>
        public abstract void dispose();

    }
}
