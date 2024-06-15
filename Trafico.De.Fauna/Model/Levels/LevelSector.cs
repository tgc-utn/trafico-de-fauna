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
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.Bananas;
using Trafico.De.Fauna.Model.CustomEvents;
using Trafico.De.Fauna.Model.Enemies;
using Trafico.De.Fauna.Model.Enemies.Ataque;
using Trafico.De.Fauna.Model.Escondites;
using Trafico.De.Fauna.Model.ItemsAnimal;
using Trafico.De.Fauna.Model.MainCharacters;
using Trafico.De.Fauna.Model.Meshes;
using Trafico.De.Fauna.Model.Saltos;

namespace Trafico.De.Fauna.Model.Levels
{
    /// <summary>
    ///     Representa un sector de un nivel
    /// </summary>
    public class LevelSector
    {
        private Level level;


        public LevelSector()
        {
            Conectores = new List<Conector>();
            ObjetosColisionables = new List<ColliderObject>();
            GuardiasManager = new GuardiasManager(this);
            ColisionesLogicas = new List<TgcBoundingAxisAlignBox>();
            Llaves = new List<Llave>();
            ItemsAnimal = new List<ItemAnimal>();
            EsconditesFijos = new List<EsconditeFijo>();
            EsconditesMoviles = new List<EsconditeMovil>();
            WaypointsAtaqueManager = new WaypointsAtaqueManager();
            CustomEvents = new List<CustomEvent>();
            SaltosManager = new SaltosManager();
            Bananas = new List<Banana>();
            CajasBananas = new List<CajaBanana>();
        }

        /// <summary>
        ///     Objetos estaticos del sector
        /// </summary>
        public List<TgcMeshShader> StaticMeshes { get; private set; }

        /// <summary>
        ///     Objetos estaticos del sector que no participan en la deteccion
        ///     de colisiones (son decorativos, sprites, etc)
        /// </summary>
        public List<TgcMeshShader> MeshesSinColision { get; private set; }

        /// <summary>
        ///     Conectores con otros sectores
        /// </summary>
        public List<Conector> Conectores { get; private set; }

        /// <summary>
        ///     Ultimo conector utilizado
        /// </summary>
        public Conector CurrentConector { get; private set; }

        /// <summary>
        ///     Llaves
        /// </summary>
        public List<Llave> Llaves { get; private set; }

        /// <summary>
        ///     Items animales a rescatar
        /// </summary>
        public List<ItemAnimal> ItemsAnimal { get; private set; }

        /// <summary>
        ///     Lugares fijos para esconderse
        /// </summary>
        public List<EsconditeFijo> EsconditesFijos { get; private set; }

        /// <summary>
        ///     Objetos para agarrar y esconderse
        /// </summary>
        public List<EsconditeMovil> EsconditesMoviles { get; private set; }

        /// <summary>
        ///     Objetos invisibles pero que igual se usan para colision en el sector
        /// </summary>
        public List<TgcBoundingAxisAlignBox> ColisionesLogicas { get; private set; }

        /// <summary>
        ///     Nombre del sector
        /// </summary>
        public string SectorName { get; private set; }

        /// <summary>
        ///     Nombre del sector que se muestra en pantalla al usuario
        /// </summary>
        public string DisplaySectorName { get; private set; }

        /// <summary>
        ///     Lista de todos los objetos colisionables del sector
        /// </summary>
        public List<ColliderObject> ObjetosColisionables { get; private set; }

        /// <summary>
        ///     Utilidad para manejar los Waypoints de Ataque de este sector para los guardias
        /// </summary>
        public WaypointsAtaqueManager WaypointsAtaqueManager { get; private set; }

        /// <summary>
        ///     Utilidad para manejar los Saltos entre plataformas
        /// </summary>
        public SaltosManager SaltosManager { get; private set; }

        /// <summary>
        ///     Bananas del sector
        /// </summary>
        public List<Banana> Bananas { get; private set; }

