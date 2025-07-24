using content_management_system.Helpers;
using content_management_system.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace content_management_system.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private List<User> _users;
        public LoginPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            string xmlPath = "../../../Data/Users.xml";

            DataIO io = new DataIO();
            _users = io.DeSerializeObject<List<User>>(xmlPath);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Unesite i korisničko ime i lozinku.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                MessageBox.Show("Pogrešno korisničko ime ili lozinka.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavigationService?.Navigate(new TablePage(user));
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
