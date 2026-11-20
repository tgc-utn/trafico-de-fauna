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
    ///     Configuracion de todo el juego
    /// </summary>
    public class GameConfig
    {
        //atributos simples
        public string basePath;
        public Camara camara;
        public bool debugModeEnabled;
        public Fisica fisica;
        public Guardia guardia;
        public Keys keys;

        //atributos compuestos
        public List<Level> levels;
        public string levelsPath;
        public string menuPath;
        public string shadersPath;
        public string sonidosPath;

        public GameConfig(string mediaPath, string fileName, string shaderPath)
        {
            //paths
            basePath = mediaPath;
            levelsPath = mediaPath + "Levels\\";
            shadersPath = shaderPath;
            sonidosPath = mediaPath + "Sonidos\\";
            menuPath = mediaPath + "Menu\\";

            //init
            levels = new List<Level>();
            fisica = new Fisica();
            camara = new Camara();
            guardia = new Guardia();
            keys = new Keys();

            //Parsear xml
            var xmlString = File.ReadAllText(mediaPath + fileName);
            var dom = new XmlDocument();
            dom.LoadXml(xmlString);
            var root = dom.DocumentElement;

            //Levels
            var levelsNode = (XmlElement)root.GetElementsByTagName("levels")[0];
            var levelIndex = 0;
            foreach (XmlNode levelNode in levelsNode.ChildNodes)
            {
                if (levelNode.Attributes != null)
                {
                    var level = new Level();
                    level.name = levelNode.Attributes["name"].Value;
                    level.sectorInicial = levelNode.Attributes["sectorInicial"].Value;
                    level.conectorInicial = levelNode.Attributes["conectorInicial"].Value;
                    level.index = levelIndex;
                    levelIndex++;
                    levels.Add(level);
                }
            }


            var fisicaNode = (XmlElement)root.GetElementsByTagName("fisica")[0];
            fisica.velocidadCaminar =
                TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("velocidadCaminar")[0].InnerText);
            fisica.velocidadCargandoEsconditeMovil =
                TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("velocidadCargandoEsconditeMovil")[0]
                    .InnerText);
            fisica.duracionSegundosSalto =
                TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("duracionSegundosSalto")[0].InnerText);
            fisica.velocidadRotacion =
                TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("velocidadRotacion")[0].InnerText);
            fisica.velocidadSalto =
                TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("velocidadSalto")[0].InnerText);
            fisica.gravedad = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("gravedad")[0].InnerText);
            fisica.slideFactor = TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("slideFactor")[0].InnerText);
            fisica.boundingSphereCenterY =
                TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("boundingSphereCenterY")[0].InnerText);
            fisica.boundingSphereAgachadoCenterY =
                TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("boundingSphereAgachadoCenterY")[0]
                    .InnerText);
            fisica.boundingSphereRadio =
                TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("boundingSphereRadio")[0].InnerText);
            fisica.boundingSphereRadioAgachado =
                TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("boundingSphereRadioAgachado")[0].InnerText);
            fisica.anguloSalto =
                Geometry.DegreeToRadian(
                    TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("anguloSalto")[0].InnerText));
            fisica.anguloGolpe =
                Geometry.DegreeToRadian(
                    TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("anguloGolpe")[0].InnerText));
            fisica.frecuenciaSonidoCorrer =
                TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("frecuenciaSonidoCorrer")[0].InnerText);
            fisica.frecuenciaSonidoCaminar =
                TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("frecuenciaSonidoCaminar")[0].InnerText);
            fisica.frecuenciaSonidoCaminar =
                TgcParserUtils.parseFloat(fisicaNode.GetElementsByTagName("frecuenciaSonidoCaminar")[0].InnerText);
            fisica.init();

            debugModeEnabled = int.Parse(fisicaNode.GetElementsByTagName("boundingSphereDebug")[0].InnerText) == 1
                ? true
                : false;

            var camaraNode = (XmlElement)root.GetElementsByTagName("camara")[0];
            camara.offsetY = TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("offsetY")[0].InnerText);
            camara.offsetZ = TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("offsetZ")[0].InnerText);
            camara.displacementY =
                TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("displacementY")[0].InnerText);
            camara.minOffsetZ = TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("minOffsetZ")[0].InnerText);
            camara.offsetYForMinOffsetY =
                TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("offsetYForMinOffsetY")[0].InnerText);
            camara.distanciaVisual =
                TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("distanciaVisual")[0].InnerText);
            camara.distanciaVisualSq = camara.distanciaVisual * camara.distanciaVisual;
            camara.facingBackwardOffsetZ =
                TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("facingBackwardOffsetZ")[0].InnerText);
            camara.facingBackwardMinOffsetZ =
                TgcParserUtils.parseFloat(camaraNode.GetElementsByTagName("facingBackwardMinOffsetZ")[0].InnerText);


            var guardiaNode = (XmlElement)root.GetElementsByTagName("guardia")[0];
            guardia.duracionSegundosVigilancia =
                TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("duracionSegundosVigilancia")[0].InnerText);
            guardia.velocidadCaminar =
                TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("velocidadCaminar")[0].InnerText);
            guardia.velocidadCorrer =
                TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("velocidadCorrer")[0].InnerText);
            guardia.velocidadRotacion =
                Geometry.DegreeToRadian(
                    TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("velocidadRotacion")[0].InnerText));
            guardia.distanciaVisual =
                TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("distanciaVisual")[0].InnerText);
            guardia.distanciaVisualEspaldas =
                TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("distanciaVisualEspaldas")[0].InnerText);
            guardia.anguloVision =
                Geometry.DegreeToRadian(
                    TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("anguloVision")[0].InnerText));
            guardia.anguloRotacionVigilancia = Geometry.DegreeToRadian(
                TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("anguloRotacionVigilancia")[0].InnerText));
            guardia.distanciaAtaque =
                TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("distanciaAtaque")[0].InnerText);
            guardia.velocidadRotacionVigilancia = Geometry.DegreeToRadian(
                TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("velocidadRotacionVigilancia")[0]
                    .InnerText));
            guardia.alturaVision =
                TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("alturaVision")[0].InnerText);
            guardia.alturaVisionAgachado =
                TgcParserUtils.parseFloat(guardiaNode.GetElementsByTagName("alturaVisionAgachado")[0].InnerText);
            guardia.init();

            var keysNode = (XmlElement)root.GetElementsByTagName("keys")[0];
            keys.forward = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("forward")[0].InnerText);
            keys.backward = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("backward")[0].InnerText);
            keys.turnLeft = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("turnLeft")[0].InnerText);
            keys.turnRight = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("turnRight")[0].InnerText);
            keys.action = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("action")[0].InnerText);
            keys.crouch = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("crouch")[0].InnerText);
            keys.enter = (Key)Enum.Parse(typeof(Key), keysNode.GetElementsByTagName("enter")[0].InnerText);
        }

        /// <summary>
        ///     Devuelve el primer nivel del juego
        /// </summary>
        public Level getFirstLevel()
        {
            return levels[0];
        }

        /// <summary>
        ///     Devuelve el siguiente nivel, o null si no hay mas
        /// </summary>
        public Level getNextLevel(int levelIndex)
        {
            var level = levels[levelIndex];
            if (level.index < levels.Count - 1)
            {
                return levels[level.index + 1];
            }

            return null;
        }

        public class Level
        {
            public string conectorInicial;
            public int index;
            public string name;
            public string sectorInicial;
        }

        public class Fisica
        {
            public float anguloGolpe;
            public float anguloGolpeDotValue;
            public float anguloSalto;
            public float anguloSaltoDotValue;
            public float boundingSphereAgachadoCenterY;
            public float boundingSphereCenterY;
            public float boundingSphereRadio;
            public float boundingSphereRadioAgachado;
            public float duracionSegundosSalto;
            public float frecuenciaSonidoCaminar;
            public float frecuenciaSonidoCorrer;
            public float gravedad;
            public float slideFactor;
            public float velocidadCaminar;
            public float velocidadCargandoEsconditeMovil;
            public float velocidadRotacion;
            public float velocidadSalto;

            public void init()
            {
                anguloSaltoDotValue = FastMath.Acos(anguloSalto);
                anguloGolpeDotValue = FastMath.Acos(anguloGolpe);
            }
        }

        public class Camara
        {
            public float displacementY;
            public float distanciaVisual;
            public float distanciaVisualSq;
            public float facingBackwardMinOffsetZ;
            public float facingBackwardOffsetZ;
            public float minOffsetZ;
            public float offsetY;
            public float offsetYForMinOffsetY;
            public float offsetZ;
        }

        public class Guardia
        {
            public float alturaVision;
            public float alturaVisionAgachado;
            public float anguloRotacionVigilancia;
            public float anguloVision;
            public float anguloVisionDotValue;
            public float distanciaAtaque;
            public float distanciaVisual;
            public float distanciaVisualEspaldas;
            public float duracionSegundosVigilancia;
            public float radioPerpendicularVision;
            public float velocidadCaminar;
            public float velocidadCorrer;
            public float velocidadRotacion;
            public float velocidadRotacionVigilancia;

            public void init()
            {
                radioPerpendicularVision = FastMath.Sin(anguloVision) * distanciaVisual;
                anguloVisionDotValue = FastMath.Cos(anguloVision);
            }
        }


        public class Keys
        {
            public Key action;
            public Key backward;
            public Key crouch;
            public Key enter;
            public Key forward;
            public Key turnLeft;
            public Key turnRight;
        }
    }
}
