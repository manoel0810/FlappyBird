using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FlappyBird
{
    /// <summary>
    /// Representa o pássaro no jogo FlappyBird
    /// </summary>

    public class Bird
    {
        [AllowNull]
        private ImageSource BirdSkin;
        private readonly SoundController SoundPlay;
        private Thickness T;
        private readonly int FallVelocity = 4;
        private readonly MainWindow Main;
        private readonly GameState gameState;
        private BirdColor birdColor;
        private int SkinIndex = 0;
        private bool Reverse = false;
        private bool StartFall = false;

        private readonly ImageSource[] RedBird = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/redbird-downflap.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/redbird-midflap.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/redbird-upflap.png", UriKind.Relative)),
        };

        private readonly ImageSource[] BlueBird = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/bluebird-downflap.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/bluebird-midflap.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/bluebird-upflap.png", UriKind.Relative))
        };

        private readonly ImageSource[] YellowBird = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/yellowbird-downflap.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/yellowbird-midflap.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/yellowbird-upflap.png", UriKind.Relative))
        };


        /// <summary>
        /// Cria um novo objeto do tipo <b>Bird</b> com Margin <b>T</b>
        /// </summary>
        /// <param name="t"></param>

        public Bird(Thickness t, MainWindow main, GameState state)
        {
            T = t;
            Main = main;
            gameState = state;
            RandomSkin();
            SoundPlay = SoundController.GetInstance();
        }

        /// <summary>
        /// Obtém ou define a posição atual do pássaro
        /// </summary>

        public Thickness Position
        {
            get => T;
            set => T = value;
        }

        private void RandomSkin()
        {
            var random = new Random();
            int EnumIndex = random.Next(0, 3);
            birdColor = (BirdColor)(EnumIndex <= 1 ? EnumIndex : 2);
        }

        private int GetNextImageIndex()
        {
            if (Reverse)
            {
                SkinIndex = 0;
                Reverse = false;
                return SkinIndex;
            }

            if (SkinIndex < 2)
            {
                SkinIndex++;
                return SkinIndex;
            }
            else
            {
                SkinIndex = 1;
                Reverse = true;
                return SkinIndex;
            }
        }

        public void UpdateBirdSkin()
        {
            if (birdColor == BirdColor.Blue)
            {
                BirdImage = BlueBird[GetNextImageIndex()];
            }
            else if (birdColor == BirdColor.Red)
            {
                BirdImage = RedBird[GetNextImageIndex()];
            }
            else if (birdColor == BirdColor.Yellow)
            {
                BirdImage = YellowBird[GetNextImageIndex()];
            }
        }

        /// <summary>
        /// Obtém ou define a imagem atual do pássaro
        /// </summary>

        public ImageSource BirdImage
        {
            get => BirdSkin;
            set => BirdSkin = value;
        }

        public void BirdMove(bool moveUp)
        {
            Thickness Actual = T;
            if (moveUp)
            {
                if (Actual.Top <= 0)
                    return;

                T = new Thickness(50, Actual.Top - FallVelocity, 300, Actual.Bottom + FallVelocity);
                Main.Bird.Margin = T;

                if (!StartFall)
                    SoundPlay.Play(SoundController.Sounds.FlyingUp);

                StartFall = true;
            }
            else
            {
                if (Actual.Bottom <= 80)
                {
                    gameState.IsGameOver = true;
                    return;
                }

                T = new Thickness(50, Actual.Top + FallVelocity, 300, Actual.Bottom - FallVelocity);
                Main.Bird.Margin = T;

                if (StartFall)
                {
                    SoundPlay.Play(SoundController.Sounds.FlyingDown);
                    StartFall = false;
                }
            }
        }

        private enum BirdColor
        {
            Blue,
            Red,
            Yellow
        }
    }
}
