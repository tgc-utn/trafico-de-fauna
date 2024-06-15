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
using TGC.Core.Sound;

namespace Trafico.De.Fauna.Model.Sonidos
{
    /// <summary>
    ///     Sonidos y musicas del juego
    /// </summary>
    public class GameSounds
    {
        public TGCStaticSound alarma_guardias;
        public TGCStaticSound banana_agarrar;
        public TGCStaticSound banana_agarrar_varias;
        public TGCStaticSound caja_bananas_golpear;
        public TGCStaticSound caja_bananas_romper;
        public TGCStaticSound dialogo_abrir;
        public TGCStaticSound game_over;
        public TGCStaticSound game_win;
        public TGCStaticSound itemAnimal_fallo_liberar;
        public TGCStaticSound itemAnimal_liberar;

        public TGCStaticSound llave_agarrar;
        public TGCMP3Player mp3Player;
        public string music_alarm;

        public string music_default;
        public string music_menu;
        public TGCStaticSound personaje_agacharse;
        public TGCStaticSound personaje_ganar_vida;
        public TGCStaticSound personaje_golpe;
        public TGCStaticSound personaje_levantar_escondite_movil;
        public TGCStaticSound personaje_pasos_cargando_escondite;
        public TGCStaticSound personaje_pasos_correr;
        public TGCStaticSound personaje_perder_vida;
        public TGCStaticSound personaje_saltar;
        public TGCStaticSound piedra_golpear_personaje;
        public TGCStaticSound piedra_rodando;

        private readonly List<TGCStaticSound> sounds;
        private readonly List<TemporalSound> temporalSounds;

