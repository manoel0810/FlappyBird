using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FlappyBird
{
    public class GameState
    {
        [AllowNull]
        private static GameState Instance;
        //private readonly Counter Contador;
        private readonly SoundController SoundPlayer;
        private readonly MainWindow Main;
        private readonly Pipe Pipes;
        private readonly Bird Passaro;

        private GameTime TimeOfDay;
        private readonly int FrameRate = 60;
        private readonly double TimePerFrame = 0d;

        public bool IsGameOver = false;
        //private bool IsGameRunnin = false;
        private bool StartGame = false;
        private bool MouseUp = false;

        private int Decremente = 2;
        private int FrameCount = 0;
        private int Points = 0;

        public static GameState GetInstace(int Rate = 60)
        {
            Instance ??= new GameState(Rate);
            return Instance;
        }

        private GameState(int Rate = 60)
        {
            Main = MainWindow.Instance;
            RandomDayTime();

            FrameRate = Rate;
            //Contador = Counter.GetInstance();
            TimePerFrame = FrameToTime();
            Pipes = Pipe.GetInstance(TimeOfDay);
            Passaro = new Bird(new Thickness(Main.Bird.Margin.Left, Main.Bird.Margin.Top, Main.Bird.Margin.Right, Main.Bird.Margin.Bottom), Main, this);
            SoundPlayer = SoundController.GetInstance();
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


        private void UpdateBirdSourceImage()
        {
            if (!(FrameCount % 4 != 0))
                Passaro.UpdateBirdSkin();

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

        private void UpdateGame()
        {
            if (!StartGame)
                SetBird();
            else
            {
                if (!IsGameOver)
                {
                    UpdateBirdSourceImage();
                    Passaro.BirdMove(!MouseUp);
                    int newPoint = Pipes.Move(3);
                    if (newPoint != 0)
                        SoundPlayer.Play(SoundController.Sounds.PointWint);

                    Points += newPoint;
                    bool pipeHit = Pipes.BirdHits(Passaro);

                    if (!IsGameOver)
                        IsGameOver = pipeHit;

                    if (IsGameOver)
                        SoundPlayer.Play(SoundController.Sounds.Hit);
                }
            }
        }

        private void GameOver()
        {
            Main.GameOverMenu.Visibility = Visibility.Visible;
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

        public enum GameTime
        {
            Day,
            Night
        }
    }
}
