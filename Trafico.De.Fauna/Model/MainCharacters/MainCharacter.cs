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

using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Utils;
using TgcViewer.Model.Billboards;
using TgcViewer.Model.Camera;
using TgcViewer.Model.Escondites;
using TgcViewer.Model.ItemsAnimal;
using TgcViewer.Model.Levels;
using TgcViewer.Model.Meshes;
using TgcViewer.Model.Particles;
using TgcViewer.Model.Saltos;
using TgcViewer.Model.Sonidos;

namespace TgcViewer.Model.MainCharacters
{
    /// <summary>
    /// Personaje principal del juego
    /// </summary>
    public class MainCharacter
    {
        /// <summary>
        /// Cuanto para adelelante movemos el efecto de punch con billboard.
        /// Multiplica la direccion actual del personaje.
        /// </summary>
        const float PUNCH_BILLBOARD_POS_SCALE = 20;


        TgcSkeletalMesh mesh;
        SphereCollisionManager collisionManager;
        State currentState;
        EsconditeMovil esconditeMovil;
        EsconditeFijo esconditeFijo;
        Salto.Plataforma plataformaSalto;
        bool punching;
        bool facingBackward;
        ParticleEmitter stepsEmitter;
        ParticleEmitter punchEmitter;
        QuadAnimatedBillboard punchBillboard;
        TemporalSound sonidoPasosCorrer;
        TemporalSound sonidoPasosCaminandoEsconditeMovil;


        //Salto
        float origPosY;
        float origAngleRot;
        Vector3 jumpDir;
        float jumpDistanceToTravel;
        float jumpPosY;
        bool jumpYPositive;
        float beforeJumpRotY;


        TgcBoundingSphere characterSphere;
        /// <summary>
        /// BoundingSphere del personaje
        /// </summary>
        public TgcBoundingSphere CharacterSphere
        {
            get { return characterSphere; }
        }

        /// <summary>
        /// Posicion actual
        /// Varia segun si ya se aplico el moveWithCollisions()
        /// </summary>
        public Vector3 CurrentPos
        {
            get { return mesh.Position; }
        }

        bool hidden;
        /// <summary>
        /// Indica si el personaje esta escondido
        /// </summary>
        public bool Hidden
        {
            get { return hidden; }
        }

        bool crouched;
        /// <summary>
        /// Indica si el personaje esta agachado
        /// </summary>
        public bool Crouched
        {
            get { return crouched; }
        }

        GameThirdPersonCamera camera;
        /// <summary>
        /// Camara en tercera persona que se ajusta segun los movimientos del personaje
        /// </summary>
        public GameThirdPersonCamera Camera
        {
            get { return camera; }
        }

        int bananasAgarradas;
        /// <summary>
        /// Cantidad de bananas agarradas
        /// </summary>
        public int BananasAgarradas
        {
            get { return bananasAgarradas; }
        }

        int itemsAnimalLiberados;
        /// <summary>
        /// Cantidad de ItemsAnimal liberados
        /// </summary>
        public int ItemsAnimalLiberados
        {
            get { return itemsAnimalLiberados; }
        }

        int vidas;
        /// <summary>
        /// Vidas del personaje
        /// </summary>
        public int Vidas
        {
            get { return vidas; }
            set { vidas = value; }
        }

        float currentCameraOffsetForward;
        /// <summary>
        /// Offset Z actual de la camara
        /// </summary>
        public float CurrentCameraOffsetForward
        {
            get { return currentCameraOffsetForward; }
        }

        float currentCameraMinOffsetForward;
        /// <summary>
        /// Offset minimp de Z actual de la camara
        /// </summary>
        public float CurrentMinCameraOffsetForward
        {
            get { return currentCameraMinOffsetForward; }
        }


        public MainCharacter()
        {
        }

