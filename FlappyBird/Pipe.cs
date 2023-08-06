using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FlappyBird
{
    /// <summary>
    /// Objeto <b>Pipe</b> que contém as definições e métodos necessários para criar e exibir os Pipes na área do jogo
    /// </summary>

    public class Pipe
    {
        [AllowNull]
        private static Pipe _instance;
        private readonly MainWindow _main;
        private readonly Random _randomizer = new();
        private readonly GameState.GameTime _timeOfDay;

        private readonly List<Thickness> Thicknesses = new();
        private readonly int Spacing = 150;
        private const int Tolerance = 35;

        /// <summary>
        /// Tubos verde/vermelho (0, 1) respectivamente. Horientação acendente
        /// </summary>

        private readonly ImageSource[] _pipesUp = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/pipe-green.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/pipe-red.png", UriKind.Relative)),
        };

        /// <summary>
        /// Tubos verde/vermelho (0, 1) respectivamente. Horientação descedente
        /// </summary>

        private readonly ImageSource[] _pipesDown = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/pipe-green-down.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/pipe-red-down.png", UriKind.Relative)),
        };

        /// <summary>
        /// Inicia uma nova instância para <b>Pipe</b>
        /// </summary>
        /// <param name="time">Tema de cor</param>

        private Pipe(GameState.GameTime time)
        {
            _main = MainWindow.Instance;
            _timeOfDay = time;
        }

        /// <summary>
        /// Retorna uma instância do objeto <b>Pipe</b>
        /// </summary>
        /// <param name="time">Tema de cores</param>
        /// <returns>
        /// Uma nova instância de <b>Pipe</b> se a atual não existir,
        /// caso contrário, retorna a instância para a chamada
        /// </returns>

        public static Pipe GetInstance(GameState.GameTime time)
        {
            _instance ??= new Pipe(time);
            return _instance;
        }

        /// <summary>
        /// Randomiza valores para serem usados como margens na geração dos espaços entre dois pipes co-lineares
        /// </summary>
        /// <returns></returns>

        private double[] GetRandomizedTopBottom()
        {
            int point = _randomizer.Next(200, 400);
            return new double[] { point + Spacing, _main.Height - (point + Spacing / 2) };
        }

        /// <summary>
        /// Cria um <i>struct</i> do tipo <b>Thickness</b> e atualiza as margens dos controles de exibição dos pipes
        /// </summary>

        private void CreateMargins()
        {
            for (int i = 0; i < 2; i++)
            {
                int plus = i == 0 ? 0 : 250;
                double[] values = GetRandomizedTopBottom();
                Thicknesses.Add(new Thickness(400 + plus, 0, -50 + -plus, values[0]));
                Thicknesses.Add(new Thickness(400 + plus, values[1], -50 + -plus, 0));
            }
        }

        /// <summary>
        /// Recalcula as margens dos controles, levando em consideração o movimento dos pipes, somando um incremento em <i>left e right</i>
        /// </summary>
        /// <param name="Increment">Valor de acréssimo</param>

        private void RecalculateMargins(double Increment)
        {
            if (Thicknesses.Count < 4)
                CreateMargins();

            for (int i = 0; i < Thicknesses.Count; i++)
                Thicknesses[i] = new Thickness(Thicknesses[i].Left - Increment, Thicknesses[i].Top, Thicknesses[i].Right + Increment, Thicknesses[i].Bottom);
        }

        /// <summary>
        /// Itera sobre <c>Thicknesses</c> e verifica se estão dentro das margens visíveis da tela. Caso contrário, atualiza seu <b>Thicknesses[ i ].Margin</b> para o início da tela
        /// </summary>
        /// <returns>1 (Um), se n elementos foram realocanos para dentro da tela</returns>

        private int CheckOutside()
        {
            bool PointWin = false;
            for (int i = 0; i < 2; i++)
            {
                if (Thicknesses[(i == 0 ? 0 : 2)].Right >= 400)
                {
                    //int dist = _randomizer.Next(-200, -100);
                    int dist = -120;
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

                    PointWin = true;
                }
            }

            return PointWin ? 1 : 0;
        }

        /// <summary>
        /// Move todos os pipes n posições da direita para a esquerda
        /// </summary>
        /// <param name="length">Número de posições a se movimentar</param>
        /// <returns>1 (Um), se houve realocação de pipes</returns>

        public int Move(int length)
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
            return CheckOutside();
        }

        /// <summary>
        /// Verifica se um valor <b>midle</b> está entre dois extremos <b>extremes</b>, levando em consideração seu valor absoluto
        /// </summary>
        /// <param name="midle">Valor tendencioso a estar no meio</param>
        /// <param name="extremes">Extremos de teste</param>
        /// <returns>
        /// <b>true</b> se, <c> extremes[m] > midle > extremes[n]</c>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>

        private static bool IsBetween(double midle, double[] extremes)
        {
            if (extremes.Length < 2)
                throw new ArgumentOutOfRangeException(nameof(extremes), "At least two arguments was expected, but just one was supplied");

            double a, b, c, d, e;
            a = Math.Abs(extremes[0]);
            b = Math.Abs(extremes[1]);
            c = Math.Abs(midle);

            d = Math.Max(a, b);
            e = Math.Min(a, b);

            return c > e && c < d;

        }

        /// <summary>
        /// Verifica se um <b>Thickness</b> foi interceptador por outro struct do mesmo tipo
        /// </summary>
        /// <param name="birdMargin">Thickness pertencente ao objeto <b>Bird</b></param>
        /// <param name="obstacleMargin">Thickness pertencente ao objeto <b>Pipe Top</b></param>
        /// <param name="par">Thickness pertencente ao objeto <b>Pipe Bottom</b></param>
        /// <returns>
        /// <b>true</b>, se houver interceptação
        /// </returns>

        private bool Intersects(Thickness birdMargin, Thickness obstacleMargin, Thickness par)
        {
            if (obstacleMargin.Left >= 400 || obstacleMargin.Left <= 0)
                return false;

            bool isBetween = IsBetween(birdMargin.Left + Tolerance, new double[] { obstacleMargin.Right - _main.Height, obstacleMargin.Left });
            if (isBetween)
            {
                if (birdMargin.Top + Tolerance < _main.Height - obstacleMargin.Bottom || birdMargin.Bottom + Tolerance < _main.Height - par.Top)
                    if (obstacleMargin.Right < 300)
                        return true;

                return false;
            }

            return false;
        }

        /// <summary>
        /// Verifica se o objeto <b>Bird</b> colidiu com algum Pipe
        /// </summary>
        /// <param name="Passaro">objeto <b>Bird</b></param>
        /// <returns>
        /// <b>true</b>, se houve colisão
        /// </returns>

        public bool BirdHits(Bird Passaro)
        {
            if (Intersects(Passaro.Position, Thicknesses[0], Thicknesses[1]))
                return true;
            else if (Intersects(Passaro.Position, Thicknesses[2], Thicknesses[3]))
                return true;

            return false;
        }

        /// <summary>
        /// Define as imagens e margens dos <b>Image</b> nos Children de <b>_main</b> que é instância referenciada de <b>MainWindow</b>
        /// </summary>

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

        /// <summary>
        /// Prepara o objeto <b>Pipe</b> para iniciar suas rotinas
        /// </summary>

        public void Load()
        {
            CreateMargins();
            SetImage();
        }
    }
}