        /// <summary>
        ///     Cajas con bananas del sector
        /// </summary>
        public List<CajaBanana> CajasBananas { get; private set; }

        /// <summary>
        ///     Eventos especificos del sector
        /// </summary>
        public List<CustomEvent> CustomEvents { get; private set; }

        /// <summary>
        ///     Manejador de Guardias del sector
        /// </summary>
        public GuardiasManager GuardiasManager { get; set; }

        /// <summary>
        ///     Cargar el sector con todos sus modelos
        /// </summary>
        public void loadScene(Level level, string sectorName, string sectorPath, string texturePath,
            string lightmapPath)
        {
            this.level = level;
            this.SectorName = sectorName;

            var filePath = sectorPath + "Scene-TgcScene.xml";
            var xmlString = File.ReadAllText(filePath);


            //Cargar meshes
            var parser = new TgcSceneParser();
            var sceneData = parser.parseSceneFromString(xmlString);
            var staticMeshesData = new List<TgcMeshData>();
            var sinColisionMeshesData = new List<TgcMeshData>();
            DisplaySectorName = sceneData.name;


            //Diferenciar info segun layer
            foreach (var meshData in sceneData.meshesData)
            {
                switch (meshData.layerName)
                {
                    case "Conectores":
                        loadConector(level, meshData);
                        break;
                    case "Guardias":
                        loadGuardia(meshData);
                        break;
                    case "ItemsAnimal":
                        loadItemAnimal(meshData);
                        break;
                    case "Llaves":
                        loadLlave(meshData);
                        break;
                    case "Sonido":
                        loadSonido(meshData);
                        break;
                    case "SinColision":
                        sinColisionMeshesData.Add(meshData);
                        break;
                    case "ColisionesLogicas":
                        loadColisionesLogicas(meshData);
                        break;
                    case "EsconditesFijos":
                        loadEsconditesFijos(meshData, texturePath, sceneData);
                        break;
                    case "EsconditesMoviles":
                        loadEsconditesMoviles(meshData, texturePath, sceneData);
                        break;
                    case "WaypointsAtaque":
                        loadWaypointsAtaque(meshData);
                        break;
                    case "CustomEvents":
                        loadCustomEvent(meshData, texturePath, sceneData);
                        break;
                    case "Saltos":
                        loadSalto(meshData);
                        break;
                    case "Bananas":
                        loadBanana(meshData);
                        break;
                    case "CajasBananas":
                        loadCajaBananas(meshData);
                        break;
                    default:
                        //Otro layer de SinColision
                        if (meshData.layerName.Contains("SinColision_"))
                        {
                            sinColisionMeshesData.Add(meshData);
                        }
                        //Mesh default
                        else
                        {
                            staticMeshesData.Add(meshData);
                        }

                        break;
                }
            }


            //Cargar meshes staticos y los que son sin colision
            loadStaticMeshes(texturePath, lightmapPath, sceneData, staticMeshesData);
            loadMeshesSinColision(texturePath, lightmapPath, sceneData, sinColisionMeshesData);

            //Configurar nodos de Guardias
            GuardiasManager.arrangeNodes();

            //Configurar waypoints de Ataque de Guardias
            WaypointsAtaqueManager.arrangeNodes();

            //Configurar plataformas de salto
            SaltosManager.arrangeNodes();

            //Agregar colisionadores
            addColliderObjects();
        }


        /// <summary>
        ///     Cargar los objetos contra los que se colisiona en el sector
        /// </summary>
        private void addColliderObjects()
        {
            //Mesh estaticos
            foreach (var mesh in StaticMeshes)
            {
                ObjetosColisionables.Add(new ColliderObject(mesh));
            }

            //Agregar colisiones logicas
            foreach (var collider in ColisionesLogicas)
            {
                ObjetosColisionables.Add(new ColliderObject(collider));
            }

            //Agregar ItemsAnimal para colisionar
            foreach (var itemAnimal in ItemsAnimal)
            {
                ObjetosColisionables.Add(itemAnimal.ColliderObject);
            }

            //Agregar cajas de bananas
            foreach (var caja in CajasBananas)
            {
                ObjetosColisionables.Add(caja.ColliderObject);
            }

            //Agregar CustomEvents que sean colisionables
            foreach (var customEvent in CustomEvents)
            {
                var collider = customEvent.getColliderObject();
                if (collider != null)
                {
                    ObjetosColisionables.Add(collider);
                }
            }
        }


