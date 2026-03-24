using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SimplePlatformer
{
    public partial class MainWindow : Window
    {
        private bool canShootBow = true;
        private double bowShootCooldownTimer = 0;
        private double bowShootCooldown = 0.5;

        private BitmapImage[] bowFrames;
        private int lastBowFrameIndex = -1;

        private BitmapImage[] idleFrames;
        private int idleFrameIndex = 0;
        private double idleFrameTimer = 0;

        private readonly double[] idleFrameDurations = { 1.0, 0.1, 1.0, 0.1 };

        private bool canMeleeAttack = true;
        private double meleeCooldownTimer = 0;
        private double meleeCooldown = 0.5;

        private double currentAimRatioY = 0;

        private Line aimLineLeft;
        private Line aimLineRight;
        private Ellipse aimDot;

        private double aimLineHeight = 120;

        private const int MaxChargeSteps = 5;
        private const double ChargeStepTime = 0.2;

        private int currentChargeStep = 0;

        private bool bowKeyPressed = false;
        private bool isChargingBow = false;

        private double bowChargeTime = 0;

        private List<Arrow> arrows = new List<Arrow>();

        private Rectangle bowChargeBarBg;
        private Rectangle bowChargeBarFill;

        private double playerKnockbackX = 350;
        private double playerKnockbackY = 300;

        private double playerStunTimer = 0;
        private double playerStunDuration = 0.4;
        private bool IsPlayerStunned => playerStunTimer > 0;

        private double enemyKnockbackX = 300;
        private double enemyKnockbackY = 250;

        private bool attackPressed = false;
        private double attackRange = 80;
        private double attackDamage = 40;

        private bool canTakeDamage = true;
        private double damageTimer = 0;

        private double damageCooldown = 3.0;

        private double invulnerableVisualTime = 0.5;
        private double invulnerableVisualTimer = 0;

        private double maxHp = 100;
        private double currentHp = 100;

        private void UpdateHpBar()
        {
            double hpPercent = currentHp / maxHp;
            hpPercent = Math.Max(0, Math.Min(1, hpPercent));

            double maxWidth = 100;
            HpBarFill.Width = maxWidth * hpPercent;
        }

        private List<Enemy> enemies = new List<Enemy>();

        private bool isPaused = false;
        private bool isDead = false;

        private double platform1X = 10;
        private double platform2X = 360;
        private double platform3X = 1630;
        private double platform4X = 1254;

        private double playerWalkSpeed = 200;
        private double playerRunSpeed = 350;

        private bool shiftPressed = false;

        private double jumpForce = 450;
        private double gravity = 1500;

        private double playerVelocityX = 0;
        private double playerVelocityY = 0;
        private bool isJumping = false;
        private bool isOnGround = false;

        private bool isFacingRight = true;

        private bool leftPressed = false;
        private bool rightPressed = false;
        private bool spacePressed = false;

        private DateTime lastUpdateTime;
        private double deltaTime;

        private List<string> debugInfo = new List<string>();
        private int frameCount = 0;
        private double fps = 0;
        private DateTime lastFpsUpdate = DateTime.Now;

        private double WorldWidth = 3000;

        private double playerWorldX = 100;
        private double playerWorldY = 300;

        private double cameraX = 0;

        private double screenLeftLimit = 300;
        private double screenRightLimit = 500;

        public MainWindow()
        {
            InitializeComponent();
            this.Focus();

            LoadPlayerImage();
        }

        private void LoadPlayerImage()
        {
            try
            {
                string imagePath = "Assets/Borien_stand_test.png";

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

                var rectangle = new System.Windows.Shapes.Rectangle();
                rectangle.Width = 60;
                rectangle.Height = 60;
                rectangle.Fill = Brushes.Green;

                Player.Source = null;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lastUpdateTime = DateTime.Now;

            CompositionTarget.Rendering += GameLoop;

            DebugText.Text = "Game Started! Use A/D or Left/Right to move, Space to jump";

            this.Focus();

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

            GameCanvas.Children.Add(aimLineLeft);
            GameCanvas.Children.Add(aimLineRight);
            GameCanvas.Children.Add(aimDot);

            LoadIdleFrames();

            LoadBowFrames();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (isPaused || isDead)
                return;

            DateTime currentTime = DateTime.Now;
            deltaTime = (currentTime - lastUpdateTime).TotalSeconds;

            if (deltaTime > 0.1) deltaTime = 0.1;

            lastUpdateTime = currentTime;

            UpdateFPS();

            debugInfo.Clear();

            HandleMovement();

            ApplyPhysics();

            CheckCollisions();

            UpdatePlayerPosition();

            UpdateCamera();

            UpdatePlayerFacingDirection();

            UpdateDebugInfo();

            UpdateHpBar();

            CheckDeath();

            UpdateEnemies();

            CheckEnemyCollisions();

            UpdateDamageCooldown();

            HandlePlayerAttack();

            UpdatePlayerStun();

            UpdateBowCharge();

            UpdateArrows();

            UpdateAimUI();

            UpdateMeleeCooldown();

            UpdateIdleAnimation();

            UpdateBowAnimation();

            UpdateIdleAnimation();

            UpdateBowCooldown();
        }

        private void HandleMovement()
        {
            if (IsPlayerStunned)
                return;

            playerVelocityX = 0;

            double speed = shiftPressed
                ? playerRunSpeed
                : playerWalkSpeed;

            if (leftPressed)
                playerVelocityX = -speed;

            if (rightPressed)
                playerVelocityX = speed;

            if (spacePressed && isOnGround)
            {
                playerVelocityY = -jumpForce;
                isJumping = true;
                isOnGround = false;
            }
        }

        private void ApplyPhysics()
        {
            if (!isOnGround)
            {
                playerVelocityY += gravity * deltaTime;
            }

            if (playerVelocityY > 800) playerVelocityY = 800;
        }

        private void CheckCollisions()
        {
            isOnGround = false;

            double playerWidth = Player.ActualWidth;
            double playerHeight = Player.ActualHeight;
            Rect playerRect = new Rect(playerWorldX, playerWorldY, playerWidth, playerHeight);

            if (playerRect.Bottom >= GameCanvas.ActualHeight)
            {
                playerWorldY = GameCanvas.ActualHeight - playerHeight;
                playerVelocityY = 0;
                isOnGround = true;
                return;
            }

            ResolvePlatformCollision(Platform1, platform1X);
            ResolvePlatformCollision(Platform2, platform2X);
            ResolvePlatformCollision(Platform3, platform3X);
            ResolvePlatformCollision(Platform4, platform4X);
        }

        private void ResolvePlatformCollision(System.Windows.Shapes.Rectangle platform, double platformWorldX)
        {
            double playerWidth = Player.ActualWidth;
            double playerHeight = Player.ActualHeight;

            Rect playerRect = new Rect(
                playerWorldX,
                playerWorldY,
                playerWidth,
                playerHeight
            );

            Rect platformRect = new Rect(
                platformWorldX,
                Canvas.GetTop(platform),
                platform.Width,
                platform.Height
            );

            if (playerVelocityY <= 0)
                return;

            if (playerRect.Bottom >= platformRect.Top &&
                playerRect.Top < platformRect.Top &&
                playerRect.Right > platformRect.Left &&
                playerRect.Left < platformRect.Right)
            {
                playerWorldY = platformRect.Top - playerHeight;
                playerVelocityY = 0;
                isOnGround = true;
            }
        }

        private void UpdatePlayerPosition()
        {
            playerWorldX += playerVelocityX * deltaTime;
            playerWorldY += playerVelocityY * deltaTime;

            if (playerWorldX < 0) playerWorldX = 0;
            if (playerWorldX + Player.ActualWidth > WorldWidth)
                playerWorldX = WorldWidth - Player.ActualWidth;

            Canvas.SetLeft(Player, playerWorldX - cameraX);
            Canvas.SetTop(Player, playerWorldY);
        }

        private void UpdateCamera()
        {
            double screenX = playerWorldX - cameraX;

            if (screenX > screenRightLimit)
            {
                cameraX = playerWorldX - screenRightLimit;
            }

            else if (screenX < screenLeftLimit)
            {
                cameraX = playerWorldX - screenLeftLimit;
            }

            if (cameraX < 0)
                cameraX = 0;

            if (cameraX > WorldWidth - GameCanvas.ActualWidth)
                cameraX = WorldWidth - GameCanvas.ActualWidth;

            Canvas.SetLeft(Platform1, platform1X - cameraX);
            Canvas.SetLeft(Platform2, platform2X - cameraX);
            Canvas.SetLeft(Platform3, platform3X - cameraX);
            Canvas.SetLeft(Platform4, platform4X - cameraX);
        }

        private void UpdatePlayerFacingDirection()
        {
            if (playerVelocityX > 0.1 && !isFacingRight)
            {
                PlayerScaleTransform.ScaleX = 1;
                isFacingRight = true;
            }
            else if (playerVelocityX < -0.1 && isFacingRight)
            {
                PlayerScaleTransform.ScaleX = -1;
                isFacingRight = false;
            }
        }

        private void UpdateFPS()
        {
            frameCount++;

            if ((DateTime.Now - lastFpsUpdate).TotalSeconds >= 0.5)
            {
                fps = frameCount / (DateTime.Now - lastFpsUpdate).TotalSeconds;
                frameCount = 0;
                lastFpsUpdate = DateTime.Now;
            }
        }

        private void UpdateDebugInfo()
        {
            debugInfo.Add($"FPS: {fps:F1}");
            debugInfo.Add($"Delta Time: {deltaTime * 1000:F1}ms");
            debugInfo.Add($"Pos: ({Canvas.GetLeft(Player):F0}, {Canvas.GetTop(Player):F0})");
            debugInfo.Add($"Velocity: ({playerVelocityX:F0}, {playerVelocityY:F0})");
            debugInfo.Add($"On Ground: {isOnGround}");
            debugInfo.Add($"Jumping: {isJumping}");
            debugInfo.Add($"Facing: {(isFacingRight ? "Right" : "Left")}");
            debugInfo.Add($"Keys: A/D={leftPressed}/{rightPressed}, Space={spacePressed}");

            DebugText.Text = string.Join("\n", debugInfo);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (isDead)
                return;

            if (isPaused && e.Key != Key.Escape)
                return;

            switch (e.Key)
            {
                case Key.G:
                    bowKeyPressed = true;
                    break;
                case Key.LeftShift:
                case Key.RightShift:
                    shiftPressed = true;
                    break;
                case Key.H:
                    currentHp -= 10;
                    if (currentHp < 0)
                        currentHp = 0;
                    break;
                case Key.A:
                case Key.Left:
                    leftPressed = true;
                    break;
                case Key.D:
                case Key.Right:
                    rightPressed = true;
                    break;
                case Key.Space:
                    spacePressed = true;
                    break;
                case Key.Escape:
                    TogglePause();
                    break;
                case Key.F:
                    attackPressed = true;
                    break;
                case Key.F1:
                    DebugText.Visibility = DebugText.Visibility == Visibility.Visible
                        ? Visibility.Hidden
                        : Visibility.Visible;
                    break;
            }

            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.G:
                    bowKeyPressed = false;
                    ReleaseArrow();
                    break;
                case Key.LeftShift:
                case Key.RightShift:
                    shiftPressed = false;
                    break;
                case Key.A:
                case Key.Left:
                    leftPressed = false;
                    break;
                case Key.D:
                case Key.Right:
                    rightPressed = false;
                    break;
                case Key.Space:
                    spacePressed = false;
                    break;
                case Key.F:
                    attackPressed = false;
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CompositionTarget.Rendering -= GameLoop;
        }
        private void TogglePause()
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

        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            TogglePause();
        }

        private void ExitToMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenuWindow menu = new MainMenuWindow();
            menu.Show();

            this.Close();
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Respawn()
        {
            currentHp = maxHp;
            UpdateHpBar();

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

        private void Respawn_Click(object sender, RoutedEventArgs e)
        {
            Respawn();
        }

        private void UpdateEnemies()
        {
            foreach (var enemy in enemies)
            {
                double distanceToPlayer = Math.Abs(playerWorldX - enemy.WorldX);

                if (!enemy.IsAggro && distanceToPlayer <= enemy.AggroRange)
                {
                    enemy.IsAggro = true;
                }

                if (enemy.StunTimer > 0)
                {
                    enemy.StunTimer -= deltaTime;
                }
                else
                {
                    if (enemy.IsAggro)
                    {
                        double speed = enemy.AggroSpeed;

                        if (playerWorldX < enemy.WorldX)
                            enemy.VelocityX = -speed;
                        else
                            enemy.VelocityX = speed;
                    }
                    else
                    {
                        double speed = enemy.WalkSpeed;

                        if (enemy.WorldX < enemy.LeftLimit)
                            enemy.VelocityX = speed;
                        else if (enemy.WorldX > enemy.RightLimit)
                            enemy.VelocityX = -speed;
                    }
                }

                if (!enemy.OnGround)
                    enemy.VelocityY += gravity * deltaTime;

                if (enemy.VelocityY > 800)
                    enemy.VelocityY = 800;

                enemy.WorldX += enemy.VelocityX * deltaTime;
                enemy.WorldY += enemy.VelocityY * deltaTime;

                enemy.OnGround = false;

                ResolveEnemyPlatform(enemy, Platform1, platform1X);
                ResolveEnemyPlatform(enemy, Platform2, platform2X);
                ResolveEnemyPlatform(enemy, Platform3, platform3X);
                ResolveEnemyPlatform(enemy, Platform4, platform4X);

                Canvas.SetLeft(enemy.Visual, enemy.WorldX - cameraX);
                Canvas.SetTop(enemy.Visual, enemy.WorldY);

                if (enemy.HitFlashTimer > 0)
                {
                    enemy.HitFlashTimer -= deltaTime;
                    if (enemy.HitFlashTimer <= 0)
                    {
                        enemy.Visual.Fill = enemy.OriginalBrush;
                        enemy.HitFlashTimer = 0;
                    }
                }

                enemy.UpdateHpBar(cameraX);
            }
        }

        private void ResolveEnemyPlatform(Enemy enemy, System.Windows.Shapes.Rectangle platform, double platformWorldX)
        {
            Rect enemyRect = new Rect(
                enemy.WorldX,
                enemy.WorldY,
                enemy.Visual.Width,
                enemy.Visual.Height
            );

            Rect platformRect = new Rect(
                platformWorldX,
                Canvas.GetTop(platform),
                platform.Width,
                platform.Height
            );

            if (enemy.VelocityY <= 0)
                return;

            if (enemyRect.Bottom >= platformRect.Top &&
                enemyRect.Top < platformRect.Top &&
                enemyRect.Right > platformRect.Left &&
                enemyRect.Left < platformRect.Right)
            {
                enemy.WorldY = platformRect.Top - enemy.Visual.Height;
                enemy.VelocityY = 0;
                enemy.OnGround = true;
            }
        }

        private void CheckEnemyCollisions()
        {
            if (isDead)
                return;

            Rect playerRect = new Rect(
                playerWorldX,
                playerWorldY,
                Player.ActualWidth,
                Player.ActualHeight
            );

            foreach (var enemy in enemies)
            {
                Rect enemyRect = new Rect(
                    enemy.WorldX,
                    enemy.WorldY,
                    enemy.Visual.Width,
                    enemy.Visual.Height
                );


                if (playerRect.IntersectsWith(enemyRect))
                {
                    if (canTakeDamage)
                    {
                        DamagePlayer(20, enemy);

                        enemy.Visual.Fill = Brushes.LightGreen;
                        enemy.HitFlashTimer = 0.2;
                    }
                }
            }
        }

        private void UpdateDamageCooldown()
        {
            if (!canTakeDamage)
            {
                damageTimer -= deltaTime;
                if (damageTimer <= 0)
                {
                    canTakeDamage = true;
                    damageTimer = 0;
                }
            }

            if (invulnerableVisualTimer > 0)
            {
                invulnerableVisualTimer -= deltaTime;
                if (invulnerableVisualTimer <= 0)
                {
                    invulnerableVisualTimer = 0;
                    Player.Opacity = 1.0;
                }
            }
        }

        private void DamagePlayer(double amount, Enemy sourceEnemy = null)
        {
            if (!canTakeDamage || isDead)
                return;

            currentHp -= amount;
            if (currentHp < 0)
                currentHp = 0;

            canTakeDamage = false;
            damageTimer = damageCooldown;

            invulnerableVisualTimer = invulnerableVisualTime;
            Player.Opacity = 0.5;

            playerStunTimer = playerStunDuration;

            if (sourceEnemy != null)
            {
                if (sourceEnemy.WorldX < playerWorldX)
                    playerVelocityX = playerKnockbackX;
                else
                    playerVelocityX = -playerKnockbackX;

                playerVelocityY = -playerKnockbackY;
            }
        }

        private void HandlePlayerAttack()
        {
            if (!attackPressed || !canMeleeAttack)
                return;

            foreach (var enemy in enemies)
            {
                double dx = enemy.WorldX - playerWorldX;
                double dy = Math.Abs(enemy.WorldY - playerWorldY);

                bool enemyInFront =
                    (isFacingRight && dx > 0 && dx < attackRange) ||
                    (!isFacingRight && dx < 0 && -dx < attackRange);

                if (enemyInFront && dy < 50)
                {
                    DamageEnemy(enemy);

                    canMeleeAttack = false;
                    meleeCooldownTimer = meleeCooldown;

                    break;
                }
            }
        }

        private void DamageEnemy(Enemy enemy)
        {
            enemy.CurrentHp -= attackDamage;

            enemy.IsAggro = true;

            enemy.Visual.Fill = Brushes.LightGreen;
            enemy.HitFlashTimer = 0.2;

            if (playerWorldX < enemy.WorldX)
                enemy.VelocityX = enemyKnockbackX;
            else
                enemy.VelocityX = -enemyKnockbackX;

            enemy.VelocityY = -enemyKnockbackY;

            enemy.StunTimer = Enemy.StunDuration;

            if (enemy.CurrentHp <= 0)
            {
                KillEnemy(enemy);
            }
        }

        private void DamageEnemy(Enemy enemy, double damage)
        {
            enemy.CurrentHp -= damage;
            enemy.IsAggro = true;

            enemy.Visual.Fill = Brushes.LightGreen;
            enemy.HitFlashTimer = 0.2;

            enemy.StunTimer = Enemy.StunDuration;

            if (enemy.CurrentHp <= 0)
            {
                KillEnemy(enemy);
            }
        }

        private void KillEnemy(Enemy enemy)
        {
            GameCanvas.Children.Remove(enemy.Visual);
            GameCanvas.Children.Remove(enemy.HpBarBackground);
            GameCanvas.Children.Remove(enemy.HpBarFill);

            enemies.Remove(enemy);
        }

        private void UpdatePlayerStun()
        {
            if (playerStunTimer > 0)
            {
                playerStunTimer -= deltaTime;
                if (playerStunTimer <= 0)
                {
                    playerStunTimer = 0;
                }
            }
        }

        private void UpdateBowCharge()
        {
            if (!canShootBow)
                return;

            if (bowKeyPressed && !IsPlayerStunned)
            {
                if (!isChargingBow)
                {
                    isChargingBow = true;
                    bowChargeTime = 0;
                    currentChargeStep = 0;

                    bowChargeBarBg.Visibility = Visibility.Visible;
                    bowChargeBarFill.Visibility = Visibility.Visible;
                }

                bowChargeTime += deltaTime;

                int step = (int)(bowChargeTime / ChargeStepTime);
                currentChargeStep = Math.Min(step, MaxChargeSteps);

                bowChargeBarFill.Width = 20 * currentChargeStep;

                Canvas.SetLeft(bowChargeBarBg, Canvas.GetLeft(Player) - 20);
                Canvas.SetTop(bowChargeBarBg, Canvas.GetTop(Player) - 20);

                Canvas.SetLeft(bowChargeBarFill, Canvas.GetLeft(Player) - 18);
                Canvas.SetTop(bowChargeBarFill, Canvas.GetTop(Player) - 18);
            }
        }

        private void ReleaseArrow()
        {
            if (!isChargingBow)
                return;

            canShootBow = false;
            bowShootCooldownTimer = bowShootCooldown;

            isChargingBow = false;
            bowKeyPressed = false;

            bowChargeBarBg.Visibility = Visibility.Hidden;
            bowChargeBarFill.Visibility = Visibility.Hidden;

            lastBowFrameIndex = -1;

            if (idleFrames != null && idleFrames.Length > 0)
            {
                idleFrameIndex = 0;
                idleFrameTimer = 0;
                Player.Source = idleFrames[0];
            }

            double[] chargeLevels = { 0.2, 0.4, 0.6, 0.8, 1.0 };
            double[] damageLevels = { 20, 30, 40, 50, 60 };

            int index = Math.Clamp(currentChargeStep - 1, 0, chargeLevels.Length - 1);

            double power = chargeLevels[index];
            double damage = damageLevels[index];

            double speedX = 600 + 1800 * power;
            double baseSpeedY = -80 * power;
            double aimInfluence = 400 * currentAimRatioY;

            double speedY = baseSpeedY + aimInfluence;

            if (!isFacingRight)
                speedX = -speedX;

            Arrow arrow = new Arrow(
                GameCanvas,
                playerWorldX + (isFacingRight ? Player.ActualWidth : -10),
                playerWorldY + Player.ActualHeight / 2,
                speedX,
                speedY,
                damage
            );

            arrows.Add(arrow);
        }


        private void UpdateArrows()
        {
            for (int i = arrows.Count - 1; i >= 0; i--)
            {
                var arrow = arrows[i];

                arrow.Update(deltaTime);

                Canvas.SetLeft(arrow.Visual, arrow.WorldX - cameraX);
                Canvas.SetTop(arrow.Visual, arrow.WorldY);

                Rect arrowRect = arrow.GetRect();

                if (arrow.WorldX < 0 || arrow.WorldX > WorldWidth ||
                    arrow.WorldY > GameCanvas.ActualHeight)
                {
                    GameCanvas.Children.Remove(arrow.Visual);
                    arrows.RemoveAt(i);
                    continue;
                }

                bool hitEnemy = false;
                foreach (var enemy in enemies)
                {
                    Rect enemyRect = new Rect(
                        enemy.WorldX,
                        enemy.WorldY,
                        enemy.Visual.Width,
                        enemy.Visual.Height
                    );

                    if (arrowRect.IntersectsWith(enemyRect))
                    {
                        DamageEnemy(enemy, arrow.Damage);
                        hitEnemy = true;
                        break;
                    }
                }

                if (hitEnemy)
                {
                    GameCanvas.Children.Remove(arrow.Visual);
                    arrows.RemoveAt(i);
                    continue;
                }

                if (arrowRect.IntersectsWith(GetPlatformRect(Platform1, platform1X)) ||
                    arrowRect.IntersectsWith(GetPlatformRect(Platform2, platform2X)) ||
                    arrowRect.IntersectsWith(GetPlatformRect(Platform3, platform3X)) ||
                    arrowRect.IntersectsWith(GetPlatformRect(Platform4, platform4X)))
                {
                    GameCanvas.Children.Remove(arrow.Visual);
                    arrows.RemoveAt(i);
                }
            }
        }

        private Rect GetPlatformRect(Rectangle platform, double worldX)
        {
            return new Rect(
                worldX,
                Canvas.GetTop(platform),
                platform.Width,
                platform.Height
            );
        }

        private void UpdateAimUI()
        {
            if (!isChargingBow)
            {
                aimLineLeft.Visibility = Visibility.Hidden;
                aimLineRight.Visibility = Visibility.Hidden;
                aimDot.Visibility = Visibility.Hidden;
                return;
            }

            double playerScreenX = playerWorldX - cameraX;
            double playerCenterY = playerWorldY + Player.ActualHeight / 2;

            double lineTop = playerCenterY - aimLineHeight / 2;
            double lineBottom = playerCenterY + aimLineHeight / 2;

            aimLineLeft.X1 = playerScreenX - 30;
            aimLineLeft.X2 = aimLineLeft.X1;
            aimLineLeft.Y1 = lineTop;
            aimLineLeft.Y2 = lineBottom;

            aimLineRight.X1 = playerScreenX + Player.ActualWidth + 30;
            aimLineRight.X2 = aimLineRight.X1;
            aimLineRight.Y1 = lineTop;
            aimLineRight.Y2 = lineBottom;

            Point mouse = Mouse.GetPosition(GameCanvas);

            bool mouseOnRight = mouse.X > playerScreenX + Player.ActualWidth / 2;

            aimLineLeft.Visibility = mouseOnRight ? Visibility.Hidden : Visibility.Visible;
            aimLineRight.Visibility = mouseOnRight ? Visibility.Visible : Visibility.Hidden;

            double clampedY = Math.Clamp(mouse.Y, lineTop, lineBottom);

            double dotX = mouseOnRight
                ? aimLineRight.X1
                : aimLineLeft.X1;

            Canvas.SetLeft(aimDot, dotX - aimDot.Width / 2);
            Canvas.SetTop(aimDot, clampedY - aimDot.Height / 2);

            double centerY = (lineTop + lineBottom) / 2;
            double halfHeight = aimLineHeight / 2;

            currentAimRatioY = (clampedY - centerY) / halfHeight;
            currentAimRatioY = Math.Clamp(currentAimRatioY, -1.0, 1.0);

            aimDot.Visibility = Visibility.Visible;

            if (mouseOnRight && !isFacingRight)
            {
                PlayerScaleTransform.ScaleX = 1;
                isFacingRight = true;
            }
            else if (!mouseOnRight && isFacingRight)
            {
                PlayerScaleTransform.ScaleX = -1;
                isFacingRight = false;
            }
        }

        private void UpdateMeleeCooldown()
        {
            if (!canMeleeAttack)
            {
                meleeCooldownTimer -= deltaTime;
                if (meleeCooldownTimer <= 0)
                {
                    canMeleeAttack = true;
                    meleeCooldownTimer = 0;
                }
            }
        }

        private void LoadIdleFrames()
        {
            idleFrames = new BitmapImage[4];

            for (int i = 0; i < 4; i++)
            {
                string path = $"Assets/Sprites/Borien/Borien_Stand_{i + 1}.png";

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(path, UriKind.Relative);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                bmp.Freeze();

                idleFrames[i] = bmp;
            }

            Player.Source = idleFrames[0];
        }

        private void UpdateIdleAnimation()
        {
            if (idleFrames == null || idleFrames.Length == 0)
                return;

            bool isIdle =
                Math.Abs(playerVelocityX) < 0.01 &&
                Math.Abs(playerVelocityY) < 0.01 &&
                !leftPressed && !rightPressed && !spacePressed &&
                !attackPressed &&
                !isChargingBow &&
                !IsPlayerStunned &&
                canMeleeAttack;

            if (!isIdle)
            {
                idleFrameTimer = 0;
                idleFrameIndex = 0;
                return;
            }

            idleFrameTimer += deltaTime;

            double currentDuration = idleFrameDurations[idleFrameIndex];

            if (idleFrameTimer >= currentDuration)
            {
                idleFrameTimer -= currentDuration;

                idleFrameIndex++;
                if (idleFrameIndex >= idleFrames.Length)
                    idleFrameIndex = 0;

                Player.Source = idleFrames[idleFrameIndex];
            }

        }

        private void LoadBowFrames()
        {
            bowFrames = new BitmapImage[4];

            for (int i = 0; i < 4; i++)
            {
                string path = $"Assets/Sprites/Borien/Borien_Bow_{i + 1}.png";

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(path, UriKind.Relative);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();
                bmp.Freeze();

                bowFrames[i] = bmp;
            }
        }
        private void UpdateBowAnimation()
        {
            if (bowFrames == null || bowFrames.Length == 0)
                return;

            if (!isChargingBow)
            {
                lastBowFrameIndex = -1;
                return;
            }

            int bowFrameIndex;
            if (currentChargeStep <= 0) bowFrameIndex = 0;
            else if (currentChargeStep == 1) bowFrameIndex = 1;
            else if (currentChargeStep == 2) bowFrameIndex = 2;
            else bowFrameIndex = 3;

            if (bowFrameIndex != lastBowFrameIndex)
            {
                Player.Source = bowFrames[bowFrameIndex];
                lastBowFrameIndex = bowFrameIndex;
            }
        }

        private void UpdateBowCooldown()
        {
            if (!canShootBow)
            {
                bowShootCooldownTimer -= deltaTime;
                if (bowShootCooldownTimer <= 0)
                {
                    canShootBow = true;
                    bowShootCooldownTimer = 0;
                }
            }
        }
    }
}