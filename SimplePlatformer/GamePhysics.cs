using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace SimplePlatformer
{
    internal sealed class GamePhysics
    {
        private readonly TestMap map;

        public GamePhysics(TestMap map)
        {
            this.map = map;
        }

        public void ApplyPhysics()
        {
            if (!map.isOnGround)
            {
                map.playerVelocityY += map.gravity * map.deltaTime;
            }

            if (map.playerVelocityY > 800) map.playerVelocityY = 800;
        }

        public void CheckCollisions()
        {
            map.isOnGround = false;

            Rect playerRect = map.GetPlayerHitboxRect();
            double playerWidth = playerRect.Width;
            double playerHeight = playerRect.Height;

            if (playerRect.Bottom >= map.GameCanvas.ActualHeight)
            {
                map.playerWorldY = map.GameCanvas.ActualHeight - playerHeight;
                map.playerVelocityY = 0;
                map.isOnGround = true;
                return;
            }

            ResolvePlatformCollision(map.Ground, map.GroundX);
            ResolvePlatformCollision(map.HousePlatform1, map.HousePlatform1X);
            ResolvePlatformCollision(map.HousePlatform2, map.HousePlatform2X);
            ResolvePlatformCollision(map.HousePlatform3, map.HousePlatform3X);
            ResolvePlatformCollision(map.HousePlatform4, map.HousePlatform4X);
            ResolvePlatformCollision(map.HousePlatform5, map.HousePlatform5X);
            ResolvePlatformCollision(map.HousePlatform6, map.HousePlatform6X);
            ResolvePlatformCollision(map.HousePlatform7, map.HousePlatform7X);
            ResolvePlatformCollision(map.HousePlatform8, map.HousePlatform8X);
        }

        public void ResolvePlatformCollision(Rectangle platform, double platformWorldX)
        {
            Rect curr = map.GetPlayerHitboxRect();

            Rect plat = new Rect(
                platformWorldX,
                Canvas.GetTop(platform),
                platform.Width,
                platform.Height
            );

            double dx = map.playerVelocityX * map.deltaTime;
            double dy = map.playerVelocityY * map.deltaTime;

            Rect next = new Rect(
                curr.X + dx,
                curr.Y + dy,
                curr.Width,
                curr.Height
            );

            bool horizOverlapCurr = curr.Right > plat.Left && curr.Left < plat.Right;
            bool horizOverlapNext = next.Right > plat.Left && next.Left < plat.Right;

            bool vertOverlapNext = next.Bottom > plat.Top && next.Top < plat.Bottom;

            if (dx > 0 && vertOverlapNext && curr.Right <= plat.Left && next.Right >= plat.Left)
            {
                double hitboxLeftTarget = plat.Left - curr.Width;
                map.playerWorldX = hitboxLeftTarget - TestMap.PlayerHitboxOffsetX;
                map.playerVelocityX = 0;
                return;
            }

            if (dx < 0 && vertOverlapNext && curr.Left >= plat.Right && next.Left <= plat.Right)
            {
                double hitboxLeftTarget = plat.Right;
                map.playerWorldX = hitboxLeftTarget - TestMap.PlayerHitboxOffsetX;
                map.playerVelocityX = 0;
                return;
            }

            if (dy > 0 && horizOverlapNext && curr.Bottom <= plat.Top && next.Bottom >= plat.Top)
            {
                map.playerWorldY = plat.Top - curr.Height;
                map.playerVelocityY = 0;
                map.isOnGround = true;
                return;
            }

            if (dy < 0 && horizOverlapNext && curr.Top >= plat.Bottom && next.Top <= plat.Bottom)
            {
                map.playerWorldY = plat.Bottom;
                map.playerVelocityY = 0;
                return;
            }

            const double groundEpsilon = 6.0;

            if (dy == 0 && horizOverlapCurr &&
                curr.Bottom >= plat.Top && curr.Bottom <= plat.Top + groundEpsilon)
            {
                map.playerWorldY = plat.Top - curr.Height;
                map.playerVelocityY = 0;
                map.isOnGround = true;
                return;
            }
        }

        public void UpdateCamera()
        {
            double screenX = map.playerWorldX - map.cameraX;

            if (screenX > map.screenRightLimit)
            {
                map.cameraX = map.playerWorldX - map.screenRightLimit;
            }
            else if (screenX < map.screenLeftLimit)
            {
                map.cameraX = map.playerWorldX - map.screenLeftLimit;
            }

            if (map.cameraX < 0)
                map.cameraX = 0;

            if (map.cameraX > map.WorldWidth - map.GameCanvas.ActualWidth)
                map.cameraX = map.WorldWidth - map.GameCanvas.ActualWidth;

            Canvas.SetLeft(map.Ground, map.GroundX - map.cameraX);
            Canvas.SetLeft(map.HousePlatform1, map.HousePlatform1X - map.cameraX);
            Canvas.SetLeft(map.HousePlatform2, map.HousePlatform2X - map.cameraX);
            Canvas.SetLeft(map.HousePlatform3, map.HousePlatform3X - map.cameraX);
            Canvas.SetLeft(map.HousePlatform4, map.HousePlatform4X - map.cameraX);
            Canvas.SetLeft(map.HousePlatform5, map.HousePlatform5X - map.cameraX);
            Canvas.SetLeft(map.HousePlatform6, map.HousePlatform6X - map.cameraX);
            Canvas.SetLeft(map.HousePlatform7, map.HousePlatform7X - map.cameraX);
            Canvas.SetLeft(map.HousePlatform8, map.HousePlatform8X - map.cameraX);
            Canvas.SetLeft(map.Tree1, 4000 - map.cameraX);

            Canvas.SetLeft(map.House1, 100 - map.cameraX);
            Canvas.SetLeft(map.House2, 500 - map.cameraX);
            Canvas.SetLeft(map.House3, 900 - map.cameraX);
            Canvas.SetLeft(map.House4, 1300 - map.cameraX);
            Canvas.SetLeft(map.House5, 1700 - map.cameraX);
            Canvas.SetLeft(map.House6, 2100 - map.cameraX);
            Canvas.SetLeft(map.House7, 2500 - map.cameraX);
            Canvas.SetLeft(map.House8, 2900 - map.cameraX);

            Canvas.SetLeft(map.HouseBack1, 300 - map.cameraX);
            Canvas.SetLeft(map.HouseBack2, 650 - map.cameraX);
            Canvas.SetLeft(map.HouseBack3, 1170 - map.cameraX);
            Canvas.SetLeft(map.HouseBack4, 1500 - map.cameraX);
            Canvas.SetLeft(map.HouseBack5, 1950 - map.cameraX);
            Canvas.SetLeft(map.HouseBack6, 2370 - map.cameraX);
            Canvas.SetLeft(map.HouseBack7, 2700 - map.cameraX);

            Canvas.SetLeft(map.MarketStall1, 3500 - map.cameraX);
            Canvas.SetLeft(map.MarketStall2, 3820 - map.cameraX);
            Canvas.SetLeft(map.MarketStall3, 4140 - map.cameraX);
            Canvas.SetLeft(map.MarketStall4, 4460 - map.cameraX);
        }

        public Rect GetPlatformRect(Rectangle platform, double worldX)
        {
            return new Rect(
                worldX,
                Canvas.GetTop(platform),
                platform.Width,
                platform.Height
            );
        }
    }
}