        public void loadModel()
        {
            //Cargar mesh
            string meshPath = GameModel.instance.gameConfig.basePath + "MainCharacterMono\\";
            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            mesh = skeletalLoader.loadMeshAndAnimationsFromFile(
                meshPath + "Mono-TgcSkeletalMesh.xml",
                meshPath,
                new string[] { 
                    meshPath + "StandBy-TgcSkeletalAnim.xml",
                    meshPath + "Run-TgcSkeletalAnim.xml",
                    meshPath + "Crouch-TgcSkeletalAnim.xml",
                    meshPath + "CrouchLeft-TgcSkeletalAnim.xml",
                    meshPath + "CrouchRight-TgcSkeletalAnim.xml",
                    meshPath + "WalkCarryingObject-TgcSkeletalAnim.xml",
                    meshPath + "StandByCarryingObject-TgcSkeletalAnim.xml",
                    meshPath + "Jumping-TgcSkeletalAnim.xml",
                    meshPath + "Punch-TgcSkeletalAnim.xml"
                });
            mesh.AutoTransformEnable = true;
            mesh.rotateY(Geometry.DegreeToRadian(180f));
            origAngleRot = FastMath.PI;
            origPosY = 0;
            mesh.AnimationEnds += new TgcSkeletalMesh.AnimationEndsHandler(mesh_AnimationEnds);

            //BoundingSphere que va a usar el mesh
            mesh.AutoUpdateBoundingBox = false;
            GameConfig.Fisica fisica = GameModel.instance.gameConfig.fisica;
            characterSphere = new TgcBoundingSphere(new Vector3(0, fisica.boundingSphereCenterY, 0), fisica.boundingSphereRadio);

            //Crear manejador de colisiones
            collisionManager = new SphereCollisionManager();
            collisionManager.GravityEnabled = false;
            collisionManager.GravityForce = new Vector3(0f, fisica.gravedad, 0f);
            collisionManager.SlideFactor = fisica.slideFactor;

            //Configurar camara en Tercer Persona personalizada
            GameConfig.Camara camaraConfig = GameModel.instance.gameConfig.camara;
            camera = new GameThirdPersonCamera();
            GameModel.instance.Camara = camera;
            camera.Target = mesh.Position;
            camera.OffsetHeight = camaraConfig.offsetY;
            camera.OffsetForward = camaraConfig.offsetZ;
            camera.TargetDisplacement = new Vector3(0, camaraConfig.displacementY, 0);

            //Particulas para las pisadas del personaje
            stepsEmitter = new ParticleEmitter(GameModel.instance.gameConfig.basePath + "Shaders\\Particles\\pisada_mono.png", 4);
            stepsEmitter.Dispersion = 400;
            stepsEmitter.ParticleTimeToLive = 0.5f;
            stepsEmitter.CreationFrecuency = 0.1f;
            stepsEmitter.Speed = new Vector3(5, 5, 5);
            stepsEmitter.MinSizeParticle = 2f;
            stepsEmitter.MaxSizeParticle = 1f;

            //Particulas para el punch al romper cajas
            punchEmitter = new ParticleEmitter(GameModel.instance.gameConfig.basePath + "Shaders\\Particles\\punch_mono.png", 2);
            punchEmitter.Dispersion = 100;
            punchEmitter.ParticleTimeToLive = 0.4f;
            punchEmitter.CreationFrecuency = 1f;
            punchEmitter.Speed = new Vector3(0, 10, 0);
            punchEmitter.MinSizeParticle = 40f;
            punchEmitter.MaxSizeParticle = 40f;

            punchBillboard = new QuadAnimatedBillboard(GameModel.instance.gameConfig.basePath + "Billboards\\personaje_punch_effect.png", new Size(64, 64), 16, 16);
            punchBillboard.Size = new Vector2(50, 50);
            punchBillboard.Position = mesh.Position;
            punchBillboard.Enabled = false;

            //Sonidos
            sonidoPasosCorrer = new TemporalSound();
            sonidoPasosCorrer.Sound = GameModel.instance.sonidos.personaje_pasos_correr;
            sonidoPasosCorrer.Frecuency = fisica.frecuenciaSonidoCorrer;
            sonidoPasosCorrer.Enabled = false;
            sonidoPasosCaminandoEsconditeMovil = new TemporalSound();
            sonidoPasosCaminandoEsconditeMovil.Sound = GameModel.instance.sonidos.personaje_pasos_cargando_escondite;
            sonidoPasosCaminandoEsconditeMovil.Frecuency = fisica.frecuenciaSonidoCaminar;
            sonidoPasosCaminandoEsconditeMovil.Enabled = false;
        }

        //Cargar valores iniciales para el primer nivel de todo el juego
        public void resetForFirstLevel()
        {
            vidas = 5;
        }

        /// <summary>
        /// Cargar valores iniciales para empezar un nuevo nivel
        /// </summary>
        public void resetForNewLevel()
        {
            bananasAgarradas = 0;
            itemsAnimalLiberados = 0;
            stepsEmitter.Playing = false;
            punchEmitter.Playing = false;
            punchBillboard.Enabled = false;
            facingBackward = false;
            currentCameraOffsetForward = GameModel.instance.gameConfig.camara.offsetZ;
            currentCameraMinOffsetForward = GameModel.instance.gameConfig.camara.minOffsetZ;
            punching = false;
        }

        /// <summary>
        /// Configurar personaje para arrancar en un sector
        /// </summary>
        public void initInConectorPos(Vector3 conectorPos, GameUtils.Orientacion orientacion)
        {
            //Posicionar
            Vector3 diffPos = Vector3.Subtract(conectorPos, mesh.Position);
            mesh.move(diffPos);
            characterSphere.moveCenter(diffPos);

            //Rotar hacia la orientacion indicada
            restoreFaceForward();
            mesh.Rotation = new Vector3(0, origAngleRot, 0);
            float angleRot = 0;
            switch (orientacion)
            {
                case GameUtils.Orientacion.Norte:
                    angleRot = 0;
                    break;
                case GameUtils.Orientacion.Sur:
                    angleRot = FastMath.PI;
                    break;
                case GameUtils.Orientacion.Este:
                    angleRot = FastMath.PI_HALF;
                    break;
                case GameUtils.Orientacion.Oeste:
                    angleRot = -FastMath.PI_HALF;
                    break;
            }
            mesh.rotateY(angleRot);
            camera.RotationY = angleRot;

            //Pasar a estado normal
            setNormal();
        }


