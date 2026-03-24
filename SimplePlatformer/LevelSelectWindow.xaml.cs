using System.Windows;

namespace SimplePlatformer
{
    public partial class LevelSelectWindow : Window
    {
        public LevelSelectWindow()
        {
            InitializeComponent();
        }

        private void Level1_Click(object sender, RoutedEventArgs e)
        {
            MainWindow gameWindow = new MainWindow();
            gameWindow.Show();

            this.Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainMenuWindow menu = new MainMenuWindow();
            menu.Show();

            this.Close();
        }
        private void TestMap_Click(object sender, RoutedEventArgs e)
        {
            TestMap testMap = new TestMap();
            testMap.Show();

            this.Close();
        }

    }
}
