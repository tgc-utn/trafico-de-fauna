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
using TraficoFauna.Examples;

namespace Examples.TraficoFauna.Sonidos
{
    /// <summary>
    /// Sonidos y musicas del juego
    /// </summary>
    public class GameSounds
    {
        public TgcMp3Player mp3Player;

        public TgcStaticSound llave_agarrar;
        public TgcStaticSound itemAnimal_liberar;
        public TgcStaticSound itemAnimal_fallo_liberar;
        public TgcStaticSound game_over;
        public TgcStaticSound game_win;
        public TgcStaticSound alarma_guardias;
        public TgcStaticSound dialogo_abrir;
        public TgcStaticSound banana_agarrar;
        public TgcStaticSound banana_agarrar_varias;
        public TgcStaticSound caja_bananas_romper;
        public TgcStaticSound caja_bananas_golpear;
        public TgcStaticSound personaje_golpe;
        public TgcStaticSound personaje_saltar;
        public TgcStaticSound personaje_agacharse;
        public TgcStaticSound personaje_ganar_vida;
        public TgcStaticSound personaje_perder_vida;
        public TgcStaticSound personaje_pasos_correr;
        public TgcStaticSound personaje_pasos_cargando_escondite;
        public TgcStaticSound piedra_rodando;
        public TgcStaticSound piedra_golpear_personaje;
        public TgcStaticSound personaje_levantar_escondite_movil;
        


        public string music_default;
        public string music_alarm;
        public string music_menu;

        List<TgcStaticSound> sounds;
        List<TemporalSound> temporalSounds;

