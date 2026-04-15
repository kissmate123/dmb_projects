using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ThePixelRealms
{
    public partial class Level1 : Window
    {
        internal Input input;
        private PlayerMovement playerMovement;
        private GamePhysics gamePhysics;
        internal UIAndDebug uiAndDebug;
        private EnemyMovement enemyMovement;
        private EnemyCombat enemyCombat;
        private PlayerCombat playerCombat;
        private NpcMovement npcMovement;
        private NpcCombat npcCombat;
        private PropManager propManager;
        internal DialogueSystem dialogueSystem;
        internal CutsceneSystem cutscene;
        internal GuideSystem guide;
        internal StoryManager story;
        internal RespawnSystem respawn;

        //Player

        internal double playerWorldX = 3800;
        internal double playerWorldY = 700;

        internal double playerVelocityX = 0;
        internal double playerVelocityY = 0;

        internal bool isOnGround = false;
        internal bool isJumping = false;
        internal bool wasOnGround = false;

        internal bool isFacingRight = true;

        internal double playerWalkSpeed = 150;
        internal double playerRunSpeed = 350;

        internal double jumpForce = 450;
        internal double gravity = 1500;

        internal bool leftPressed = false;
        internal bool rightPressed = false;
        internal bool spacePressed = false;
        internal bool shiftPressed = false;

        //Combat

        internal double maxHp = 100;
        internal double currentHp = 100;

        internal bool attackPressed = false;
        internal double attackRange = 100;
        internal double attackDamage = 40;

        internal bool canTakeDamage = true;
        internal double damageTimer = 0;
        internal double damageCooldown = 3.0;

        internal double playerKnockbackX = 350;
        internal double playerKnockbackY = 300;

        internal double enemyKnockbackX = 300;
        internal double enemyKnockbackY = 250;

        internal double playerStunTimer = 0;
        internal double playerStunDuration = 0.4;
        internal bool IsPlayerStunned => playerStunTimer > 0;

        internal double invulnerableVisualTime = 0.5;
        internal double invulnerableVisualTimer = 0;

        internal bool canMeleeAttack = true;
        internal double meleeCooldownTimer = 0;
        internal double meleeCooldown = 0.5;

        internal BitmapImage[] meleeFrames;
        internal int meleeFrameIndex = 0;
        internal double meleeFrameTimer = 0;

        internal bool isMeleeAnimating = false;
        internal double meleeFrameDuration = 0.05;

        internal bool canShootBow = true;
        internal double bowShootCooldownTimer = 0;
        internal double bowShootCooldown = 0.5;

        internal bool bowKeyPressed = false;
        internal bool isChargingBow = false;

        internal double bowChargeTime = 0;
        internal int currentChargeStep = 0;

        internal const int MaxChargeSteps = 5;
        internal const double ChargeStepTime = 0.2;

        internal BitmapImage[] bowFrames;
        internal int lastBowFrameIndex = -1;

        internal List<Arrow> arrows = new List<Arrow>();

        internal double currentAimRatioY = 0;

        internal Line aimLineLeft;
        internal Line aimLineRight;
        internal Ellipse aimDot;

        internal double aimLineHeight = 120;

        internal Rectangle bowChargeBarBg;
        internal Rectangle bowChargeBarFill;

        //Animációk

        internal BitmapImage[] idleFrames;
        internal int idleFrameIndex = 0;
        internal double idleFrameTimer = 0;
        internal readonly double[] idleFrameDurations = { 0.5, 0.05, 0.5, 0.05 };

        internal BitmapImage[] walkFrames;
        internal int walkFrameIndex = 0;
        internal double walkFrameTimer = 0;
        internal bool wasWalking = false;
        internal double walkFrameDuration = 0.08;

        internal BitmapImage jumpAirFrame;

        internal BitmapImage shieldSprite;
        internal bool isShielding = false;

        //World

        internal double WorldWidth = 19000;
        internal double cameraX = 0;

        internal double screenLeftLimit = 400;
        internal double screenRightLimit = 500;

        internal double GroundX = 10;

        internal const double PlayerHitboxWidth = 48;
        internal const double PlayerHitboxHeight = 60;
        internal const double PlayerHitboxOffsetX = 30;

        internal List<Enemy> enemies = new List<Enemy>();
        public List<Npc> npcs = new List<Npc>();

        internal List<Rectangle> platforms = new List<Rectangle>();
        internal List<Image> worldObjects = new List<Image>();

        internal TextBlock interactionHint;

        internal TextBlock npcSpeech;
        internal double npcSpeechTimer = 0;

        internal bool isGreetingActive = false;
        internal double npcResponseDelay = 0;

        internal Image npcBubbleLeft;
        internal Image npcBubbleMiddle;
        internal Image npcBubbleRight;

        internal Npc currentNpc = null;

        internal List<NpcType> greetingAllowedTypes = new List<NpcType>
        {
            NpcType.Guard1,
            NpcType.Guard2,
            NpcType.Civilian
        };

        internal Dictionary<int, NpcType> talkSequence = new Dictionary<int, NpcType>
        {
            { 0, NpcType.Eldon },
            { 1, NpcType.Mira },
            { 2, NpcType.Dorin },
            { 3, NpcType.Noril },
            { 6, NpcType.Eldon },
            { 9, NpcType.Eldon }
        };

        internal double shieldWorldX = 1500;
        internal double shieldWorldY = 745;

        internal const double maxShield = 100;
        internal double currentShield = 0;

        internal double appleWorldX = 1700;
        internal double appleWorldY = 760;
        internal double appleHealAmount = 35;

        internal bool isPaused = false;
        internal bool isDead = false;

        internal int enemiesSpawned = 0;
        internal int enemiesKilled = 0;

        internal bool fight1Started = false;
        internal bool fight2Started = false;
        internal bool fight2Finished = false;

        internal bool rooftopChaseStarted = false;

        bool cutscene1Played = false;
        bool cutscene2Played = false;
        bool cutscene3Played = false;

        bool rooftopChaseInitialized = false;
        bool finishShown = false;

        bool miraAppleSpawned = false;
        bool dorinShieldSpawned = false;
        bool eldonSuppliesSpawned = false;

        internal DateTime lastUpdateTime;
        internal double deltaTime;

        internal List<string> debugInfo = new List<string>();
        internal int frameCount = 0;
        internal double fps = 0;
        internal DateTime lastFpsUpdate = DateTime.Now;

        public Level1()
        {
            InitializeComponent();
            Closing += Window_Closing;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Focus();

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
            story = new StoryManager();
            npcMovement = new NpcMovement(this);
            npcCombat = new NpcCombat(this);
            propManager = new PropManager(this);
            dialogueSystem = new DialogueSystem(this);
            cutscene = new CutsceneSystem(this);
            guide = new GuideSystem(this);
            respawn = new RespawnSystem(this);

            InitializeEntities();

            uiAndDebug.LoadPlayerImage();
            playerCombat.LoadIdleFrames();
            playerCombat.LoadWalkFrames();
            playerCombat.LoadBowFrames();
            playerCombat.LoadMeleeFrames();
            playerCombat.LoadJumpAirFrame();
            playerCombat.LoadShieldSprite();
            uiAndDebug.InitializeSpeechUi();
            uiAndDebug.InitializeBowAndAimUI();
            uiAndDebug.UpdateHpBar();

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
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            CompositionTarget.Rendering -= GameLoop;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) => input.Window_KeyDown(sender, e);
        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) => input.Window_KeyUp(sender, e);

        private void GameLoop(object sender, EventArgs e)
        {
            if (cutscene.IsActive)
            {
                if (input.EPressed)
                {
                    cutscene.Advance();
                    input.EPressed = false;
                }

                cutscene.Update();
                return;
            }

            dialogueSystem.Update();

            if (dialogueSystem.IsActive || cutscene.IsActive)
            {
                leftPressed = false;
                rightPressed = false;
                spacePressed = false;
                bowKeyPressed = false;
                attackPressed = false;
                isShielding = false;

                playerVelocityX = 0;
                playerVelocityY = 0;
            }

            if (isPaused)
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

            npcCombat.CheckNpcCollisions();
            npcMovement.UpdateNpcs();

            uiAndDebug.CheckDeath();

            propManager.UpdateApples();
            propManager.UpdateShields();

            dialogueSystem.Update();

            if (isShielding && shieldSprite != null)
                Player.Source = shieldSprite;

            foreach (var npc in npcs)
            {
                if (npc.InteractCooldown > 0)
                    npc.InteractCooldown -= deltaTime;
            }

            if (story.CurrentStep == 0)
            {
                currentHp = maxHp * 0.75;
                guide.ShowLine(0);
                EnableBarricade(Barricade1Platform, Barricade1);
            }

            if (story.CurrentStep == 1)
            {
                guide.ShowLine(1);
            }

            if (story.CurrentStep == 2 && !miraAppleSpawned)
            {
                miraAppleSpawned = true;

                propManager.SpawnApple(6550, 760);

                guide.ShowLine(2);
            }

            if (story.CurrentStep == 3 && !dorinShieldSpawned)
            {
                dorinShieldSpawned = true;

                propManager.SpawnShield(8950, 720);

                guide.ShowLine(3);
            }

            if (!cutscene1Played && story.CurrentStep == 4)
            {
                cutscene1Played = true;
                OnCutsceneStart(1);
                cutscene.Start("Assets/Cutscenes/Cutscene1");
                DisableBarricade(Barricade1Platform, Barricade1);
            }

            if (story.CurrentStep == 5 && !fight1Started)
            {
                fight1Started = true;

                DespawnAllNpcs();
                SpawnFight1Enemies();

                guide.ShowLine(4);

                respawn.SetCheckpoint(9600, 700);
                EnableBarricade(Barricade2Platform, Barricade2);
            }

            if (fight1Started && enemiesKilled >= enemiesSpawned)
            {
                fight1Started = false;
                story.AdvanceStep();
                SpawnAfterFightNpcs();
                guide.ShowLine(5);
            }

            if (story.CurrentStep == 6 && !eldonSuppliesSpawned)
            {
                eldonSuppliesSpawned = true;

                guide.ShowLine(6);

                propManager.SpawnApple(2500, 760);
                propManager.SpawnShield(2700, 720);

            }

            bool everyoneArrived = true;

            foreach (var npc in npcs)
            {
                if (npc.Mode == NpcMode.Waypoint && !npc.ReachedTarget)
                {
                    everyoneArrived = false;
                    break;
                }
            }

            uiAndDebug.UpdateSpeech();
            uiAndDebug.HandleNpcInteraction();

            if (!cutscene2Played && story.CurrentStep == 7 && playerWorldX <= 30)
            {
                cutscene2Played = true;
                OnCutsceneStart(2);
                cutscene.Start("Assets/Cutscenes/Cutscene2");
                DisableBarricade(Barricade2Platform, Barricade2);
            }

            if (story.CurrentStep == 8 && !rooftopChaseInitialized)
            {
                rooftopChaseInitialized = true;
                rooftopChaseStarted = true;
                StartRooftopChase();

                guide.ShowLine(7);
                respawn.SetCheckpoint(900, 300);
            }

            if (rooftopChaseStarted && playerWorldX >= 5000)
            {
                rooftopChaseStarted = false;
                respawn.SetCheckpoint(5000, 700);
                story.AdvanceStep();
            }

            if (story.CurrentStep == 9 && playerWorldX >= 10000)
            {
                respawn.SetCheckpoint(10000, 700);
            }

            if (story.CurrentStep == 9 && playerWorldX >= 13500)
            {
                respawn.SetCheckpoint(13500, 700);
            }

            if (rooftopChaseStarted && enemiesKilled >= enemiesSpawned)
            {
                rooftopChaseStarted = false;

                RemoveChaseEnemies();

                story.AdvanceStep();

                guide.ShowLine(8);
            }

            if (!cutscene3Played && story.CurrentStep == 10 && playerWorldX >= 10500)
            {
                cutscene3Played = true;
                OnCutsceneStart(3);
                cutscene.Start("Assets/Cutscenes/Cutscene3");
            }

            if (story.CurrentStep == 11 && !finishShown)
            {
                finishShown = true;
                ShowFinishScreen();
            }

            guide.Update();
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

        private void Resume_Click(object sender, RoutedEventArgs e) => TogglePause();

        private void ExitToMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenuWindow menu = new MainMenuWindow();
            menu.Show();
            Close();
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Respawn_Click(object sender, RoutedEventArgs e)
        {
            respawn.RespawnPlayer();
        }

        private void InitializeEntities()
        {
            npcs.Add(Npc.Create(GameCanvas, NpcType.Guard1, 6000, 600, 5700, 7000, NpcMode.Wander));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Guard2, 6000, 600, 5700, 7000, NpcMode.Wander));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Eldon, 3500, 600, 200, 700, NpcMode.Stand));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Mira, 6500, 600, 550, 900, NpcMode.Stand));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Dorin, 8900, 600, 800, 1100, NpcMode.Stand));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Noril, 5800, 600, 5700, 8500, NpcMode.Wander));
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

        internal void DespawnAllNpcs()
        {
            foreach (var npc in npcs)
            {
                GameCanvas.Children.Remove(npc.Visual);
            }

            npcs.Clear();
        }

        internal void SpawnFight1Enemies()
        {
            enemies.Clear();

            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Brogur_light, 11000, 700, 10800, 11200, EnemyMode.Wander));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 11500, 700, 11400, 12000, EnemyMode.Wander));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 11600, 700, 11400, 12000, EnemyMode.Wander));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 11700, 700, 11400, 12000, EnemyMode.Wander));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 11800, 700, 11400, 12000, EnemyMode.Wander));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Brogur_light, 13000, 700, 12500, 13200, EnemyMode.Wander));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Brogur_light, 13000, 700, 12500, 13200, EnemyMode.Wander));
            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Brogur_medium, 14700, 700, 14500, 14900, EnemyMode.Wander));

            enemiesSpawned = 8;
            enemiesKilled = 0;
        }

        internal void SpawnAfterFightNpcs()
        {
            var eldon = Npc.Create(GameCanvas, NpcType.Eldon, 13500, 600, 0, 0, NpcMode.Waypoint);
            eldon.TargetX = 14250;

            var guard1 = Npc.Create(GameCanvas, NpcType.Guard1, 13350, 600, 0, 0, NpcMode.Waypoint);
            guard1.TargetX = 14150;

            var guard2 = Npc.Create(GameCanvas, NpcType.Guard2, 13900, 600, 0, 0, NpcMode.Waypoint);
            guard2.TargetX = 14100;

            var guard3 = Npc.Create(GameCanvas, NpcType.Guard3, 13800, 600, 0, 0, NpcMode.Waypoint);
            guard3.TargetX = 14050;

            npcs.Add(eldon);
            npcs.Add(guard1);
            npcs.Add(guard2);
            npcs.Add(guard3);
        }

        internal void OnCutsceneStart(int id)
        {
            if (id == 1)
            {
                DespawnAllNpcs();

                playerWorldX = 9600;
                playerWorldY = 700;

                cameraX = playerWorldX - 300;
            }
            else if (id == 2)
            {
                DespawnAllNpcs();

                playerWorldX = 900;
                playerWorldY = 100;

                cameraX = playerWorldX - 300;
            }
        }

        internal void StartRooftopChase()
        {
            enemies.Clear();
            npcs.Clear();

            var g1 = Enemy.Create(GameCanvas, EnemyType.Grivak, 900, 150, 0, 0, EnemyMode.Waypoint);
            g1.TargetX = 30000;
            g1.AggroOnSight = false;
            g1.CountsForKill = false;
            enemies.Add(g1);

            var g2 = Enemy.Create(GameCanvas, EnemyType.Grivak, 850, 200, 0, 0, EnemyMode.Waypoint);
            g2.TargetX = 30000;
            g2.AggroOnSight = false;
            g2.CountsForKill = false;
            enemies.Add(g2);

            var g3 = Enemy.Create(GameCanvas, EnemyType.Grivak, 900, 250, 0, 0, EnemyMode.Waypoint);
            g3.TargetX = 30000;
            g3.AggroOnSight = false;
            g3.CountsForKill = false;
            enemies.Add(g3);

            var g4 = Enemy.Create(GameCanvas, EnemyType.Grivak, 1000, 150, 0, 0, EnemyMode.Waypoint);
            g4.TargetX = 30000;
            g4.AggroOnSight = false;
            g4.CountsForKill = false;
            enemies.Add(g4);

            var g5 = Enemy.Create(GameCanvas, EnemyType.Grivak, 1000, 200, 0, 0, EnemyMode.Waypoint);
            g5.TargetX = 30000;
            g5.AggroOnSight = false;
            g5.CountsForKill = false;
            enemies.Add(g5);

            var carrier = Enemy.Create(GameCanvas, EnemyType.CarryGrivak, 1050, 200, 0, 0, EnemyMode.Waypoint);
            carrier.TargetX = 30000;
            carrier.IsInvulnerable = true;
            carrier.AggroOnSight = false;
            carrier.AggroRange = 0;
            carrier.ContactDamage = 0;
            carrier.CountsForKill = false;
            enemies.Add(carrier);

            var e1 = Enemy.Create(GameCanvas, EnemyType.Skulk, 6500, 700, 6200, 7000, EnemyMode.Wander);
            var e2 = Enemy.Create(GameCanvas, EnemyType.Skulk, 6500, 700, 6200, 7000, EnemyMode.Wander);
            var e3 = Enemy.Create(GameCanvas, EnemyType.Skulk, 6500, 700, 6200, 7000, EnemyMode.Wander);
            var e4 = Enemy.Create(GameCanvas, EnemyType.Brogur_medium, 6500, 700, 6200, 7000, EnemyMode.Wander);
            var e5 = Enemy.Create(GameCanvas, EnemyType.Skulk, 8500, 700, 8200, 9200, EnemyMode.Wander);
            var e6 = Enemy.Create(GameCanvas, EnemyType.Brogur_light, 8500, 700, 8200, 9200, EnemyMode.Wander);
            var e7 = Enemy.Create(GameCanvas, EnemyType.Brogur_medium, 8500, 700, 8200, 9200, EnemyMode.Wander);
            var e8 = Enemy.Create(GameCanvas, EnemyType.Brogur_light, 8500, 700, 8200, 9200, EnemyMode.Wander);
            var e9 = Enemy.Create(GameCanvas, EnemyType.Brogur_light, 10700, 700, 10500, 12000, EnemyMode.Wander);
            var e10 = Enemy.Create(GameCanvas, EnemyType.Skulk, 10700, 700, 10500, 12000, EnemyMode.Wander);
            var e11 = Enemy.Create(GameCanvas, EnemyType.Brogur_medium, 10700, 700, 10500, 12000, EnemyMode.Wander);
            var e12 = Enemy.Create(GameCanvas, EnemyType.Skulk, 10700, 700, 10500, 12000, EnemyMode.Wander);
            var e13 = Enemy.Create(GameCanvas, EnemyType.Brogur_heavy, 14500, 700, 14300, 15000, EnemyMode.Wander);
            var e14 = Enemy.Create(GameCanvas, EnemyType.Brogur_heavy, 16500, 700, 16000, 17000, EnemyMode.Wander);
            var e15 = Enemy.Create(GameCanvas, EnemyType.Brogur_heavy, 16500, 700, 16000, 17000, EnemyMode.Wander);
            var eldonDead = Npc.Create(GameCanvas, NpcType.Eldon, 17820, 700, 0, 0, NpcMode.Dead);

            enemies.Add(e1);
            enemies.Add(e2);
            enemies.Add(e3);
            enemies.Add(e4);
            enemies.Add(e5);
            enemies.Add(e6);
            enemies.Add(e7);
            enemies.Add(e8);
            enemies.Add(e9);
            enemies.Add(e10);
            enemies.Add(e11);
            enemies.Add(e12);
            enemies.Add(e13);
            enemies.Add(e14);
            enemies.Add(e15);
            npcs.Add(eldonDead);

            enemiesSpawned = 15;
            enemiesKilled = 0;

            propManager.SpawnApple(10000, 760);
            propManager.SpawnShield(10000, 720);

            propManager.SpawnShield(14000, 720);
        }

        internal void RemoveChaseEnemies()
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var e = enemies[i];

                if (!e.CountsForKill)
                {
                    GameCanvas.Children.Remove(e.Visual);
                    GameCanvas.Children.Remove(e.HpBarBackground);
                    GameCanvas.Children.Remove(e.HpBarFill);
                    GameCanvas.Children.Remove(e.AggroAlertIcon);

                    enemies.RemoveAt(i);
                }
            }
        }

        internal void EnableBarricade(Rectangle platform, Image visual)
        {
            visual.Visibility = Visibility.Visible;
            platform.Visibility = Visibility.Visible;

            if (!platforms.Contains(platform))
                platforms.Add(platform);
        }

        internal void DisableBarricade(Rectangle platform, Image visual)
        {
            visual.Visibility = Visibility.Collapsed;
            platform.Visibility = Visibility.Collapsed;

            platforms.Remove(platform);
        }

        private void ShowFinishScreen()
        {
            isPaused = true;
            FinishOverlay.Visibility = Visibility.Visible;
        }
    }
}