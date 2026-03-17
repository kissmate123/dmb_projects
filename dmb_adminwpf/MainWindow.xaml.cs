using dmb_adminwpf.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace dmb_adminwpf
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<dmb_adminwpf.Models.Data> users = new ObservableCollection<dmb_adminwpf.Models.Data>();
        private readonly string _connStr = "Server=localhost;Port=3306;Database=dmb;User Id=root;Password=;";

        public MainWindow()
        {
            InitializeComponent();
            dtgAdatok.ItemsSource = users;
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            txtStatusz.Text = "Betöltés...";
            try
            {
                users.Clear();
                using var conn = new MySqlConnection(_connStr);
                await conn.OpenAsync();

                string sql = "SELECT Id, UserName, Email, PasswordHash FROM aspnetusers ORDER BY UserName;";
                using var cmd = new MySqlCommand(sql, conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(new dmb_adminwpf.Models.Data
                    {
                        Id = reader["Id"]?.ToString() ?? "",
                        UserName = reader["UserName"]?.ToString() ?? "",
                        Email = reader["Email"]?.ToString() ?? "",
                        PasswordHash = reader["PasswordHash"]?.ToString() ?? ""
                    });
                }
                txtStatusz.Text = $"Betöltve: {users.Count} felhasználó";
            }
            catch (Exception ex) { MessageBox.Show("Hiba az adatok betöltésekor: " + ex.Message); }
        }

        public async void ModifyData(dmb_adminwpf.Models.Data modifiedData)
        {
            int index = dtgAdatok.SelectedIndex;
            if (index < 0) return;

            try
            {
                using var conn = new MySqlConnection(_connStr);
                await conn.OpenAsync();
                string sql = "UPDATE aspnetusers SET UserName = @name, Email = @email, PasswordHash = @pass WHERE Id = @id";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", modifiedData.UserName);
                cmd.Parameters.AddWithValue("@email", modifiedData.Email);
                cmd.Parameters.AddWithValue("@pass", modifiedData.PasswordHash);
                cmd.Parameters.AddWithValue("@id", modifiedData.Id);

                await cmd.ExecuteNonQueryAsync();
                users[index] = modifiedData;
                txtStatusz.Text = "Sikeres módosítás.";
            }
            catch (Exception ex) { MessageBox.Show("Sikertelen módosítás: " + ex.Message); }
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (dtgAdatok.SelectedItem is dmb_adminwpf.Models.Data selected)
            {
                DataStore ds = new DataStore(selected, "Modify");
                ds.Owner = this;
                ds.ShowDialog();
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dtgAdatok.SelectedItem is not dmb_adminwpf.Models.Data selected) return;

            if (MessageBox.Show("Biztosan törlöd?", "Törlés", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using var conn = new MySqlConnection(_connStr);
                    await conn.OpenAsync();
                    string sql = "DELETE FROM aspnetusers WHERE Id = @id";
                    using var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", selected.Id);
                    await cmd.ExecuteNonQueryAsync();
                    users.Remove(selected);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}