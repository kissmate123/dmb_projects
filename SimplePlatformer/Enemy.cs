using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace SimplePlatformer
{
    public enum EnemyType
    {
        Brogur_light,
        Brogur_medium,
        Brogur_heavy,
        Skulk,
        Raider
    }

    public enum EnemyMode
    {
        Stand,
        Patrol,
        Wander
    }

    public class Enemy
    {
        public EnemyType Type { get; }
        public EnemyMode Mode { get; }

        public Image Visual;
        public Rectangle HpBarBackground;
        public Rectangle HpBarFill;

        public BitmapImage[] IdleFrames;
        public BitmapImage[] WalkFrames;
        public BitmapImage[] AttackFrames;

        public int FrameIndex = 0;
        public double FrameTimer = 0;

        public double WalkFrameDuration = 0.08;
        public double AttackFrameDuration = 0.05;
        public double[] IdleFrameDurations = { 0.5, 0.05, 0.5, 0.05 };

        public bool IsAttacking = false;
        public string CurrentAnimState = "";

        public double WorldX;
        public double WorldY;
        public double VelocityX;
        public double VelocityY;

        public double HpBarWidth;

        public double WalkSpeed;
        public double AggroSpeed;

        public double LeftLimit;
        public double RightLimit;
        public bool OnGround;

        public double MeleeCooldown { get; }
        public double MeleeCooldownTimer { get; set; } = 0;

        public double MaxHp;
        public double CurrentHp;

        public double ContactDamage;

        public double DamageCooldownTimer = 0;
        public double DamageCooldown = 1.0;

        public double HitFlashTimer = 0;
        public Brush OriginalBrush;

        public bool IsAggro = false;
        public double AggroRange;

        public double StunTimer = 0;
        public const double StunDuration = 0.5;

        public double HitFlashOpacity { get; set; } = 0.5;
        public double NormalOpacity { get; set; } = 1.0;

        private readonly Canvas canvas;

        public double AttackRange { get; }
        public double StopDistance { get; }

        public double PatrolWaitTime { get; set; } = 4.0;
        public double PatrolWaitTimer { get; set; } = 0.0;

        public double WanderTimer { get; set; } = 0.0;

        public ScaleTransform Scale { get; }

        public int MoveDir { get; set; } = 1;
        public bool IsWaiting { get; set; } = false;

        public bool IsInAggroAlert { get; set; } = false;
        public double AggroAlertTimer { get; set; } = 0;

        public const double AggroAlertDuration = 1.0;
        public Image AggroAlertIcon { get; private set; }
        public double AggroShareRadius { get; }

        public Enemy(
            Canvas canvas,
            EnemyType type,
            EnemyMode mode,
            double startX,
            double startY,
            double leftLimit,
            double rightLimit,
            double width,
            double height,
            Brush fill,
            double maxHp,
            double walkSpeed,
            double aggroSpeed,
            double aggroRange,
            double contactDamage,
            double damageCooldown,
            double attackRange,
            double stopDistance,
            double aggroShareRadius,
            double hpBarWidth)
        {
            this.canvas = canvas;

            Type = type;
            Mode = mode;

            WorldX = startX;
            WorldY = startY;

            LeftLimit = leftLimit;
            RightLimit = rightLimit;

            WalkSpeed = walkSpeed;
            AggroSpeed = aggroSpeed;
            AggroRange = aggroRange;

            MaxHp = maxHp;
            CurrentHp = maxHp;

            ContactDamage = contactDamage;

            AttackRange = attackRange;
            StopDistance = stopDistance;

            DamageCooldown = damageCooldown;
            DamageCooldownTimer = 0;
            HpBarWidth = hpBarWidth;

            VelocityX = WalkSpeed;
            VelocityY = 0;


            AggroShareRadius = aggroShareRadius;

            Visual = new Image
            {
                Width = width,
                Height = height,
                Source = (fill as ImageBrush)?.ImageSource
            };

            Visual.Opacity = 1.0;

            RenderOptions.SetBitmapScalingMode(Visual, BitmapScalingMode.NearestNeighbor);

            Scale = new ScaleTransform(1, 1);
            Visual.RenderTransformOrigin = new Point(0.5, 0.5);
            Visual.RenderTransform = Scale;

            HpBarBackground = new Rectangle
            {
                Width = HpBarWidth + 2,
                Height = 6,
                Fill = Brushes.Black
            };

            HpBarFill = new Rectangle
            {
                Width = HpBarWidth,
                Height = 4,
                Fill = Brushes.LimeGreen
            };

            Panel.SetZIndex(Visual, 900);
            Panel.SetZIndex(HpBarBackground, 901);
            Panel.SetZIndex(HpBarFill, 902);

            canvas.Children.Add(Visual);
            canvas.Children.Add(HpBarBackground);
            canvas.Children.Add(HpBarFill);

            AggroAlertIcon = new Image
            {
                Width = 7,
                Height = 30,
                Source = new BitmapImage(new Uri("Assets/Props/Alert.png", UriKind.Relative)),
                Visibility = Visibility.Hidden
            };

            Panel.SetZIndex(AggroAlertIcon, 1000);
            canvas.Children.Add(AggroAlertIcon);
        }

        public static Enemy Create(
            Canvas canvas,
            EnemyType type,
            double startX,
            double startY,
            double leftLimit,
            double rightLimit,
            EnemyMode mode = EnemyMode.Patrol)
        {
            switch (type)
            {
                case EnemyType.Raider:
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri("Assets/Sprites/Enemy/Skeleton/Skeleton.png", UriKind.Relative);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        bmp.Freeze();

                        var brush = new ImageBrush(bmp)
                        {
                            Stretch = Stretch.Fill
                        };

                        var enemy = new Enemy(
                            canvas, type, mode,
                            startX, startY, leftLimit, rightLimit,
                            width: 108,
                            height: 60,
                            fill: brush,
                            maxHp: 200,
                            walkSpeed: 60,
                            aggroSpeed: 150,
                            attackRange: 50,
                            stopDistance: 50,
                            aggroRange: 200,
                            contactDamage: 20,
                            damageCooldown: 2,
                            aggroShareRadius: 200,
                            hpBarWidth: 50
                        );

                        enemy.IdleFrames = LoadFrames("Assets/Sprites/Borien/Borien_Stand_", 4);
                        enemy.WalkFrames = LoadFrames("Assets/Sprites/Borien/Borien_Walk_", 4);
                        enemy.AttackFrames = LoadFrames("Assets/Sprites/Borien/Borien_Sword1_", 3);
                        enemy.CurrentAnimState = "Idle";
                        enemy.FrameIndex = 0;
                        enemy.FrameTimer = 0;
                        enemy.Visual.Source = enemy.IdleFrames[0];

                        return enemy;
                    }

                case EnemyType.Skulk:
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri("Assets/Sprites/Enemy/Skeleton/Skeleton_Walk2.png", UriKind.Relative);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        bmp.Freeze();

                        var brush = new ImageBrush(bmp)
                        {
                            Stretch = Stretch.Fill
                        };

                        var enemy = new Enemy(
                            canvas, type, mode,
                            startX, startY, leftLimit, rightLimit,
                            width: 54,
                            height: 60,
                            fill: brush,
                            maxHp: 100,
                            walkSpeed: 50,
                            aggroSpeed: 200,
                            attackRange: 50,
                            stopDistance: 50,
                            aggroRange: 200,
                            contactDamage: 5,
                            damageCooldown: 2,
                            aggroShareRadius: 200,
                            hpBarWidth: 30
                        );

                        enemy.IdleFrames = new[] { bmp };
                        enemy.WalkFrames = LoadFrames("Assets/Sprites/Enemy/Skeleton/Skeleton_Walk", 4);
                        enemy.AttackFrames = LoadFrames("Assets/Sprites/Enemy/Skeleton/Skeleton_Attack", 3);

                        enemy.CurrentAnimState = "Idle";
                        enemy.FrameIndex = 0;
                        enemy.FrameTimer = 0;
                        enemy.Visual.Source = enemy.IdleFrames[0];

                        return enemy;
                    }

                case EnemyType.Brogur_heavy:
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri("Assets/Sprites/Brogur/Brogur_Heavy.png", UriKind.Relative);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        bmp.Freeze();

                        var brush = new ImageBrush(bmp)
                        {
                            Stretch = Stretch.Fill
                        };

                        var enemy = new Enemy(
                            canvas, type, mode,
                            startX, startY, leftLimit, rightLimit,
                            width: 62,
                            height: 78,
                            fill: brush,
                            maxHp: 650,
                            walkSpeed: 65,
                            aggroSpeed: 120,
                            attackRange: 60,
                            stopDistance: 50,
                            aggroRange: 300,
                            contactDamage: 60,
                            damageCooldown: 4.5,
                            aggroShareRadius: 300,
                            hpBarWidth: 150
                        );

                        enemy.IdleFrames = new[] { bmp };
                        enemy.WalkFrames = new[] { bmp };
                        enemy.AttackFrames = new[] { bmp };

                        enemy.CurrentAnimState = "Idle";
                        enemy.FrameIndex = 0;
                        enemy.FrameTimer = 0;
                        enemy.Visual.Source = enemy.IdleFrames[0];

                        return enemy;
                    }

                case EnemyType.Brogur_medium:
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri("Assets/Sprites/Brogur/Brogur_Medium.png", UriKind.Relative);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        bmp.Freeze();

                        var brush = new ImageBrush(bmp)
                        {
                            Stretch = Stretch.Fill
                        };

                        var enemy = new Enemy(
                            canvas, type, mode,
                            startX, startY, leftLimit, rightLimit,
                            width: 58,
                            height: 78,
                            fill: brush,
                            maxHp: 420,
                            walkSpeed: 70,
                            aggroSpeed: 140,
                            attackRange: 60,
                            stopDistance: 50,
                            aggroRange: 300,
                            contactDamage: 35,
                            damageCooldown: 3,
                            aggroShareRadius: 300,
                            hpBarWidth: 90
                        );

                        enemy.IdleFrames = new[] { bmp };
                        enemy.WalkFrames = new[] { bmp };
                        enemy.AttackFrames = new[] { bmp };

                        enemy.CurrentAnimState = "Idle";
                        enemy.FrameIndex = 0;
                        enemy.FrameTimer = 0;
                        enemy.Visual.Source = enemy.IdleFrames[0];

                        return enemy;
                    }
                    

                case EnemyType.Brogur_light:
                default:
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri("Assets/Sprites/Brogur/Brogur_Light.png", UriKind.Relative);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        bmp.Freeze();

                        var brush = new ImageBrush(bmp)
                        {
                            Stretch = Stretch.Fill
                        };

                        var enemy = new Enemy(
                            canvas, type, mode,
                            startX, startY, leftLimit, rightLimit,
                            width: 58,
                            height: 78,
                            fill: brush,
                            maxHp: 280,
                            walkSpeed: 80,
                            aggroSpeed: 160,
                            attackRange: 60,
                            stopDistance: 50,
                            aggroRange: 300,
                            contactDamage: 20,
                            damageCooldown: 2.5,
                            aggroShareRadius: 300,
                            hpBarWidth: 60
                        );

                        enemy.IdleFrames = new[] { bmp };
                        enemy.WalkFrames = new[] { bmp };
                        enemy.AttackFrames = new[] { bmp };

                        enemy.CurrentAnimState = "Idle";
                        enemy.FrameIndex = 0;
                        enemy.FrameTimer = 0;
                        enemy.Visual.Source = enemy.IdleFrames[0];

                        return enemy;
                    }
            }
        }

        public void UpdateHpBar(double cameraX)
        {
            double hpPercent = CurrentHp / MaxHp;
            if (hpPercent < 0) hpPercent = 0;

            HpBarFill.Width = HpBarWidth * hpPercent;

            double enemyCenterX = WorldX + Visual.Width / 2;
            double barLeft = enemyCenterX - (HpBarWidth / 2);

            Canvas.SetLeft(HpBarBackground, barLeft - cameraX - 1);
            Canvas.SetTop(HpBarBackground, WorldY - 10);

            Canvas.SetLeft(HpBarFill, barLeft - cameraX);
            Canvas.SetTop(HpBarFill, WorldY - 9);
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