        public void render(float elapsedTime)
        {
            //Hacer que la camara siga al personaje en su nueva posicion
            targetCamera();

            //Render personaje
            mesh.animateAndRender(elapsedTime);

            //Render pisadas
            GameModel.instance.currentLevel.AlphaObjects.Add(stepsEmitter);

            //Render punch
            //Game.instance.currentLevel.AlphaObjects.Add(punchEmitter);
            
            //Desactivo hasta que se mejore
            //Game.instance.currentLevel.AlphaObjects.Add(punchBillboard);

            //Sonidos temporales
            sonidoPasosCorrer.update(elapsedTime);
            sonidoPasosCaminandoEsconditeMovil.update(elapsedTime);
        }

        /// <summary>
        /// Hacer que la camara siga al personaje en su nueva posicion
        /// </summary>
        private void targetCamera()
        {
            camera.Target = characterSphere.Center;
        }

        /// <summary>
        /// Colision de un AABB con el BoundingSphere del personaje
        /// </summary>
        public bool collideWithAABB(TgcBoundingAxisAlignBox aabb)
        {
            return TgcCollisionUtils.testSphereAABB(characterSphere, aabb);
        }

        /// <summary>
        /// Colision de un BoundingSphere con el BoundingSphere del personaje
        /// </summary>
        public bool collideWithBSphere(TgcBoundingSphere sphere)
        {
            return TgcCollisionUtils.testSphereSphere(characterSphere, sphere);
        }

        /// <summary>
        /// Indica si un objeto ubicado en la posicion especificada se encuentra dentro del
        /// radio de alcance visible por el personaje
        /// </summary>
        public bool isInProximityRange(Vector3 pos)
        {
            //Ver si esta dentro del radio de vision
            return Vector3.Subtract(pos, this.CurrentPos).LengthSq() < GameModel.instance.gameConfig.camara.distanciaVisualSq;
        }


        /// <summary>
        /// Agrega bananas al personaje principal.
        /// Si es multiplo de 100 gana una vida
        /// </summary>
        public void agregarBananas(int n)
        {
            int enteros = bananasAgarradas / 100;
            bananasAgarradas += n;
            int enteros2 = bananasAgarradas / 100;
            if (enteros2 > enteros)
            {
                vidas += enteros2 - enteros;
                bananasAgarradas = bananasAgarradas % 100;
                GameModel.instance.sonidos.personaje_ganar_vida.play(false);
                GameModel.instance.hud.efectoGanarVida();
            }
        }

        /// <summary>
        /// Agregar un item animal liberado.
        /// Si se liberaron todos, ganaste el juego.
        /// </summary>
        public void agregarItemAnimalLiberado(ItemAnimal itemAnimal)
        {
            itemsAnimalLiberados++;
            if (itemsAnimalLiberados == GameModel.instance.currentLevel.TotalItemsAnimal)
            {
                GameModel.instance.transiciones.initGameWin();
                GameModel.instance.currentGameMode = GameModel.GameMode.GameWin;
            }
        }



        #region Maquina de estados


        /// <summary>
        /// Estados del personaje principal
        /// </summary>
        enum State
        {
            Normal,
            Agachado,
            AgachadoEsconditeFijo,
            CargandoEsconditeMovil,
            AgachadoEsconditeMovil,
            SaltandoPlataformas,
        }


        /// <summary>
        /// Actualizar estado del personaje
        /// </summary>
        public void update(float elapsedTime)
        {
            switch (currentState)
            {
                case State.Normal:
                    normal(elapsedTime);
                    break;
                case State.Agachado:
                    agachado(elapsedTime);
                    break;
                case State.AgachadoEsconditeFijo:
                    agachadoEsconditeFijo(elapsedTime);
                    break;
                case State.CargandoEsconditeMovil:
                    cargandoEsconditeMovil(elapsedTime);
                    break;
                case State.AgachadoEsconditeMovil:
                    agachadoEsconditeMovil(elapsedTime);
                    break;
                case State.SaltandoPlataformas:
                    saltandoPlataformas(elapsedTime);
                    break;
            }
        }

        /// <summary>
        /// Pasar a estado normal
        /// </summary>
        private void setNormal()
        {
            mesh.playAnimation("StandBy", true);
            hidden = false;
            crouched = false;
            currentState = State.Normal;
            punching = false;
            punchEmitter.Enabled = false;
            restoreFaceForward();
            sonidoPasosCorrer.Enabled = false;
            sonidoPasosCaminandoEsconditeMovil.Enabled = false;

            //Acomodar BoundingSphere de parado
            setBoundingSphereParado();
        }


