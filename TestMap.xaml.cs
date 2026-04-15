using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ThePixelRealms;

namespace ThePixelRealms
{
    public partial class TestMap : Window
    {
        internal Input input;
        private PlayerMovement playerMovement;
        private GamePhysics gamePhysics;
        internal UIAndDebug uiAndDebug;
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

            propManager = new PropManager(this);


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

            dialogueSystem = new DialogueSystem(this);
            cutscene = new CutsceneSystem(this);

            currentHp = maxHp / 2;
            uiAndDebug.UpdateHpBar();
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

            if (story.CurrentStep == 2 && !miraSuppliesSpawned)
            {
                miraSuppliesSpawned = true;

                propManager.SpawnApple(800, 760);
            }

            if (story.CurrentStep == 3 && !dorinSuppliesSpawned)
            {
                dorinSuppliesSpawned = true;

                propManager.SpawnShield(1000, 760);
            }

            if (!cutscene1Played && story.CurrentStep == 4)
            {
                cutscene1Played = true;
                OnCutsceneStart(1);
                cutscene.Start("Assets/Cutscenes/Cutscene1");
            }

            if (story.CurrentStep == 5 && !fight1Started)
            {
                fight1Started = true;

                DespawnAllNpcs();
                SpawnFight1Enemies();
            }

            if (fight1Started && enemiesKilled >= enemiesSpawned)
            {
                fight1Started = false;
                story.AdvanceStep();

                SpawnAfterFightNpcs();
            }

            if (story.CurrentStep == 6 && !eldonSuppliesSpawned)
            {
                eldonSuppliesSpawned = true;

                propManager.SpawnApple(3500, 760);
                propManager.SpawnShield(3800, 760);
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
            }

            if (story.CurrentStep == 8 && !rooftopChaseInitialized)
            {
                rooftopChaseInitialized = true;
                rooftopChaseStarted = true;
                StartRooftopChase();
            }

            if (rooftopChaseStarted && playerWorldX >= 5500)
            {
                rooftopChaseStarted = false;
                story.AdvanceStep();
            }

            if (rooftopChaseStarted && enemiesKilled >= enemiesSpawned)
            {
                rooftopChaseStarted = false;

                RemoveChaseEnemies();

                story.AdvanceStep();
            }

            if (!cutscene3Played && story.CurrentStep == 10 && playerWorldX >= 6500)
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
            //enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 3000, 300, 2800, 3500, EnemyMode.Patrol));
            //enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 2850, 300, 2800, 3000, EnemyMode.Patrol));
            //enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 3000, 300, 2900, 3200, EnemyMode.Patrol));
            //enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 3050, 300, 3000, 3600, EnemyMode.Patrol));
            //enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 2900, 300, 2850, 3250, EnemyMode.Patrol));
            //enemies.Add(Enemy.Create(GameCanvas, EnemyType.Brogur_light, 5000, 300, 4800, 5500, EnemyMode.Patrol));
            //enemies.Add(Enemy.Create(GameCanvas, EnemyType.Brogur_medium, 6000, 300, 5800, 6500, EnemyMode.Stand));
            //enemies.Add(Enemy.Create(GameCanvas, EnemyType.Brogur_heavy, 7500, 300, 7000, 8000, EnemyMode.Stand));
            //enemies.Add(Enemy.Create(GameCanvas, EnemyType.Grivak, 1000, 150, 800, 1800, EnemyMode.Patrol));

