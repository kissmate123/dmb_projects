using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimplePlatformer
{
    public partial class TestMap : Window
    {
        private Input input;
        private PlayerMovement playerMovement;
        private GamePhysics gamePhysics;
        private UIAndDebug uiAndDebug;
        private EnemyMovement enemyMovement;
        private EnemyCombat enemyCombat;
        private PlayerCombat playerCombat;

        public TestMap()
        {
            InitializeComponent();
            Closing += Window_Closing;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Focus();

            LoadPlayerImage();

            lastUpdateTime = DateTime.Now;
            CompositionTarget.Rendering += GameLoop;

            DebugText.Text = "Game Started! Use A/D or Left/Right to move, Space to jump";

            enemyCombat = new EnemyCombat(this);
            gamePhysics = new GamePhysics(this);
            playerCombat = new PlayerCombat(this, enemyCombat, gamePhysics);
            playerMovement = new PlayerMovement(this);
            enemyMovement = new EnemyMovement(this);
            uiAndDebug = new UIAndDebug(this);
            input = new Input(this);

            InitializeEnemies();
            InitializeBowAndAimUI();
            SpawnShieldPickup();

            playerCombat.LoadIdleFrames();
            playerCombat.LoadWalkFrames();
            playerCombat.LoadBowFrames();
            playerCombat.LoadMeleeFrames();
            playerCombat.LoadJumpAirFrame();
            playerCombat.LoadShieldSprite();

            npcMovement = new NpcMovement(this);
            npcCombat = new NpcCombat(this);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            CompositionTarget.Rendering -= GameLoop;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) => input.Window_KeyDown(sender, e);
        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) => input.Window_KeyUp(sender, e);

        private void GameLoop(object sender, EventArgs e)
        {
            if (isPaused || isDead)
                return;

            DateTime currentTime = DateTime.Now;
            deltaTime = (currentTime - lastUpdateTime).TotalSeconds;
            if (deltaTime > 0.1) deltaTime = 0.1;
            lastUpdateTime = currentTime;

            uiAndDebug.UpdateFPS();
            debugInfo.Clear();

            playerMovement.HandleMovement();
            gamePhysics.ApplyPhysics();

            bool prevOnGround = wasOnGround;

            gamePhysics.CheckCollisions();

            if (!prevOnGround && isOnGround)
            {
                if (idleFrames != null && idleFrames.Length > 0)
                {
                    idleFrameIndex = 0;
                    idleFrameTimer = 0;
                    Player.Source = idleFrames[0];
                }
            }

            wasOnGround = isOnGround;

            playerMovement.UpdatePlayerPosition();
            gamePhysics.UpdateCamera();
            playerMovement.UpdatePlayerFacingDirection();

            uiAndDebug.UpdateDebugInfo();
            uiAndDebug.UpdateHpBar();

            CheckDeath();

            enemyMovement.UpdateEnemies();
            enemyCombat.CheckEnemyCollisions();

            playerCombat.UpdateDamageCooldown();
            playerCombat.HandlePlayerAttack();
            playerCombat.UpdatePlayerStun();

            playerCombat.UpdateBowCharge();
            playerCombat.UpdateArrows();
            playerCombat.UpdateAimUI();

            playerCombat.UpdateMeleeCooldown();
            playerCombat.UpdateJumpAirSprite();
            playerCombat.UpdateBowAnimation();
            playerCombat.UpdateMeleeAnimation();
            playerCombat.UpdateWalkAnimation();
            playerCombat.UpdateBowCooldown();
            playerCombat.UpdateIdleAnimation();

            npcMovement.UpdateNpcs();
            npcCombat.CheckNpcCollisions();

            UpdateShieldPickup();

            if (isShielding && shieldSprite != null)
                Player.Source = shieldSprite;
        }

        internal void TogglePause()
        {
            isPaused = !isPaused;

            PauseOverlay.Visibility = isPaused
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (isPaused)
            {
                leftPressed = false;
                rightPressed = false;
                spacePressed = false;
            }
        }

        private void CheckDeath()
        {
            if (isDead)
                return;

            if (currentHp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;
            isPaused = true;

            Player.Visibility = Visibility.Collapsed;
            Dead.Visibility = Visibility.Visible;

            leftPressed = false;
            rightPressed = false;
            spacePressed = false;
        }

        private void Resume_Click(object sender, RoutedEventArgs e) => TogglePause();

        private void ExitToMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenuWindow menu = new MainMenuWindow();
            menu.Show();
            Close();
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Respawn()
        {
            currentHp = maxHp;
            uiAndDebug.UpdateHpBar();

            playerWorldX = 100;
            playerWorldY = 300;

            playerVelocityX = 0;
            playerVelocityY = 0;

            cameraX = 0;

            isDead = false;
            isPaused = false;

            Player.Visibility = Visibility.Visible;
            Dead.Visibility = Visibility.Collapsed;
        }

        private void Respawn_Click(object sender, RoutedEventArgs e) => Respawn();

        private void InitializeEnemies()
        {
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Troop_easy, 4000, 300, 3800, 4500, EnemyMode.Patrol));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Troop_medium, 5000, 300, 4800, 5500, EnemyMode.Stand));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Civilian, 1200, 600, 1100, 1400, NpcMode.Stand));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Guard, 800, 600, 700, 1000, NpcMode.Patrol));
        }
    
        private void InitializeBowAndAimUI()
        {
            bowChargeBarBg = new Rectangle
            {
                Width = 104,
                Height = 12,
                Fill = Brushes.Black,
                Visibility = Visibility.Hidden
            };

            bowChargeBarFill = new Rectangle
            {
                Width = 0,
                Height = 8,
                Fill = Brushes.Gold,
                Visibility = Visibility.Hidden
            };

            GameCanvas.Children.Add(bowChargeBarBg);
            GameCanvas.Children.Add(bowChargeBarFill);

            aimLineLeft = new Line
            {
                Stroke = Brushes.White,
                StrokeThickness = 2,
                Visibility = Visibility.Hidden
            };

            aimLineRight = new Line
            {
                Stroke = Brushes.White,
                StrokeThickness = 2,
                Visibility = Visibility.Hidden
            };

            aimDot = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Red,
                Visibility = Visibility.Hidden
            };

            Panel.SetZIndex(bowChargeBarBg, 1200);
            Panel.SetZIndex(bowChargeBarFill, 1201);
            Panel.SetZIndex(aimLineLeft, 1200);
            Panel.SetZIndex(aimLineRight, 1200);
            Panel.SetZIndex(aimDot, 1201);

            GameCanvas.Children.Add(aimLineLeft);
            GameCanvas.Children.Add(aimLineRight);
            GameCanvas.Children.Add(aimDot);
        }

        private void SpawnShieldPickup()
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri("Assets/Items/Shield.png", UriKind.Relative);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze();

            shieldPickup = new Image
            {
                Width = 46,
                Height = 52,
                Source = bmp
            };

            GameCanvas.Children.Add(shieldPickup);
            Panel.SetZIndex(shieldPickup, 950);

            Canvas.SetLeft(shieldPickup, shieldWorldX - cameraX);
            Canvas.SetTop(shieldPickup, shieldWorldY);
        }

        private void UpdateShieldPickup()
        {
            uiAndDebug.UpdateShieldBar();

            if (shieldTaken || shieldPickup == null)
                return;

            Canvas.SetLeft(shieldPickup, shieldWorldX - cameraX);
            Canvas.SetTop(shieldPickup, shieldWorldY);

            Rect playerRect = GetPlayerHitboxRect();
            Rect itemRect = new Rect(shieldWorldX, shieldWorldY, shieldPickup.Width, shieldPickup.Height);

            if (playerRect.IntersectsWith(itemRect))
            {
                shieldTaken = true;
                currentShield = maxShield;
                ShieldBarFill.Width = 320;

                GameCanvas.Children.Remove(shieldPickup);
                shieldPickup = null;
            }
        }

        internal void ReleaseArrow() => playerCombat.ReleaseArrow();
        internal void DamagePlayer(double amount, Enemy sourceEnemy = null) => playerCombat.DamagePlayer(amount, sourceEnemy);

        internal Rect GetPlayerHitboxRect()
        {
            return new Rect(
                playerWorldX + PlayerHitboxOffsetX,
                playerWorldY,
                PlayerHitboxWidth,
                PlayerHitboxHeight
            );
        }

        private void LoadPlayerImage()
        {
            try
            {
                string imagePath = "Assets/Sprites/Borien/Borien_Stand_1.png";

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                Player.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kép betöltése sikertelen: {ex.Message}\nZöld téglalap lesz helyette.");
                Player.Source = null;
            }
        }

        internal bool canShootBow = true;
        internal double bowShootCooldownTimer = 0;
        internal double bowShootCooldown = 0.5;

        internal BitmapImage[] bowFrames;
        internal int lastBowFrameIndex = -1;

        internal BitmapImage[] idleFrames;
        internal int idleFrameIndex = 0;
        internal double idleFrameTimer = 0;

        internal readonly double[] idleFrameDurations = { 0.5, 0.05, 0.5, 0.05 };

        internal double currentAimRatioY = 0;

        internal Line aimLineLeft;
        internal Line aimLineRight;
        internal Ellipse aimDot;

        internal double aimLineHeight = 120;

        internal const int MaxChargeSteps = 5;
        internal const double ChargeStepTime = 0.2;

        internal int currentChargeStep = 0;

        internal bool bowKeyPressed = false;
        internal bool isChargingBow = false;

        internal double bowChargeTime = 0;

        internal List<Arrow> arrows = new List<Arrow>();

        internal Rectangle bowChargeBarBg;
        internal Rectangle bowChargeBarFill;

        internal bool canMeleeAttack = true;
        internal double meleeCooldownTimer = 0;
        internal double meleeCooldown = 0.5;

        internal double playerKnockbackX = 350;
        internal double playerKnockbackY = 300;

        internal double playerStunTimer = 0;
        internal double playerStunDuration = 0.4;
        internal bool IsPlayerStunned => playerStunTimer > 0;

        internal double enemyKnockbackX = 300;
        internal double enemyKnockbackY = 250;

        internal bool attackPressed = false;
        internal double attackRange = 140;
        internal double attackDamage = 40;

        internal bool canTakeDamage = true;
        internal double damageTimer = 0;
        internal double damageCooldown = 3.0;

        internal double invulnerableVisualTime = 0.5;
        internal double invulnerableVisualTimer = 0;

        internal BitmapImage[] meleeFrames;
        internal int meleeFrameIndex = 0;
        internal double meleeFrameTimer = 0;

        internal bool isMeleeAnimating = false;
        internal double meleeFrameDuration = 0.05;

        internal double maxHp = 100;
        internal double currentHp = 100;

        internal List<Enemy> enemies = new List<Enemy>();

        internal bool isPaused = false;
        internal bool isDead = false;

        internal double GroundX = 10;
        internal double HousePlatform1X = 122;
        internal double HousePlatform2X = 522;
        internal double HousePlatform3X = 922;
        internal double HousePlatform4X = 1322;
        internal double HousePlatform5X = 1722;
        internal double HousePlatform6X = 2122;
        internal double HousePlatform7X = 2522;
        internal double HousePlatform8X = 2922;

        internal double playerWalkSpeed = 150;
        internal double playerRunSpeed = 350;

        internal bool shiftPressed = false;

        internal double jumpForce = 450;
        internal double gravity = 1500;

        internal double playerVelocityX = 0;
        internal double playerVelocityY = 0;
        internal bool isJumping = false;
        internal bool isOnGround = false;

        internal bool isFacingRight = true;

        internal bool leftPressed = false;
        internal bool rightPressed = false;
        internal bool spacePressed = false;

        internal BitmapImage jumpAirFrame;

        internal bool wasOnGround = false;

        internal BitmapImage[] walkFrames;
        internal int walkFrameIndex = 0;
        internal double walkFrameTimer = 0;
        internal bool wasWalking = false;
        internal double walkFrameDuration = 0.08;

        internal DateTime lastUpdateTime;
        internal double deltaTime;

        internal List<string> debugInfo = new List<string>();
        internal int frameCount = 0;
        internal double fps = 0;
        internal DateTime lastFpsUpdate = DateTime.Now;

        internal double WorldWidth = 30000;

        internal double playerWorldX = 100;
        internal double playerWorldY = 300;

        internal double cameraX = 0;

        internal double screenLeftLimit = 300;
        internal double screenRightLimit = 500;

        internal const double PlayerHitboxWidth = 48;
        internal const double PlayerHitboxHeight = 60;
        internal const double PlayerHitboxOffsetX = 30;

        internal Image shieldPickup;
        internal bool shieldTaken = false;

        internal double shieldWorldX = 1000;
        internal double shieldWorldY = 750;

        internal const double maxShield = 100;
        internal double currentShield = 0;

        internal bool isShielding = false;
        internal BitmapImage shieldSprite;

        public List<Npc> npcs = new List<Npc>();

        private NpcMovement npcMovement;
        private NpcCombat npcCombat;
    }
}
