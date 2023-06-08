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
using TgcViewer.Model.MainCharacters;

namespace TgcViewer.Model.Saltos
{
    public class SaltosManager
    {

        List<Salto> saltos;
        /// <summary>
        /// Saltos
        /// </summary>
        public List<Salto> Saltos
        {
            get { return saltos; }
        }

        private List<SaltoNode> nodos;

        public SaltosManager()
        {
            nodos = new List<SaltoNode>();
            saltos = new List<Salto>();
        }

        /// <summary>
        /// Cargar una plataforma de salto individual
        /// </summary>
        public void addNode(TgcMeshData meshDataNode)
        {
            //Parsear: SaltoXXX_1 y SaltoXXX_2 (NOMBRE_INDICE)
            string[] split = meshDataNode.name.Split('_');
            if (split.Length != 2)
            {
                throw new Exception("Nombre de Salto incorrecto: " + meshDataNode.name);
            }
            int index = -1;
            if (!int.TryParse(split[1], out index))
            {
                throw new Exception("Indice del nombre del Salto incorrecto: " + meshDataNode.name);
            }
            if (index != 1 && index != 2)
            {
                throw new Exception("El indice del nombre del Salto debe ser 1 o 2: " + meshDataNode.name);
            }

            //Crear nodo
            SaltoNode node = new SaltoNode();
            node.mesh = GameUtils.createBBFromMeshData(meshDataNode);
            node.name = split[0].ToUpper();
            node.index = index;
            nodos.Add(node);
        }

        /// <summary>
        /// Organizar plataformas individuales en saltos de a pares
        /// </summary>
        public void arrangeNodes()
        {
            //Agrupar todos los nodos en tuplas de a dos
            Dictionary<string, SaltoNode[]> names = new Dictionary<string, SaltoNode[]>();
            foreach (SaltoNode node in nodos)
            {
                if (names.ContainsKey(node.name))
                {
                    names[node.name][node.index - 1] = node;
                }
                else
                {
                    SaltoNode[] tupla = new SaltoNode[2];
                    tupla[node.index - 1] = node;
                    names[node.name] = tupla;
                }
            }

            
            //Crear saltos definitivos
            foreach (KeyValuePair<string, SaltoNode[]> entry in names)
            {
                //Chequear incompletos
                SaltoNode[] tupla = entry.Value;
                if (tupla[0] == null || tupla[1] == null)
                {
                    throw new Exception("A un nodo de Salto le falta su compañero. Nombre del salto: " + entry.Key);
                }

                //Crear salto
                Salto salto = new Salto(tupla[0].mesh, tupla[1].mesh);
                this.saltos.Add(salto);
            }

            //Tirar nodos auxiliares
            nodos.Clear();
            nodos = null;
        }



        public void dispose()
        {
            foreach (Salto s in saltos)
            {
                s.dispose();
            }
            saltos.Clear();
            saltos = null;
        }


        /// <summary>
        /// Estructura auxiliar para ir cargando plataformas de salto y luego acomodarlos
        /// </summary>
        class SaltoNode
        {
            public TgcBoundingAxisAlignBox mesh;
            public string name;
            public int index;
        }


        /// <summary>
        /// Ver si hay colision con alguna plataforma
        /// </summary>
        public Salto.Plataforma checkSaltos(MainCharacter mainCharacter)
        {
            foreach (Salto s in saltos)
            {
                Salto.Plataforma p = s.checkCollision(mainCharacter);
                if (p != null)
                {
                    return p;
                }
            }
            return null;
        }
    }
}
