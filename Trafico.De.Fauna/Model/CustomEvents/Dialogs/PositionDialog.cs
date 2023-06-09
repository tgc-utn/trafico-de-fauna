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
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.Dialogs;
using Trafico.De.Fauna.Model.MainCharacters;
using Trafico.De.Fauna.Model.Meshes;

namespace Trafico.De.Fauna.Model.CustomEvents.Dialogs
{
    /// <summary>
    /// Evento que levanta un cuadro de dialogo al pasar por el BoundingBox.
    /// Propiedades:
    ///  - onlyOnce=true => Solo se activa la primera vez. En false se activa siempre que pasas por el BoundingBox (default false)
    ///  - text=Bla Bla => Texto a mostrar en el dialogo. Sin enter. Se pagina solo. 
    /// 
    /// </summary>
    public class PositionDialog : CustomEvent, IAlphaObject
    {
        enum State
        {
            ACTIVE,
            INACTIVE,
            GOING_OUT,
        }

        TgcBoundingAxisAlignBox boundingBox;
        bool onlyOnce;
        string text;
        State currentState;

        public override void load(TgcMeshData meshData, string texturePath, TgcSceneData originalSceneData)
        {
            boundingBox = GameUtils.createBBFromMeshData(meshData);

            //Propiedad: onlyOnce=true (default=false)
            if (GameUtils.containsUserProp(meshData.userProperties, "onlyOnce"))
            {
                onlyOnce = bool.Parse(meshData.userProperties["onlyOnce"]);
            }
            else
            {
                onlyOnce = false;
            }
            //Propiedad: text=Hola como estas... (obligatoria)
            if (GameUtils.containsUserProp(meshData.userProperties, "text"))
            {
                text = meshData.userProperties["text"];
            }
            else
            {
                throw new Exception("The dialog has not text: " + meshData.name);
            }
        }

        public override void init()
        {
            currentState = State.INACTIVE;
        }

        public override void end()
        {
        }

        public override void update(float elapsedTime)
        {
            TextDialog dialog = GameModel.instance.textDialog;
            MainCharacter mainCharacter = GameModel.instance.mainCharacter;

            switch (currentState)
            {
                case State.ACTIVE:
                    bool finished = dialog.update();
                    if (finished)
                    {
                        currentState = State.INACTIVE;
                        if (onlyOnce)
                        {
                            this.Enabled = false;
                        }
                        else
                        {
                            if (mainCharacter.collideWithAABB(boundingBox))
                            {
                                currentState = State.GOING_OUT;
                            }
                        }
                    }
                    break;
                case State.INACTIVE:
                    if (mainCharacter.collideWithAABB(boundingBox))
                    {
                        currentState = State.ACTIVE;
                        dialog.setText(text);
                        GameModel.instance.sonidos.dialogo_abrir.play();
                    }
                    break;
                case State.GOING_OUT:
                    if (!mainCharacter.collideWithAABB(boundingBox))
                    {
                        currentState = State.INACTIVE;
                    }
                    break;
            }
        }

        public override void render()
        {
            if (currentState == State.ACTIVE)
            {
                GameModel.instance.currentLevel.AlphaObjects.Add(this);
            }

            if (GameModel.instance.debugMode.Enabled && GameModel.instance.debugMode.ShowBoundingVolumes)
            {
                boundingBox.Render();
            }
        }

        public void renderWithAlpha()
        {
            GameModel.instance.textDialog.render();
        }

        public override void renderDebug()
        {
            boundingBox.Render();
        }

        public override ColliderObject getColliderObject()
        {
            return null;
        }

        public override void dispose()
        {
            boundingBox.Dispose();
            boundingBox = null;
        }

        

    }
}
