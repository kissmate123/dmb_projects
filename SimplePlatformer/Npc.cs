using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimplePlatformer
{
    public enum NpcType
    {
        Civilian,
        Guard
    }

    public enum NpcMode
    {
        Stand,
        Patrol,
        Wander
    }

    public class Npc
    {
        public NpcType Type { get; }
        public NpcMode Mode { get; }

        public Rectangle Visual;

        public double WorldX;
        public double WorldY;
        public double VelocityX;
        public double VelocityY;

        public double WalkSpeed;

        public double LeftLimit;
        public double RightLimit;
        public bool OnGround;

        public double PatrolWaitTime { get; set; } = 4.0;
        public double PatrolWaitTimer { get; set; } = 0.0;

        public double WanderTimer { get; set; } = 0.0;

        public int MoveDir { get; set; } = 1;
        public bool IsWaiting { get; set; } = false;

        public ScaleTransform Scale { get; }

        private readonly Canvas canvas;

        public Npc(
            Canvas canvas,
            NpcType type,
            NpcMode mode,
            double startX,
            double startY,
            double leftLimit,
            double rightLimit,
            double width,
            double height,
            Brush fill,
            double walkSpeed)
        {
            this.canvas = canvas;

            Type = type;
            Mode = mode;

            WorldX = startX;
            WorldY = startY;

            LeftLimit = leftLimit;
            RightLimit = rightLimit;

            WalkSpeed = walkSpeed;

            VelocityX = WalkSpeed;
            VelocityY = 0;

            Visual = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = fill,
                Opacity = 1.0
            };

            RenderOptions.SetBitmapScalingMode(Visual, BitmapScalingMode.NearestNeighbor);

            Scale = new ScaleTransform(1, 1);
            Visual.RenderTransformOrigin = new Point(0.5, 0.5);
            Visual.RenderTransform = Scale;

            Panel.SetZIndex(Visual, 900);

            canvas.Children.Add(Visual);
        }

        public static Npc Create(
            Canvas canvas,
            NpcType type,
            double startX,
            double startY,
            double leftLimit,
            double rightLimit,
            NpcMode mode = NpcMode.Patrol)
        {
            switch (type)
            {
                case NpcType.Guard:
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri("Assets/Sprites/Guards/RoyalGuard2.png", UriKind.Relative);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        bmp.Freeze();

                        var brush = new ImageBrush(bmp) { Stretch = Stretch.Fill };

                        return new Npc(
                            canvas, type, mode,
                            startX, startY, leftLimit, rightLimit,
                            width: 48,
                            height: 60,
                            fill: brush,
                            walkSpeed: 70
                        );
                    }

                case NpcType.Civilian:
                default:
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri("Assets/Sprites/Guards/RoyalGuard1.png", UriKind.Relative);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        bmp.Freeze();

                        var brush = new ImageBrush(bmp) { Stretch = Stretch.Fill };

                        return new Npc(
                            canvas, type, mode,
                            startX, startY, leftLimit, rightLimit,
                            width: 48,
                            height: 60,
                            fill: brush,
                            walkSpeed: 60
                        );
                    }
            }
        }
    }
}
