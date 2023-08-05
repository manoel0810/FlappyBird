using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FlappyBird
{
    public class Pipe
    {
        [AllowNull]
        private static Pipe _instance;
        private readonly MainWindow _main;
        private readonly Random _randomizer = new();
        private readonly GameState.GameTime _timeOfDay;
        private List<Thickness> Thicknesses = new();
        private readonly int Spacing = 150;

        private readonly ImageSource[] _pipesUp = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/pipe-green.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/pipe-red.png", UriKind.Relative)),
        };

        private readonly ImageSource[] _pipesDown = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/pipe-green-down.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/pipe-red-down.png", UriKind.Relative)),
        };

        private Pipe(GameState.GameTime time)
        {
            _main = MainWindow.Instance;
            _timeOfDay = time;
        }

        public static Pipe GetInstance(GameState.GameTime time)
        {
            _instance ??= new Pipe(time);
            return _instance;
        }

        private double[] GetRandomizedTopBottom()
        {
            int point = _randomizer.Next(200, 400);
            return new double[] { point + Spacing, 600 - (point + Spacing / 2) };
        }

        private void CreateMargins()
        {
            for (int i = 0; i < 2; i++)
            {
                int plus = i == 0 ? 0 : 150;
                double[] values = GetRandomizedTopBottom();
                Thicknesses.Add(new Thickness(400 + plus, 0, -50 + -plus, values[0]));
                Thicknesses.Add(new Thickness(400 + plus, values[1], -50 + -plus, 0));
            }
        }

        private void RecalculateMargins(double Increment)
        {
            if (Thicknesses.Count < 4)
                CreateMargins();

            for (int i = 0; i < Thicknesses.Count; i++)
                Thicknesses[i] = new Thickness(Thicknesses[i].Left - Increment, Thicknesses[i].Top, Thicknesses[i].Right + Increment, Thicknesses[i].Bottom);
        }

        private void CheckOutside()
        {
            for (int i = 0; i < 2; i++)
            {
                if (Thicknesses[(i == 0 ? 0 : 2)].Right >= 400)
                {

                    int dist = _randomizer.Next(-100, -100);
                    int left = Math.Abs(50 - Math.Abs(dist));

                    if (i == 0)
                    {
                        double[] values = GetRandomizedTopBottom();
                        Thicknesses[i] = new Thickness(400 + left, 0, dist, values[0]);
                        Thicknesses[i + 1] = new Thickness(400 + left, values[1], dist, 0);
                    }
                    else
                    {
                        double[] values = GetRandomizedTopBottom();
                        Thicknesses[2] = new Thickness(400 + left, 0, dist, values[0]);
                        Thicknesses[3] = new Thickness(400 + left, values[1], dist, 0);
                    }
                }
            }
        }

        public void Move(int length)
        {
            RecalculateMargins(length);
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        _main.p1.Margin = Thicknesses[0];
                        break;
                    case 1:
                        _main.p2.Margin = Thicknesses[1];
                        break;
                    case 2:
                        _main.p3.Margin = Thicknesses[2];
                        break;
                    case 3:
                        _main.p4.Margin = Thicknesses[3];
                        break;
                }
            }

            CheckOutside();
        }

        public bool BirdHits(Bird Passaro)
        {
            if ((Passaro.Position.Bottom >= 600 - Thicknesses[1].Top && Passaro.Position.Top >= 600 - Thicknesses[0].Bottom) || Passaro.Position.Right >= Thicknesses[0].Right)
                return false;

            return true;
        }

        private void SetImage()
        {
            _main.p1.Source = _timeOfDay == GameState.GameTime.Day ? _pipesDown[0] : _pipesDown[1];
            _main.p1.Margin = Thicknesses[0];
            _main.p1.Visibility = Visibility.Visible;

            _main.p2.Source = _timeOfDay == GameState.GameTime.Day ? _pipesUp[0] : _pipesUp[1];
            _main.p2.Margin = Thicknesses[1];
            _main.p2.Visibility = Visibility.Visible;

            _main.p3.Source = _timeOfDay == GameState.GameTime.Day ? _pipesDown[0] : _pipesDown[1];
            _main.p3.Margin = Thicknesses[2];
            _main.p3.Visibility = Visibility.Visible;

            _main.p4.Source = _timeOfDay == GameState.GameTime.Day ? _pipesUp[0] : _pipesUp[1];
            _main.p4.Margin = Thicknesses[3];
            _main.p4.Visibility = Visibility.Visible;
        }

        public void Load()
        {
            CreateMargins();
            SetImage();
        }
    }
}