        public GameSounds(TgcDirectSound directSound)
        {
            string sonidosPath = GameModel.instance.gameConfig.sonidosPath;
            sounds = new List<TgcStaticSound>();
            temporalSounds = new List<TemporalSound>();

            //Music
            mp3Player = new TgcMp3Player();
            music_default = sonidosPath + "Music\\" + "music_normal.mp3";
            music_alarm = sonidosPath + "Music\\" + "music_alarm.mp3";
            music_menu = sonidosPath + "Music\\" + "music_menu.mp3";
            mp3Player.FileName = music_default;

            //Llaves
            llave_agarrar = new TgcStaticSound();
            llave_agarrar.loadSound(sonidosPath + "llave_agarrar.wav", directSound.DsDevice);
            sounds.Add(llave_agarrar);

            //ItemsAnimal
            itemAnimal_liberar = new TgcStaticSound();
            itemAnimal_liberar.loadSound(sonidosPath + "itemAnimal_liberar.wav", directSound.DsDevice);
            sounds.Add(itemAnimal_liberar);
            itemAnimal_fallo_liberar = new TgcStaticSound();
            itemAnimal_fallo_liberar.loadSound(sonidosPath + "itemAnimal_fallo_liberar.wav", directSound.DsDevice);
            sounds.Add(itemAnimal_fallo_liberar);

            //Game over
            game_over = new TgcStaticSound();
            game_over.loadSound(sonidosPath + "game_over.wav", directSound.DsDevice);
            sounds.Add(game_over);

            //Game win
            game_win = new TgcStaticSound();
            game_win.loadSound(sonidosPath + "game_win.wav", directSound.DsDevice);
            sounds.Add(game_win);

            //Alarma cuando te detecta un guardia
            alarma_guardias = new TgcStaticSound();
            alarma_guardias.loadSound(sonidosPath + "alarma_guardias.wav", directSound.DsDevice);
            sounds.Add(alarma_guardias);

            //Dialogo de texto
            dialogo_abrir = new TgcStaticSound();
            dialogo_abrir.loadSound(sonidosPath + "dialogo_abrir.wav", directSound.DsDevice);
            sounds.Add(dialogo_abrir);

            //Agarrar banana
            banana_agarrar = new TgcStaticSound();
            banana_agarrar.loadSound(sonidosPath + "banana_agarrar.wav", directSound.DsDevice);
            sounds.Add(banana_agarrar);

            //Agarrar varias bananas
            banana_agarrar_varias = new TgcStaticSound();
            banana_agarrar_varias.loadSound(sonidosPath + "banana_agarrar_varias.wav", directSound.DsDevice);
            sounds.Add(banana_agarrar_varias);

            //Romper caja de bananas
            caja_bananas_romper = new TgcStaticSound();
            caja_bananas_romper.loadSound(sonidosPath + "caja_bananas_romper.wav", directSound.DsDevice);
            sounds.Add(caja_bananas_romper);

            //Golpear caja de bananas
            caja_bananas_golpear = new TgcStaticSound();
            caja_bananas_golpear.loadSound(sonidosPath + "caja_bananas_golpear.wav", directSound.DsDevice);
            sounds.Add(caja_bananas_golpear);

            //Golpe de personaje principal
            personaje_golpe = new TgcStaticSound();
            personaje_golpe.loadSound(sonidosPath + "personaje_golpe.wav", directSound.DsDevice);
            sounds.Add(personaje_golpe);
            
            //Cuando el personaje salta
            personaje_saltar = new TgcStaticSound();
            personaje_saltar.loadSound(sonidosPath + "personaje_saltar.wav", directSound.DsDevice);
            sounds.Add(personaje_saltar);
         
            //Cuando el personaje se agacha
            personaje_agacharse = new TgcStaticSound();
            personaje_agacharse.loadSound(sonidosPath + "personaje_agacharse.wav", directSound.DsDevice);
            sounds.Add(personaje_agacharse);
            
            //Cuando el personaje gana una vida
            personaje_ganar_vida = new TgcStaticSound();
            personaje_ganar_vida.loadSound(sonidosPath + "personaje_ganar_vida.wav", directSound.DsDevice);
            sounds.Add(personaje_ganar_vida);

            //Cuando el personaje pierde una vida (te atrapa un guardia, caes en un pozo, etc)
            personaje_perder_vida = new TgcStaticSound();
            personaje_perder_vida.loadSound(sonidosPath + "personaje_perder_vida.wav", directSound.DsDevice);
            sounds.Add(personaje_perder_vida);

            //Pasos para cuando el personaje corre
            personaje_pasos_correr = new TgcStaticSound();
            personaje_pasos_correr.loadSound(sonidosPath + "personaje_pasos_correr.wav", directSound.DsDevice);
            sounds.Add(personaje_pasos_correr);

            //Pasos para cuando el personaje camina cargando un escondite movil
            personaje_pasos_cargando_escondite = new TgcStaticSound();
            personaje_pasos_cargando_escondite.loadSound(sonidosPath + "personaje_pasos_cargando_escondite.wav", directSound.DsDevice);
            sounds.Add(personaje_pasos_cargando_escondite);

            //Cuando el personaje levanta un escondite movil
            personaje_levantar_escondite_movil = new TgcStaticSound();
            personaje_levantar_escondite_movil.loadSound(sonidosPath + "personaje_levantar_escondite_movil.wav", directSound.DsDevice);
            sounds.Add(personaje_levantar_escondite_movil);

            //Piedra rodando
            piedra_rodando = new TgcStaticSound();
            piedra_rodando.loadSound(sonidosPath + "piedra_rodando.wav", directSound.DsDevice);
            sounds.Add(piedra_rodando);

            //Piedra cuando golpea al personaje
            piedra_golpear_personaje = new TgcStaticSound();
            piedra_golpear_personaje.loadSound(sonidosPath + "piedra_golpear_personaje.wav", directSound.DsDevice);
            sounds.Add(piedra_golpear_personaje);
        }

        public void playMusic(string music)
        {
            mp3Player.closeFile();
            mp3Player.FileName = music;
            mp3Player.play(true);
        }

        /// <summary>
        /// Para todas las musicas y sonidos
        /// </summary>
        public void stopAllSounds()
        {
            foreach (TgcStaticSound sound in sounds)
            {
                sound.stop();
            }
            foreach (TemporalSound sound in temporalSounds)
            {
                sound.Sound.stop();
            }
            mp3Player.closeFile();
        }

        public void addTemporalSound(TemporalSound s)
        {
            this.temporalSounds.Add(s);
        }

    }
}
