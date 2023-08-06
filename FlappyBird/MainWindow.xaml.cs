using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;

namespace FlappyBird
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int FrameRate = 60;
        private readonly GameState gameState;
        [AllowNull]
        private static MainWindow Main;

        public static MainWindow Instance
        {
            get => Main;
        }

        public MainWindow()
        {
            InitializeComponent();
            Main = this;

            gameState = GameState.GetInstace(FrameRate);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gameState.InitializeGameState();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartImage.Visibility = Visibility.Hidden;
            gameState.StartFlappyBird();
            gameState.MouseState(false);
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            gameState.MouseState(true);
        }
    }
}
