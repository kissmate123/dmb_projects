using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimplePlatformer
{
    public class Arrow
    {
        public Rectangle Visual;
        public double Damage;

        public double WorldX;
        public double WorldY;

        public double VelocityX;
        public double VelocityY;

        private double gravity = 900;
        private double lifeTime = 0;
        private double flyStraightTime = 0.15;

        public Arrow(
            Canvas canvas,
            double x,
            double y,
            double vx,
            double vy,
            double damage)
        {
            WorldX = x;
            WorldY = y;

            VelocityX = vx;
            VelocityY = vy;

            Damage = damage;

            Visual = new Rectangle
            {
                Width = 16,
                Height = 4,
                Fill = Brushes.SaddleBrown
            };

            canvas.Children.Add(Visual);
        }

        public void Update(double deltaTime)
        {
            lifeTime += deltaTime;

            if (lifeTime > flyStraightTime)
            {
                VelocityY += gravity * deltaTime;
            }

            WorldX += VelocityX * deltaTime;
            WorldY += VelocityY * deltaTime;
        }

        public Rect GetRect()
        {
            return new Rect(WorldX, WorldY, Visual.Width, Visual.Height);
        }
    }
}
