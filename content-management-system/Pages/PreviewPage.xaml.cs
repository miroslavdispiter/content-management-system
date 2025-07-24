using content_management_system.Helpers;
using content_management_system.Models;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace content_management_system.Pages
{
    /// <summary>
    /// Interaction logic for PreviewPage.xaml
    /// </summary>
    public partial class PreviewPage : Page
    {
        private NavigationService navService;
        private MainWindow mainWindow;

        public PreviewPage(Obrenovic obrenovic, NavigationService navigationService)
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
            this.DataContext = obrenovic;
            this.navService = navigationService;

            LoadRtfDescription(obrenovic.RtfPath);
        }

        private void LoadRtfDescription(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                try
                {
                    using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    var hiddenRtb = new RichTextBox();
                    TextRange range = new TextRange(hiddenRtb.Document.ContentStart, hiddenRtb.Document.ContentEnd);
                    range.Load(fs, DataFormats.Rtf);

                    string plainText = range.Text.Trim();

                    if (string.IsNullOrWhiteSpace(plainText))
                    {
                        txtDescription.Text = "No description provided.";
                    }
                    else
                    {
                        txtDescription.Text = plainText;
                    }
                }
                catch
                {
                    txtDescription.Text = "Error loading description.";
                    mainWindow.ShowToastNotification(new ToastNotification("Error", "Failed to load the description.", NotificationType.Error));
                }
            }
            else
            {
                txtDescription.Text = "No description available.";
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            navService.GoBack();
        }
    }
}