        /// <summary>
        /// Estado normal del personaje
        /// </summary>
        private void normal(float elapsedTime)
        {
            //obtener velocidades de Modifiers
            GameConfig.Fisica fisica = GameModel.instance.gameConfig.fisica;
            GameConfig.Keys keys = GameModel.instance.gameConfig.keys;
            TgcD3dInput d3dInput = GameModel.instance.Input;
            Level level = GameModel.instance.currentLevel;

            //Crouch
            if (d3dInput.keyDown(keys.crouch))
            {
                //Ver si hay un escondite fijo
                EsconditeFijo esconditeFijo = level.CurrentSector.checkEsconditesFijos(this);
                if (esconditeFijo != null)
                {
                    this.esconditeFijo = esconditeFijo;
                    esconditeFijo.Escondido = true;
                    setAgachadoEsconditeFijo();
                    return;
                }

                //Sino agacharse
                setAgachado();
                return;
            }

            //Action
            if (d3dInput.keyPressed(keys.action))
            {
                //Ver si hay colision con algun conector
                Conector conectorCollision = level.CurrentSector.checkConectores(this);
                if (conectorCollision != null)
                {
                    //ejecutar conector
                    level.CurrentSector.executeConector(conectorCollision);
                    return;
                }

                //Ver si hay colision con un item Animal para rescatar
                ItemAnimal item = level.CurrentSector.checkItemsAnimal(this);
                if (item != null)
                {
                    bool liberado = item.intentarLiberar();
                    return;
                }

                //Ver si hay un escondite movil para agarrar
                EsconditeMovil esconditeMovil = level.CurrentSector.checkEsconditesMoviles(this);
                if (esconditeMovil != null)
                {
                    this.esconditeMovil = esconditeMovil;
                    this.esconditeMovil.Agarrado = true;
                    this.esconditeMovil.Escondido = false;
                    setCargandoEsconditeMovil();
                    return;
                }
            }

            //Action pero manteniendo
            if (d3dInput.keyDown(keys.action))
            {
                //Ver si hay colision con alguna plataforma de Salto
                Salto.Plataforma plataformaSaltoCollision = level.CurrentSector.checkSaltos(this);
                if (plataformaSaltoCollision != null)
                {
                    //Solo puede saltar si antes estuvo corriendo
                    if (/*mesh.CurrentAnimation.Name == "Run"*/ true)
                    {
                        this.plataformaSalto = plataformaSaltoCollision;
                        GameModel.instance.sonidos.personaje_saltar.play();
                        setSaltandoPlataformas();
                        return;
                    }
                }

                //Hacer golpe, solo si ya no estaba golpeando de antes
                if (!punching)
                {
                    punching = true;
                    mesh.playAnimation("Punch", false);
                    sonidoPasosCorrer.Enabled = false;
                    GameModel.instance.sonidos.personaje_golpe.play();

                    //Intentar golpear cajas de bananas cercanas
                    level.CurrentSector.golpearCajasBananas(this);

                    //Activar efecto de particulas para el golpe
                    punchEmitter.Enabled = true;
                    punchEmitter.Playing = true;
                    punchBillboard.Enabled = true;
                }
            }




            //Movimiento
            float moveForward = 0f;
            float rotate = 0;
            bool moving = false;
            bool rotating = false;

            //Adelante
            if (d3dInput.keyDown(keys.forward))
            {
                moveForward = -fisica.velocidadCaminar;
                moving = true;
            }
            //Atras
            else if (d3dInput.keyDown(keys.backward))
            {
                moveForward = fisica.velocidadCaminar;
                moving = true;
            }

            //Derecha
            if (d3dInput.keyDown(keys.turnRight))
            {
                rotate = fisica.velocidadRotacion;
                rotating = true;
            }
            //Izquierda
            else if (d3dInput.keyDown(keys.turnLeft))
            {
                rotate = -fisica.velocidadRotacion;
                rotating = true;
            }

            //Si esta mirando para atras (del cuadro anterior), enderezar antes de aplicar rotaciones
            restoreFaceForward();

            //Si hubo rotacion
            if (rotating)
            {
                //Rotar mesh y camara
                float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                mesh.rotateY(rotAngle);
                camera.rotateY(rotAngle);
            }


            //Vector de movimiento
            Vector3 movementVector = Vector3.Empty;
            if (moving)
            {
                if (!punching)
                {
                    mesh.playAnimation("Run", true);

                    //Desactivar sonido de correr
                    sonidoPasosCorrer.Enabled = true;
                }

                //Ajustar por el tiempo transcurrido
                moveForward *= elapsedTime;

                //Aplicar movimiento, desplazarse en base a la rotacion actual del mesh
                movementVector = new Vector3(
                    FastMath.Sin(mesh.Rotation.Y) * moveForward,
                    0,
                    FastMath.Cos(mesh.Rotation.Y) * moveForward
                    );

                //Si avanza hacia atrás, rotar personaje para que mire a la camara (pero que no afecte en nada su movimiento, solo decorativo)
                if (moveForward > 0)
                {
                    faceBackward();
                }
            }
            else
            {
                if (!punching)
                {
                    mesh.playAnimation("StandBy", true);

                    //Desactivar sonido de correr
                    sonidoPasosCorrer.Enabled = false;
                }
            }

            //Deteccion de colisiones con meshes estaticos del Sector
            Vector3 newPosition = moveWithCollisions(level.CurrentSector.ObjetosColisionables, movementVector);

            //Colision con llaves del sector
            level.CurrentSector.checkLlaves(this);

            //Agarrar bananas
            level.CurrentSector.checkBananas(this);

            //Habilitar efecto de pisadas del personaje
            if (moving)
            {
                stepsEmitter.Position = CurrentPos;
                stepsEmitter.Playing = true;
            }
            else
            {
                stepsEmitter.Playing = false;
            }

            //Si esta pegando, posicionar efecto de particulas de punch
            if (punching)
            {
                punchEmitter.Position = CurrentPos;

                //Ubicar billboard de punch un poco adelante del personaje
                punchBillboard.Position = CurrentPos + Vector3.Scale(getCurrentVecRot(), PUNCH_BILLBOARD_POS_SCALE);
                punchBillboard.RotationY = camera.RotationY;
            }
        }

        

