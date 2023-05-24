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
using System.IO;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using TgcViewer;
using Examples.TraficoFauna.Enemies;
using Examples.TraficoFauna.Meshes;
using TraficoFauna.Examples;
using Microsoft.DirectX.Direct3D;
using Examples.TraficoFauna.ItemsAnimal;
using TgcViewer.Utils.Sound;
using Examples.TraficoFauna.Escondites;
using Examples.TraficoFauna.Enemies.Ataque;
using Examples.TraficoFauna.CustomEvents;
using Examples.TraficoFauna.Saltos;
using Examples.TraficoFauna.Camera;
using Examples.TraficoFauna.Bananas;
using Examples.TraficoFauna.Sonidos;

namespace Examples.TraficoFauna
{
    /// <summary>
    /// Representa un sector de un nivel
    /// </summary>
    public class LevelSector
    {

        private List<TgcMeshShader> staticMeshes;
        /// <summary>
        /// Objetos estaticos del sector
        /// </summary>
        public List<TgcMeshShader> StaticMeshes
        {
            get { return staticMeshes; }
        }

        private List<TgcMeshShader> meshesSinColision;
        /// <summary>
        /// Objetos estaticos del sector que no participan en la deteccion
        /// de colisiones (son decorativos, sprites, etc)
        /// </summary>
        public List<TgcMeshShader> MeshesSinColision
        {
            get { return meshesSinColision; }
        }
        
        private List<Conector> conectores;
        /// <summary>
        /// Conectores con otros sectores
        /// </summary>
        public List<Conector> Conectores
        {
            get { return conectores; }
        }

        private Conector currentConector;
        /// <summary>
        /// Ultimo conector utilizado
        /// </summary>
        public Conector CurrentConector
        {
            get { return currentConector; }
        }

        private List<Llave> llaves;
        /// <summary>
        /// Llaves
        /// </summary>
        public List<Llave> Llaves
        {
            get { return llaves; }
        }

        private List<ItemAnimal> itemsAnimal;
        /// <summary>
        /// Items animales a rescatar
        /// </summary>
        public List<ItemAnimal> ItemsAnimal
        {
            get { return itemsAnimal; }
        }

        private List<EsconditeFijo> esconditesFijos;
        /// <summary>
        /// Lugares fijos para esconderse
        /// </summary>
        public List<EsconditeFijo> EsconditesFijos
        {
            get { return esconditesFijos; }
        }

        private List<EsconditeMovil> esconditesMoviles;
        /// <summary>
        /// Objetos para agarrar y esconderse
        /// </summary>
        public List<EsconditeMovil> EsconditesMoviles
        {
            get { return esconditesMoviles; }
        }

        private List<TgcBoundingBox> colisionesLogicas;
        /// <summary>
        /// Objetos invisibles pero que igual se usan para colision en el sector
        /// </summary>
        public List<TgcBoundingBox> ColisionesLogicas
        {
            get { return colisionesLogicas; }
        }

        string sectorName;
        /// <summary>
        /// Nombre del sector
        /// </summary>
        public string SectorName
        {
            get { return sectorName; }
        }

        string displaySectorName;
        /// <summary>
        /// Nombre del sector que se muestra en pantalla al usuario
        /// </summary>
        public string DisplaySectorName
        {
            get { return displaySectorName; }
        }

        List<ColliderObject> objetosColisionables;
        /// <summary>
        /// Lista de todos los objetos colisionables del sector
        /// </summary>
        public List<ColliderObject> ObjetosColisionables
        {
            get { return objetosColisionables; }
        }

        WaypointsAtaqueManager waypointsAtaqueManager;
        /// <summary>
        /// Utilidad para manejar los Waypoints de Ataque de este sector para los guardias
        /// </summary>
        public WaypointsAtaqueManager WaypointsAtaqueManager
        {
            get { return waypointsAtaqueManager; }
        }

        SaltosManager saltosManager;
        /// <summary>
        /// Utilidad para manejar los Saltos entre plataformas
        /// </summary>
        public SaltosManager SaltosManager
        {
            get { return saltosManager; }
        }

        List<Banana> bananas;
        /// <summary>
        /// Bananas del sector
        /// </summary>
        public List<Banana> Bananas
        {
            get { return bananas; }
        }