            npcs.Add(Npc.Create(GameCanvas, NpcType.Guard1, 300, 600, 200, 600, NpcMode.Wander));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Guard3, 400, 600, 100, 500, NpcMode.Wander));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Eldon, 600, 600, 200, 700, NpcMode.Stand));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Mira, 800, 600, 550, 900, NpcMode.Stand));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Dorin, 1000, 600, 800, 1100, NpcMode.Stand));
            npcs.Add(Npc.Create(GameCanvas, NpcType.Noril, 1200, 600, 800, 1300, NpcMode.Stand));
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

            enemies.Add(Enemy.Create(GameCanvas, EnemyType.Skulk, 4000, 700, 3800, 4200, EnemyMode.Wander));

            enemiesSpawned = 1;
            enemiesKilled = 0;
        }

        internal void SpawnAfterFightNpcs()
        {
            var eldon = Npc.Create(GameCanvas, NpcType.Eldon, 3000, 600, 0, 0, NpcMode.Waypoint);
            eldon.TargetX = 4000;

            var guard1 = Npc.Create(GameCanvas, NpcType.Guard1, 2850, 600, 0, 0, NpcMode.Waypoint);
            guard1.TargetX = 3900;

            var guard2 = Npc.Create(GameCanvas, NpcType.Guard2, 2780, 600, 0, 0, NpcMode.Waypoint);
            guard2.TargetX = 3850;

            var guard3 = Npc.Create(GameCanvas, NpcType.Guard3, 2700, 600, 0, 0, NpcMode.Waypoint);
            guard3.TargetX = 3800;

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

                playerWorldX = 3000;
                playerWorldY = 700;

                cameraX = playerWorldX - 300;
            }
            else if (id == 2)
            {
                DespawnAllNpcs();

                playerWorldX = 100;
                playerWorldY = 300;

                cameraX = playerWorldX - 300;
            }
        }

        internal void StartRooftopChase()
        {
            enemies.Clear();
            npcs.Clear();

            var g1 = Enemy.Create(GameCanvas, EnemyType.Grivak, 500, 150, 0, 0, EnemyMode.Waypoint);
            g1.TargetX = 20500;
            g1.AggroOnSight = false;
            g1.CountsForKill = false;
            enemies.Add(g1);

            var g2 = Enemy.Create(GameCanvas, EnemyType.Grivak, 450, 200, 0, 0, EnemyMode.Waypoint);
            g2.TargetX = 20450;
            g2.AggroOnSight = false;
            g2.CountsForKill = false;
            enemies.Add(g2);

            var g3 = Enemy.Create(GameCanvas, EnemyType.Grivak, 500, 250, 0, 0, EnemyMode.Waypoint);
            g3.TargetX = 20500;
            g3.AggroOnSight = false;
            g3.CountsForKill = false;
            enemies.Add(g3);

            var g4 = Enemy.Create(GameCanvas, EnemyType.Grivak, 600, 150, 0, 0, EnemyMode.Waypoint);
            g4.TargetX = 20600;
            g4.AggroOnSight = false;
            g4.CountsForKill = false;
            enemies.Add(g4);

            var g5 = Enemy.Create(GameCanvas, EnemyType.Grivak, 600, 200, 0, 0, EnemyMode.Waypoint);
            g5.TargetX = 20600;
            g5.AggroOnSight = false;
            g5.CountsForKill = false;
            enemies.Add(g5);

            var carrier = Enemy.Create(GameCanvas, EnemyType.CarryGrivak, 650, 200, 0, 0, EnemyMode.Waypoint);
            carrier.TargetX = 20650;
            carrier.IsInvulnerable = true;
            carrier.AggroOnSight = false;
            carrier.AggroRange = 0;
            carrier.ContactDamage = 0;
            carrier.CountsForKill = false;
            enemies.Add(carrier);

            var e1 = Enemy.Create(GameCanvas, EnemyType.Skulk, 5000, 700, 4800, 5200, EnemyMode.Wander);
            var eldonDead = Npc.Create(GameCanvas, NpcType.Eldon, 6000, 700, 0, 0, NpcMode.Dead);

            enemies.Add(e1);
            npcs.Add(eldonDead);

            enemiesSpawned = 1;
            enemiesKilled = 0;

            propManager.SpawnApple(5200, 720);

            propManager.SpawnApple(6000, 720);
            propManager.SpawnShield(6200, 720);
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

        private void ShowFinishScreen()
        {
            isPaused = true;
            FinishOverlay.Visibility = Visibility.Visible;
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
        internal double attackRange = 100;
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

        internal double screenLeftLimit = 400;
        internal double screenRightLimit = 500;

        internal const double PlayerHitboxWidth = 48;
        internal const double PlayerHitboxHeight = 60;
        internal const double PlayerHitboxOffsetX = 30;

        internal bool isShielding = false;
        internal BitmapImage shieldSprite;

        public List<Npc> npcs = new List<Npc>();

        private NpcMovement npcMovement;
        private NpcCombat npcCombat;

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

        internal DialogueSystem dialogueSystem;

        internal double shieldWorldX = 1500;
        internal double shieldWorldY = 745;

        internal const double maxShield = 100;
        internal double currentShield = 0;

        internal double appleWorldX = 1700;
        internal double appleWorldY = 760;

        internal double appleHealAmount = 35;
        private PropManager propManager;

        internal Dictionary<int, NpcType> talkSequence = new Dictionary<int, NpcType>
        {
            { 0, NpcType.Eldon },
            { 1, NpcType.Mira },
            { 2, NpcType.Dorin },
            { 3, NpcType.Noril },
            { 6, NpcType.Eldon },
            { 9, NpcType.Eldon }
        };

        internal StoryManager story;

        internal CutsceneSystem cutscene;

        internal bool fight1Started = false;

        internal int enemiesSpawned = 0;
        internal int enemiesKilled = 0;
        bool cutscene1Played = false;
        bool cutscene2Played = false;
        bool cutscene3Played = false;
        internal bool rooftopChaseStarted = false;
        internal bool fight2Started = false;
        internal bool fight2Finished = false;
        bool rooftopChaseInitialized = false;
        bool finishShown = false;

        bool miraSuppliesSpawned = false;
        bool dorinSuppliesSpawned = false;
        bool eldonSuppliesSpawned = false;
    }
}