        /// <summary>
        /// Mover al personaje con deteccion de colisiones por el escenario
        /// </summary>
        public Vector3 moveWithCollisions(List<ColliderObject> objetosColisionables, Vector3 movementVector)
        {
            Vector3 realMovement = collisionManager.moveCharacter(characterSphere, movementVector, objetosColisionables);
            mesh.move(realMovement);
            return realMovement;
        }

        /// <summary>
        /// Evento cuando termina de ejecutarse una animacion
        /// </summary>
        void mesh_AnimationEnds(TgcSkeletalMesh mesh)
        {
            if (currentState == State.Normal && mesh.CurrentAnimation.Name == "Punch")
            {
                punching = false;
                punchEmitter.Playing = false;
                punchBillboard.Enabled = false;
            }
        }

        /// <summary>
        /// Rotar el personaje para que mire hacia la camara
        /// </summary>
        private void faceBackward()
        {
            mesh.rotateY(FastMath.PI);
            facingBackward = true;
            currentCameraOffsetForward = GameModel.instance.gameConfig.camara.facingBackwardOffsetZ;
            currentCameraMinOffsetForward = GameModel.instance.gameConfig.camara.facingBackwardMinOffsetZ;
            camera.OffsetForward = currentCameraOffsetForward;
        }

        /// <summary>
        /// Recuperar la rotacion del personaje si es que antes estaba girado mirando a la camara
        /// </summary>
        private void restoreFaceForward()
        {
            if (facingBackward)
            {
                mesh.rotateY(FastMath.PI);
                facingBackward = false;
                currentCameraOffsetForward = GameModel.instance.gameConfig.camara.offsetZ;
                currentCameraMinOffsetForward = GameModel.instance.gameConfig.camara.minOffsetZ;
                camera.OffsetForward = currentCameraOffsetForward;
            }
        }



        /// <summary>
        /// Pasar a estado agachado
        /// </summary>
        private void setAgachado()
        {
            currentState = State.Agachado;
            crouched = true;
            mesh.playAnimation("Crouch", true);
            punching = false;
            punchEmitter.Enabled = false;
            punchBillboard.Enabled = false;
            restoreFaceForward();
            sonidoPasosCorrer.Enabled = false;
            sonidoPasosCaminandoEsconditeMovil.Enabled = false;

            //Acomodar BoundingSphere de agachado
            setBoundingSphereAgachado();

            //Sonido de agacharse
            GameModel.instance.sonidos.personaje_agacharse.play(false);
        }

        /// <summary>
        /// Estado agachado del personaje
        /// </summary>
        private void agachado(float elapsedTime)
        {
            //Sigue agachado mientras se mantiene presionado crouch
            TgcD3dInput d3dInput = GameModel.instance.Input;
            GameConfig.Keys keys = GameModel.instance.gameConfig.keys;

            if (d3dInput.keyDown(keys.crouch))
            {
                //No se puede mover, solo rotar
                GameConfig.Fisica fisica = GameModel.instance.gameConfig.fisica;
                float rotate = 0;
                bool rotating = false;

                //Derecha
                if (d3dInput.keyDown(keys.turnRight))
                {
                    rotate = fisica.velocidadRotacion;
                    rotating = true;
                }

                //Izquierda
                if (d3dInput.keyDown(keys.turnLeft))
                {
                    rotate = -fisica.velocidadRotacion;
                    rotating = true;
                }

                //Si hubo rotacion
                if (rotating)
                {
                    //Rotar mesh y camara
                    float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                    mesh.rotateY(rotAngle);
                    camera.rotateY(rotAngle);

                    //Animacion
                    string rotAnim = rotate > 0 ? "CrouchRight" : "CrouchLeft";
                    mesh.playAnimation(rotAnim, true);
                }
                else
                {
                    mesh.playAnimation("Crouch", true);
                }
            }
            //Si suelta pasa a normal
            else if (d3dInput.keyUp(keys.crouch))
            {
                setNormal();
            }
        }

