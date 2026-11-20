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
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SkeletalAnimation;
using Trafico.De.Fauna.Model.Billboards;
using Trafico.De.Fauna.Model.Camera;
using Trafico.De.Fauna.Model.Escondites;
using Trafico.De.Fauna.Model.ItemsAnimal;
using Trafico.De.Fauna.Model.Meshes;
using Trafico.De.Fauna.Model.Particles;
using Trafico.De.Fauna.Model.Saltos;
using Trafico.De.Fauna.Model.Sonidos;

namespace Trafico.De.Fauna.Model.MainCharacters
{
    /// <summary>
    ///     Personaje principal del juego
    /// </summary>
    public class MainCharacter
    {
        /// <summary>
        ///     Cuanto para adelelante movemos el efecto de punch con billboard.
        ///     Multiplica la direccion actual del personaje.
        /// </summary>
        private const float PUNCH_BILLBOARD_POS_SCALE = 20;

        private float beforeJumpRotY;

        private SphereCollisionManager collisionManager;

        private State currentState;
        private EsconditeFijo esconditeFijo;
        private EsconditeMovil esconditeMovil;
        private bool facingBackward;

        private TGCVector3 jumpDir;
        private float jumpDistanceToTravel;
        private float jumpPosY;
        private bool jumpYPositive;

        private TgcSkeletalMesh mesh;
        private float origAngleRot;

        //Salto
        private float origPosY;
        private Salto.Plataforma plataformaSalto;
        private QuadAnimatedBillboard punchBillboard;
        private ParticleEmitter punchEmitter;
        private bool punching;
        private TemporalSound sonidoPasosCaminandoEsconditeMovil;
        private TemporalSound sonidoPasosCorrer;
        private ParticleEmitter stepsEmitter;

        /// <summary>
        ///     BoundingSphere del personaje
        /// </summary>
        public TgcBoundingSphere CharacterSphere { get; private set; }

        /// <summary>
        ///     Posicion actual
        ///     Varia segun si ya se aplico el moveWithCollisions()
        /// </summary>
        public TGCVector3 CurrentPos => mesh.Position;

        /// <summary>
        ///     Indica si el personaje esta escondido
        /// </summary>
        public bool Hidden { get; private set; }

        /// <summary>
        ///     Indica si el personaje esta agachado
        /// </summary>
        public bool Crouched { get; private set; }

        /// <summary>
        ///     Camara en tercera persona que se ajusta segun los movimientos del personaje
        /// </summary>
        public GameThirdPersonCamera Camera { get; private set; }

        /// <summary>
        ///     Cantidad de bananas agarradas
        /// </summary>
        public int BananasAgarradas { get; private set; }

        /// <summary>
        ///     Cantidad de ItemsAnimal liberados
        /// </summary>
        public int ItemsAnimalLiberados { get; private set; }

        /// <summary>
        ///     Vidas del personaje
        /// </summary>
        public int Vidas { get; set; }

        /// <summary>
        ///     Offset Z actual de la camara
        /// </summary>
        public float CurrentCameraOffsetForward { get; private set; }

        /// <summary>
        ///     Offset minimp de Z actual de la camara
        /// </summary>
        public float CurrentMinCameraOffsetForward { get; private set; }

