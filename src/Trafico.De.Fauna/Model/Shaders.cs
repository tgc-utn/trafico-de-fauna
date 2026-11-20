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
using Microsoft.DirectX.Direct3D;
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model
{
    /// <summary>
    ///     Utilidad para shaders del juego
    /// </summary>
    public class Shaders
    {
        public readonly string ANIMATE_UV = "AnimateUVTechnique";
        public readonly string DEFAULT = "DefaultTechnique";
        public readonly string LIGHTMAP = "LightmapTechnique";

        public readonly string ONLY_COLOR = "OnlyColorTechnique";

        //public readonly string DEFAULT = "PhongTechnique";
        public readonly string RESALTADO = "ResaltadoTechnique";


        /// <summary>
        ///     Shader principal
        /// </summary>
        public Effect effect;

        /// <summary>
        ///     Shader para particulas
        /// </summary>
        public ParticlesShaders particles;


        public Shaders(Device device)
        {
            //Cargar Shader de StaticMesh
            string compilationErrors;
            effect = Effect.FromFile(device, GameModel.instance.gameConfig.shadersPath + "StaticMeshShader.fx",
                null, null, ShaderFlags.None, null, out compilationErrors);
            if (effect == null)
            {
                throw new Exception("Error al cargar shader. Errores: " + compilationErrors);
            }

            //Configurar Technique
            effect.Technique = DEFAULT;


            //Cargar efectos para particulas
            //particles = new ParticlesShaders();
        }

        /// <summary>
        ///     Carga el Technique default correspondiente segun el tipo de formato de mesh
        /// </summary>
        public void setCorrectTechnique(TgcMeshShader mesh)
        {
            switch (mesh.RenderType)
            {
                case TgcMesh.MeshRenderType.VERTEX_COLOR:
                    mesh.Technique = ONLY_COLOR;
                    break;
                case TgcMesh.MeshRenderType.DIFFUSE_MAP:
                    mesh.Technique = DEFAULT;
                    break;
                case TgcMesh.MeshRenderType.DIFFUSE_MAP_AND_LIGHTMAP:
                    mesh.Technique = LIGHTMAP;
                    break;
            }
        }

        /// <summary>
        ///     Shader para efectos de particulas
        /// </summary>
        public class ParticlesShaders
        {
            public readonly string ALPHA_EXIT = "AlphaExitTechnique";
            public readonly string DEFAULT = "DefaultTechnique";

            /// <summary>
            ///     Shader principal
            /// </summary>
            public Effect effect;

            public ParticlesShaders(Device device)
            {
                //Cargar Shader de StaticMesh
                string compilationErrors;
                effect = Effect.FromFile(device, GameModel.instance.gameConfig.shadersPath + "ParticlesShader.fx",
                    null, null, ShaderFlags.None, null, out compilationErrors);
                if (effect == null)
                {
                    throw new Exception("Error al cargar shader. Errores: " + compilationErrors);
                }

                //Configurar Technique
                effect.Technique = DEFAULT;
            }
        }
    }
}
