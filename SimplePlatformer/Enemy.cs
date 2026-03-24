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
        Troop_easy,
        Troop_medium
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

        public Rectangle Visual;
        public Rectangle HpBarBackground;
        public Rectangle HpBarFill;

        public double WorldX;
        public double WorldY;
        public double VelocityX;
        public double VelocityY;

        public double WalkSpeed;
        public double AggroSpeed;

        public double LeftLimit;
        public double RightLimit;
        public bool OnGround;

        public double MeleeDamage { get; } 
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
            double meleeDamage,
            double meleeCooldown,
            double aggroShareRadius)
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

            VelocityX = WalkSpeed;
            VelocityY = 0;

            MeleeDamage = meleeDamage;
            MeleeCooldown = meleeCooldown;


            AggroShareRadius = aggroShareRadius;

            Visual = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = fill
            };

            Visual.Opacity = 1.0;

            RenderOptions.SetBitmapScalingMode(Visual, BitmapScalingMode.NearestNeighbor);

            Scale = new ScaleTransform(1, 1);
            Visual.RenderTransformOrigin = new Point(0.5, 0.5);
            Visual.RenderTransform = Scale;

            OriginalBrush = Visual.Fill;

            HpBarBackground = new Rectangle
            {
                Width = width + 2,
                Height = 6,
                Fill = Brushes.Black
            };

            HpBarFill = new Rectangle
            {
                Width = width,
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
                case EnemyType.Troop_medium:
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri("Assets/Sprites/Brogur/BrogurMedium_Base.png", UriKind.Relative);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        bmp.Freeze();

                        var brush = new ImageBrush(bmp)
                        {
                            Stretch = Stretch.Fill
                        };

                        return new Enemy(
                        canvas, type, mode,
                        startX, startY, leftLimit, rightLimit,
                        width: 58,
                        height: 78,
                        fill: brush,
                        maxHp: 180,
                        walkSpeed: 70,
                        aggroSpeed: 130,
                        attackRange: 60,
                        stopDistance: 50,
                        aggroRange: 250,
                        contactDamage: 10,
                        damageCooldown: 2.5,
                        meleeDamage: 20,
                        meleeCooldown: 1.0,
                        aggroShareRadius: 500
                    );
                    }
                    

                case EnemyType.Troop_easy:
                default:
                    {
                        var bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.UriSource = new Uri("Assets/Sprites/Brogur/BrogurEasy_Base.png", UriKind.Relative);
                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                        bmp.EndInit();
                        bmp.Freeze();

                        var brush = new ImageBrush(bmp)
                        {
                            Stretch = Stretch.Fill
                        };

                        return new Enemy(
                            canvas, type, mode,
                            startX, startY, leftLimit, rightLimit,
                            width: 58,
                            height: 78,
                            fill: brush,
                            maxHp: 100,
                            walkSpeed: 80,
                            aggroSpeed: 150,
                            attackRange: 60,
                            stopDistance: 50,
                            aggroRange: 250,
                            contactDamage: 8,
                            damageCooldown: 2.0,
                            meleeDamage: 20,
                            meleeCooldown: 1.0,
                            aggroShareRadius: 500
                        );
                    }
            }
        }

        public void UpdateHpBar(double cameraX)
        {
            double hpPercent = CurrentHp / MaxHp;
            if (hpPercent < 0) hpPercent = 0;

            HpBarFill.Width = Visual.Width * hpPercent;

            Canvas.SetLeft(HpBarBackground, WorldX - cameraX - 1);
            Canvas.SetTop(HpBarBackground, WorldY - 10);

            Canvas.SetLeft(HpBarFill, WorldX - cameraX);
            Canvas.SetTop(HpBarFill, WorldY - 9);
        }
    }
}