        List<CajaBanana> cajasBananas;
        /// <summary>
        /// Cajas con bananas del sector
        /// </summary>
        public List<CajaBanana> CajasBananas
        {
            get { return cajasBananas; }
        }

        List<CustomEvent> customEvents;
        /// <summary>
        /// Eventos especificos del sector
        /// </summary>
        public List<CustomEvent> CustomEvents
        {
            get { return customEvents; }
        }

        GuardiasManager guardiasManager;
        /// <summary>
        /// Manejador de Guardias del sector
        /// </summary>
        public GuardiasManager GuardiasManager
        {
            get { return guardiasManager; }
            set { guardiasManager = value; }
        }
        



        Level level;
        


        public LevelSector()
        {
            conectores = new List<Conector>();
            objetosColisionables = new List<ColliderObject>();
            guardiasManager = new GuardiasManager(this);
            colisionesLogicas = new List<TgcBoundingBox>();
            llaves = new List<Llave>();
            itemsAnimal = new List<ItemAnimal>();
            esconditesFijos = new List<EsconditeFijo>();
            esconditesMoviles = new List<EsconditeMovil>();
            waypointsAtaqueManager = new WaypointsAtaqueManager();
            customEvents = new List<CustomEvent>();
            saltosManager = new SaltosManager();
            bananas = new List<Banana>();
            cajasBananas = new List<CajaBanana>();
        }

        /// <summary>
        /// Cargar el sector con todos sus modelos
        /// </summary>
        public void loadScene(Level level, string sectorName, string sectorPath, string texturePath, string lightmapPath)
        {
            this.level = level;
            this.sectorName = sectorName;

            string filePath = sectorPath + "Scene-TgcScene.xml";
            string xmlString = File.ReadAllText(filePath);


            //Cargar meshes
            TgcSceneParser parser = new TgcSceneParser();
            TgcSceneData sceneData = parser.parseSceneFromString(xmlString);
            List<TgcMeshData> staticMeshesData = new List<TgcMeshData>();
            List<TgcMeshData> sinColisionMeshesData = new List<TgcMeshData>();
            this.displaySectorName = sceneData.name;


            //Diferenciar info segun layer
            foreach (TgcMeshData meshData in sceneData.meshesData)
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
            guardiasManager.arrangeNodes();

            //Configurar waypoints de Ataque de Guardias
            waypointsAtaqueManager.arrangeNodes();

            //Configurar plataformas de salto
            saltosManager.arrangeNodes();

            //Agregar colisionadores
            addColliderObjects();
        }

        

        

        

        

        

        /// <summary>
        /// Cargar los objetos contra los que se colisiona en el sector
        /// </summary>
        private void addColliderObjects()
        {
            //Mesh estaticos
            foreach (TgcMeshShader mesh in staticMeshes)
            {
                objetosColisionables.Add(new ColliderObject(mesh));
            }

            //Agregar colisiones logicas
            foreach (TgcBoundingBox collider in colisionesLogicas)
            {
                objetosColisionables.Add(new ColliderObject(collider));
            }

            //Agregar ItemsAnimal para colisionar
            foreach (ItemAnimal itemAnimal in itemsAnimal)
            {
                objetosColisionables.Add(itemAnimal.ColliderObject);
            }

            //Agregar cajas de bananas
            foreach (CajaBanana caja in cajasBananas)
            {
                objetosColisionables.Add(caja.ColliderObject);
            }

            //Agregar CustomEvents que sean colisionables
            foreach (CustomEvent customEvent in customEvents)
            {
                ColliderObject collider = customEvent.getColliderObject();
                if (collider != null)
                {
                    objetosColisionables.Add(collider);
                }
            }
        }

        

        /// <summary>
        /// Carga los meshes staticos
        /// </summary>
        private void loadStaticMeshes(string texturePath, string lightmapPath, TgcSceneData sceneData, List<TgcMeshData> staticMeshesData)
        {
            //Usar TgcMeshShader
            sceneData.meshesData = staticMeshesData.ToArray();
            TgcSceneLoader loader = new TgcSceneLoader();
            loader.TexturesPath = texturePath;
            loader.LightmapPath = lightmapPath;
            loader.MeshFactory = new TgcMeshShaderFactory();
            TgcScene scene = loader.loadScene(sceneData, texturePath);
            staticMeshes = new List<TgcMeshShader>();
            foreach (TgcMesh mesh in scene.Meshes)
            {
                staticMeshes.Add((TgcMeshShader)mesh);
            }
        }

