using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FlappyBird
{
    public class Pipe
    {
        [AllowNull]
        private static Pipe Instance;
        private const int MinimumSpace = 20;
        private readonly MainWindow Main;
        private readonly Random Randomizer = new();
        private readonly GameState.GameTime TimeOfDay;
        private List<PipeImage[]> Pares = new();
        private int ElementCount = 0;
        private bool AddDistance = false;

        private readonly ImageSource[] Pipes = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/pipe-green.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/pipe-red.png", UriKind.Relative)),
        };

        public static Pipe GetInstance(ref MainWindow Window, GameState.GameTime Time)
        {
            Instance ??= new Pipe(ref Window, Time);
            return Instance;
        }

        private Pipe(ref MainWindow Window, GameState.GameTime Time)
        {
            Main = Window;
            TimeOfDay = Time;
        }

        private Thickness CalculateThinckness(PipeModel Model, int Height)
        {
            Thickness T = new()
            {
                Left = Main.Width,
                Right = AddDistance ? -40 : 0
            };

            AddDistance = !AddDistance;
            if (Model == PipeModel.Bottom)
            {
                T.Bottom = Height;
                T.Top = Main.Height - T.Bottom;
            }
            else if (Model == PipeModel.Top)
            {
                T.Top = Height;
                T.Bottom = Main.Height - T.Top;
            }

            return T;
        }

        private void GeneratePipes(ref List<PipeImage[]> pipes)
        {
            int iteracoes = 2 - pipes.Count;
            for (int i = 0; i < iteracoes; i++)
            {
                PipeImage[] PipesVector = new PipeImage[2];
                for (int j = 0; j < 2; j++)
                {
                    if (j == 0) //Bottom-top pipe
                    {
                        int Height = Randomizer.Next(200, 400);
                        PipesVector[j] = new PipeImage((TimeOfDay == GameState.GameTime.Day ? Pipes[0] : Pipes[1]), CalculateThinckness(PipeModel.Bottom, Height), $"ui_element{ElementCount++}");
                    }
                    else
                    {
                        int Resto = (int)Main.Height - (int)PipesVector[0].Margin.Bottom - MinimumSpace;
                        int Height = Randomizer.Next(60, Resto);
                        PipesVector[j] = new PipeImage((TimeOfDay == GameState.GameTime.Day ? Pipes[0] : Pipes[1]), CalculateThinckness(PipeModel.Bottom, Height), $"ui_element{ElementCount++}");
                    }
                }

                pipes.Add(PipesVector);
            }

            Pares = pipes;
        }

        private void RemoveOusideElement(PipeImage Element)
        {
            for (int i = 0; i < Main.MainGrid.Children.Count; i++)
            {
                if (Main.MainGrid.Children[i] is Image Img)
                {
                    if (Img.Name == Element.Name)
                    {
                        Main.MainGrid.Children.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public List<PipeImage[]> GetAndUpdate()
        {
            List<PipeImage[]> temp = new();
            if (Pares != null)
            {
                if (Pares.Count == 0)
                {
                    GeneratePipes(ref temp);
                    return temp;
                }
                else
                {
                    foreach (var pipe in Pares)
                    {
                        if (!pipe[0].IsOutDisplay(Main))
                            temp.Add(pipe);
                        else
                        {
                            RemoveOusideElement(pipe[0]);
                            RemoveOusideElement(pipe[1]);
                        }
                    }

                    if (temp.Count < 2)
                        GeneratePipes(ref temp);

                    return temp;
                }
            }
            else
            {
                GeneratePipes(ref temp);
                return temp;
            }
        }

        private static Thickness GetMove(Thickness CurrentPosition, int Length)
        {
            return new Thickness(CurrentPosition.Left - Length, CurrentPosition.Top, CurrentPosition.Right + Length, CurrentPosition.Bottom);
        }

        private void MovePipes(int Length)
        {
            foreach (var pipe in Pares)
            {
                for (int i = 0; i < 2; i++)
                    pipe[i].Margin = GetMove(pipe[i].Margin, Length);
            }
        }

        public void Move(int Length)
        {
            if (Pares.Count != 0)
            {
                foreach (var pipe in Pares)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < Main.MainGrid.Children.Count; j++)
                        {
                            if (Main.MainGrid.Children[j] is Image imageControl)
                            {
                                if (imageControl.Name == pipe[i].Name)
                                {
                                    imageControl.Margin = GetMove(imageControl.Margin, Length);
                                }
                            }
                        }
                    }
                }
            }

            _ = GetAndUpdate();
        }

        public void Load()
        {
            var UIElements = GetAndUpdate();
            foreach (var element in UIElements)
            {
                for (int i = 0; i < 2; i++)
                {
                    int index = Main.MainGrid.Children.Add(element[i].ControlImage);
                    Main.MainGrid.Children[index].Visibility = Visibility.Visible;
                }
            }
        }

        public class PipeImage
        {
            public Image ControlImage;
            public ImageSource img;
            public Thickness Margin;
            public string Name;

            public PipeImage(ImageSource Pipe, Thickness T, string ElementName)
            {
                img = Pipe;
                Margin = T;
                Name = ElementName;

                ControlImage = new Image
                {
                    Source = img,
                    Margin = Margin,
                    Name = ElementName,
                };
            }

            public bool IsOutDisplay(MainWindow Window)
            {
                return Margin.Right >= Window.Margin.Right;
            }

            public bool BirdTouch(Bird bird)
            {
                return (bird.Position.Top >= 0 && bird.Position.Top <= Margin.Top) || (bird.Position.Bottom >= 0 && bird.Position.Bottom <= Margin.Bottom);
            }
        }

        public enum PipeModel
        {
            Bottom,
            Top
        }

    }
}