        /// <summary>
        /// Pasar a estado agachado en un escondite fijo
        /// </summary>
        private void setAgachadoEsconditeFijo()
        {
            currentState = State.AgachadoEsconditeFijo;
            crouched = true;
            hidden = true;
            mesh.playAnimation("Crouch", true);
            punching = false;
            punchEmitter.Enabled = false;
            punchBillboard.Enabled = false;
            restoreFaceForward();
            sonidoPasosCorrer.Enabled = false;
            sonidoPasosCaminandoEsconditeMovil.Enabled = false;

            //Acomodar BoundingSphere de agachado
            setBoundingSphereAgachado();

            //Sonido de agacharse
            GameModel.instance.sonidos.personaje_agacharse.play(false);
        }

        /// <summary>
        /// Estado agachado en un escondite fijo
        /// </summary>
        private void agachadoEsconditeFijo(float elapsedTime)
        {
            //Sigue agachado mientras se mantiene presionado crouch
            TgcD3dInput d3dInput = GameModel.instance.Input;
            GameConfig.Keys keys = GameModel.instance.gameConfig.keys;

            if (d3dInput.keyDown(keys.crouch))
            {
                //No se puede mover, solo rotar
                GameConfig.Fisica fisica = GameModel.instance.gameConfig.fisica;
                float rotate = 0;
                bool rotating = false;

                //Derecha
                if (d3dInput.keyDown(keys.turnRight))
                {
                    rotate = fisica.velocidadRotacion;
                    rotating = true;
                }

                //Izquierda
                if (d3dInput.keyDown(keys.turnLeft))
                {
                    rotate = -fisica.velocidadRotacion;
                    rotating = true;
                }

                //Si hubo rotacion
                if (rotating)
                {
                    //Rotar mesh y camara
                    float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                    mesh.rotateY(rotAngle);
                    camera.rotateY(rotAngle);

                    //Animacion
                    string rotAnim = rotate > 0 ? "CrouchRight" : "CrouchLeft";
                    mesh.playAnimation(rotAnim, true);
                }
                else
                {
                    mesh.playAnimation("Crouch", true);
                }
            }
            //Si suelta sale del escondite fijo y pasa a normal
            else if (d3dInput.keyUp(keys.crouch))
            {
                this.esconditeFijo.Escondido = false;
                this.esconditeFijo = null;
                setNormal();
            }
        }


        /// <summary>
        /// Acomodar BoundingSphere de agachado
        /// </summary>
        private void setBoundingSphereAgachado()
        {
            GameConfig.Fisica fisica = GameModel.instance.gameConfig.fisica;
            characterSphere.setValues(
                new Vector3(characterSphere.Center.X, fisica.boundingSphereAgachadoCenterY, characterSphere.Center.Z),
                fisica.boundingSphereRadioAgachado);
        }

        /// <summary>
        /// Acomodar BoundingSphere de parado
        /// </summary>
        private void setBoundingSphereParado()
        {
            GameConfig.Fisica fisica = GameModel.instance.gameConfig.fisica;
            characterSphere.setValues(
                new Vector3(characterSphere.Center.X, fisica.boundingSphereCenterY, characterSphere.Center.Z),
                fisica.boundingSphereRadio);
        }

        /// <summary>
        /// Pasar a estado cargando un escondite movil
        /// </summary>
        private void setCargandoEsconditeMovil()
        {
            currentState = State.CargandoEsconditeMovil;
            crouched = false;
            hidden = false;
            mesh.playAnimation("StandByCarryingObject", true);
            punching = false;
            punchEmitter.Enabled = false;
            punchBillboard.Enabled = false;
            restoreFaceForward();
            sonidoPasosCorrer.Enabled = false;
            sonidoPasosCaminandoEsconditeMovil.Enabled = false;

            //Acomodar BoundingSphere de parado
            setBoundingSphereParado();

            //Mover escondite arriba del personaje
            Vector3 objCenter = esconditeMovil.BoundingBox.calculateBoxCenter();
            objCenter.Y = esconditeMovil.BoundingBox.PMin.Y;
            Vector3 objDiff = characterSphere.Center - objCenter;
            objDiff.Y += characterSphere.Radius;
            this.esconditeMovil.Mesh.move(objDiff);

            //Marcar como que no esta escondido
            this.esconditeMovil.Escondido = false;

            //Sonido de agarrar escondite
            GameModel.instance.sonidos.personaje_levantar_escondite_movil.play(false);
        }