        /// <summary>
        ///     Carga los meshes staticos
        /// </summary>
        private void loadStaticMeshes(string texturePath, string lightmapPath, TgcSceneData sceneData,
            List<TgcMeshData> staticMeshesData)
        {
            //Usar TgcMeshShader
            sceneData.meshesData = staticMeshesData.ToArray();
            var loader = new TgcSceneLoader();
            loader.MeshFactory = new TgcMeshShaderFactory();
            var scene = loader.loadScene(sceneData, texturePath);
            StaticMeshes = new List<TgcMeshShader>();
            foreach (var mesh in scene.Meshes)
            {
                mesh.AutoTransformEnable = true;
                StaticMeshes.Add((TgcMeshShader)mesh);
            }
        }

        /// <summary>
        ///     Carga los meshes que son sin sin colision
        /// </summary>
        private void loadMeshesSinColision(string texturePath, string lightmapPath, TgcSceneData sceneData,
            List<TgcMeshData> sinColisionMeshesData)
        {
            //Usar TgcMeshShader
            sceneData.meshesData = sinColisionMeshesData.ToArray();
            var loader = new TgcSceneLoader();
            loader.MeshFactory = new TgcMeshShaderFactory();
            var scene = loader.loadScene(sceneData, texturePath);
            MeshesSinColision = new List<TgcMeshShader>();
            foreach (var mesh in scene.Meshes)
            {
                mesh.AutoTransformEnable = true;
                MeshesSinColision.Add((TgcMeshShader)mesh);
            }
        }


        /// <summary>
        ///     Cargar conector
        /// </summary>
        private void loadConector(Level level, TgcMeshData meshData)
        {
            var conector = new Conector(this, meshData);
            Conectores.Add(conector);
            try
            {
                level.Conectores.Add(conector.Id, conector);
            }
            catch (Exception)
            {
                throw new Exception("Hay dos conectores con el mismo nombre: " + conector.Id);
            }
        }

        private void loadSonido(TgcMeshData meshData)
        {
        }

        /// <summary>
        ///     Cargar llaves del sector
        /// </summary>
        private void loadLlave(TgcMeshData meshData)
        {
            var llave = new Llave(meshData);
            Llaves.Add(llave);
            level.setLlave(llave);
        }

        /// <summary>
        ///     Cargar items de animales a rescatar
        /// </summary>
        private void loadItemAnimal(TgcMeshData meshData)
        {
            var item = new ItemAnimal(meshData);
            ItemsAnimal.Add(item);
        }

        /// <summary>
        ///     Cargar guardia
        /// </summary>
        private void loadGuardia(TgcMeshData meshData)
        {
            GuardiasManager.addNode(meshData);
        }

        /// <summary>
        ///     Cargar colisiones logicas
        /// </summary>
        private void loadColisionesLogicas(TgcMeshData meshData)
        {
            var boundingBox = GameUtils.createBBFromMeshData(meshData);
            ColisionesLogicas.Add(boundingBox);
        }

        /// <summary>
        ///     Cargar escondites moviles
        /// </summary>
        private void loadEsconditesMoviles(TgcMeshData meshData, string texturePath, TgcSceneData sceneData)
        {
            var escondite = new EsconditeMovil(meshData, texturePath, sceneData);
            EsconditesMoviles.Add(escondite);
        }

        /// <summary>
        ///     Cargar escondites fijos
        /// </summary>
        private void loadEsconditesFijos(TgcMeshData meshData, string texturePath, TgcSceneData sceneData)
        {
            var escondite = new EsconditeFijo(meshData, texturePath, sceneData);
            EsconditesFijos.Add(escondite);
        }