        public void loadModel()
        {
            //Cargar mesh
            var meshPath = GameModel.instance.gameConfig.basePath + "MainCharacterMono\\";
            var skeletalLoader = new TgcSkeletalLoader();
            mesh = skeletalLoader.loadMeshAndAnimationsFromFile(
                meshPath + "Mono-TgcSkeletalMesh.xml",
                meshPath,
                new[]
                {
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
            mesh.RotateY(Geometry.DegreeToRadian(180f));
            origAngleRot = FastMath.PI;
            origPosY = 0;
            mesh.AnimationEnds += mesh_AnimationEnds;

            //BoundingSphere que va a usar el mesh
            mesh.AutoUpdateBoundingBox = false;
            var fisica = GameModel.instance.gameConfig.fisica;
            CharacterSphere = new TgcBoundingSphere(new TGCVector3(0, fisica.boundingSphereCenterY, 0),
                fisica.boundingSphereRadio);

            //Crear manejador de colisiones
            collisionManager = new SphereCollisionManager();
            collisionManager.GravityEnabled = false;
            collisionManager.GravityForce = new TGCVector3(0f, fisica.gravedad, 0f);
            collisionManager.SlideFactor = fisica.slideFactor;

            //Configurar camara en Tercer Persona personalizada
            var camaraConfig = GameModel.instance.gameConfig.camara;
            Camera = new GameThirdPersonCamera();
            GameModel.instance.Camera = Camera;
            Camera.Target = mesh.Position;
            Camera.OffsetHeight = camaraConfig.offsetY;
            Camera.OffsetForward = camaraConfig.offsetZ;
            Camera.TargetDisplacement = new TGCVector3(0, camaraConfig.displacementY, 0);

            //Particulas para las pisadas del personaje
            stepsEmitter =
                new ParticleEmitter(GameModel.instance.gameConfig.basePath + "Shaders\\Particles\\pisada_mono.png", 4);
            stepsEmitter.Dispersion = 400;
            stepsEmitter.ParticleTimeToLive = 0.5f;
            stepsEmitter.CreationFrecuency = 0.1f;
            stepsEmitter.Speed = new TGCVector3(5, 5, 5);
            stepsEmitter.MinSizeParticle = 2f;
            stepsEmitter.MaxSizeParticle = 1f;

            //Particulas para el punch al romper cajas
            punchEmitter =
                new ParticleEmitter(GameModel.instance.gameConfig.basePath + "Shaders\\Particles\\punch_mono.png", 2);
            punchEmitter.Dispersion = 100;
            punchEmitter.ParticleTimeToLive = 0.4f;
            punchEmitter.CreationFrecuency = 1f;
            punchEmitter.Speed = new TGCVector3(0, 10, 0);
            punchEmitter.MinSizeParticle = 40f;
            punchEmitter.MaxSizeParticle = 40f;

            punchBillboard = new QuadAnimatedBillboard(
                GameModel.instance.gameConfig.basePath + "Billboards\\personaje_punch_effect.png", new Size(64, 64), 16,
                16);
            punchBillboard.Size = new TGCVector2(50, 50);
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
            Vidas = 5;
        }

        /// <summary>
        ///     Cargar valores iniciales para empezar un nuevo nivel
        /// </summary>
        public void resetForNewLevel()
        {
            BananasAgarradas = 0;
            ItemsAnimalLiberados = 0;
            stepsEmitter.Playing = false;
            punchEmitter.Playing = false;
            punchBillboard.Enabled = false;
            facingBackward = false;
            CurrentCameraOffsetForward = GameModel.instance.gameConfig.camara.offsetZ;
            CurrentMinCameraOffsetForward = GameModel.instance.gameConfig.camara.minOffsetZ;
            punching = false;
        }

        /// <summary>
        ///     Configurar personaje para arrancar en un sector
        /// </summary>
        public void initInConectorPos(TGCVector3 conectorPos, GameUtils.Orientacion orientacion)
        {
            //Posicionar
            var diffPos = TGCVector3.Subtract(conectorPos, mesh.Position);
            mesh.Move(diffPos);
            CharacterSphere.moveCenter(diffPos);

            //Rotar hacia la orientacion indicada
            restoreFaceForward();
            mesh.Rotation = new TGCVector3(0, origAngleRot, 0);
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

            mesh.RotateY(angleRot);
            Camera.RotationY = angleRot;

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
        ///     Hacer que la camara siga al personaje en su nueva posicion
        /// </summary>
        private void targetCamera()
        {
            Camera.Target = CharacterSphere.Center;
        }

        /// <summary>
        ///     Colision de un AABB con el BoundingSphere del personaje
        /// </summary>
        public bool collideWithAABB(TgcBoundingAxisAlignBox aabb)
        {
            return TgcCollisionUtils.testSphereAABB(CharacterSphere, aabb);
        }

        /// <summary>
        ///     Colision de un BoundingSphere con el BoundingSphere del personaje
        /// </summary>
        public bool collideWithBSphere(TgcBoundingSphere sphere)
        {
            return TgcCollisionUtils.testSphereSphere(CharacterSphere, sphere);
        }

        /// <summary>
        ///     Indica si un objeto ubicado en la posicion especificada se encuentra dentro del
        ///     radio de alcance visible por el personaje
        /// </summary>
        public bool isInProximityRange(TGCVector3 pos)
        {
            //Ver si esta dentro del radio de vision
            return TGCVector3.Subtract(pos, CurrentPos).LengthSq() <
                   GameModel.instance.gameConfig.camara.distanciaVisualSq;
        }

        /// <summary>
        ///     Agrega bananas al personaje principal.
        ///     Si es multiplo de 100 gana una vida
        /// </summary>
        public void agregarBananas(int n)
        {
            var enteros = BananasAgarradas / 100;
            BananasAgarradas += n;
            var enteros2 = BananasAgarradas / 100;
            if (enteros2 > enteros)
            {
                Vidas += enteros2 - enteros;
                BananasAgarradas = BananasAgarradas % 100;
                GameModel.instance.sonidos.personaje_ganar_vida.Play(false);
                GameModel.instance.hud.efectoGanarVida();
            }
        }

        /// <summary>
        ///     Agregar un item animal liberado.
        ///     Si se liberaron todos, ganaste el juego.
        /// </summary>
        public void agregarItemAnimalLiberado(ItemAnimal itemAnimal)
        {
            ItemsAnimalLiberados++;
            if (ItemsAnimalLiberados == GameModel.instance.currentLevel.TotalItemsAnimal)
            {
                GameModel.instance.transiciones.initGameWin();
                GameModel.instance.currentGameMode = GameMode.GameWin;
            }
        }

        #region Maquina de estados

        /// <summary>
        ///     Estados del personaje principal
        /// </summary>
        private enum State
        {
            Normal,
            Agachado,
            AgachadoEsconditeFijo,
            CargandoEsconditeMovil,
            AgachadoEsconditeMovil,
            SaltandoPlataformas
        }

        /// <summary>
        ///     Actualizar estado del personaje
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
        ///     Pasar a estado normal
        /// </summary>
        private void setNormal()
        {
            mesh.playAnimation("StandBy", true);
            Hidden = false;
            Crouched = false;
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
        ///     Estado normal del personaje
        /// </summary>
        private void normal(float elapsedTime)
        {
            //obtener velocidades de Modifiers
            var fisica = GameModel.instance.gameConfig.fisica;
            var keys = GameModel.instance.gameConfig.keys;
            var d3dInput = GameModel.instance.Input;
            var level = GameModel.instance.currentLevel;

            //Crouch
            if (d3dInput.keyDown(keys.crouch))
            {
                //Ver si hay un escondite fijo
                var esconditeFijo = level.CurrentSector.checkEsconditesFijos(this);
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
                var conectorCollision = level.CurrentSector.checkConectores(this);
                if (conectorCollision != null)
                {
                    //ejecutar conector
                    level.CurrentSector.executeConector(conectorCollision);
                    return;
                }

                //Ver si hay colision con un item Animal para rescatar
                var item = level.CurrentSector.checkItemsAnimal(this);
                if (item != null)
                {
                    var liberado = item.intentarLiberar();
                    return;
                }

                //Ver si hay un escondite movil para agarrar
                var esconditeMovil = level.CurrentSector.checkEsconditesMoviles(this);
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
                var plataformaSaltoCollision = level.CurrentSector.checkSaltos(this);
                if (plataformaSaltoCollision != null)
                {
                    //Solo puede saltar si antes estuvo corriendo
                    if ( /*mesh.CurrentAnimation.Name == "Run"*/ true)
                    {
                        plataformaSalto = plataformaSaltoCollision;
                        GameModel.instance.sonidos.personaje_saltar.Play();
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
                    GameModel.instance.sonidos.personaje_golpe.Play();

                    //Intentar golpear cajas de bananas cercanas
                    level.CurrentSector.golpearCajasBananas(this);

                    //Activar efecto de particulas para el golpe
                    punchEmitter.Enabled = true;
                    punchEmitter.Playing = true;
                    punchBillboard.Enabled = true;
                }
            }

            //Movimiento
            var moveForward = 0f;
            float rotate = 0;
            var moving = false;
            var rotating = false;

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
                var rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                mesh.RotateY(rotAngle);
                Camera.rotateY(rotAngle);
            }

            //Vector de movimiento
            var movementVector = TGCVector3.Empty;
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
                movementVector = new TGCVector3(
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
            var newPosition = moveWithCollisions(level.CurrentSector.ObjetosColisionables, movementVector);

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
                punchBillboard.Position = CurrentPos + TGCVector3.Scale(getCurrentVecRot(), PUNCH_BILLBOARD_POS_SCALE);
                punchBillboard.RotationY = Camera.RotationY;
            }
        }

        /// <summary>
        ///     Mover al personaje con deteccion de colisiones por el escenario
        /// </summary>
        public TGCVector3 moveWithCollisions(List<ColliderObject> objetosColisionables, TGCVector3 movementVector)
        {
            var realMovement = collisionManager.moveCharacter(CharacterSphere, movementVector, objetosColisionables);
            mesh.Move(realMovement);
            return realMovement;
        }

        /// <summary>
        ///     Evento cuando termina de ejecutarse una animacion
        /// </summary>
        private void mesh_AnimationEnds(TgcSkeletalMesh mesh)
        {
            if (currentState == State.Normal && mesh.CurrentAnimation.Name == "Punch")
            {
                punching = false;
                punchEmitter.Playing = false;
                punchBillboard.Enabled = false;
            }
        }

        /// <summary>
        ///     Rotar el personaje para que mire hacia la camara
        /// </summary>
        private void faceBackward()
        {
            mesh.RotateY(FastMath.PI);
            facingBackward = true;
            CurrentCameraOffsetForward = GameModel.instance.gameConfig.camara.facingBackwardOffsetZ;
            CurrentMinCameraOffsetForward = GameModel.instance.gameConfig.camara.facingBackwardMinOffsetZ;
            Camera.OffsetForward = CurrentCameraOffsetForward;
        }

        /// <summary>
        ///     Recuperar la rotacion del personaje si es que antes estaba girado mirando a la camara
        /// </summary>
        private void restoreFaceForward()
        {
            if (facingBackward)
            {
                mesh.RotateY(FastMath.PI);
                facingBackward = false;
                CurrentCameraOffsetForward = GameModel.instance.gameConfig.camara.offsetZ;
                CurrentMinCameraOffsetForward = GameModel.instance.gameConfig.camara.minOffsetZ;
                Camera.OffsetForward = CurrentCameraOffsetForward;
            }
        }

        /// <summary>
        ///     Pasar a estado agachado
        /// </summary>
        private void setAgachado()
        {
            currentState = State.Agachado;
            Crouched = true;
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
            GameModel.instance.sonidos.personaje_agacharse.Play(false);
        }

        /// <summary>
        ///     Estado agachado del personaje
        /// </summary>
        private void agachado(float elapsedTime)
        {
            //Sigue agachado mientras se mantiene presionado crouch
            var d3dInput = GameModel.instance.Input;
            var keys = GameModel.instance.gameConfig.keys;

            if (d3dInput.keyDown(keys.crouch))
            {
                //No se puede mover, solo rotar
                var fisica = GameModel.instance.gameConfig.fisica;
                float rotate = 0;
                var rotating = false;

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
                    var rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                    mesh.RotateY(rotAngle);
                    Camera.rotateY(rotAngle);

                    //Animacion
                    var rotAnim = rotate > 0 ? "CrouchRight" : "CrouchLeft";
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
        ///     Pasar a estado agachado en un escondite fijo
        /// </summary>
        private void setAgachadoEsconditeFijo()
        {
            currentState = State.AgachadoEsconditeFijo;
            Crouched = true;
            Hidden = true;
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
            GameModel.instance.sonidos.personaje_agacharse.Play(false);
        }

        /// <summary>
        ///     Estado agachado en un escondite fijo
        /// </summary>
        private void agachadoEsconditeFijo(float elapsedTime)
        {
            //Sigue agachado mientras se mantiene presionado crouch
            var d3dInput = GameModel.instance.Input;
            var keys = GameModel.instance.gameConfig.keys;

            if (d3dInput.keyDown(keys.crouch))
            {
                //No se puede mover, solo rotar
                var fisica = GameModel.instance.gameConfig.fisica;
                float rotate = 0;
                var rotating = false;

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
                    var rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                    mesh.RotateY(rotAngle);
                    Camera.rotateY(rotAngle);

                    //Animacion
                    var rotAnim = rotate > 0 ? "CrouchRight" : "CrouchLeft";
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
                esconditeFijo.Escondido = false;
                esconditeFijo = null;
                setNormal();
            }
        }

        /// <summary>
        ///     Acomodar BoundingSphere de agachado
        /// </summary>
        private void setBoundingSphereAgachado()
        {
            var fisica = GameModel.instance.gameConfig.fisica;
            CharacterSphere.setValues(
                new TGCVector3(CharacterSphere.Center.X, fisica.boundingSphereAgachadoCenterY,
                    CharacterSphere.Center.Z),
                fisica.boundingSphereRadioAgachado);
        }

        /// <summary>
        ///     Acomodar BoundingSphere de parado
        /// </summary>
        private void setBoundingSphereParado()
        {
            var fisica = GameModel.instance.gameConfig.fisica;
            CharacterSphere.setValues(
                new TGCVector3(CharacterSphere.Center.X, fisica.boundingSphereCenterY, CharacterSphere.Center.Z),
                fisica.boundingSphereRadio);
        }

        /// <summary>
        ///     Pasar a estado cargando un escondite movil
        /// </summary>
        private void setCargandoEsconditeMovil()
        {
            currentState = State.CargandoEsconditeMovil;
            Crouched = false;
            Hidden = false;
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
            var objCenter = esconditeMovil.BoundingBox.calculateBoxCenter();
            objCenter.Y = esconditeMovil.BoundingBox.PMin.Y;
            var objDiff = CharacterSphere.Center - objCenter;
            objDiff.Y += CharacterSphere.Radius;
            esconditeMovil.Mesh.Move(objDiff);

            //Marcar como que no esta escondido
            esconditeMovil.Escondido = false;

            //Sonido de agarrar escondite
            GameModel.instance.sonidos.personaje_levantar_escondite_movil.Play(false);
        }

        /// <summary>
        ///     Estado cargando escondite movil del personaje
        /// </summary>
        private void cargandoEsconditeMovil(float elapsedTime)
        {
            //Leer input
            var d3dDevice = D3DDevice.Instance.Device;

            //obtener velocidades de Modifiers
            var fisica = GameModel.instance.gameConfig.fisica;
            var keys = GameModel.instance.gameConfig.keys;
            var d3dInput = GameModel.instance.Input;
            var level = GameModel.instance.currentLevel;

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
                var objSize = esconditeMovil.BoundingBox.calculateSize();
                esconditeMovil.Mesh.Move(0, -objSize.Y, 0);
                esconditeMovil.Agarrado = false;
                esconditeMovil.Escondido = false;
                esconditeMovil = null;

                setNormal();
                return;
            }

            //Movimiento
            var moveForward = 0f;
            float rotate = 0;
            var moving = false;
            var rotating = false;

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
                var rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                mesh.RotateY(rotAngle);
                Camera.rotateY(rotAngle);
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
            var movementVector = TGCVector3.Empty;
            if (moving)
            {
                //Ajustar por el tiempo transcurrido
                moveForward *= elapsedTime;

                //Aplicar movimiento, desplazarse en base a la rotacion actual del mesh
                movementVector = new TGCVector3(
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
            var realMovement = moveWithCollisions(level.CurrentSector.ObjetosColisionables, movementVector);

            //Mover tambien escondite movil
            esconditeMovil.Mesh.Move(realMovement);

            //Colision con llaves del sector
            level.CurrentSector.checkLlaves(this);

            //Agarrar bananas
            level.CurrentSector.checkBananas(this);


            //Colision con llaves del sector
            level.CurrentSector.checkLlaves(this);
        }

        /// <summary>
        ///     Pasar a estado Agachado con escondite movil
        /// </summary>
        private void setAgachadoEsconditeMovil()
        {
            currentState = State.AgachadoEsconditeMovil;
            Crouched = true;
            Hidden = true;
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
            var objSize = esconditeMovil.BoundingBox.calculateSize();
            esconditeMovil.Mesh.Move(0, -objSize.Y, 0);

            //Marcar escondite como escondido
            esconditeMovil.Escondido = true;

            //Sonido de agacharse
            GameModel.instance.sonidos.personaje_agacharse.Play(false);
        }

        /// <summary>
        ///     Estado agachado escondite movil del personaje
        /// </summary>
        private void agachadoEsconditeMovil(float elapsedTime)
        {
            //Sigue escondido agachado con el objeto mientras se mantiene presionado crouch
            var d3dInput = GameModel.instance.Input;
            var keys = GameModel.instance.gameConfig.keys;

            if (d3dInput.keyDown(keys.crouch))
            {
                //No se puede mover, solo rotar
                var fisica = GameModel.instance.gameConfig.fisica;
                float rotate = 0;
                var rotating = false;

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
                    var rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                    mesh.RotateY(rotAngle);
                    Camera.rotateY(rotAngle);
                }
            }
            //Si suelta, el objeto se para cargando el escondite
            else if (d3dInput.keyUp(keys.crouch))
            {
                setCargandoEsconditeMovil();
            }
        }

        /// <summary>
        ///     Pasar a estado Saltando entre plataformas
        /// </summary>
        private void setSaltandoPlataformas()
        {
            currentState = State.SaltandoPlataformas;
            Crouched = false;
            Hidden = false;
            mesh.playAnimation("Jumping", true);
            punching = false;
            punchEmitter.Enabled = false;
            punchBillboard.Enabled = false;
            sonidoPasosCorrer.Enabled = false;
            sonidoPasosCaminandoEsconditeMovil.Enabled = false;

            //No restauramos la orientacion a proposito, asi sigue saltando para atras mirando a la camara
            //restoreFaceForward();

            //Posicion deseada
            var initPos = mesh.Position;
            var destPos = plataformaSalto.NextPlatform.Position;

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
            var currentVecRot = getCurrentVecRot();
            var angle = FastMath.Atan2(currentVecRot.Z, currentVecRot.X) - FastMath.Atan2(jumpDir.Z, jumpDir.X);
            mesh.RotateY(angle);
            Camera.rotateY(angle);
        }

        /// <summary>
        ///     Estado saltando entre plataformas
        /// </summary>
        private void saltandoPlataformas(float elapsedTime)
        {
            //Ir avanzando gradualmente
            var speed = GameModel.instance.gameConfig.fisica.velocidadSalto;
            var movement = speed * elapsedTime;
            jumpDistanceToTravel -= movement;
            if (jumpDistanceToTravel < 0)
            {
                //llego a la siguiente plataforma, volver a acomodar a posicion y orientacion correcta
                var pos = plataformaSalto.NextPlatform.Position;
                pos.Y = origPosY;
                mesh.Position = pos;
                //TGCVector3 beforeJumpRot = new TGCVector3(0, beforeJumpRotY, 0);
                //mesh.Rotation = beforeJumpRot;
                //camera.RotationY = beforeJumpRot.Y + origAngleRot;
                plataformaSalto = null;

                //acomodar bounding sphere
                var fisica = GameModel.instance.gameConfig.fisica;
                CharacterSphere.setValues(
                    new TGCVector3(pos.X, fisica.boundingSphereCenterY, pos.Z),
                    fisica.boundingSphereRadio);

                //pasar a estado normal
                setNormal();
            }
            else
            {
                //Movimiento en XZ
                var move = TGCVector3.Scale(jumpDir, movement);

                //Movimiento vertical a la mitad de velocidad
                var movementJump = movement / 2;
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
                mesh.Move(move);
                CharacterSphere.moveCenter(move);
            }
        }

        /// <summary>
        ///     Devuelve el vector de orientacion actual del personaje, respecto de su orientacion inicial.
        /// </summary>
        public TGCVector3 getCurrentVecRot()
        {
            var currentAngle = getCurrentRotAngle();
            var currentVecRot = new TGCVector3(FastMath.Sin(currentAngle), 0, FastMath.Cos(currentAngle));
            return currentVecRot;
        }

        /// <summary>
        ///     Angulo actual de rotacion, respecto de su orientacion inicial
        /// </summary>
        public float getCurrentRotAngle()
        {
            return mesh.Rotation.Y - origAngleRot;
        }

        #endregion
    }
}
