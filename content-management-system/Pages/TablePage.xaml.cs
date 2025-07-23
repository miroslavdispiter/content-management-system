using content_management_system.Helpers;
using content_management_system.Models;
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
            mainWindow = (MainWindow)Application.Current.MainWindow;
            Obrenovici = mainWindow.Obrenovici;
            this.DataContext = this;
        }

        public void SaveDataToXml()
        {
            DataIO io = new DataIO();
            string xmlPath = "Data/Obrenovici.xml";
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
                MessageBox.Show("Nijedan član dinastije nije selektovan za brisanje.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                
                return;
            }

            MessageBoxResult result = MessageBox.Show($"Da li ste sigurni da želite da obrišete {itemsToDelete.Count} selektovanih članova dinastije?", "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var item in itemsToDelete)
                {
                    Obrenovici.Remove(item);
                    // Treba mi samo dodati i brisanje rtf fajla + slike?
                }

                SaveDataToXml();
                ObrenoviciDataGrid.Items.Refresh();
                MessageBox.Show("Uspešno obrisani selektovani članovi.", "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Information);
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


        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in Obrenovici)
                item.IsSelected = true;

            ObrenoviciDataGrid.Items.Refresh();
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
        }
    }
}