        /// <summary>
        /// Estado cargando escondite movil del personaje
        /// </summary>
        private void cargandoEsconditeMovil(float elapsedTime)
        {
            //Leer input
            var d3dDevice = D3DDevice.Instance.Device;

            //obtener velocidades de Modifiers
            GameConfig.Fisica fisica = GameModel.instance.gameConfig.fisica;
            GameConfig.Keys keys = GameModel.instance.gameConfig.keys;
            TgcD3dInput d3dInput = GameModel.instance.Input;
            Level level = GameModel.instance.currentLevel;

            //Crouch, esconderse con el objeto
            if (d3dInput.keyDown(keys.crouch))
            {
                setAgachadoEsconditeMovil();
                return;
            }

            //Action, soltar escondite
            if (d3dInput.keyPressed(keys.action))
            {
                //Dejar escondite en el piso
                Vector3 objSize = esconditeMovil.BoundingBox.calculateSize();
                esconditeMovil.Mesh.move(0, -objSize.Y, 0);
                esconditeMovil.Agarrado = false;
                esconditeMovil.Escondido = false;
                esconditeMovil = null;
                
                setNormal();
                return;
            }



            //Movimiento
            float moveForward = 0f;
            float rotate = 0;
            bool moving = false;
            bool rotating = false;

            //Adelante
            if (d3dInput.keyDown(keys.forward))
            {
                moveForward = -fisica.velocidadCargandoEsconditeMovil;
                moving = true;
            }
            //Atras
            else if (d3dInput.keyDown(keys.backward))
            {
                moveForward = fisica.velocidadCargandoEsconditeMovil;
                moving = true;
            }

            //Derecha
            if (d3dInput.keyDown(keys.turnRight))
            {
                rotate = fisica.velocidadRotacion;
                rotating = true;
            }
            //Izquierda
            else if (d3dInput.keyDown(keys.turnLeft))
            {
                rotate = -fisica.velocidadRotacion;
                rotating = true;
            }

            //Si esta mirando para atras (del cuadro anterior), enderezar antes de aplicar rotaciones
            restoreFaceForward();

            //Si hubo rotacion
            if (rotating)
            {
                //Rotar mesh y camara
                float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                mesh.rotateY(rotAngle);
                camera.rotateY(rotAngle);
            }


            //Si hubo desplazamiento
            if (moving)
            {
                mesh.playAnimation("WalkCarryingObject", true);

                //Activar sonido de caminar cargando escondite
                sonidoPasosCaminandoEsconditeMovil.Enabled = true;
            }
            else
            {
                mesh.playAnimation("StandByCarryingObject", true);

                //Desactivar sonido de caminar cargando escondite
                sonidoPasosCaminandoEsconditeMovil.Enabled = false;
            }



            //Vector de movimiento
            Vector3 movementVector = Vector3.Empty;
            if (moving)
            {
                //Ajustar por el tiempo transcurrido
                moveForward *= elapsedTime;

                //Aplicar movimiento, desplazarse en base a la rotacion actual del mesh
                movementVector = new Vector3(
                    FastMath.Sin(mesh.Rotation.Y) * moveForward,
                    0,
                    FastMath.Cos(mesh.Rotation.Y) * moveForward
                    );

                //Si avanza hacia atrás, rotar personaje para que mire a la camara (pero que no afecte en nada su movimiento, solo decorativo)
                if (moveForward > 0)
                {
                    faceBackward();
                }
            }

            //Deteccion de colisiones con meshes estaticos del Sector
            Vector3 realMovement = moveWithCollisions(level.CurrentSector.ObjetosColisionables, movementVector);

            //Mover tambien escondite movil
            this.esconditeMovil.Mesh.move(realMovement);

            //Colision con llaves del sector
            level.CurrentSector.checkLlaves(this);

            //Agarrar bananas
            level.CurrentSector.checkBananas(this);


            //Colision con llaves del sector
            level.CurrentSector.checkLlaves(this);
        }

        /// <summary>
        /// Pasar a estado Agachado con escondite movil
        /// </summary>
        private void setAgachadoEsconditeMovil()
        {
            currentState = State.AgachadoEsconditeMovil;
            crouched = true;
            hidden = true;
            mesh.playAnimation("Crouch", true);
            punching = false;
            punchEmitter.Enabled = false;
            punchBillboard.Enabled = false;
            restoreFaceForward();
            sonidoPasosCorrer.Enabled = false;
            sonidoPasosCaminandoEsconditeMovil.Enabled = false;

            //Acomodar BoundingSphere de agachado
            setBoundingSphereAgachado();

            //Poner escondite en el piso
            Vector3 objSize = esconditeMovil.BoundingBox.calculateSize();
            esconditeMovil.Mesh.move(0, -objSize.Y, 0);

            //Marcar escondite como escondido
            this.esconditeMovil.Escondido = true;

            //Sonido de agacharse
            GameModel.instance.sonidos.personaje_agacharse.play(false);
        }

