using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FlappyBird
{
    public class GameState
    {
        [AllowNull]
        private static GameState Instance;
        private readonly Counter Contador;
        private readonly MainWindow Main;
        private readonly Pipe Pipes;
        private readonly int FrameRate = 60;
        private readonly double TimePerFrame = 0d;

        private readonly Bird Passaro;
        private BirdColor birdColor;
        private GameTime TimeOfDay;
        private bool IsGameOver = false;
        private bool IsGameRunnin = false;
        private bool StartGame = false;
        private bool Reverse = false;
        private bool MouseUp = false;

        private int FallVelocity = 8;
        private int Decremente = 2;
        private int SkinIndex = 0;
        private int FrameCount = 0;


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

        private readonly ImageSource GameOverImage = new BitmapImage(new Uri("Assets/gameover.png", UriKind.Relative));

        public static GameState GetInstace(ref MainWindow Windows, int Rate = 60)
        {
            Instance ??= new GameState(ref Windows, Rate);
            return Instance;
        }

        private GameState(ref MainWindow Windows, int Rate = 60)
        {
            Main = Windows;
            RandomDayTime();

            FrameRate = Rate;
            Contador = Counter.GetInstance();
            TimePerFrame = FrameToTime();
            Pipes = Pipe.GetInstance(ref Windows, TimeOfDay);
            Passaro = new Bird(new Thickness(Windows.Bird.Margin.Left, Windows.Bird.Margin.Top, Windows.Bird.Margin.Right, Windows.Bird.Margin.Bottom));
            RandomSkin();
        }

        public void MouseState(bool state)
        {
            MouseUp = state;
        }

        private void RandomDayTime()
        {
            var random = new Random();
            int EnumIndex = random.Next(0, 2);
            TimeOfDay = (GameTime)(EnumIndex <= 1 ? EnumIndex : 2);

            if (TimeOfDay == GameTime.Night)
                Main.GameTemplate.Source = new BitmapImage(new Uri("Assets/background-night.png", UriKind.Relative));
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

        private void UpdateBirdSkin()
        {
            if (FrameCount % 4 != 0)
                return;

            if (birdColor == BirdColor.Blue)
            {
                Passaro.BirdImage = BlueBird[GetNextImageIndex()];
            }
            else if (birdColor == BirdColor.Red)
            {
                Passaro.BirdImage = RedBird[GetNextImageIndex()];
            }
            else if (birdColor == BirdColor.Yellow)
            {
                Passaro.BirdImage = YellowBird[GetNextImageIndex()];
            }
        }

        private void UpdateBirdSourceImage()
        {
            UpdateBirdSkin();
            Main.Bird.Source = Passaro.BirdImage;
        }

        private Thickness GetNewThickness(Thickness Actual)
        {
            if (Actual.Top >= 360 || Actual.Top <= 340)
                Decremente *= -1;

            return new Thickness(50, Actual.Top - Decremente, 300, Actual.Bottom + Decremente);
        }

        private void SetBird()
        {
            Main.Bird.Margin = GetNewThickness(Main.Bird.Margin);
            UpdateBirdSourceImage();
        }

        private void BirdMove()
        {
            Thickness Actual = Passaro.Position;
            if (!MouseUp)
            {
                if (Actual.Top <= 0)
                    return;

                Passaro.Position = new Thickness(50, Actual.Top - FallVelocity, 300, Actual.Bottom + FallVelocity);
                Main.Bird.Margin = Passaro.Position;
            }
            else
            {
                if (Actual.Bottom <= 80)
                {
                    IsGameOver = true;
                    return;
                }

                Passaro.Position = new Thickness(50, Actual.Top + FallVelocity, 300, Actual.Bottom - FallVelocity);
                Main.Bird.Margin = Passaro.Position;
            }
        }

        private void UpdateGame()
        {
            if (!StartGame)
                SetBird();
            else
            {
                if (!IsGameOver)
                {
                    UpdateBirdSourceImage();
                    BirdMove();
                    Pipes.Move(10);
                }
            }
        }

        private void GameOver()
        {
            Main.StartImage.Source = GameOverImage;
            Main.StartImage.Visibility = Visibility.Visible;
        }

        public void InitializeGameState()
        {
            Pipes.Load();
            Tick();
        }

        public void StartFlappyBird()
        {
            StartGame = true;
        }

        private double FrameToTime()
        {
            return 1000d / FrameRate;
        }

        private async void Tick()
        {
            while (!IsGameOver)
            {
                UpdateGame();
                await Task.Delay((int)TimePerFrame);
                FrameCount++;
            }

            GameOver();
        }

        private enum BirdColor
        {
            Blue,
            Red,
            Yellow
        }

        public enum GameTime
        {
            Day,
            Night
        }
    }
}
