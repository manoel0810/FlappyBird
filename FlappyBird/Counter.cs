using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FlappyBird
{
    /// <summary>
    /// Fornece suporte para exibir os pontos do jogador em forma de imagem personalizada
    /// </summary>

    public class Counter
    {
        [AllowNull]
        private static Counter Instance;

        /// <summary>
        /// Referência de número e imagem
        /// </summary>

        private readonly Dictionary<int, ImageSource> Numeros = new()
        {
            {0, new BitmapImage(new Uri("Assets/0.png", UriKind.Relative)) },
            {1, new BitmapImage(new Uri("Assets/1.png", UriKind.Relative)) },
            {2, new BitmapImage(new Uri("Assets/2.png", UriKind.Relative)) },
            {3, new BitmapImage(new Uri("Assets/3.png", UriKind.Relative)) },
            {4, new BitmapImage(new Uri("Assets/4.png", UriKind.Relative)) },
            {5, new BitmapImage(new Uri("Assets/5.png", UriKind.Relative)) },
            {6, new BitmapImage(new Uri("Assets/6.png", UriKind.Relative)) },
            {7, new BitmapImage(new Uri("Assets/7.png", UriKind.Relative)) },
            {8, new BitmapImage(new Uri("Assets/8.png", UriKind.Relative)) },
            {9, new BitmapImage(new Uri("Assets/9.png", UriKind.Relative)) }
        };

        /// <summary>
        /// Retorna uma instância para a chamada, caso já exista outra. Caso contrário, cria uma nova instância do objeto <b>Counter</b> e o retorna
        /// </summary>
        /// <returns></returns>

        public static Counter GetInstance()
        {
            Instance ??= new Counter();
            return Instance;
        }

        /// <summary>
        /// Obtém um vetor de imagens que descrevem um número em sua ordem e valor
        /// </summary>
        /// <param name="number">Número para descrever</param>
        /// <returns></returns>

        public ImageSource[] GetImageFromNumber(int number)
        {
            string nunString = number.ToString();
            ImageSource[] Vector = new ImageSource[nunString.Length];

            for (int i = 0; i < nunString.Length; i++)
                Vector[i] = Numeros[int.Parse(nunString[i].ToString())];

            return Vector;
        }

        public ImageSource this[int i]
        {
            get => Numeros[i];
        }
    }
}