        /// <summary>
        ///     Cargar Waypoints y relaciones entre Waypoints
        /// </summary>
        private void loadWaypointsAtaque(TgcMeshData meshData)
        {
            WaypointsAtaqueManager.addNode(meshData);
        }

        /// <summary>
        ///     Cargar CustomEvent
        /// </summary>
        private void loadCustomEvent(TgcMeshData meshData, string texturePath, TgcSceneData sceneData)
        {
            var customEvent = GameModel.instance.customEventsManager.loadCustomEvent(meshData, texturePath, sceneData);
            CustomEvents.Add(customEvent);
        }

        /// <summary>
        ///     Cargar un salto
        /// </summary>
        private void loadSalto(TgcMeshData meshData)
        {
            SaltosManager.addNode(meshData);
        }

        /// <summary>
        ///     Cargar banana
        /// </summary>
        private void loadBanana(TgcMeshData meshData)
        {
            var b = new Banana(meshData);
            Bananas.Add(b);
        }

        /// <summary>
        ///     Cargar Caja de bananas
        /// </summary>
        private void loadCajaBananas(TgcMeshData meshData)
        {
            var caja = new CajaBanana(meshData);
            CajasBananas.Add(caja);
        }

        /// <summary>
        ///     Buscar un sector por su ID
        /// </summary>
        public Conector findSector(string conectorId)
        {
            foreach (var c in Conectores)
            {
                if (c.Id == conectorId)
                {
                    return c;
                }
            }

            throw new Exception("Conector: " + conectorId + " no encontrado");
        }

        /// <summary>
        ///     Irse de un sector
        /// </summary>
        public void hideSector()
        {
            //deshabilitar Sonidos
            GuardiasManager.hideGuardias();
            stopSounds();
            GameModel.instance.postProcessEffects.setAlarmModeEnable(false);

            //Liberar bananas
            GameModel.instance.bananasPool.freeAllMeshes();
            foreach (var b in Bananas)
            {
                b.hide();
            }

            //Liberar cajas de bananas
            GameModel.instance.cajasBananasPool.freeAllMeshes();
            foreach (var caja in CajasBananas)
            {
                caja.hide();
            }

            //Terminar CustomEvents
            foreach (var customEvent in CustomEvents)
            {
                customEvent.end();
            }

            //Soltar todos los escondites moviles
            foreach (var esm in EsconditesMoviles)
            {
                esm.Agarrado = false;
                esm.Escondido = false;
            }

            //Soltar todos los escondites fijos
            foreach (var esf in EsconditesFijos)
            {
                esf.Escondido = false;
            }
        }

        /// <summary>
        ///     Iniciar un sector
        /// </summary>
        public void initSector(MainCharacter mainCharacter, Conector nextConector)
        {
            //Musica
            GameModel.instance.sonidos.playMusic(GameModel.instance.sonidos.music_default);

            //Restaurar posicion de escondites moviles
            foreach (var esc in EsconditesMoviles)
            {
                esc.restorePosition();
            }

            //Iniciar personaje en sector
            CurrentConector = nextConector;
            mainCharacter.initInConectorPos(CurrentConector.Center, CurrentConector.Orientacion);
            GuardiasManager.initGuardias();

            //Iniciar modelos de bananas
            foreach (var b in Bananas)
            {
                b.init();
            }

            //Iniciar modelos de cajas de bananas
            foreach (var caja in CajasBananas)
            {
                caja.init();
            }

            //Iniciar CustomEvents
            foreach (var customEvent in CustomEvents)
            {
                customEvent.init();
            }
        }

        /// <summary>
        ///     Actualizar estado del sector
        /// </summary>
        public void update(float elapsedTime)
        {
            //Actualizar posicion de enemigos
            GuardiasManager.update(elapsedTime);

            //Actualizar estado de customEvents
            foreach (var customEvent in CustomEvents)
            {
                if (customEvent.Enabled)
                {
                    customEvent.update(elapsedTime);
                }
            }
        }

