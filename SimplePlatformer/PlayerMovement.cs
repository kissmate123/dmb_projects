using System.Windows.Controls;
using System.Windows.Media;

namespace SimplePlatformer
{
    internal sealed class PlayerMovement
    {
        private readonly TestMap map;

        public PlayerMovement(TestMap map)
        {
            this.map = map;
        }

        public void HandleMovement()
        {
            if (map.IsPlayerStunned || map.isChargingBow || map.isMeleeAnimating || map.isShielding)
            {
                map.playerVelocityX = 0;
                return;
            }

            map.playerVelocityX = 0;

            double speed = map.shiftPressed
                ? map.playerRunSpeed
                : map.playerWalkSpeed;

            if (map.leftPressed)
                map.playerVelocityX = -speed;

            if (map.rightPressed)
                map.playerVelocityX = speed;

            if (map.spacePressed && map.isOnGround)
            {
                map.playerVelocityY = -map.jumpForce;
                map.isJumping = true;
                map.isOnGround = false;
            }
        }

        public void UpdatePlayerPosition()
        {
            map.playerWorldX += map.playerVelocityX * map.deltaTime;
            map.playerWorldY += map.playerVelocityY * map.deltaTime;

            if (map.playerWorldX < 0) map.playerWorldX = 0;
            if (map.playerWorldX + map.Player.ActualWidth > map.WorldWidth)
                map.playerWorldX = map.WorldWidth - map.Player.ActualWidth;

            Canvas.SetLeft(map.Player, map.playerWorldX - map.cameraX);
            Canvas.SetTop(map.Player, map.playerWorldY);
        }

        public void UpdatePlayerFacingDirection()
        {
            if (map.playerVelocityX > 0.1 && !map.isFacingRight)
            {
                map.PlayerScaleTransform.ScaleX = 1;
                map.isFacingRight = true;
            }
            else if (map.playerVelocityX < -0.1 && map.isFacingRight)
            {
                map.PlayerScaleTransform.ScaleX = -1;
                map.isFacingRight = false;
            }
        }
    }
}