        /// <summary>
        /// Estado agachado escondite movil del personaje
        /// </summary>
        private void agachadoEsconditeMovil(float elapsedTime)
        {
            //Sigue escondido agachado con el objeto mientras se mantiene presionado crouch
            TgcD3dInput d3dInput = GameModel.instance.Input;
            GameConfig.Keys keys = GameModel.instance.gameConfig.keys;

            if (d3dInput.keyDown(keys.crouch))
            {
                //No se puede mover, solo rotar
                GameConfig.Fisica fisica = GameModel.instance.gameConfig.fisica;
                float rotate = 0;
                bool rotating = false;

                //Derecha
                if (d3dInput.keyDown(keys.turnRight))
                {
                    rotate = fisica.velocidadRotacion;
                    rotating = true;
                }

                //Izquierda
                if (d3dInput.keyDown(keys.turnLeft))
                {
                    rotate = -fisica.velocidadRotacion;
                    rotating = true;
                }

                //Si hubo rotacion
                if (rotating)
                {
                    //Rotar mesh y camara
                    float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                    mesh.rotateY(rotAngle);
                    camera.rotateY(rotAngle);
                }
            }
            //Si suelta, el objeto se para cargando el escondite
            else if (d3dInput.keyUp(keys.crouch))
            {
                setCargandoEsconditeMovil();

            }
        }

        /// <summary>
        /// Pasar a estado Saltando entre plataformas
        /// </summary>
        private void setSaltandoPlataformas()
        {
            currentState = State.SaltandoPlataformas;
            crouched = false;
            hidden = false;
            mesh.playAnimation("Jumping", true);
            punching = false;
            punchEmitter.Enabled = false;
            punchBillboard.Enabled = false;
            sonidoPasosCorrer.Enabled = false;
            sonidoPasosCaminandoEsconditeMovil.Enabled = false;
            
            //No restauramos la orientacion a proposito, asi sigue saltando para atras mirando a la camara
            //restoreFaceForward();

            //Posicion deseada
            Vector3 initPos = mesh.Position;
            Vector3 destPos = this.plataformaSalto.NextPlatform.Position;

            //Direccion hacia la que hay que saltar
            jumpDir = destPos - initPos;
            jumpDir.Y = 0;
            jumpDistanceToTravel = jumpDir.Length();
            jumpDir.Normalize();
            jumpYPositive = true;

            //Hay que saltar de alto la cuarta parte de lo que hay que transitar de largo
            jumpPosY = jumpDistanceToTravel / 4;

            //Rotar personaje hacia esa direccion
            beforeJumpRotY = getCurrentRotAngle();
            Vector3 currentVecRot = getCurrentVecRot();
            float angle = FastMath.Atan2(currentVecRot.Z, currentVecRot.X) - FastMath.Atan2(jumpDir.Z, jumpDir.X);
            mesh.rotateY(angle);
            camera.rotateY(angle);
        }

        /// <summary>
        /// Estado saltando entre plataformas
        /// </summary>
        private void saltandoPlataformas(float elapsedTime)
        {
            //Ir avanzando gradualmente
            float speed = GameModel.instance.gameConfig.fisica.velocidadSalto;
            float movement = speed * elapsedTime;
            jumpDistanceToTravel -= movement;
            if (jumpDistanceToTravel < 0)
            {
                //llego a la siguiente plataforma, volver a acomodar a posicion y orientacion correcta
                Vector3 pos = this.plataformaSalto.NextPlatform.Position;
                pos.Y = origPosY;
                mesh.Position = pos;
                //Vector3 beforeJumpRot = new Vector3(0, beforeJumpRotY, 0);
                //mesh.Rotation = beforeJumpRot;
                //camera.RotationY = beforeJumpRot.Y + origAngleRot;
                this.plataformaSalto = null;

                //acomodar bounding sphere
                GameConfig.Fisica fisica = GameModel.instance.gameConfig.fisica;
                characterSphere.setValues(
                    new Vector3(pos.X, fisica.boundingSphereCenterY, pos.Z),
                    fisica.boundingSphereRadio);

                //pasar a estado normal
                setNormal();
            }
            else
            {
                //Movimiento en XZ
                Vector3 move = Vector3.Scale(jumpDir, movement);

                //Movimiento vertical a la mitad de velocidad
                float movementJump = movement / 2; 
                if (jumpYPositive)
                {
                    move.Y += movementJump;
                    if (mesh.Position.Y + move.Y > jumpPosY)
                    {
                        move.Y = jumpPosY - mesh.Position.Y;
                        jumpYPositive = false;
                    }
                }
                else
                {
                    move.Y -= movementJump;
                }
                

                //Avanzar
                mesh.move(move);
                characterSphere.moveCenter(move);
            }
        }

        /// <summary>
        /// Devuelve el vector de orientacion actual del personaje, respecto de su orientacion inicial.
        /// </summary>
        public Vector3 getCurrentVecRot()
        {
            float currentAngle = getCurrentRotAngle();
            Vector3 currentVecRot = new Vector3(FastMath.Sin(currentAngle), 0, FastMath.Cos(currentAngle));
            return currentVecRot;
        }

        /// <summary>
        /// Angulo actual de rotacion, respecto de su orientacion inicial
        /// </summary>
        public float getCurrentRotAngle()
        {
            return mesh.Rotation.Y - origAngleRot;
        }

        

        #endregion






        
    }
}
