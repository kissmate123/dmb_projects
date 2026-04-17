using System.Windows;
using System.Windows.Input;

namespace ThePixelRealms
{
    public class Input
    {
        public IGameState map;

        public Input(IGameState map)
        {
            this.map = map;
        }

        public bool EPressed = false;


        public void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (map.isDead)
                return;

            if (map.isPaused && !map.cutscene.IsActive && e.Key != Key.Escape)
                return;

            switch (e.Key)
            {
                case Key.G:
                    map.bowKeyPressed = true;
                    break;

                case Key.E:
                    EPressed = true;
                    break;

                case Key.V:
                    if (map.currentShield > 0)
                        map.isShielding = true;
                    break;

                case Key.LeftShift:
                case Key.RightShift:
                    map.shiftPressed = true;
                    break;

                case Key.H:
                    map.currentHp -= 10;
                    if (map.currentHp < 0)
                        map.currentHp = 0;
                    break;

                case Key.A:
                case Key.Left:
                    map.leftPressed = true;
                    break;

                case Key.D:
                case Key.Right:
                    map.rightPressed = true;
                    break;

                case Key.Space:
                case Key.Up:
                    map.spacePressed = true;
                    break;

                case Key.Escape:
                    map.TogglePause();
                    break;

                case Key.F:
                    map.attackPressed = true;
                    break;

                case Key.F1:
                    map.DebugText.Visibility = map.DebugText.Visibility == Visibility.Visible
                        ? Visibility.Hidden
                        : Visibility.Visible;
                    break;
            }

            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        public void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.G:
                    map.bowKeyPressed = false;
                    map.ReleaseArrow();
                    break;

                case Key.V:
                    map.isShielding = false;

                    if (map.idleFrames != null && map.idleFrames.Length > 0)
                    {
                        map.idleFrameIndex = 0;
                        map.idleFrameTimer = 0;
                        map.Player.Source = map.idleFrames[0];
                    }
                    break;

                case Key.LeftShift:
                case Key.RightShift:
                    map.shiftPressed = false;
                    break;

                case Key.A:
                case Key.Left:
                    map.leftPressed = false;
                    break;

                case Key.D:
                case Key.Right:
                    map.rightPressed = false;
                    break;

                case Key.Space:
                case Key.Up:
                    map.spacePressed = false;
                    break;

                case Key.F:
                    map.attackPressed = false;
                    break;

                case Key.E:
                    EPressed = false;
                    break;
            }
        }
    }
}