        public GameSounds(TGCDirectSound directSound)
        {
            var sonidosPath = GameModel.instance.gameConfig.sonidosPath;
            sounds = new List<TGCStaticSound>();
            temporalSounds = new List<TemporalSound>();

            //Music
            mp3Player = new TGCMP3Player();
            music_default = sonidosPath + "Music\\" + "music_normal.mp3";
            music_alarm = sonidosPath + "Music\\" + "music_alarm.mp3";
            music_menu = sonidosPath + "Music\\" + "music_menu.mp3";
            mp3Player.FileName = music_default;

            //Llaves
            llave_agarrar = new TGCStaticSound();
            llave_agarrar.LoadSound(sonidosPath + "llave_agarrar.wav", directSound.DsDevice);
            sounds.Add(llave_agarrar);

            //ItemsAnimal
            itemAnimal_liberar = new TGCStaticSound();
            itemAnimal_liberar.LoadSound(sonidosPath + "itemAnimal_liberar.wav", directSound.DsDevice);
            sounds.Add(itemAnimal_liberar);
            itemAnimal_fallo_liberar = new TGCStaticSound();
            itemAnimal_fallo_liberar.LoadSound(sonidosPath + "itemAnimal_fallo_liberar.wav", directSound.DsDevice);
            sounds.Add(itemAnimal_fallo_liberar);

            //Game over
            game_over = new TGCStaticSound();
            game_over.LoadSound(sonidosPath + "game_over.wav", directSound.DsDevice);
            sounds.Add(game_over);

            //Game win
            game_win = new TGCStaticSound();
            game_win.LoadSound(sonidosPath + "game_win.wav", directSound.DsDevice);
            sounds.Add(game_win);

            //Alarma cuando te detecta un guardia
            alarma_guardias = new TGCStaticSound();
            alarma_guardias.LoadSound(sonidosPath + "alarma_guardias.wav", directSound.DsDevice);
            sounds.Add(alarma_guardias);

            //Dialogo de texto
            dialogo_abrir = new TGCStaticSound();
            dialogo_abrir.LoadSound(sonidosPath + "dialogo_abrir.wav", directSound.DsDevice);
            sounds.Add(dialogo_abrir);

            //Agarrar banana
            banana_agarrar = new TGCStaticSound();
            banana_agarrar.LoadSound(sonidosPath + "banana_agarrar.wav", directSound.DsDevice);
            sounds.Add(banana_agarrar);

            //Agarrar varias bananas
            banana_agarrar_varias = new TGCStaticSound();
            banana_agarrar_varias.LoadSound(sonidosPath + "banana_agarrar_varias.wav", directSound.DsDevice);
            sounds.Add(banana_agarrar_varias);

            //Romper caja de bananas
            caja_bananas_romper = new TGCStaticSound();
            caja_bananas_romper.LoadSound(sonidosPath + "caja_bananas_romper.wav", directSound.DsDevice);
            sounds.Add(caja_bananas_romper);

            //Golpear caja de bananas
            caja_bananas_golpear = new TGCStaticSound();
            caja_bananas_golpear.LoadSound(sonidosPath + "caja_bananas_golpear.wav", directSound.DsDevice);
            sounds.Add(caja_bananas_golpear);

            //Golpe de personaje principal
            personaje_golpe = new TGCStaticSound();
            personaje_golpe.LoadSound(sonidosPath + "personaje_golpe.wav", directSound.DsDevice);
            sounds.Add(personaje_golpe);

            //Cuando el personaje salta
            personaje_saltar = new TGCStaticSound();
            personaje_saltar.LoadSound(sonidosPath + "personaje_saltar.wav", directSound.DsDevice);
            sounds.Add(personaje_saltar);

            //Cuando el personaje se agacha
            personaje_agacharse = new TGCStaticSound();
            personaje_agacharse.LoadSound(sonidosPath + "personaje_agacharse.wav", directSound.DsDevice);
            sounds.Add(personaje_agacharse);

            //Cuando el personaje gana una vida
            personaje_ganar_vida = new TGCStaticSound();
            personaje_ganar_vida.LoadSound(sonidosPath + "personaje_ganar_vida.wav", directSound.DsDevice);
            sounds.Add(personaje_ganar_vida);

            //Cuando el personaje pierde una vida (te atrapa un guardia, caes en un pozo, etc)
            personaje_perder_vida = new TGCStaticSound();
            personaje_perder_vida.LoadSound(sonidosPath + "personaje_perder_vida.wav", directSound.DsDevice);
            sounds.Add(personaje_perder_vida);

            //Pasos para cuando el personaje corre
            personaje_pasos_correr = new TGCStaticSound();
            personaje_pasos_correr.LoadSound(sonidosPath + "personaje_pasos_correr.wav", directSound.DsDevice);
            sounds.Add(personaje_pasos_correr);

            //Pasos para cuando el personaje camina cargando un escondite movil
            personaje_pasos_cargando_escondite = new TGCStaticSound();
            personaje_pasos_cargando_escondite.LoadSound(sonidosPath + "personaje_pasos_cargando_escondite.wav",
                directSound.DsDevice);
            sounds.Add(personaje_pasos_cargando_escondite);

            //Cuando el personaje levanta un escondite movil
            personaje_levantar_escondite_movil = new TGCStaticSound();
            personaje_levantar_escondite_movil.LoadSound(sonidosPath + "personaje_levantar_escondite_movil.wav",
                directSound.DsDevice);
            sounds.Add(personaje_levantar_escondite_movil);

            //Piedra rodando
            piedra_rodando = new TGCStaticSound();
            piedra_rodando.LoadSound(sonidosPath + "piedra_rodando.wav", directSound.DsDevice);
            sounds.Add(piedra_rodando);

            //Piedra cuando golpea al personaje
            piedra_golpear_personaje = new TGCStaticSound();
            piedra_golpear_personaje.LoadSound(sonidosPath + "piedra_golpear_personaje.wav", directSound.DsDevice);
            sounds.Add(piedra_golpear_personaje);
        }

        public void playMusic(string music)
        {
            mp3Player.CloseFile();
            mp3Player.FileName = music;
            mp3Player.Play(true);
        }

        /// <summary>
        ///     Para todas las musicas y sonidos
        /// </summary>
        public void stopAllSounds()
        {
            foreach (var sound in sounds)
            {
                sound.Stop();
            }

            foreach (var sound in temporalSounds)
            {
                sound.Sound.Stop();
            }

            mp3Player.CloseFile();
        }

        public void addTemporalSound(TemporalSound s)
        {
            temporalSounds.Add(s);
        }
    }
}
