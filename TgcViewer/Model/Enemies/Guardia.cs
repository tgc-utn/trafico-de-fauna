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
using Microsoft.DirectX;
using TgcViewer;
using TraficoFauna.Examples;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TgcViewer.Utils;
using Examples.TraficoFauna.Meshes;
using Examples.TraficoFauna.Enemies.Ataque;
using TGC.Core.Geometry;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Core.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;

namespace Examples.TraficoFauna.Enemies
{
    /// <summary>
    /// Enemigo Guardia
    /// Tiene la IA del enemigo
    /// </summary>
    public class Guardia
    {
        /// <summary>
        /// Colores de la vision debug
        /// </summary>
        static readonly int NORMAL_VISION_COLOR = Color.Green.ToArgb();
        static readonly int ALARMA_VISION_COLOR = Color.Red.ToArgb();


        /// <summary>
        /// Estados del guardia
        /// </summary>
        enum State
        {
            Watching,
            Rotating,
            Walking,
            Chasing,
            RotatingForWatching,
        }

        /// <summary>
        /// Estados del guardia cuando esta persiguiendo al personaje
        /// </summary>
        enum ChasingState
        {
            AligningToWaypoints,
            TravelingWaypoints,
            Attacking,
        }

        string id;
        public string Id
        {
            get { return id; }
        }

        List<GuardiaWaypoint> waypoints;
        public List<GuardiaWaypoint> Waypoints
        {
            get { return waypoints; }
        }

        bool activo;
        public bool Activo
        {
            get { return activo; }
        }

        Vector3 center;
        TgcSkeletalMesh mesh;
        GameUtils.Orientacion orientacion;
        State currentState;
        ChasingState currentChasingState;
        TgcBoundingSphere bSphereVisual;
        TgcBoundingSphere bSphereAttack;
        TgcBoundingSphere bSphereVisualEspaldas;

        //Variables de movimiento
        float watchingAcumRotation;
        float watchingRotationDir;
        float watchingElapsedTime;
        float rotatingAcumRotation;
        int currentWaypointIndex;
        Vector3 walkingDir;
        float rotatingNextAngle;
        float rotatingNextAngleSign;
        float walkingDistanceToTravel;
        float origAngleRot;
        Vector3 origVecRot;
        
        //Variables de chasing
        WaypointAtaque waypointAtaqueA;
        WaypointAtaque waypointAtaqueB;
        Vector3 chaseAristaNextPos;


        //Para render debug de la vision
        CustomVertex.PositionColored[] visionVertices = new CustomVertex.PositionColored[3];
        int currentVisionColor;
        

        public Guardia(string id)
        {  
            this.waypoints = new List<GuardiaWaypoint>();
            this.id = id;
        }

        public void setData(Dictionary<string, string> userProperties)
        {
            if (GameUtils.containsUserProp(userProperties, "orientacion"))
            {
                string orientacionStr = userProperties["orientacion"].ToUpper();
                switch (orientacionStr)
                {
                    case "NORTE":
                        orientacion = GameUtils.Orientacion.Norte;
                        break;
                    case "SUR":
                        orientacion = GameUtils.Orientacion.Sur;
                        break;
                    case "ESTE":
                        orientacion = GameUtils.Orientacion.Este;
                        break;
                    case "OESTE":
                        orientacion = GameUtils.Orientacion.Oeste;
                        break;
                    default:
                        throw new Exception("Orientacion incorrecta: " + orientacionStr + ", en Guardia: " + id);
                }
            }
            else
            {
                orientacion = GameUtils.Orientacion.Norte;
            }

            if (GameUtils.containsUserProp(userProperties, "activo"))
            {
                activo = bool.Parse(userProperties["activo"]);
            }
            else
            {
                activo = true;
            }

            //BoundingSpheres
            bSphereVisual = new TgcBoundingSphere();
            bSphereAttack = new TgcBoundingSphere();
            bSphereVisualEspaldas = new TgcBoundingSphere();
        }

        public void arrangeWaypoints()
        {
            //ordenar por index
            waypoints.Sort(new GuardiaWaypoint.WaypointComparer());

            //Fijar centro del mesh, y ubicar a nivel del piso
            this.center = waypoints[0].Center;
            this.center.Y = 0;
            foreach (GuardiaWaypoint w in waypoints)
            {
                w.setCenterY(this.center.Y);
            }
        }


        