        /// <summary>
        ///     Render del sector
        /// </summary>
        public void render(float elapsedTime, TgcFrustum frustum)
        {
            //Hacer FrustumCulling de los objetos que vale la pena
            frustumCulling(frustum);

            //Ajustar distancia de camara por colision contra objetos
            adaptCameraPosition();


            //Render de mallas estaticas
            foreach (var mesh in StaticMeshes)
            {
                GameModel.instance.shaders.setCorrectTechnique(mesh);
                mesh.render();
            }

            //Render de mallas sin colision
            foreach (var mesh in MeshesSinColision)
            {
                GameModel.instance.shaders.setCorrectTechnique(mesh);
                mesh.render();
            }

            //Guaridas
            GuardiasManager.render(elapsedTime);

            //Llaves
            foreach (var llave in Llaves)
            {
                llave.render();
            }

            //Items animal
            foreach (var item in ItemsAnimal)
            {
                item.render();
            }

            //Escondites moviles
            foreach (var escondite in EsconditesMoviles)
            {
                escondite.render();
            }

            //Escondites fijos
            foreach (var escondite in EsconditesFijos)
            {
                escondite.render();
            }

            //Ver si el personaje principal colisiona con algun conector (para mostrar su cartel)
            var conectorColision = checkConectores(GameModel.instance.mainCharacter);
            if (conectorColision != null)
            {
                conectorColision.render();
            }

            //Bananas
            foreach (var b in Bananas)
            {
                b.render();
            }

            //Cajas de bananas
            foreach (var caja in CajasBananas)
            {
                caja.render(elapsedTime);
            }

            //CustomEvents
            foreach (var customEvent in CustomEvents)
            {
                if (customEvent.Enabled)
                {
                    customEvent.render();
                }
            }
        }

        /// <summary>
        ///     Hacer FrustumCulling de meshes estaticos y meshes sin colision
        ///     Por fuerza bruta
        /// </summary>
        private void frustumCulling(TgcFrustum frustum)
        {
            var mainCharacter = GameModel.instance.mainCharacter;
            //Frustum Culling meshes estaticos
            foreach (var mesh in StaticMeshes)
            {
                //Frustum Culling brute force
                if (TgcCollisionUtils.classifyFrustumAABB(frustum, mesh.BoundingBox) ==
                    TgcCollisionUtils.FrustumResult.OUTSIDE)
                {
                    mesh.Enabled = false;
                }
                else
                {
                    mesh.Enabled = true;
                }
            }

            //Frustum Culling meshes sin colision
            foreach (var mesh in MeshesSinColision)
            {
                //Ver primero si esta dentro de un cierto radio de alcance del personaje
                if (mainCharacter.isInProximityRange(mesh.BoundingBox.PMin))
                {
                    //Frustum Culling brute force
                    if (TgcCollisionUtils.classifyFrustumAABB(frustum, mesh.BoundingBox) ==
                        TgcCollisionUtils.FrustumResult.OUTSIDE)
                    {
                        mesh.Enabled = false;
                    }
                    else
                    {
                        mesh.Enabled = true;
                    }
                }
                else
                {
                    mesh.Enabled = false;
                }
            }
        }

        /// <summary>
        ///     Ajustar distancia de camara por colision contra objetos
        /// </summary>
        private void adaptCameraPosition()
        {
            var camera = GameModel.instance.mainCharacter.Camera;
            var camaraConfig = GameModel.instance.gameConfig.camara;
            var mainCharacter = GameModel.instance.mainCharacter;
            camera.OffsetForward = mainCharacter.CurrentCameraOffsetForward;
            camera.OffsetHeight = camaraConfig.offsetY;

            TGCVector3 segmentA;
            TGCVector3 segmentB;
            camera.generateViewMatrix(out segmentA, out segmentB);
            TGCVector3 q;
            var minDistSq = FastMath.Pow2(camera.OffsetForward);
            foreach (var collObj in ObjetosColisionables)
            {
                if (collObj.Enabled)
                {
                    if (TgcCollisionUtils.intersectSegmentAABB(segmentB, segmentA, collObj.BoundingBox, out q))
                    {
                        var distSq = TGCVector3.Subtract(q, segmentB).LengthSq();
                        if (distSq < minDistSq)
                        {
                            minDistSq = distSq;
                        }
                    }
                }
            }

            //Ajustar distancia de camara si llego al minimo tolerable de cercania
            camera.OffsetForward = -FastMath.Sqrt(minDistSq);
            if (FastMath.Abs(camera.OffsetForward) < FastMath.Abs(camaraConfig.minOffsetZ))
            {
                camera.OffsetForward = mainCharacter.CurrentMinCameraOffsetForward;
                camera.OffsetHeight = camaraConfig.offsetYForMinOffsetY;
            }
        }

