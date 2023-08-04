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
        private readonly int FrameRate = 60;
        private readonly double TimePerFrame = 0d;

        private readonly Bird Passaro;
        private BirdColor birdColor;
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

        public static GameState GetInstace(MainWindow Windows, int Rate = 60)
        {
            Instance ??= new GameState(Windows, Rate);
            return Instance;
        }

        private GameState(MainWindow Windows, int Rate = 60)
        {
            Main = Windows;
            FrameRate = Rate;
            Contador = Counter.GetInstance();
            TimePerFrame = FrameToTime();
            Passaro = new Bird(Windows);
            RandomSkin();
        }

        public void MouseState(bool state)
        {
            MouseUp = state;
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
            if (FrameCount % 3 != 0)
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
            Passaro.Position = new Pos(60f, Passaro.Position.GetPosition().Y - 2f);
            Main.Bird.Margin = GetNewThickness(Main.Bird.Margin);
            UpdateBirdSourceImage();
        }

        private BitmapSource Rotate(BitmapSource originalImage, double angleInDegrees)
        {
            // Crie uma matriz de transformação para aplicar a rotação
            var transform = new RotateTransform(angleInDegrees);

            // Crie um DrawingVisual para renderizar a imagem com a rotação
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.PushTransform(transform);
                drawingContext.DrawImage(originalImage, new Rect(new Size(originalImage.PixelWidth, originalImage.PixelHeight)));
                drawingContext.Pop();
            }

            // Renderize o DrawingVisual em um novo RenderTargetBitmap para criar a imagem BitmapSource rotacionada
            var rotatedImage = new RenderTargetBitmap(
                originalImage.PixelWidth, originalImage.PixelHeight,
                originalImage.DpiX, originalImage.DpiY,
                PixelFormats.Default);
            rotatedImage.Render(drawingVisual);

            return rotatedImage;
        }

        private void RotateSkin(double Angle, ImageSource Src)
        {
            var IMG = Rotate((BitmapSource)Src, Angle);
            Passaro.BirdImage = IMG;
        }

        private void BirdMove()
        {
            Thickness Actual = Main.Bird.Margin;
            if (!MouseUp)
            {
                if (Actual.Top <= 0)
                    return;

                Main.Bird.Margin = new Thickness(50, Actual.Top - FallVelocity, 300, Actual.Bottom + FallVelocity);
                //RotateSkin(-(Math.PI / 4), Passaro.BirdImage);
            }
            else
            {
                if (Actual.Bottom <= 80)
                {
                    IsGameOver = true;
                    return;
                }


                Main.Bird.Margin = new Thickness(50, Actual.Top + FallVelocity, 300, Actual.Bottom - FallVelocity);
                //RotateSkin(Math.PI / 4, Passaro.BirdImage);
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
    }
}
