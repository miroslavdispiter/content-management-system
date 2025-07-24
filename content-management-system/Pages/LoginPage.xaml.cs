using content_management_system.Helpers;
using content_management_system.Models;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace content_management_system.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private List<User> _users;

        private MainWindow mainWindow;

        public LoginPage()
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
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

            bool isValid = true;

            UsernameError.Text = "";
            PasswordError.Text = "";

            if (string.IsNullOrWhiteSpace(username))
            {
                UsernameError.Text = "Username is required.";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordError.Text = "Password is required.";
                isValid = false;
            }

            if (!isValid)
                return;

            var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                PasswordError.Text = "Invalid username or password.";
                return;
            }

            mainWindow.ShowToastNotification(new ToastNotification("Success", $"Welcome, {user.Username}!", NotificationType.Success));
            NavigationService?.Navigate(new TablePage(user));
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
