using content_management_system.Helpers;
using content_management_system.Models;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace content_management_system.Pages
{
    public partial class TablePage : Page
    {
        public ObservableCollection<Obrenovic> Obrenovici { get; set; }
        
        private MainWindow mainWindow;

        private User currentUser { get; set; }

        public TablePage(User user)
        {
            InitializeComponent();

            currentUser = user;

            if (user.Role == UserRole.Admin)
            {
                btn_Add.Visibility = Visibility.Visible;
                btn_Delete.Visibility = Visibility.Visible;
            }
            else
            {
                SelectColumn.Visibility = Visibility.Collapsed;
            }

            mainWindow = (MainWindow)Application.Current.MainWindow;
            Obrenovici = mainWindow.Obrenovici;
            this.DataContext = this;
        }

        public void SaveDataToXml()
        {
            DataIO io = new DataIO();
            string xmlPath = "../../../Data/Obrenovici.xml";
            io.SerializeObject(Obrenovici.ToList(), xmlPath);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addPage = new AddEditObrenovicPage(this);
            mainWindow.MainFrame.Navigate(addPage);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var itemsToDelete = Obrenovici.Where(o => o.IsSelected).ToList();
            
            if (itemsToDelete.Count == 0)
            {
                mainWindow.ShowToastNotification(new ToastNotification("Warning", "No members of the dynasty were selected for deletion.", NotificationType.Warning));
                return;
            }

            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {itemsToDelete.Count} selected dynasty members?", "Confirm deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var item in itemsToDelete)
                {
                    Obrenovici.Remove(item);
                }

                SaveDataToXml();
                ObrenoviciDataGrid.Items.Refresh();
                mainWindow.ShowToastNotification(new ToastNotification("Success", "Successfully deleted selected members.", NotificationType.Success));
            }
        }

        private void NameHyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hyperlink && hyperlink.DataContext is Obrenovic obrenovic)
            {
                var navService = mainWindow?.MainFrame.NavigationService;

                if (navService == null) 
                    return;

                if (currentUser.Role == UserRole.Admin)
                {
                    var editPage = new AddEditObrenovicPage(this, obrenovic);
                    navService.Navigate(editPage);
                }
                else
                {
                    var previewPage = new PreviewPage(obrenovic, navService);
                    navService.Navigate(previewPage);
                }
            }
        }

        private void RowCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is Obrenovic obrenovic)
            {
                obrenovic.IsSelected = checkBox.IsChecked == true;
            }
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (SelectAllCheckBox.IsChecked == true)
            {
                foreach (var obrenovic in Obrenovici)
                {
                    obrenovic.IsSelected = true;
                }
            }
            else
            {
                foreach (var obrenovic in Obrenovici)
                {
                    obrenovic.IsSelected = false;
                }
            }

            ObrenoviciDataGrid.Items.Refresh();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Logout();
            mainWindow.ShowToastNotification(new ToastNotification("Success", "Successfully logged out.", NotificationType.Success));

        }
    }
}
