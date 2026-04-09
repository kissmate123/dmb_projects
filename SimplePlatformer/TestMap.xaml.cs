using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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

            DebugText.Text = "";

            enemyCombat = new EnemyCombat(this);
            gamePhysics = new GamePhysics(this);
            playerCombat = new PlayerCombat(this, enemyCombat, gamePhysics);
            playerMovement = new PlayerMovement(this);
            enemyMovement = new EnemyMovement(this);
            uiAndDebug = new UIAndDebug(this);
            input = new Input(this);

            InitializeEntities();
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

            foreach (UIElement element in GameCanvas.Children)
            {
                if (element is Rectangle rect && rect.Name.Contains("Platform"))
                {
                    platforms.Add(rect);
                }

                if (element is Image img && img.Name.Contains("House"))
                {
                    worldObjects.Add(img);
                }
            }

            foreach (UIElement element in GameCanvas.Children)
            {
                if (element is FrameworkElement fe)
                {
                    fe.Tag = Canvas.GetLeft(fe);
                }
            }

            InitializeSpeechUi();
        }

        private void InitializeSpeechUi()
        {
            playerSpeech = new TextBlock
            {
                Foreground = Brushes.Black,
                FontSize = 16,
                Visibility = Visibility.Hidden
            };

            npcSpeech = new TextBlock
            {
                Foreground = Brushes.Black,
                FontSize = 16,
                Visibility = Visibility.Hidden
            };

            interactionHint = new TextBlock
            {
                Text = "Press (E) to talk",
                Foreground = Brushes.White,
                FontSize = 16,
                Visibility = Visibility.Hidden
            };

            BitmapImage leftImg = new BitmapImage(new Uri("Assets/UI/CommentBubble_Left.png", UriKind.Relative));
            BitmapImage midImg = new BitmapImage(new Uri("Assets/UI/CommentBubble_Middle.png", UriKind.Relative));
            BitmapImage rightImg = new BitmapImage(new Uri("Assets/UI/CommentBubble_Right.png", UriKind.Relative));

            playerBubbleLeft = new Image { Source = leftImg, Height = 44, Stretch = Stretch.Fill };
            playerBubbleMiddle = new Image { Source = midImg, Height = 44, Stretch = Stretch.Fill };
            playerBubbleRight = new Image { Source = rightImg, Height = 44, Stretch = Stretch.Fill };

            npcBubbleLeft = new Image { Source = leftImg, Height = 44, Stretch = Stretch.Fill };
            npcBubbleMiddle = new Image { Source = midImg, Height = 44, Stretch = Stretch.Fill };
            npcBubbleRight = new Image { Source = rightImg, Height = 44, Stretch = Stretch.Fill };  

            Panel.SetZIndex(playerBubbleLeft, 1997);
            Panel.SetZIndex(playerBubbleMiddle, 1998);
            Panel.SetZIndex(playerBubbleRight, 1999);

            Panel.SetZIndex(npcBubbleLeft, 1992);
            Panel.SetZIndex(npcBubbleMiddle, 1993);
            Panel.SetZIndex(npcBubbleRight, 1994);

            Panel.SetZIndex(playerSpeech, 2000);
            Panel.SetZIndex(npcSpeech, 1995);
            Panel.SetZIndex(interactionHint, 2001);

            GameCanvas.Children.Add(playerBubbleLeft);
            GameCanvas.Children.Add(playerBubbleMiddle);
            GameCanvas.Children.Add(playerBubbleRight);

            GameCanvas.Children.Add(npcBubbleLeft);
            GameCanvas.Children.Add(npcBubbleMiddle);
            GameCanvas.Children.Add(npcBubbleRight);

            GameCanvas.Children.Add(playerSpeech);
            GameCanvas.Children.Add(npcSpeech);
            GameCanvas.Children.Add(interactionHint);
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

            HandleNpcInteraction();
            UpdateSpeech();

            foreach (var npc in npcs)
            {
                if (npc.InteractCooldown > 0)
                    npc.InteractCooldown -= deltaTime;
            }
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

        private void InitializeEntities()
        {
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 3000, 300, 2800, 3500, EnemyMode.Patrol));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 2850, 300, 2800, 3000, EnemyMode.Patrol));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 3000, 300, 2900, 3200, EnemyMode.Patrol));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 3050, 300, 3000, 3600, EnemyMode.Patrol));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 2900, 300, 2850, 3250, EnemyMode.Patrol));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Brogur_light, 5000, 300, 4800, 5500, EnemyMode.Patrol));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Brogur_medium, 6000, 300, 5800, 6500, EnemyMode.Stand));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Brogur_heavy, 7500, 300, 7000, 8000, EnemyMode.Stand));

            npcs.Add(Npc.Create(GameCanvas, NpcType.Guard1, 500, 600, 200, 600, NpcMode.Wander));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Guard2, 600, 600, 550, 900, NpcMode.Wander));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Guard3, 700, 600, 600, 1000, NpcMode.Wander));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Eldon, 300, 600, 200, 500, NpcMode.Stand));
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

        private void HandleNpcInteraction()
        {
            Npc nearest = null;
            double minDist = double.MaxValue;

            foreach (var npc in npcs)
            {
                double dist = Math.Abs(playerWorldX - npc.WorldX);

                if (dist < 120 && dist < minDist)
                {
                    nearest = npc;
                    minDist = dist;
                }
            }

            if (nearest != null)
            {
                interactionHint.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double npcCenterX = nearest.WorldX + (nearest.Visual.Width / 2.0);

                Canvas.SetLeft(interactionHint,
                    npcCenterX - cameraX - (interactionHint.DesiredSize.Width / 2.0));

                Canvas.SetTop(interactionHint, nearest.WorldY - 28);

                interactionHint.Visibility =
                    nearest.InteractCooldown <= 0
                    ? Visibility.Visible
                    : Visibility.Hidden;

                if (input.EPressed && nearest.InteractCooldown <= 0)
                {
                    input.EPressed = false;

                    StartGreeting(nearest);

                    nearest.InteractCooldown = 10.0;

                    interactionHint.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                interactionHint.Visibility = Visibility.Hidden;
            }
        }

        private void StartGreeting(Npc npc)
        {
            var lines = LoadGreetings();

            if (lines.Count < 12)
                return;

            var rng = new Random();

            string playerLine = lines[rng.Next(0, 6)];
            string npcLine = lines[rng.Next(6, 12)];

            playerSpeech.Text = playerLine;
            playerSpeech.Visibility = Visibility.Visible;
            playerSpeechTimer = 2;

            npcSpeech.Text = npcLine;
            npcSpeech.Visibility = Visibility.Hidden;
            npcResponseDelay = 1;
            npcSpeechTimer = 2;

            currentNpc = npc;
            isGreetingActive = true;
        }

        private List<string> LoadGreetings()
        {
            var lines = new List<string>();

            try
            {
                using (var reader = new System.IO.StreamReader("Assets/Dialog/greetings.txt", Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                            lines.Add(line);
                    }
                }
            }
            catch
            {
                lines.Add("Hiba");
            }

            return lines;
        }

        private void HideBubble(Image left, Image middle, Image right, TextBlock text)
        {
            left.Visibility = Visibility.Hidden;
            middle.Visibility = Visibility.Hidden;
            right.Visibility = Visibility.Hidden;
            text.Visibility = Visibility.Hidden;
        }

        private void DrawBubble(Image left, Image middle, Image right, TextBlock text, double centerX, double worldY)
        {
            const double sideWidth = 12.0;
            const double height = 44.0;
            const double padding = 20.0;

            text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double textWidth = Math.Ceiling(text.DesiredSize.Width);
            double totalWidth = Math.Ceiling(textWidth + padding + (sideWidth * 2));

            double startX = Math.Round(centerX - cameraX - totalWidth / 2);
            double topY = Math.Round(worldY - 72);

            double middleWidth = totalWidth - (sideWidth * 2);

            left.Width = sideWidth;
            left.Height = height;
            Canvas.SetLeft(left, startX);
            Canvas.SetTop(left, topY);

            middle.Width = middleWidth;
            middle.Height = height;
            Canvas.SetLeft(middle, startX + sideWidth);
            Canvas.SetTop(middle, topY);

            right.Width = sideWidth;
            right.Height = height;
            Canvas.SetLeft(right, startX + sideWidth + middleWidth);
            Canvas.SetTop(right, topY);

            Canvas.SetLeft(text, Math.Round(centerX - cameraX - textWidth / 2));
            Canvas.SetTop(text, Math.Round(worldY - 61));

            left.Visibility = Visibility.Visible;
            middle.Visibility = Visibility.Visible;
            right.Visibility = Visibility.Visible;
            text.Visibility = Visibility.Visible;
        }

        private void UpdateSpeech()
        {
            if (playerSpeechTimer > 0)
            {
                playerSpeechTimer -= deltaTime;

                double playerCenterX = playerWorldX + (Player.ActualWidth / 2.0);
                DrawBubble(playerBubbleLeft, playerBubbleMiddle, playerBubbleRight, playerSpeech, playerCenterX, playerWorldY);

                if (playerSpeechTimer <= 0)
                {
                    HideBubble(playerBubbleLeft, playerBubbleMiddle, playerBubbleRight, playerSpeech);
                }
            }
            else
            {
                HideBubble(playerBubbleLeft, playerBubbleMiddle, playerBubbleRight, playerSpeech);
            }

            if (isGreetingActive && npcResponseDelay > 0)
            {
                npcResponseDelay -= deltaTime;

                if (npcResponseDelay <= 0)
                    npcSpeech.Visibility = Visibility.Visible;
            }

            if (npcSpeech.Visibility == Visibility.Visible && npcSpeechTimer > 0)
            {
                npcSpeechTimer -= deltaTime;

                if (currentNpc != null)
                {
                    double npcCenterX = currentNpc.WorldX + (currentNpc.Visual.Width / 2.0);
                    DrawBubble(npcBubbleLeft, npcBubbleMiddle, npcBubbleRight, npcSpeech, npcCenterX, currentNpc.WorldY);
                }

                if (npcSpeechTimer <= 0)
                {
                    HideBubble(npcBubbleLeft, npcBubbleMiddle, npcBubbleRight, npcSpeech);
                    isGreetingActive = false;
                }
            }
            else if (!isGreetingActive)
            {
                HideBubble(npcBubbleLeft, npcBubbleMiddle, npcBubbleRight, npcSpeech);
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

        internal List<Rectangle> platforms = new List<Rectangle>();
        internal List<Image> worldObjects = new List<Image>();

        internal TextBlock interactionHint;

        internal TextBlock playerSpeech;
        internal TextBlock npcSpeech;

        internal double playerSpeechTimer = 0;
        internal double npcSpeechTimer = 0;

        internal bool isGreetingActive = false;
        internal double npcResponseDelay = 0;

        internal Image playerBubbleLeft;
        internal Image playerBubbleMiddle;
        internal Image playerBubbleRight;

        internal Image npcBubbleLeft;
        internal Image npcBubbleMiddle;
        internal Image npcBubbleRight;

        internal Npc currentNpc = null;
    }
}