        /// <summary>
        ///     Busca si el personaje principal colisiona con un conector.
        ///     En caso de que si, devuelve el proximo conector con el que conecta este, para que se pueda
        ///     hacer el cambio de sector
        /// </summary>
        public Conector checkConectoresGetNext(MainCharacter mainCharacter)
        {
            var conector = checkConectores(mainCharacter);
            if (conector != null)
            {
            }

            return null;
        }

        /// <summary>
        ///     Ejecutar la accion del conector, que puede ser cambiar de sector o cambiar de nivel.
        /// </summary>
        public void executeConector(Conector conector)
        {
            //Cambiar de nivel
            if (conector.ChangeLevel)
            {
                level.changeLevel();
            }
            //Cambiar de sector
            else
            {
                Conector nextConector;
                if (level.Conectores.TryGetValue(conector.SiguienteId, out nextConector))
                {
                    level.changeSector(nextConector);
                }
                else
                {
                    throw new Exception("El conector: " + conector.Id + " quiere pasar al siguienteId inexistente: " +
                                        conector.SiguienteId);
                }
            }
        }

        /// <summary>
        ///     Busca si el personaje principal colisiona con un conector.
        ///     Devuelve el conector colisionado o null.
        /// </summary>
        public Conector checkConectores(MainCharacter mainCharacter)
        {
            foreach (var conector in Conectores)
            {
                if (conector.Activo && mainCharacter.collideWithAABB(conector.BoundingBox))
                {
                    return conector;
                }
            }

            return null;
        }


        /// <summary>
        ///     Activar el modo alarma.
        ///     El personaje ha sido visto por un guardia
        /// </summary>
        public void setAlarmMode()
        {
            //No alertar a los otros
            //guardiasManager.alertAll();

            var sonidos = GameModel.instance.sonidos;
            //sonidos.playMusic(sonidos.music_alarm);
            sonidos.mp3Player.Stop();
            sonidos.alarma_guardias.Play(true);
            GameModel.instance.postProcessEffects.setAlarmModeEnable(true);
        }

