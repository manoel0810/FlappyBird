using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Windows.Media;

namespace FlappyBird
{
    public class Bird
    {
        [AllowNull]
        private ImageSource BirdSkin;
        public Pos Position;

        public Bird(MainWindow Main)
        {
            Position = new Pos(60f, (float)(Main.Height / 2));
        }

        public ImageSource BirdImage
        {
            get => BirdSkin;
            set => BirdSkin = value;
        }

    }

    public class Pos
    {
        float x;
        float y;

        public Pos(float X, float Y)
        {
            x = X;
            y = Y;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(x, y);
        }

        public void SetPosition(Vector2 newPos)
        {
            x = newPos.X;
            y = newPos.Y;
        }
    }
}
