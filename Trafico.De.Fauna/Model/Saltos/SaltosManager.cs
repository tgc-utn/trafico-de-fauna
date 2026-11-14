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
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.MainCharacters;

namespace Trafico.De.Fauna.Model.Saltos
{
    public class SaltosManager
    {
        private List<SaltoNode> nodos;

        public SaltosManager()
        {
            nodos = new List<SaltoNode>();
            Saltos = new List<Salto>();
        }

        /// <summary>
        ///     Saltos
        /// </summary>
        public List<Salto> Saltos { get; private set; }

        /// <summary>
        ///     Cargar una plataforma de salto individual
        /// </summary>
        public void addNode(TgcMeshData meshDataNode)
        {
            //Parsear: SaltoXXX_1 y SaltoXXX_2 (NOMBRE_INDICE)
            var split = meshDataNode.name.Split('_');
            if (split.Length != 2)
            {
                throw new Exception("Nombre de Salto incorrecto: " + meshDataNode.name);
            }

            var index = -1;
            if (!int.TryParse(split[1], out index))
            {
                throw new Exception("Indice del nombre del Salto incorrecto: " + meshDataNode.name);
            }

            if (index != 1 && index != 2)
            {
                throw new Exception("El indice del nombre del Salto debe ser 1 o 2: " + meshDataNode.name);
            }

            //Crear nodo
            var node = new SaltoNode();
            node.mesh = GameUtils.createBBFromMeshData(meshDataNode);
            node.name = split[0].ToUpper();
            node.index = index;
            nodos.Add(node);
        }

        /// <summary>
        ///     Organizar plataformas individuales en saltos de a pares
        /// </summary>
        public void arrangeNodes()
        {
            //Agrupar todos los nodos en tuplas de a dos
            var names = new Dictionary<string, SaltoNode[]>();
            foreach (var node in nodos)
            {
                if (names.ContainsKey(node.name))
                {
                    names[node.name][node.index - 1] = node;
                }
                else
                {
                    var tupla = new SaltoNode[2];
                    tupla[node.index - 1] = node;
                    names[node.name] = tupla;
                }
            }


            //Crear saltos definitivos
            foreach (var entry in names)
            {
                //Chequear incompletos
                var tupla = entry.Value;
                if (tupla[0] == null || tupla[1] == null)
                {
                    throw new Exception("A un nodo de Salto le falta su compañero. Nombre del salto: " + entry.Key);
                }

                //Crear salto
                var salto = new Salto(tupla[0].mesh, tupla[1].mesh);
                Saltos.Add(salto);
            }

            //Tirar nodos auxiliares
            nodos.Clear();
            nodos = null;
        }


        public void dispose()
        {
            foreach (var s in Saltos)
            {
                s.dispose();
            }

            Saltos.Clear();
            Saltos = null;
        }


        /// <summary>
        ///     Ver si hay colision con alguna plataforma
        /// </summary>
        public Salto.Plataforma checkSaltos(MainCharacter mainCharacter)
        {
            foreach (var s in Saltos)
            {
                var p = s.checkCollision(mainCharacter);
                if (p != null)
                {
                    return p;
                }
            }

            return null;
        }


        /// <summary>
        ///     Estructura auxiliar para ir cargando plataformas de salto y luego acomodarlos
        /// </summary>
        private class SaltoNode
        {
            public int index;
            public TgcBoundingAxisAlignBox mesh;
            public string name;
        }
    }
}