        /// <summary>
        ///     Busca si el personaje principal colisiona con alguna llave del sector.
        ///     Si colisiona la agarra.
        /// </summary>
        public Llave checkLlaves(MainCharacter mainCharacter)
        {
            foreach (var llave in Llaves)
            {
                if (!llave.Agarrada)
                {
                    if (mainCharacter.collideWithBSphere(llave.BoundingSphere))
                    {
                        llave.agarrar();
                        return llave;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Busca si el personaje principal colisiona con algun item animal para rescatar.
        /// </summary>
        public ItemAnimal checkItemsAnimal(MainCharacter mainCharacter)
        {
            foreach (var item in ItemsAnimal)
            {
                if (!item.Liberado)
                {
                    if (mainCharacter.collideWithBSphere(item.BSphereProximidad))
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Busca si el personaje principal colisiona con algun escondite fijo
        /// </summary>
        public EsconditeFijo checkEsconditesFijos(MainCharacter mainCharacter)
        {
            foreach (var escondite in EsconditesFijos)
            {
                if (mainCharacter.collideWithBSphere(escondite.BSphere))
                {
                    return escondite;
                }
            }

            return null;
        }

        /// <summary>
        ///     Busca si el personaje principal colisiona con algun escondite movil
        /// </summary>
        public EsconditeMovil checkEsconditesMoviles(MainCharacter mainCharacter)
        {
            foreach (var escondite in EsconditesMoviles)
            {
                if (mainCharacter.collideWithAABB(escondite.BoundingBox))
                {
                    return escondite;
                }
            }

            return null;
        }

        /// <summary>
        ///     Busca si el personaje principal colisiona con alguna plataforma de salto
        /// </summary>
        public Salto.Plataforma checkSaltos(MainCharacter mainCharacter)
        {
            return SaltosManager.checkSaltos(mainCharacter);
        }

        /// <summary>
        ///     Agarrar todas las bananas del sector que colisionan con el personaje
        /// </summary>
        public void checkBananas(MainCharacter mainCharacter)
        {
            foreach (var b in Bananas)
            {
                b.agarrar(mainCharacter);
            }
        }

        /// <summary>
        ///     Golpear todas las cajas de bananas que colisionan con el personaje
        /// </summary>
        public void golpearCajasBananas(MainCharacter mainCharacter)
        {
            foreach (var caja in CajasBananas)
            {
                caja.golpear(mainCharacter);
            }
        }

        /// <summary>
        ///     Para los sonidos del sector
        /// </summary>
        public void stopSounds()
        {
            var sonidos = GameModel.instance.sonidos;
            sonidos.alarma_guardias.Stop();
            sonidos.mp3Player.CloseFile();
        }

        /// <summary>
        ///     Elimina todos los recursos del sector
        /// </summary>
        public void unload()
        {
            //staticMeshes
            foreach (var mesh in StaticMeshes)
            {
                mesh.Dispose();
            }

            StaticMeshes.Clear();
            StaticMeshes = null;

            //meshesSinColision
            foreach (var mesh in MeshesSinColision)
            {
                mesh.Dispose();
            }

            MeshesSinColision.Clear();
            MeshesSinColision = null;

            //guardias
            GuardiasManager.dispose();
            GuardiasManager = null;

            //Llaves
            Llaves.Clear();
            Llaves = null;

            //Items animal
            foreach (var item in ItemsAnimal)
            {
                item.dispose();
            }

            ItemsAnimal.Clear();
            ItemsAnimal = null;

            //Escondites moviles
            foreach (var escondite in EsconditesMoviles)
            {
                escondite.dispose();
            }

            EsconditesMoviles.Clear();
            EsconditesMoviles = null;

            //Escondites fijos
            foreach (var escondite in EsconditesFijos)
            {
                escondite.dispose();
            }

            EsconditesFijos.Clear();
            EsconditesFijos = null;

            //Conectores
            foreach (var conector in Conectores)
            {
                conector.dispose();
            }

            Conectores.Clear();
            Conectores = null;
            CurrentConector = null;

            //colisionesLogicas
            foreach (var colLogicas in ColisionesLogicas)
            {
                colLogicas.Dispose();
            }

            ColisionesLogicas.Clear();
            ColisionesLogicas = null;

            //waypointsAtaqueManager
            WaypointsAtaqueManager.dispose();
            WaypointsAtaqueManager = null;

            //customEvents
            foreach (var customEvent in CustomEvents)
            {
                customEvent.dispose();
            }

            CustomEvents.Clear();
            CustomEvents = null;

            //saltosManager
            SaltosManager.dispose();
            SaltosManager = null;

            //Bananas
            foreach (var b in Bananas)
            {
                b.dispose();
            }

            Bananas.Clear();
            Bananas = null;

            //CajaBanana
            foreach (var c in CajasBananas)
            {
                c.dispose();
            }

            CajasBananas.Clear();
            CajasBananas = null;


            //objetosColisionables
            foreach (var c in ObjetosColisionables)
            {
                c.dispose();
            }

            ObjetosColisionables.Clear();
            ObjetosColisionables = null;

            //otros
            level = null;
        }
    }
}
