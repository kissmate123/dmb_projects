using System.Windows;

namespace dmb_adminwpf
{
    public partial class DataStore : Window
    {
        private string command;

        public DataStore(dmb_adminwpf.Models.Data data, string command)
        {
            InitializeComponent();
            this.command = command;

            tbxUN.Text = data.UserName;
            tbxEmail.Text = data.Email;
            tbxPW.Text = data.PasswordHash;

            this.Title = "Felhasználó módosítása";
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            var main = this.Owner as MainWindow;
            if (main == null) return;

            var resultData = new dmb_adminwpf.Models.Data
            {
                UserName = tbxUN.Text,
                Email = tbxEmail.Text,
                PasswordHash = tbxPW.Text
            };

            // Csak akkor hívjuk meg a módosítást, ha a parancs "Modify"
            if (command == "Modify")
            {
                if (main.dtgAdatok.SelectedItem is dmb_adminwpf.Models.Data selected)
                {
                    resultData.Id = selected.Id;
                    main.ModifyData(resultData);
                }
            }

            this.Close();
        }
    }
}