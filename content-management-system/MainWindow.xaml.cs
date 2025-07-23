using content_management_system.Helpers;
using content_management_system.Models;
using content_management_system.Pages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace content_management_system
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Obrenovic> Obrenovici { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadFromXml();
            MainFrame.Navigate(new LoginPage());
        }

        private void LoadFromXml()
        {
            string xmlPath = "Data/Obrenovici.xml";

            if (!File.Exists(xmlPath))
            {
                MessageBox.Show($"XML fajl nije pronadjen: {xmlPath}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                Obrenovici = new ObservableCollection<Obrenovic>();
                return;
            }

            var io = new DataIO();
            var list = io.DeSerializeObject<List<Obrenovic>>(xmlPath);

            if (list != null)
            {
                Obrenovici = new ObservableCollection<Obrenovic>(list);
            }
            else
            {
                Obrenovici = new ObservableCollection<Obrenovic>();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public void Logout()
        {
            MainFrame.Navigate(new LoginPage());
        }
    }
}
