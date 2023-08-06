using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace FlappyBird
{
    public class SoundController
    {
        [AllowNull]
        private static SoundController instance;
        [AllowNull]
        private SoundPlayer soundPlayer;
        private bool IsPlaying = false;

        /// <summary>
        /// Retorna uma intância do objeto <b>SoundController</b>
        /// </summary>
        /// <returns>
        /// Retorna a instância atual caso exista. Ao contrário, cria uma nova instância para o tipo e a retorna
        /// </returns>

        public static SoundController GetInstance()
        {
            instance ??= new SoundController();
            return instance;
        }

        private SoundController()
        {

        }

        private readonly Dictionary<Sounds, string> AvaibleSounds = new()
        {
            { Sounds.Hit,       $"{AppDomain.CurrentDomain.BaseDirectory}\\Assets\\hit.wav" },
            { Sounds.PointWint, $"{AppDomain.CurrentDomain.BaseDirectory}\\Assets\\point.wav" },
            { Sounds.FlyingUp,  $"{AppDomain.CurrentDomain.BaseDirectory}\\Assets\\wing.wav" },
            { Sounds.FlyingDown,$"{AppDomain.CurrentDomain.BaseDirectory}\\Assets\\swoosh.wav" },
        };

        /// <summary>
        /// Obtém um som para o jogo
        /// </summary>
        /// <param name="Sound">Enum de identidicação do som</param>
        /// <returns></returns>

        public string this[Sounds Sound]
        {
            get => AvaibleSounds[Sound];
        }

        /// <summary>
        /// Reproduz um som, se disponível
        /// </summary>
        /// <param name="sound">Som para reprodução</param>

        public async void Play(Sounds sound)
        {
            if (sound == Sounds.PointWint)
                goto SKIP;

            if (IsPlaying)
                return;

            SKIP:;
            if (File.Exists(AvaibleSounds[sound]))
            {
                IsPlaying = true;
                soundPlayer = new SoundPlayer(AvaibleSounds[sound]);
                soundPlayer.Play();

                await Task.Delay(100);
                IsPlaying = false;
            }
        }

        /// <summary>
        /// Sons disponíveis
        /// </summary>

        public enum Sounds
        {
            /// <summary>
            /// Atingido
            /// </summary>
            Hit,
            /// <summary>
            /// Ganho de pontos
            /// </summary>
            PointWint,
            /// <summary>
            /// Voando para cima
            /// </summary>
            FlyingUp,
            /// <summary>
            /// Voando para baixo
            /// </summary>
            FlyingDown
        }
    }
}
