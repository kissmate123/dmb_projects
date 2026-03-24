using System;
using System.Collections.Generic;
using System.Windows;

namespace SimplePlatformer
{
    internal sealed class UIAndDebug
    {
        private readonly TestMap map;

        public UIAndDebug(TestMap map)
        {
            this.map = map;
        }

        public void UpdateHpBar()
        {
            double hpPercent = map.currentHp / map.maxHp;
            hpPercent = Math.Max(0, Math.Min(1, hpPercent));

            double maxWidth = 320;
            map.HpBarFill.Width = maxWidth * hpPercent;
        }

        public void UpdateShieldBar()
        {
            double percent = map.currentShield / TestMap.maxShield;
            if (percent < 0) percent = 0;
            if (percent > 1) percent = 1;

            map.ShieldBarFill.Width = 320 * percent;
        }

        public void UpdateFPS()
        {
            map.frameCount++;

            if ((DateTime.Now - map.lastFpsUpdate).TotalSeconds >= 0.5)
            {
                map.fps = map.frameCount / (DateTime.Now - map.lastFpsUpdate).TotalSeconds;
                map.frameCount = 0;
                map.lastFpsUpdate = DateTime.Now;
            }
        }

        public void UpdateDebugInfo()
        {
            List<string> debugInfo = map.debugInfo;

            debugInfo.Add($"FPS: {map.fps:F1}");
            debugInfo.Add($"Position: ({System.Windows.Controls.Canvas.GetLeft(map.Player):F0}, {System.Windows.Controls.Canvas.GetTop(map.Player):F0})");
            debugInfo.Add($"On Ground: {map.isOnGround}");
            debugInfo.Add($"Jumping: {map.isJumping}");
            debugInfo.Add($"Facing: {(map.isFacingRight ? "Right" : "Left")}");
            debugInfo.Add($"Keys: A/D={map.leftPressed}/{map.rightPressed}, Space={map.spacePressed}");

            map.DebugText.Text = string.Join("\n", debugInfo);
        }

        public void ToggleDebugVisibility()
        {
            map.DebugText.Visibility = map.DebugText.Visibility == Visibility.Visible
                ? Visibility.Hidden
                : Visibility.Visible;
        }
    }
}
