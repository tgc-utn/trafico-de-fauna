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
using TGC.Core.SceneLoader;
using Trafico.De.Fauna.Model.CustomEvents.Billboards;
using Trafico.De.Fauna.Model.CustomEvents.Dialogs;
using Trafico.De.Fauna.Model.CustomEvents.Particulas;
using Trafico.De.Fauna.Model.CustomEvents.Piedras;
using Trafico.De.Fauna.Model.CustomEvents.Surfaces;
using Trafico.De.Fauna.Model.CustomEvents.ZonasEspeciales;

namespace Trafico.De.Fauna.Model.CustomEvents
{
    /// <summary>
    /// Herramienta para manejar la carga de CustomEvents
    /// </summary>
    public class CustomEventsManager
    {
        Dictionary<string, Type> events = new Dictionary<string, Type>();

        public CustomEventsManager()
        {
            //Lista de eventos
            addEvent(typeof(Prueba));
            addEvent(typeof(PositionDialog));
            addEvent(typeof(ParticulasHumo));
            addEvent(typeof(ParticulasFuego));
            addEvent(typeof(ParticulasHojasCayendo));
            addEvent(typeof(WaterEffect));
            addEvent(typeof(LanzadorPiedras));
            addEvent(typeof(BillboardExplosion));
            addEvent(typeof(WaterSurface));
            addEvent(typeof(ZonaDeMuerte));
            addEvent(typeof(MeshChorreando));
            addEvent(typeof(ParticulasAgua));
            
            
        }

        private void addEvent(Type eventType)
        {
            events[eventType.Name.ToUpper()] = eventType;
        }

        public Type getCustomEventType(string eventName)
        {
            try
            {
                Type eventType = events[eventName];
                return eventType;
            }
            catch (Exception e)
            {
                throw new Exception("CustomEvent inexistente: " + eventName, e);
            }
            
        }

        /// <summary>
        /// Crear instancia de evento
        /// </summary>
        public CustomEvent createEvent(string eventName, TgcMeshData meshData, string texturePath, TgcSceneData originalSceneData)
        {
            Type eventType = getCustomEventType(eventName);
            CustomEvent customEvent = (CustomEvent)Activator.CreateInstance(eventType);

            //enabled
            if (GameUtils.containsUserProp(meshData.userProperties, "enabled"))
            {
                customEvent.Enabled = bool.Parse(meshData.userProperties["enabled"]);
            }

            customEvent.load(meshData, texturePath, originalSceneData);
            return customEvent;
        }

        /// <summary>
        /// Parsear info de CustomEvent
        /// </summary>
        public CustomEvent loadCustomEvent(TgcMeshData meshData, string texturePath, TgcSceneData originalSceneData)
        {
            string[] split = meshData.name.Split('_');
            if (split.Length != 2)
            {
                throw new Exception("CustomEvent con nombre icorrecto: " + meshData.name);
            }

            string eventName = split[0].ToUpper();
            return createEvent(eventName, meshData, texturePath, originalSceneData);
        }
    }
}