        /// <summary>
        /// Iniciar en el sector
        /// </summary>
        public void init(TgcSkeletalMesh mesh)
        {
            this.mesh = mesh;
            mesh.Position = this.center;
            
            //rotar segun orientacion
            mesh.Rotation = Vector3.Empty;
            switch (orientacion)
            {
                case GameUtils.Orientacion.Norte:
                    origAngleRot = FastMath.PI;
                    break;
                case GameUtils.Orientacion.Sur:
                    //original
                    origAngleRot = 0;
                    break;
                case GameUtils.Orientacion.Este:
                    origAngleRot = -FastMath.PI_HALF;
                    break;
                case GameUtils.Orientacion.Oeste:
                    origAngleRot = FastMath.PI_HALF;
                    break;
            }

            //Rotar y ajustar BoundingSpheres
            mesh.rotateY(origAngleRot);
            origVecRot = getCurrentVecRot();
            bSphereVisual.setValues(mesh.Position, GameModel.instance.gameConfig.guardia.distanciaVisual);
            bSphereAttack.setValues(mesh.Position, GameModel.instance.gameConfig.guardia.distanciaAtaque);
            bSphereVisualEspaldas.setValues(mesh.BoundingBox.calculateBoxCenter(), GameModel.instance.gameConfig.guardia.distanciaVisualEspaldas);


            //Estado inicial
            currentWaypointIndex = 0;
            GuardiaWaypoint currentWaypoint = waypoints[currentWaypointIndex];
            if (currentWaypoint.Vigilar)
            {
                setRotatingForWataching();
            }
            else
            {
                setRotating();
            }
            
        }

        /// <summary>
        /// Esconder del sector
        /// </summary>
        public void hide()
        {
            this.mesh = null;
        }



        #region IA de Patrullar


