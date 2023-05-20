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

namespace TgcViewer.Example
{
    public abstract class TgcExample
    {
        /// <summary>
        /// Categoría que agrupa a este ejemplo
        /// </summary>
        public abstract string getCategory();

        /// <summary>
        /// Nombre corto del ejemplo
        /// </summary>
        public abstract string getName();

        /// <summary>
        /// Descripción del ejemplo
        /// </summary>
        public abstract string getDescription();

        /// <summary>
        /// Se llama cuando el ejemplo es elegido para ejecutar.
        /// Inicializar todos los recursos y configuraciones que se van a utilizar.
        /// </summary>
        public abstract void init();

        /// <summary>
        /// Se llama cuando el ejemplo es cerrado.
        /// Liberar todos los recursos utilizados.
        /// </summary>
        public abstract void close();

        /// <summary>
        /// Se llama para renderizar cada cuadro del ejemplo.
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public abstract void render(float elapsedTime);


        private string exampleDir;
        /// <summary>
        /// Path absoluto del directorio en donde se encuentra el ejemplo
        /// </summary>
        public string ExampleDir
        {
            get { return exampleDir; }
        }

        internal void setExampleDir(string path)
        {
            this.exampleDir = path;
        }
    }
}
