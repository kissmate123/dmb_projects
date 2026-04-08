using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimplePlatformer
{
    public enum NpcType
    {
        Civilian,
        Guard1,
        Guard2
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

        public Image Visual;

        public BitmapImage[] IdleFrames;
        public BitmapImage[] WalkFrames;

        public int FrameIndex = 0;
        public double FrameTimer = 0;

        public double WalkFrameDuration = 0.08;
        public double[] IdleFrameDurations = { 0.5, 0.05, 0.5, 0.05 };

        public string CurrentAnimState = "";

        public double WorldX;
        public double WorldY;
        public double VelocityX;
        public double VelocityY;

        public double WalkSpeed;

        public double InteractCooldown = 0;

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

            Visual = new Image
            {
                Width = width,
                Height = height,
                Source = (fill as ImageBrush)?.ImageSource
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
                case NpcType.Guard1:
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri("", UriKind.Relative);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        bmp.Freeze();

                        var brush = new ImageBrush(bmp) { Stretch = Stretch.Fill };

                        var npc = new Npc(
                            canvas, type, mode,
                            startX, startY, leftLimit, rightLimit,
                            width: 48,
                            height: 60,
                            fill: brush,
                            walkSpeed: 70
                        );

                        npc.IdleFrames = LoadFrames("Assets/Sprites/Guards/Guard1/Guard1_Stand_", 4);
                        npc.WalkFrames = LoadFrames("Assets/Sprites/Guards/Guard1/Guard1_Walk_", 4);

                        npc.CurrentAnimState = "Idle";
                        npc.FrameIndex = 0;
                        npc.FrameTimer = 0;
                        npc.Visual.Source = npc.IdleFrames[0];

                        return npc;
                    }

                case NpcType.Guard2:
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri("Assets/Sprites/Guards/RoyalGuard1.png", UriKind.Relative);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        bmp.Freeze();

                        var brush = new ImageBrush(bmp) { Stretch = Stretch.Fill };

                        var npc = new Npc(
                            canvas, type, mode,
                            startX, startY, leftLimit, rightLimit,
                            width: 48,
                            height: 60,
                            fill: brush,
                            walkSpeed: 70
                        );

                        npc.IdleFrames = LoadFrames("Assets/Sprites/Borien/Borien_Stand_", 4);
                        npc.WalkFrames = LoadFrames("Assets/Sprites/Borien/Borien_Walk_", 4);

                        npc.CurrentAnimState = "Idle";
                        npc.FrameIndex = 0;
                        npc.FrameTimer = 0;
                        npc.Visual.Source = npc.IdleFrames[0];

                        return npc;
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

                        var npc = new Npc(
                            canvas, type, mode,
                            startX, startY, leftLimit, rightLimit,
                            width: 48,
                            height: 60,
                            fill: brush,
                            walkSpeed: 60
                        );

                        npc.IdleFrames = new[] { bmp };
                        npc.WalkFrames = new[] { bmp };

                        npc.CurrentAnimState = "Idle";
                        npc.FrameIndex = 0;
                        npc.FrameTimer = 0;
                        npc.Visual.Source = npc.IdleFrames[0];

                        return npc;
                    }
            }
        }

        private static BitmapImage[] LoadFrames(string basePath, int count)
        {
            BitmapImage[] frames = new BitmapImage[count];

            for (int i = 0; i < count; i++)
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri($"{basePath}{i + 1}.png", UriKind.Relative);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                bmp.Freeze();

                frames[i] = bmp;
            }

            return frames;
        }
    }
}