        /// <summary>
        /// Actualizar estado de guardia
        /// </summary>
        public void update(float elapsedTime)
        {
            Vector3 lastPos = mesh.Position;

            switch (currentState)
            {
                case State.Watching:
                    watching(elapsedTime);
                    checkCollisionVision(mesh.Position - lastPos);
                    break;
                case State.Rotating:
                    rotating(elapsedTime);
                    checkCollisionVision(mesh.Position - lastPos);
                    break;
                case State.RotatingForWatching:
                    rotatingForWatching(elapsedTime);
                    checkCollisionVision(mesh.Position - lastPos);
                    break;
                case State.Walking:
                    walking(elapsedTime);
                    checkCollisionVision(mesh.Position - lastPos);
                    break;
                case State.Chasing:
                    chasing(elapsedTime);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Pasar a estado en el que el guardia esta rotando para enfrentar al proximo
        /// waypoint y luego empezar a vigilar parado.
        /// </summary>
        private void setRotatingForWataching()
        {
            //Alinear hacia el proximo waypoint (si hay mas de uno)
            if (waypoints.Count > 1)
            {
                //obtener direccion hacia el proximo waypoint (sin cambiar el waypoint actual)
                Vector3 lastWaypointCenter = waypoints[currentWaypointIndex].Center;
                Vector3 nextWaypointCenter = waypoints[getNextWaypoint()].Center;
                walkingDir = nextWaypointCenter - lastWaypointCenter;
                walkingDir.Y = 0;
                walkingDir.Normalize();

                //Calcular direccion a la que apunta actualmente
                Vector3 currentVecRot = getCurrentVecRot();

                //Obtener siguiente menor angulo de rotacion y signo
                rotatingNextAngle = FastMath.Atan2(currentVecRot.Z, currentVecRot.X) - FastMath.Atan2(walkingDir.Z, walkingDir.X);
                float unsignedAngle = FastMath.Abs(rotatingNextAngle);
                rotatingNextAngleSign = (float)Math.Sign(rotatingNextAngle);
                rotatingNextAngle = unsignedAngle < FastMath.PI ? rotatingNextAngle : (FastMath.TWO_PI - unsignedAngle) * -rotatingNextAngleSign;
                rotatingNextAngleSign = (float)Math.Sign(rotatingNextAngle);

                //Acomodar medio giro de vigilancia, para arrancar la vigilancia desde cero
                rotatingNextAngle += (GameModel.instance.gameConfig.guardia.anguloRotacionVigilancia / 2) * rotatingNextAngleSign;

                //Cambiar de estado
                currentState = State.RotatingForWatching;
                rotatingAcumRotation = rotatingNextAngle;
                mesh.playAnimation("StandBy", true);
                currentVisionColor = NORMAL_VISION_COLOR;
            }
            else
            {
                //Ya esta listo para empezar a vigilar
                setWatching();
            }
        }

        /// <summary>
        /// Estado en el que el personaje esta rotando para alinearse al proximo waypoint y, una
        /// vez que queda alineado, empezar a vigilar parado
        /// </summary>
        private void rotatingForWatching(float elapsedTime)
        {
            float frameRot = GameModel.instance.gameConfig.guardia.velocidadRotacionVigilancia * elapsedTime;
            if (FastMath.Abs(frameRot) > FastMath.Abs(rotatingAcumRotation))
            {
                mesh.rotateY(rotatingAcumRotation);
                setWatching();
            }
            else
            {
                frameRot *= rotatingNextAngleSign;
                mesh.rotateY(frameRot);
                rotatingAcumRotation -= frameRot;
            }
        }

        /// <summary>
        /// Pasar a estado vigilando
        /// </summary>
        private void setWatching()
        {
            currentState = State.Watching;
            watchingAcumRotation = 0;
            watchingElapsedTime = 0;
            mesh.playAnimation("StandBy", true);
            watchingRotationDir = 1;
            currentVisionColor = NORMAL_VISION_COLOR;
        }

        /// <summary>
        /// Estado vigilando
        /// </summary>
        private void watching(float elapsedTime)
        {
            GameConfig.Guardia g = GameModel.instance.gameConfig.guardia;

            watchingElapsedTime += elapsedTime;
            if (watchingElapsedTime > g.duracionSegundosVigilancia)
            {
                setRotating();
            }
            else
            {
                float rot = g.velocidadRotacionVigilancia * watchingRotationDir * elapsedTime;
                watchingAcumRotation += rot;
                if (FastMath.Abs(watchingAcumRotation) > g.anguloRotacionVigilancia)
                {
                    watchingRotationDir *= -1;
                    watchingAcumRotation = 0;
                }
                else
                {
                    mesh.rotateY(rot);
                }
            }
        }

        private void setRotating()
        {
            //Cambiar de waypoint
            Vector3 lastWaypointCenter = waypoints[currentWaypointIndex].Center;
            setNextWaypoint();
            Vector3 nextWaypointCenter = waypoints[currentWaypointIndex].Center;

            //obtener direccion para caminar hacia el proximo
            walkingDir = nextWaypointCenter - lastWaypointCenter;
            walkingDir.Y = 0;
            walkingDistanceToTravel = walkingDir.Length();
            walkingDir.Normalize();
            
            //Calcular direccion a la que apunta actualmente
            Vector3 currentVecRot = getCurrentVecRot();
            
            //Obtener siguiente menor angulo de rotacion y signo
            rotatingNextAngle = FastMath.Atan2(currentVecRot.Z, currentVecRot.X) - FastMath.Atan2(walkingDir.Z, walkingDir.X);
            float unsignedAngle = FastMath.Abs(rotatingNextAngle);
            rotatingNextAngleSign = (float)Math.Sign(rotatingNextAngle);
            rotatingNextAngle = unsignedAngle < FastMath.PI ? rotatingNextAngle : (FastMath.TWO_PI - unsignedAngle) * -rotatingNextAngleSign;
            rotatingNextAngleSign = (float)Math.Sign(rotatingNextAngle);

            //Cambiar de estado
            currentState = State.Rotating;
            rotatingAcumRotation = rotatingNextAngle;
            mesh.playAnimation("Walk", true);
            currentVisionColor = NORMAL_VISION_COLOR;
        }

        /// <summary>
        /// Estado en el que el personaje esta rotando para alinearse al proximo
        /// waypoint y luego empezar a caminar hacia el
        /// </summary>
        private void rotating(float elapsedTime)
        {
            float frameRot = GameModel.instance.gameConfig.guardia.velocidadRotacion * elapsedTime;
            if (FastMath.Abs(frameRot) > FastMath.Abs(rotatingAcumRotation))
            {
                mesh.rotateY(rotatingAcumRotation);
                setWalking();
            }
            else
            {
                frameRot *= rotatingNextAngleSign;
                mesh.rotateY(frameRot);
                rotatingAcumRotation -= frameRot;
            }
        }

        private Vector3 getCurrentVecRot()
        {
            float currentAngle = mesh.Rotation.Y - origAngleRot;
            Vector3 currentVecRot = new Vector3(FastMath.Sin(currentAngle), 0, FastMath.Cos(currentAngle));
            return currentVecRot;
        }

        /// <summary>
        /// Receives a position and yaw (from the patrol), and a position (the waypoint or enemy we are following), 
        /// and returns the yaw increment we should be applying in order to chase the target. 
        /// It detects which side it stands in, and so on:
        /// </summary>
       private float computeRotation(Vector3 pos, float yaw, Vector3 target)
       {
           Vector3 forward = pos + new Vector3(FastMath.Cos(yaw), 0, FastMath.Sin(yaw));
           Vector3 up = pos + new Vector3(0, 1, 0);
           
           Plane pl = Plane.FromPoints(pos, forward, up);
           if(TgcCollisionUtils.classifyPointPlane(target, pl) == TgcCollisionUtils.PointPlaneResult.IN_FRONT_OF)
           {
               return -0.1f;
           }
           return 0.1f;
       }



        
        /// <summary>
        /// Pasar a estado patrullando
        /// </summary>
        private void setWalking()
        {
            currentState = State.Walking;
            mesh.playAnimation("Walk", true);
            currentVisionColor = NORMAL_VISION_COLOR;
        }

        /// <summary>
        /// Caminar en la patrulla
        /// </summary>
        private void walking(float elapsedTime)
        {
            float movement = GameModel.instance.gameConfig.guardia.velocidadCaminar * elapsedTime;
            walkingDistanceToTravel -= movement;
            if (walkingDistanceToTravel < 0)
            {
                //llego al waypoint
                GuardiaWaypoint currentWaypoint = waypoints[currentWaypointIndex];
                mesh.Position = currentWaypoint.Center;

                //Es un nodo de vigilancia
                if (currentWaypoint.Vigilar)
                {
                    //acomodarse para vigilar
                    setRotatingForWataching();
                }
                else
                {
                    //Ir al proximo waypoint
                    setRotating();
                }
            }
            else
            {
                //avanzar
                mesh.move(Vector3.Scale(walkingDir, movement));
            }
        }

        /// <summary>
        /// Cambiar de waypoint en forma circular
        /// </summary>
        private void setNextWaypoint()
        {
            currentWaypointIndex = getNextWaypoint();
        }

        /// <summary>
        /// Obtener el siguiente waypoint en forma circular, sin cambiar el actual
        /// </summary>
        private int getNextWaypoint()
        {
            int n = currentWaypointIndex;
            n++;
            n = n % waypoints.Count;
            return n;
        }


        /// <summary>
        /// Ver si el guardia ve al personaje
        /// </summary>
        private void checkCollisionVision(Vector3 centerMovement)
        {
            //Mover esferas visuales
            bSphereVisual.moveCenter(centerMovement);
            bSphereVisualEspaldas.moveCenter(centerMovement);

            //Si esta escondido, no se puede detectar (al menos que te este pasando por arriba)
            MainCharacter mainCharacter = GameModel.instance.mainCharacter;
            if (mainCharacter.Hidden)
            {
                //Ver si no le pasamos por arriba
                if (mainCharacter.collideWithBSphere(bSphereVisualEspaldas))
                {
                    //Pasar a modo alarma
                    initAttackMode();
                }
                return;
            }

            //Ver si el personaje esta super cerca del enemigo (te detecta igual aunque estes de espalda)
            if (mainCharacter.collideWithBSphere(bSphereVisualEspaldas))
            {
                //Pasar a modo alarma
                initAttackMode();
            }

            //Detectar colision con angulo de vision: boundingSphere + dot de angulo
            float visionY = mainCharacter.Crouched ? GameModel.instance.gameConfig.guardia.alturaVisionAgachado : GameModel.instance.gameConfig.guardia.alturaVision;
            if (isMainCharacterVisible(visionY, true))
            {
                //El personaje se esta viendo, pasar a modo alarma
                initAttackMode();
            }
        }

        /// <summary>
        /// Arrancar el modo ataque
        /// </summary>
        private void initAttackMode()
        {
            GameModel.instance.currentLevel.CurrentSector.setAlarmMode();
            setChasing();
        }

        /// <summary>
        /// Indica si el personaje principal es visible por el enemigo.
        /// Tiene en cuenta un radio de vision y la orientacion actual del enemigo.
        /// </summary>
        /// <param name="visionY">Altura Y bajo la cual chequear</param>
        /// <param name="checkVisionAngle">En true si tiene en cuenta la orientacion del enemigo, sino se ignora</param>
        /// <returns>True si es visible</returns>
        private bool isMainCharacterVisible(float visionY, bool checkVisionAngle)
        {
            MainCharacter mainCharacter = GameModel.instance.mainCharacter;

            //Detectar colision con esfera de vision
            if (mainCharacter.collideWithBSphere(bSphereVisual))
            {
                //Ver si esta dentro del angulo de vision, con el dot product
                if (checkVisionAngle)
                {
                    Vector3 guardiaCharacterVec = Vector3.Normalize(Vector3.Subtract(mainCharacter.CurrentPos, mesh.Position));
                    Vector3 currentVecRot = getCurrentVecRot();
                    float dotAngle = Vector3.Dot(currentVecRot, guardiaCharacterVec);
                    if (dotAngle > GameModel.instance.gameConfig.guardia.anguloVisionDotValue)
                    {
                        //Hay vision, ver que nada se interponga en el camino
                        return checkDirectViewCollision(mesh.Position, bSphereVisual, mainCharacter.CurrentPos, visionY);
                    }
                    else
                    {
                        //Fuera del angulo de vision
                        return false;
                    }
                }
                //No importa hacia donde este mirando el enemigo
                else
                {
                    //Hay vision, ver que nada se interponga en el camino
                    return checkDirectViewCollision(mesh.Position, bSphereVisual, mainCharacter.CurrentPos, visionY);
                }
            }       

            return false;
        }

        /// <summary>
        /// Indica si la vista directa del enemigo choca con algun objeto colisionable del escenario al mirar hacia el jugador.
        /// Devuelve true si se puede llegar a ver porque nada se interpone.
        /// </summary>
        private bool checkDirectViewCollision(Vector3 enemyPos, TgcBoundingSphere bsphereVision, Vector3 mainCharacterPos, float visionY)
        {
            //Ver que nada se interponga en el camino
            List<ColliderObject> objetosColisionables = GameModel.instance.currentLevel.CurrentSector.ObjetosColisionables;
            foreach (ColliderObject collider in objetosColisionables)
            {
                if (TgcCollisionUtils.testSphereAABB(bsphereVision, collider.BoundingBox))
                {
                    if (viewCollideWithOccluder(enemyPos, mainCharacterPos, collider.BoundingBox, visionY))
                    {
                        //Hay colision, no se llega a ver el objetivo
                        return false;
                    }
                }
            }
            //Nada se interpone
            return true;
        }

        /// <summary>
        /// Indica si la vista directa del enemigo choca con el objeto indicado al mirar hacia el jugador.
        /// Devuelve true si la vista choca contra el objeto.
        /// </summary>
        private bool viewCollideWithOccluder(Vector3 enemyPos, Vector3 mainCharacterPos, TgcBoundingAxisAlignBox objectAabb, float visionY)
        {
            Vector3 q;
            if (TgcCollisionUtils.intersectSegmentAABB(
                    new Vector3(enemyPos.X, visionY, enemyPos.Z),
                    new Vector3(mainCharacterPos.X, visionY, mainCharacterPos.Z),
                    objectAabb, out q))
            {
                return true;
            }
            return false;
        }






        /// <summary>
        /// Iniciar persecusion
        /// </summary>
        public void setChasing()
        {
            //Cambiar de estado
            currentState = State.Chasing;
            currentVisionColor = ALARMA_VISION_COLOR;
            mesh.playAnimation("Run", true);
        }

        /// <summary>
        /// Rotar hacia el personaje y avanzar.
        /// Si se acerca lo suficiente pierde.
        /// </summary>
        private void chasing(float elapsedTime)
        {
            //Ver si el personaje esta dentro de la esfera de ataque del guardia
            MainCharacter mainCharacter = GameModel.instance.mainCharacter;
            GameConfig conf = GameModel.instance.gameConfig;
            bSphereAttack.setCenter(mesh.Position);
            if (mainCharacter.collideWithBSphere(bSphereAttack))
            {
                //Perder vida
                GameModel.instance.currentLevel.removeLifeToPlayer();
            }

            //Avanzar de alguna forma para acercarse mas al personaje
            else
            {
                //Estrategia de Rotar y avanzar hacia el enemigo (chase tradicional)

                //Calcular direccion a la que apunta actualmente
                Vector3 currentVecRot = getCurrentVecRot();

                //Orientar guardia hacia el personaje
                Vector3 dir = mainCharacter.CurrentPos - mesh.Position;
                dir.Normalize();
                float angle = FastMath.Atan2(currentVecRot.Z, currentVecRot.X) - FastMath.Atan2(dir.Z, dir.X);
                mesh.rotateY(angle);

                //Avanzar en esa direccion
                float movement = -(conf.guardia.velocidadCorrer * elapsedTime);

                Vector3 movementVector = new Vector3(
                    FastMath.Sin(mesh.Rotation.Y) * movement,
                    0,
                    FastMath.Cos(mesh.Rotation.Y) * movement
                    );

                //Se mueve sin detectar un carajo de colisiones
                mesh.move(movementVector);
            }
        }






        #endregion





        #region IA de Chasing

        /*
         
        SIN TERMINAR ESTA PARTE
         

         
         
         
         
        /// <summary>
        /// Iniciar persecusion
        /// </summary>
        public void setChasing()
        {
            //Cambiar de estado
            currentState = State.Chasing;
            currentVisionColor = ALARMA_VISION_COLOR;
            mesh.playAnimation("Run", true);


            //Ver si se puede atacar directo
            if (isMainCharacterVisible(Game.instance.gameConfig.guardia.alturaVision, false))
            {
                setChasingAttacking();
            }
            //Alinearse a la arista de waypoint mas cercana
            else
            {
                setChasingAligningToWaypoints();
            }
        }

        /// <summary>
        /// Perseguir personaje.
        /// Ejecutar el estado correspondiente a la maquina de estado de la IA de chasing
        /// </summary>
        private void chasing()
        {
            switch (currentChasingState)
            {
                case ChasingState.AligningToWaypoints:
                    chasingAligningToWaypoints();
                    break;
                case ChasingState.TravelingWaypoints:
                    chasingTravelingWaypoints();
                    break;
                case ChasingState.Attacking:
                    chasingAttacking();
                    break;
            }
        }


        /// <summary>
        /// Pasar a estado de atacar directo
        /// </summary>
        private void setChasingAttacking()
        {
            currentChasingState = ChasingState.Attacking;
        }

        /// <summary>
        /// Rotar hacia el personaje y avanzar.
        /// Si se acerca lo suficiente pierde.
        /// </summary>
        private void chasingAttacking()
        {
            //Ver si el personaje esta dentro de la esfera de ataque del guardia
            MainCharacter mainCharacter = Game.instance.mainCharacter;
            GameConfig conf = Game.instance.gameConfig;
            bSphereAttack.setCenter(mesh.Position);
            if (mainCharacter.collideWithBSphere(bSphereAttack))
            {
                //Perder vida
                Game.instance.currentLevel.removeLifeToPlayer();
            }

            //Avanzar de alguna forma para acercarse mas al personaje
            else
            {
                //Estrategia de Rotar y avanzar hacia el enemigo (chase tradicional)

                //Calcular direccion a la que apunta actualmente
                Vector3 currentVecRot = getCurrentVecRot();

                //Orientar guardia hacia el personaje
                Vector3 dir = mainCharacter.CurrentPos - mesh.Position;
                dir.Normalize();
                float angle = FastMath.Atan2(currentVecRot.Z, currentVecRot.X) - FastMath.Atan2(dir.Z, dir.X);
                mesh.rotateY(angle);

                //Avanzar en esa direccion
                float movement = -(conf.guardia.velocidadCorrer * GuiController.Instance.ElapsedTime);

                Vector3 movementVector = new Vector3(
                    FastMath.Sin(mesh.Rotation.Y) * movement,
                    0,
                    FastMath.Cos(mesh.Rotation.Y) * movement
                    );

                //Se mueve sin detectar un carajo de colisiones
                mesh.move(movementVector);
            }
        }

        /// <summary>
        /// Arrimarse a la arista mas cercana que une dos Waypoints de Ataque del escenario.
        /// Para estar dentro de los "rieles" de los waypoints.
        /// </summary>
        private void setChasingAligningToWaypoints()
        {
            //Obtener arista de Waypoints de Ataque mas cercana para avanzar hasta ahi
            WaypointAtaqueArista chaseCurrentArista = Game.instance.currentLevel.CurrentSector.WaypointsAtaqueManager.getAristaMasCercana(mesh.Position, out chaseAristaNextPos);
            waypointAtaqueA = chaseCurrentArista.W1;
            waypointAtaqueB = chaseCurrentArista.W2;

            //obtener direccion para caminar hacia el destino
            walkingDir = chaseAristaNextPos - mesh.Position;
            walkingDir.Y = 0;
            walkingDistanceToTravel = walkingDir.Length();
            walkingDir.Normalize();

            //Rotar personaje para que mire hacia el destino
            Vector3 currentVecRot = getCurrentVecRot();
            rotatingNextAngle = FastMath.Atan2(currentVecRot.Z, currentVecRot.X) - FastMath.Atan2(walkingDir.Z, walkingDir.X);
            mesh.rotateY(rotatingNextAngle);

            currentChasingState = ChasingState.AligningToWaypoints;
        }

        /// <summary>
        /// Avanzar hacia la arista mas cercana
        /// </summary>
        private void chasingAligningToWaypoints()
        {
            float movement = Game.instance.gameConfig.guardia.velocidadCorrer * GuiController.Instance.ElapsedTime;
            walkingDistanceToTravel -= movement;
            if (walkingDistanceToTravel < 0)
            {
                //llego al punto de la arista
                mesh.Position = chaseAristaNextPos;
                setChasingTravelingWaypoints();
            }
            else
            {
                //avanzar
                mesh.move(Vector3.Scale(walkingDir, movement));
            }
        }

        /// <summary>
        /// Iniciar la navegacion sobre los waypoints
        /// </summary>
        private void setChasingTravelingWaypoints()
        {
            currentChasingState = ChasingState.TravelingWaypoints;
        }

        /// <summary>
        /// Avanzar hacia el waypoint mas cercano al personaje principal.
        /// Si el personaje esta muy cerca, atacar directamente.
        /// </summary>
        private void chasingTravelingWaypoints()
        {
            //Buscar hacia cual de los dos waypoints de la arista conviene ir

        }

        */




        /*
        /// <summary>
        /// Prueba si un movimiento es valido, haciendo deteccion de colisiones con los objetos del escenario.
        /// </summary>
        /// <param name="pos">Posicion actual</param>
        /// <param name="nextPos">Posicion deseada</param>
        /// <param name="collisionObj">Objeto con el que se colisiono</param>
        /// <param name="collisionNormal">Normal contra la que se colisiona</param>
        /// <returns>true si hubo colision</returns>
        private bool tryMove(Vector3 pos, Vector3 nextPos, out TgcBoundingBox collisionObj, out Vector3 collisionNormal)
        {
            TgcBoundingBox minAABB = null;
            float minDistSq = float.MaxValue;
            Vector3 minCollisionPoint = Vector3.Empty;

            //Colision de BoundingBox de proxima posicion contra todos los objetos del escenario
            List<ColliderObject> objetosColisionables = Game.instance.currentLevel.CurrentSector.ObjetosColisionables;
            foreach (ColliderObject collider in objetosColisionables)
            {
                if (TgcCollisionUtils.testAABBAABB(collider.BoundingBox, mesh.BoundingBox))
                {
                    //Colision segmento de movimiento contra AABB del obstaculo
                    Vector3 q;
                    if (TgcCollisionUtils.intersectSegmentAABB(pos, nextPos, collider.BoundingBox, out q))
                    {
                        //Guardar el mas cercano
                        float distSq = Vector3.Subtract(q, pos).LengthSq();
                        if (distSq < minDistSq)
                        {
                            minDistSq = distSq;
                            minAABB = collider.BoundingBox;
                            minCollisionPoint = q;
                        }
                    }
                }
            }

            //No hay colision, se puede avanzar
            if (minAABB == null)
            {
                collisionNormal = Vector3.Empty;
                collisionObj = null;
                return true;
            }

            //Obtener normal de la cara del BoundingBox con la que se colisiono
            collisionNormal = getAAABBCollisionFaceNormal(minAABB, minCollisionPoint);
            collisionObj = minAABB;
            return false;
        }

        /// <summary>
        /// Obtiene la normal del BoundingBox a la que pertenece un punto de colision
        /// </summary>
        private Vector3 getAAABBCollisionFaceNormal(TgcBoundingBox aabb, Vector3 collisionPoint)
        {
            //+X: (1, 0, 0)
            if (collisionPoint.X < aabb.PMax.X + float.Epsilon && collisionPoint.X > aabb.PMax.X - float.Epsilon)
            {
                return new Vector3(1, 0, 0);
            }
            //-X: (-1, 0, 0)
            if (collisionPoint.X < aabb.PMin.X + float.Epsilon && collisionPoint.X > aabb.PMin.X - float.Epsilon)
            {
                return new Vector3(-1, 0, 0);
            }
            //+Z: (0, 0, 1)
            if (collisionPoint.Z < aabb.PMax.Z + float.Epsilon && collisionPoint.Z > aabb.PMax.Z - float.Epsilon)
            {
                return new Vector3(0, 0, 1);
            }
            //+Z: (0, 0, -1)
            if (collisionPoint.Z < aabb.PMin.Z + float.Epsilon && collisionPoint.Z > aabb.PMin.Z - float.Epsilon)
            {
                return new Vector3(0, 0, -1);
            }


            //Error
            throw new Exception("No se encontro face del BoundingBox que coincida con el punto de colision. Aabb: " + aabb.ToString() + ", Point: " + TgcParserUtils.printVector3(collisionPoint));
        }
        */
        

        #endregion


        #region Render del enemigo


        /// <summary>
        /// Render de guardia
        /// </summary>
        public void render(float elapsedTime)
        {
            //Dibujar si esta en rango visible
            if (GameModel.instance.mainCharacter.isInProximityRange(mesh.Position))
            {
                mesh.animateAndRender(elapsedTime);


                //Debug
                if (GameModel.instance.debugMode.Enabled && GameModel.instance.debugMode.ShowBoundingVolumes)
                {
                    renderVision();
                }
            }
        }

        /// <summary>
        /// Renderizar triangulo de vision, a modo debug
        /// </summary>
        private void renderVision()
        {
            Vector3 currentVecRot = getCurrentVecRot();

            float visionY = GameModel.instance.mainCharacter.Crouched ? GameModel.instance.gameConfig.guardia.alturaVisionAgachado : GameModel.instance.gameConfig.guardia.alturaVision;
            Vector3 meshPos = new Vector3(mesh.Position.X, visionY, mesh.Position.Z);
            Vector3 x = meshPos + Vector3.Scale(currentVecRot, GameModel.instance.gameConfig.guardia.distanciaVisual);
            float delta = GameModel.instance.gameConfig.guardia.radioPerpendicularVision;
            Vector3 orthonormal = Vector3.Cross((x - meshPos), new Vector3(0, 1, 0));
            orthonormal.Normalize();
            Vector3 orthonormalScaled = Vector3.Scale(orthonormal, delta);

            //int color = currentState == State.Alarm ? visionColorAlarm : visionColor;
            int color = currentVisionColor;
            visionVertices[0] = new CustomVertex.PositionColored(meshPos, color);
            visionVertices[1] = new CustomVertex.PositionColored(x + orthonormalScaled, color);
            visionVertices[2] = new CustomVertex.PositionColored(x - orthonormalScaled, color);


            var d3dDevice = D3DDevice.Instance.Device;

            TexturesManager.Instance.clear(0);
            TexturesManager.Instance.clear(1);
            d3dDevice.Material = D3DDevice.DEFAULT_MATERIAL;
            d3dDevice.Transform.World = Matrix.Identity;

            d3dDevice.VertexFormat = CustomVertex.PositionColored.Format;
            d3dDevice.DrawUserPrimitives(PrimitiveType.TriangleList, 1, visionVertices);



            //esfera de colision
            //bSphereVisual.render();
            bSphereVisualEspaldas.render();
            bSphereAttack.render();
        }



        /// <summary>
        /// Liberar recursos
        /// </summary>
        public void dispose()
        {
            this.mesh = null;
            bSphereVisual.dispose();
            bSphereAttack.dispose();
            bSphereVisualEspaldas.dispose();
            visionVertices = null;
        }

#endregion


    }
}
