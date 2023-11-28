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
using System.IO;
using System.Xml;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Mathematica;
using TGC.Core.Utils;

namespace Trafico.De.Fauna.Model
{
    /// <summary>
    /// Configuracion de todo el juego
    /// </summary>
    public class GameConfig
    {

        //atributos simples
        public string basePath;
        public string levelsPath;
        public string shadersPath;
        public string sonidosPath;
        public string menuPath;

        //atributos compuestos
        public List<Level> levels;
        public Fisica fisica;
        public Camara camara;
        public Guardia guardia;
        public Keys keys;
        public bool debugModeEnabled;

        public GameConfig(string mediaPath, string fileName, string shaderPath)
        {
            //paths
            this.basePath = mediaPath;
            this.levelsPath = mediaPath + "Levels\\";
            this.shadersPath = shaderPath;
            this.sonidosPath = mediaPath + "Sonidos\\";
            this.menuPath = mediaPath + "Menu\\";

            //init
            levels = new List<Level>();
            fisica = new Fisica();
            camara = new Camara();
            guardia = new Guardia();
            keys = new Keys();

            //Parsear xml
            string xmlString = File.ReadAllText(mediaPath + fileName);
            XmlDocument dom = new XmlDocument();
            dom.LoadXml(xmlString);
            XmlElement root = dom.DocumentElement;

            //Levels
            XmlElement levelsNode = (XmlElement)root.GetElementsByTagName("levels")[0];
            int levelIndex = 0;
            foreach (XmlNode levelNode in levelsNode.ChildNodes)
            {
                if (levelNode.Attributes != null)
                {
                    Level level = new Level();
                    level.name = levelNode.Attributes["name"].Value;
                    level.sectorInicial = levelNode.Attributes["sectorInicial"].Value;
                    level.conectorInicial = levelNode.Attributes["conectorInicial"].Value;
                    level.index = levelIndex;
                    levelIndex++;
                    levels.Add(level);
                }
            }

            

            XmlElement fisicaNode = (XmlElement)root.GetElementsByTagName("fisica")[0];
            fisica.velocidadCaminar = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("velocidadCaminar")[0].InnerText);
            fisica.velocidadCargandoEsconditeMovil = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("velocidadCargandoEsconditeMovil")[0].InnerText);
            fisica.duracionSegundosSalto = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("duracionSegundosSalto")[0].InnerText);
            fisica.velocidadRotacion = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("velocidadRotacion")[0].InnerText);
            fisica.velocidadSalto = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("velocidadSalto")[0].InnerText);
            fisica.gravedad = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("gravedad")[0].InnerText);
            fisica.slideFactor = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("slideFactor")[0].InnerText);
            fisica.boundingSphereCenterY = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("boundingSphereCenterY")[0].InnerText);
            fisica.boundingSphereAgachadoCenterY = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("boundingSphereAgachadoCenterY")[0].InnerText);
            fisica.boundingSphereRadio = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("boundingSphereRadio")[0].InnerText);
            fisica.boundingSphereRadioAgachado = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("boundingSphereRadioAgachado")[0].InnerText);
            fisica.anguloSalto = Geometry.DegreeToRadian(TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("anguloSalto")[0].InnerText));
            fisica.anguloGolpe = Geometry.DegreeToRadian(TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("anguloGolpe")[0].InnerText));
            fisica.frecuenciaSonidoCorrer = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("frecuenciaSonidoCorrer")[0].InnerText);
            fisica.frecuenciaSonidoCaminar = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("frecuenciaSonidoCaminar")[0].InnerText);
            fisica.frecuenciaSonidoCaminar = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("frecuenciaSonidoCaminar")[0].InnerText);
            fisica.init();

            this.debugModeEnabled = int.Parse(fisicaNode.GetElementsByTagName("boundingSphereDebug")[0].InnerText) == 1 ? true : false;

            XmlElement camaraNode = (XmlElement)root.GetElementsByTagName("camara")[0];
            camara.offsetY = TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("offsetY")[0].InnerText);
            camara.offsetZ = TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("offsetZ")[0].InnerText);
            camara.displacementY = TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("displacementY")[0].InnerText);
            camara.minOffsetZ = TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("minOffsetZ")[0].InnerText);
            camara.offsetYForMinOffsetY = TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("offsetYForMinOffsetY")[0].InnerText);
            camara.distanciaVisual = TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("distanciaVisual")[0].InnerText);
            camara.distanciaVisualSq = camara.distanciaVisual * camara.distanciaVisual;
            camara.facingBackwardOffsetZ = TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("facingBackwardOffsetZ")[0].InnerText);
            camara.facingBackwardMinOffsetZ = TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("facingBackwardMinOffsetZ")[0].InnerText);
            

            XmlElement guardiaNode = (XmlElement)root.GetElementsByTagName("guardia")[0];
            guardia.duracionSegundosVigilancia = TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("duracionSegundosVigilancia")[0].InnerText);
            guardia.velocidadCaminar = TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("velocidadCaminar")[0].InnerText);
            guardia.velocidadCorrer = TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("velocidadCorrer")[0].InnerText);
            guardia.velocidadRotacion = Geometry.DegreeToRadian(TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("velocidadRotacion")[0].InnerText));
            guardia.distanciaVisual = TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("distanciaVisual")[0].InnerText);
            guardia.distanciaVisualEspaldas = TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("distanciaVisualEspaldas")[0].InnerText);
            guardia.anguloVision = Geometry.DegreeToRadian(TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("anguloVision")[0].InnerText));
            guardia.anguloRotacionVigilancia = Geometry.DegreeToRadian(TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("anguloRotacionVigilancia")[0].InnerText));
            guardia.distanciaAtaque = TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("distanciaAtaque")[0].InnerText);
            guardia.velocidadRotacionVigilancia = Geometry.DegreeToRadian(TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("velocidadRotacionVigilancia")[0].InnerText));
            guardia.alturaVision = TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("alturaVision")[0].InnerText);
            guardia.alturaVisionAgachado = TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("alturaVisionAgachado")[0].InnerText);
            guardia.init();

            XmlElement keysNode = (XmlElement)root.GetElementsByTagName("keys")[0];
            keys.forward = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("forward")[0].InnerText);
            keys.backward = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("backward")[0].InnerText);
            keys.turnLeft = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("turnLeft")[0].InnerText);
            keys.turnRight = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("turnRight")[0].InnerText);
            keys.action = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("action")[0].InnerText);
            keys.crouch = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("crouch")[0].InnerText);
            keys.enter = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("enter")[0].InnerText);
        }

        /// <summary>
        /// Devuelve el primer nivel del juego
        /// </summary>
        public Level getFirstLevel()
        {
            return levels[0];
        }

        /// <summary>
        /// Devuelve el siguiente nivel, o null si no hay mas
        /// </summary>
        public Level getNextLevel(int levelIndex)
        {
            Level level = levels[levelIndex];
            if (level.index < levels.Count - 1)
            {
                return levels[level.index + 1];
            }
            return null;
        }

        public class Level
        {
            public string name;
            public string sectorInicial;
            public string conectorInicial;
            public int index;
        }

        public class Fisica
        {
            public float velocidadCaminar;
            public float velocidadCargandoEsconditeMovil;
            public float velocidadRotacion;
            public float velocidadSalto;
            public float gravedad;
            public float slideFactor;
            public float duracionSegundosSalto;
            public float boundingSphereCenterY;
            public float boundingSphereAgachadoCenterY;
            public float boundingSphereRadio;
            public float boundingSphereRadioAgachado;
            public float anguloSalto;
            public float anguloSaltoDotValue;
            public float anguloGolpe;
            public float anguloGolpeDotValue;
            public float frecuenciaSonidoCorrer;
            public float frecuenciaSonidoCaminar;

            public void init()
            {
                anguloSaltoDotValue = FastMath.Acos(anguloSalto);
                anguloGolpeDotValue = FastMath.Acos(anguloGolpe);
            }
        }

        public class Camara
        {
            public float offsetY;
            public float offsetZ;
            public float displacementY;
            public float minOffsetZ;
            public float offsetYForMinOffsetY;
            public float distanciaVisual;
            public float distanciaVisualSq;
            public float facingBackwardOffsetZ;
            public float facingBackwardMinOffsetZ;           
        }

        public class Guardia
        {
            public float duracionSegundosVigilancia;
            public float velocidadCaminar;
            public float velocidadCorrer;
            public float velocidadRotacion;
            public float distanciaVisual;
            public float distanciaVisualEspaldas;
            public float anguloVision;
            public float anguloRotacionVigilancia;
            public float distanciaAtaque;
            public float radioPerpendicularVision;
            public float velocidadRotacionVigilancia;
            public float anguloVisionDotValue;
            public float alturaVision;
            public float alturaVisionAgachado;
            
            public void init()
            {
                radioPerpendicularVision = FastMath.Sin(anguloVision) * distanciaVisual;
                anguloVisionDotValue = FastMath.Acos(anguloVision);
            }
        }


        public class Keys
        {
            public Key forward;
            public Key backward;
            public Key turnLeft;
            public Key turnRight;
            public Key action;
            public Key crouch;
            public Key enter;
        }

    }
}
