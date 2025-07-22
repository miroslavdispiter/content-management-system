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

        public TablePage()
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
            Obrenovici = mainWindow.Obrenovici;
            this.DataContext = this;
        }

        private void SaveDataToXml()
        {
            DataIO io = new DataIO();
            string filePath = "Data/Obrenovici.xml";
            io.SerializeObject(Obrenovici.ToList(), filePath);
        }

        private void RowCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is Obrenovic obrenovic)
            {
                obrenovic.IsSelected = checkBox.IsChecked == true;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add page not implemented yet.");
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

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in Obrenovici)
                item.IsSelected = true;

            ObrenoviciDataGrid.Items.Refresh();
        }

        private void NameHyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hyperlink && hyperlink.DataContext is Obrenovic o)
            {
                MessageBox.Show($"Otvaranje biografije: {o.Name}\n(RTF: {o.RtfPath})");
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
        }
    }
}
