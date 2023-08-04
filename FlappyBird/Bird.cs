using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Windows;
using System.Windows.Media;

namespace FlappyBird
{
    public class Bird
    {
        [AllowNull]
        private ImageSource BirdSkin;
        private Thickness T;

        public Bird(Thickness t)
        {
            T = t;
        }

        public Thickness Position
        {
            get => T;
            set => T = value;
        }

        public ImageSource BirdImage
        {
            get => BirdSkin;
            set => BirdSkin = value;
        }

    }
}
