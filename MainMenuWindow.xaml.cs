using System.Windows;

namespace ThePixelRealms
{
    public partial class MainMenuWindow : Window
    {
        public MainMenuWindow()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            LevelSelectWindow levelSelect = new LevelSelectWindow();
            levelSelect.Show();

            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
