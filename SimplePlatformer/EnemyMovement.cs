using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace SimplePlatformer
{
    internal sealed class EnemyMovement
    {
        private readonly TestMap map;
        private static readonly Random Rng = new Random();

        public EnemyMovement(TestMap map)
        {
            this.map = map;
        }

        public void UpdateEnemies()
        {
            foreach (var enemy in map.enemies)
            {
                double distanceToPlayer = Math.Abs(map.playerWorldX - enemy.WorldX);

                if (enemy.DamageCooldownTimer > 0)
                {
                    enemy.DamageCooldownTimer -= map.deltaTime;
                    if (enemy.DamageCooldownTimer < 0)
                        enemy.DamageCooldownTimer = 0;
                }

                if (!enemy.IsAggro && distanceToPlayer <= enemy.AggroRange)
                    EnterAggro(enemy, propagate: true);

                if (enemy.StunTimer > 0)
                {
                    enemy.StunTimer -= map.deltaTime;
                    if (enemy.StunTimer < 0) enemy.StunTimer = 0;
                }

                if (enemy.StunTimer <= 0 && !enemy.IsInAggroAlert)
                {
                    if (enemy.IsAggro)
                    {
                        Rect pr = map.GetPlayerHitboxRect();
                        double playerCenterX = pr.X + pr.Width / 2.0;

                        double enemyCenterX = enemy.WorldX + enemy.Visual.Width / 2.0;

                        double dx = playerCenterX - enemyCenterX;
                        double absDx = Math.Abs(dx);

                        if (absDx > enemy.StopDistance)
                        {
                            enemy.VelocityX = dx < 0 ? -enemy.AggroSpeed : enemy.AggroSpeed;
                        }
                        else
                        {
                            enemy.VelocityX = 0;
                        }
                    }
                    else
                    {
                        UpdateEnemyNonAggro(enemy);
                    }
                }

                if (enemy.IsInAggroAlert)
                {
                    enemy.AggroAlertTimer -= map.deltaTime;

                    enemy.VelocityX = 0;

                    Canvas.SetLeft(enemy.AggroAlertIcon,
                        enemy.WorldX - map.cameraX + enemy.Visual.Width / 2 - 7);
                    Canvas.SetTop(enemy.AggroAlertIcon,
                        enemy.WorldY - 60);

                    enemy.AggroAlertIcon.Visibility = Visibility.Visible;

                    if (enemy.AggroAlertTimer <= 0)
                    {
                        enemy.IsInAggroAlert = false;
                        enemy.AggroAlertTimer = 0;
                        enemy.AggroAlertIcon.Visibility = Visibility.Hidden;
                    }
                }

                if (!enemy.OnGround)
                    enemy.VelocityY += map.gravity * map.deltaTime;

                if (enemy.VelocityY > 800)
                    enemy.VelocityY = 800;

                enemy.WorldX += enemy.VelocityX * map.deltaTime;
                enemy.WorldY += enemy.VelocityY * map.deltaTime;

                enemy.OnGround = false;

                ResolveEnemyPlatform(enemy, map.Ground, map.GroundX);

                Canvas.SetLeft(enemy.Visual, enemy.WorldX - map.cameraX);
                Canvas.SetTop(enemy.Visual, enemy.WorldY);

                if (enemy.HitFlashTimer > 0)
                {
                    enemy.HitFlashTimer -= map.deltaTime;
                    if (enemy.HitFlashTimer <= 0)
                    {
                        enemy.HitFlashTimer = 0;
                        enemy.Visual.Opacity = enemy.NormalOpacity;
                    }
                }

                if (enemy.StunTimer <= 0)
                {
                    UpdateEnemyFacing(enemy);
                }

                enemy.UpdateHpBar(map.cameraX);
            }
        }

        private void UpdateEnemyNonAggro(Enemy enemy)
        {
            if (enemy.Mode == EnemyMode.Stand)
            {
                enemy.VelocityX = 0;
                enemy.IsWaiting = false;
                enemy.PatrolWaitTimer = 0;
                enemy.WanderTimer = 0;
                return;
            }

            if (enemy.Mode == EnemyMode.Patrol)
            {
                if (enemy.IsWaiting)
                {
                    enemy.VelocityX = 0;
                    enemy.PatrolWaitTimer -= map.deltaTime;
                    
                    if (enemy.PatrolWaitTimer <= 0)
                    {
                        enemy.IsWaiting = false;
                        enemy.PatrolWaitTimer = 0;

                        if (enemy.MoveDir == 0) enemy.MoveDir = 1;
                        enemy.VelocityX = enemy.MoveDir * enemy.WalkSpeed;
                    }
                    return;
                }

                enemy.VelocityX = enemy.MoveDir * enemy.WalkSpeed;

                if (enemy.WorldX <= enemy.LeftLimit)
                {
                    enemy.WorldX = enemy.LeftLimit;
                    enemy.MoveDir = 1;
                    StartPatrolWait(enemy);
                    return;
                }

                if (enemy.WorldX >= enemy.RightLimit)
                {
                    enemy.WorldX = enemy.RightLimit;
                    enemy.MoveDir = -1;
                    StartPatrolWait(enemy);
                    return;
                }

                return;
            }

            if (enemy.Mode == EnemyMode.Wander)
            {
                if (enemy.IsWaiting)
                {
                    enemy.VelocityX = 0;
                    enemy.WanderTimer -= map.deltaTime;

                    if (enemy.WanderTimer <= 0)
                    {
                        enemy.IsWaiting = false;

                        enemy.MoveDir = Rng.Next(0, 2) == 0 ? -1 : 1;

                        if (enemy.WorldX <= enemy.LeftLimit + 1) enemy.MoveDir = 1;
                        if (enemy.WorldX >= enemy.RightLimit - 1) enemy.MoveDir = -1;

                        enemy.WanderTimer = NextDouble(3, 8);
                        enemy.VelocityX = enemy.MoveDir * enemy.WalkSpeed;
                    }

                    return;
                }

                enemy.VelocityX = enemy.MoveDir * enemy.WalkSpeed;
                enemy.WanderTimer -= map.deltaTime;

                if (enemy.WorldX <= enemy.LeftLimit ||
                    enemy.WorldX >= enemy.RightLimit ||
                    enemy.WanderTimer <= 0)
                {
                    enemy.WorldX = Math.Clamp(enemy.WorldX, enemy.LeftLimit, enemy.RightLimit);
                    StartWanderWait(enemy);
                }

                return;
            }
        }

        private void StartPatrolWait(Enemy enemy)
        {
            enemy.IsWaiting = true;
            enemy.PatrolWaitTimer = enemy.PatrolWaitTime;
            enemy.VelocityX = 0;
        }

        private void StartWanderWait(Enemy enemy)
        {
            enemy.IsWaiting = true;
            enemy.WanderTimer = NextDouble(3, 5);
            enemy.VelocityX = 0;
        }

        private double NextDouble(double min, double max)
        {
            return min + (Rng.NextDouble() * (max - min));
        }

        public void ResolveEnemyPlatform(Enemy enemy, Rectangle platform, double platformWorldX)
        {
            Rect enemyRect = new Rect(enemy.WorldX, enemy.WorldY, enemy.Visual.Width, enemy.Visual.Height);
            Rect platformRect = new Rect(platformWorldX, Canvas.GetTop(platform), platform.Width, platform.Height);

            if (enemy.VelocityY >= 0 &&
                enemyRect.Bottom >= platformRect.Top &&
                enemyRect.Top < platformRect.Top &&
                enemyRect.Right > platformRect.Left &&
                enemyRect.Left < platformRect.Right)
            {
                enemy.WorldY = platformRect.Top - enemyRect.Height;
                enemy.VelocityY = 0;
                enemy.OnGround = true;
                return;
            }

            if (enemy.VelocityX > 0 &&
                enemyRect.Right >= platformRect.Left &&
                enemyRect.Left < platformRect.Left &&
                enemyRect.Bottom > platformRect.Top &&
                enemyRect.Top < platformRect.Bottom)
            {
                enemy.WorldX = platformRect.Left - enemyRect.Width;
                enemy.VelocityX = -enemy.WalkSpeed;
                return;
            }

            if (enemy.VelocityX < 0 &&
                enemyRect.Left <= platformRect.Right &&
                enemyRect.Right > platformRect.Right &&
                enemyRect.Bottom > platformRect.Top &&
                enemyRect.Top < platformRect.Bottom)
            {
                enemy.WorldX = platformRect.Right;
                enemy.VelocityX = enemy.WalkSpeed;
                return;
            }
        }

        private void EnterAggro(Enemy enemy, bool propagate)
        {
            if (enemy.IsAggro)
                return;

            enemy.IsAggro = true;

            enemy.IsInAggroAlert = true;
            enemy.AggroAlertTimer = Enemy.AggroAlertDuration;

            enemy.IsWaiting = false;
            enemy.PatrolWaitTimer = 0;
            enemy.WanderTimer = 0;

            enemy.VelocityX = 0;

            if (!propagate)
                return;

            if (enemy.AggroShareRadius <= 0)
                return;

            double r = enemy.AggroShareRadius;

            foreach (var other in map.enemies)
            {
                if (ReferenceEquals(other, enemy))
                    continue;

                if (other.IsAggro)
                    continue;

                double dx = Math.Abs(other.WorldX - enemy.WorldX);

                if (dx <= r)
                {
                    EnterAggro(other, propagate: false);
                }
            }
        }


        private void UpdateEnemyFacing(Enemy enemy)
        {
            if (enemy.VelocityX > 1)
            {
                enemy.Scale.ScaleX = 1;
            }
            else if (enemy.VelocityX < -1)
            {
                enemy.Scale.ScaleX = -1;
            }
        }
    }
}