        /// <summary>
        /// Carga los meshes que son sin sin colision
        /// </summary>
        private void loadMeshesSinColision(string texturePath, string lightmapPath, TgcSceneData sceneData, List<TgcMeshData> sinColisionMeshesData)
        {
            //Usar TgcMeshShader
            sceneData.meshesData = sinColisionMeshesData.ToArray();
            TgcSceneLoader loader = new TgcSceneLoader();
            loader.TexturesPath = texturePath;
            loader.LightmapPath = lightmapPath;
            loader.MeshFactory = new TgcMeshShaderFactory();
            TgcScene scene = loader.loadScene(sceneData, texturePath);
            meshesSinColision = new List<TgcMeshShader>();
            foreach (TgcMesh mesh in scene.Meshes)
            {
                meshesSinColision.Add((TgcMeshShader)mesh);
            }
        }

        
        /// <summary>
        /// Cargar conector
        /// </summary>
        private void loadConector(Level level, TgcMeshData meshData)
        {
            Conector conector = new Conector(this, meshData);
            this.conectores.Add(conector);
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
        /// Cargar llaves del sector
        /// </summary>
        private void loadLlave(TgcMeshData meshData)
        {
            Llave llave = new Llave(meshData);
            this.llaves.Add(llave);
            level.setLlave(llave);
        }

        /// <summary>
        /// Cargar items de animales a rescatar
        /// </summary>
        private void loadItemAnimal(TgcMeshData meshData)
        {
            ItemAnimal item = new ItemAnimal(meshData);
            this.itemsAnimal.Add(item);
        }

        /// <summary>
        /// Cargar guardia
        /// </summary>
        private void loadGuardia(TgcMeshData meshData)
        {
            guardiasManager.addNode(meshData);
        }

        /// <summary>
        /// Cargar colisiones logicas
        /// </summary>
        private void loadColisionesLogicas(TgcMeshData meshData)
        {
            TgcBoundingBox boundingBox = GameUtils.createBBFromMeshData(meshData);
            colisionesLogicas.Add(boundingBox);
        }

        /// <summary>
        /// Cargar escondites moviles
        /// </summary>
        private void loadEsconditesMoviles(TgcMeshData meshData, string texturePath, TgcSceneData sceneData)
        {
            EsconditeMovil escondite = new EsconditeMovil(meshData, texturePath, sceneData);
            this.esconditesMoviles.Add(escondite);
        }

        /// <summary>
        /// Cargar escondites fijos
        /// </summary>
        private void loadEsconditesFijos(TgcMeshData meshData, string texturePath, TgcSceneData sceneData)
        {
            EsconditeFijo escondite = new EsconditeFijo(meshData, texturePath, sceneData);
            this.esconditesFijos.Add(escondite);
        }

        /// <summary>
        /// Cargar Waypoints y relaciones entre Waypoints
        /// </summary>
        private void loadWaypointsAtaque(TgcMeshData meshData)
        {
            waypointsAtaqueManager.addNode(meshData);
        }

        /// <summary>
        /// Cargar CustomEvent
        /// </summary>
        private void loadCustomEvent(TgcMeshData meshData, string texturePath, TgcSceneData sceneData)
        {
            CustomEvent customEvent = Game.instance.customEventsManager.loadCustomEvent(meshData, texturePath, sceneData);
            this.customEvents.Add(customEvent);
        }

        /// <summary>
        /// Cargar un salto
        /// </summary>
        private void loadSalto(TgcMeshData meshData)
        {
            this.saltosManager.addNode(meshData);
        }

        /// <summary>
        /// Cargar banana
        /// </summary>
        private void loadBanana(TgcMeshData meshData)
        {
            Banana b = new Banana(meshData);
            this.bananas.Add(b);
        }

        /// <summary>
        /// Cargar Caja de bananas
        /// </summary>
        private void loadCajaBananas(TgcMeshData meshData)
        {
            CajaBanana caja = new CajaBanana(meshData);
            this.cajasBananas.Add(caja);
        }

        /// <summary>
        /// Buscar un sector por su ID
        /// </summary>
        public Conector findSector(string conectorId)
        {
            foreach (Conector c in conectores)
            {
                if (c.Id == conectorId)
                {
                    return c;
                }
            }
            throw new Exception("Conector: " + conectorId + " no encontrado");
        }

        /// <summary>
        /// Irse de un sector
        /// </summary>
        public void hideSector()
        {
            //deshabilitar Sonidos
            guardiasManager.hideGuardias();
            stopSounds();
            Game.instance.postProcessEffects.setAlarmModeEnable(false);

            //Liberar bananas
            Game.instance.bananasPool.freeAllMeshes();
            foreach (Banana b in bananas)
            {
                b.hide();
            }

            //Liberar cajas de bananas
            Game.instance.cajasBananasPool.freeAllMeshes();
            foreach (CajaBanana caja in cajasBananas)
            {
                caja.hide();
            }

            //Terminar CustomEvents
            foreach (CustomEvent customEvent in customEvents)
            {
                customEvent.end();
            }

            //Soltar todos los escondites moviles
            foreach (EsconditeMovil esm in esconditesMoviles)
            {
                esm.Agarrado = false;
                esm.Escondido = false;
            }
            //Soltar todos los escondites fijos
            foreach (EsconditeFijo esf in esconditesFijos)
            {
                esf.Escondido = false;
            }
        }

        /// <summary>
        /// Iniciar un sector
        /// </summary>
        public void initSector(MainCharacter mainCharacter, Conector nextConector)
        {
            //Musica
            Game.instance.sonidos.playMusic(Game.instance.sonidos.music_default);

            //Restaurar posicion de escondites moviles
            foreach (EsconditeMovil esc in esconditesMoviles)
            {
                esc.restorePosition();
            }

            //Iniciar personaje en sector
            currentConector = nextConector;
            mainCharacter.initInConectorPos(currentConector.Center, currentConector.Orientacion);
            guardiasManager.initGuardias();

            //Iniciar modelos de bananas
            foreach (Banana b in bananas)
            {
                b.init();
            }

            //Iniciar modelos de cajas de bananas
            foreach (CajaBanana caja in cajasBananas)
            {
                caja.init();
            }

            //Iniciar CustomEvents
            foreach (CustomEvent customEvent in customEvents)
            {
                customEvent.init();
            }
        }

        /// <summary>
        /// Actualizar estado del sector
        /// </summary>
        public void update()
        {
            //Actualizar posicion de enemigos
            guardiasManager.update();

            //Actualizar estado de customEvents
            foreach (CustomEvent customEvent in customEvents)
            {
                if (customEvent.Enabled)
                {
                    customEvent.update();
                }
            }
        }

        /// <summary>
        /// Render del sector
        /// </summary>
        public void render()
        {
            //Hacer FrustumCulling de los objetos que vale la pena
            frustumCulling();

            //Ajustar distancia de camara por colision contra objetos
            adaptCameraPosition();
            

            //Render de mallas estaticas
            foreach (TgcMeshShader mesh in staticMeshes)
            {
                Game.instance.shaders.setCorrectTechnique(mesh);
                mesh.render();
            }

            //Render de mallas sin colision
            foreach (TgcMeshShader mesh in meshesSinColision)
            {
                Game.instance.shaders.setCorrectTechnique(mesh);
                mesh.render();
            }

            //Guaridas
            guardiasManager.render();

            //Llaves
            foreach (Llave llave in llaves)
            {
                llave.render();
            }

            //Items animal
            foreach (ItemAnimal item in itemsAnimal)
            {
                item.render();
            }

            //Escondites moviles
            foreach (EsconditeMovil escondite in esconditesMoviles)
            {
                escondite.render();
            }

            //Escondites fijos
            foreach (EsconditeFijo escondite in esconditesFijos)
            {
                escondite.render();
            }

            //Ver si el personaje principal colisiona con algun conector (para mostrar su cartel)
            Conector conectorColision = checkConectores(Game.instance.mainCharacter);
            if (conectorColision != null)
            {
                conectorColision.render();
            }

            //Bananas
            foreach (Banana b in bananas)
            {
                b.render();
            }

            //Cajas de bananas
            foreach (CajaBanana caja in cajasBananas)
            {
                caja.render();
            }

            //CustomEvents
            foreach (CustomEvent customEvent in customEvents)
            {
                if (customEvent.Enabled)
                {
                    customEvent.render();
                }
            }
        }

        /// <summary>
        /// Hacer FrustumCulling de meshes estaticos y meshes sin colision
        /// Por fuerza bruta
        /// </summary>
        private void frustumCulling()
        {
            MainCharacter mainCharacter = Game.instance.mainCharacter;
            //Frustum Culling meshes estaticos
            foreach (TgcMeshShader mesh in staticMeshes)
            {
                //Frustum Culling brute force
                if (TgcCollisionUtils.classifyFrustumAABB(GuiController.Instance.Frustum, mesh.BoundingBox) == TgcCollisionUtils.FrustumResult.OUTSIDE)
                {
                    mesh.Enabled = false;
                }
                else
                {
                    mesh.Enabled = true;
                }
            }
            //Frustum Culling meshes sin colision
            foreach (TgcMeshShader mesh in meshesSinColision)
            {
                //Ver primero si esta dentro de un cierto radio de alcance del personaje
                if (mainCharacter.isInProximityRange(mesh.BoundingBox.PMin))
                {
                    //Frustum Culling brute force
                    if (TgcCollisionUtils.classifyFrustumAABB(GuiController.Instance.Frustum, mesh.BoundingBox) == TgcCollisionUtils.FrustumResult.OUTSIDE)
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
        /// Ajustar distancia de camara por colision contra objetos
        /// </summary>
        private void adaptCameraPosition()
        {
            GameThirdPersonCamera camera = Game.instance.mainCharacter.Camera;
            GameConfig.Camara camaraConfig = Game.instance.gameConfig.camara;
            MainCharacter mainCharacter = Game.instance.mainCharacter;
            camera.OffsetForward = mainCharacter.CurrentCameraOffsetForward;
            camera.OffsetHeight = camaraConfig.offsetY;
            Vector3 segmentA;
            Vector3 segmentB;
            camera.generateViewMatrix(out segmentA, out segmentB);
            Vector3 q;
            float minDistSq = FastMath.Pow2(camera.OffsetForward);
            foreach (ColliderObject collObj in objetosColisionables)
            {
                if (collObj.Enabled)
                {
                    if (TgcCollisionUtils.intersectSegmentAABB(segmentB, segmentA, collObj.BoundingBox, out q))
                    {
                        float distSq = (Vector3.Subtract(q, segmentB)).LengthSq();
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
        /// Busca si el personaje principal colisiona con un conector.
        /// En caso de que si, devuelve el proximo conector con el que conecta este, para que se pueda
        /// hacer el cambio de sector
        /// </summary>
        public Conector checkConectoresGetNext(MainCharacter mainCharacter)
        {
            Conector conector = checkConectores(mainCharacter);
            if (conector != null)
            {
                
            }
            return null;
        }

        /// <summary>
        /// Ejecutar la accion del conector, que puede ser cambiar de sector o cambiar de nivel.
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
                    throw new Exception("El conector: " + conector.Id + " quiere pasar al siguienteId inexistente: " + conector.SiguienteId);
                }
            }
        }

        /// <summary>
        /// Busca si el personaje principal colisiona con un conector.
        /// Devuelve el conector colisionado o null.
        /// </summary>
        public Conector checkConectores(MainCharacter mainCharacter)
        {
            foreach (Conector conector in conectores)
            {
                if (conector.Activo && mainCharacter.collideWithAABB(conector.BoundingBox))
                {
                    return conector;
                }
            }
            return null;
        }


        /// <summary>
        /// Activar el modo alarma.
        /// El personaje ha sido visto por un guardia
        /// </summary>
        public void setAlarmMode()
        {
            //No alertar a los otros
            //guardiasManager.alertAll();

            GameSounds sonidos = Game.instance.sonidos;
            //sonidos.playMusic(sonidos.music_alarm);
            sonidos.mp3Player.stop();
            sonidos.alarma_guardias.play(true);
            Game.instance.postProcessEffects.setAlarmModeEnable(true);
        }

        /// <summary>
        /// Busca si el personaje principal colisiona con alguna llave del sector.
        /// Si colisiona la agarra.
        /// </summary>
        public Llave checkLlaves(MainCharacter mainCharacter)
        {
            foreach (Llave llave in llaves)
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
        /// Busca si el personaje principal colisiona con algun item animal para rescatar.
        /// </summary>
        public ItemAnimal checkItemsAnimal(MainCharacter mainCharacter)
        {
            foreach (ItemAnimal item in itemsAnimal)
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
        /// Busca si el personaje principal colisiona con algun escondite fijo
        /// </summary>
        public EsconditeFijo checkEsconditesFijos(MainCharacter mainCharacter)
        {
            foreach (EsconditeFijo escondite in esconditesFijos)
            {
                if (mainCharacter.collideWithBSphere(escondite.BSphere))
                {
                    return escondite;
                }
            }
            return null;
        }

        /// <summary>
        /// Busca si el personaje principal colisiona con algun escondite movil
        /// </summary>
        public EsconditeMovil checkEsconditesMoviles(MainCharacter mainCharacter)
        {
            foreach (EsconditeMovil escondite in esconditesMoviles)
            {
                if (mainCharacter.collideWithAABB(escondite.BoundingBox))
                {
                    return escondite;
                }
            }
            return null;
        }

        /// <summary>
        /// Busca si el personaje principal colisiona con alguna plataforma de salto
        /// </summary>
        public Salto.Plataforma checkSaltos(MainCharacter mainCharacter)
        {
            return saltosManager.checkSaltos(mainCharacter);
        }

        /// <summary>
        /// Agarrar todas las bananas del sector que colisionan con el personaje
        /// </summary>
        public void checkBananas(MainCharacter mainCharacter)
        {
            foreach (Banana b in bananas)
            {
                b.agarrar(mainCharacter);
            }
        }

        /// <summary>
        /// Golpear todas las cajas de bananas que colisionan con el personaje
        /// </summary>
        public void golpearCajasBananas(MainCharacter mainCharacter)
        {
            foreach (CajaBanana caja in cajasBananas)
            {
                caja.golpear(mainCharacter);
            }
        }

        /// <summary>
        /// Para los sonidos del sector
        /// </summary>
        public void stopSounds()
        {
            GameSounds sonidos = Game.instance.sonidos;
            sonidos.alarma_guardias.stop();
            sonidos.mp3Player.closeFile();
        }

        /// <summary>
        /// Elimina todos los recursos del sector
        /// </summary>
        public void unload()
        {
            //staticMeshes
            foreach (TgcMeshShader mesh in staticMeshes)
            {
                mesh.dispose();
            }
            staticMeshes.Clear();
            staticMeshes = null;

            //meshesSinColision
            foreach (TgcMeshShader mesh in meshesSinColision)
            {
                mesh.dispose();
            }
            meshesSinColision.Clear();
            meshesSinColision = null;

            //guardias
            guardiasManager.dispose();
            guardiasManager = null;

            //Llaves
            llaves.Clear();
            llaves = null;

            //Items animal
            foreach (ItemAnimal item in itemsAnimal)
            {
                item.dispose();
            }
            itemsAnimal.Clear();
            itemsAnimal = null;

            //Escondites moviles
            foreach (EsconditeMovil escondite in esconditesMoviles)
            {
                escondite.dispose();
            }
            esconditesMoviles.Clear();
            esconditesMoviles = null;

            //Escondites fijos
            foreach (EsconditeFijo escondite in esconditesFijos)
            {
                escondite.dispose();
            }
            esconditesFijos.Clear();
            esconditesFijos = null;

            //Conectores
            foreach (Conector conector in conectores)
            {
                conector.dispose();
            }
            conectores.Clear();
            conectores = null;
            currentConector = null;

            //colisionesLogicas
            foreach (TgcBoundingBox colLogicas in colisionesLogicas)
            {
                colLogicas.dispose();
            }
            colisionesLogicas.Clear();
            colisionesLogicas = null;

            //waypointsAtaqueManager
            waypointsAtaqueManager.dispose();
            waypointsAtaqueManager = null;

            //customEvents
            foreach (CustomEvent customEvent in customEvents)
            {
                customEvent.dispose();
            }
            customEvents.Clear();
            customEvents = null;

            //saltosManager
            saltosManager.dispose();
            saltosManager = null;

            //Bananas
            foreach (Banana b in bananas)
            {
                b.dispose();
            }
            bananas.Clear();
            bananas = null;

            //CajaBanana
            foreach (CajaBanana c in cajasBananas)
            {
                c.dispose();
            }
            cajasBananas.Clear();
            cajasBananas = null;

            
            //objetosColisionables
            foreach (ColliderObject c in objetosColisionables)
            {
                c.dispose();
            }
            objetosColisionables.Clear();
            objetosColisionables = null;

            //otros
            level = null;
        }




        
    }
}
