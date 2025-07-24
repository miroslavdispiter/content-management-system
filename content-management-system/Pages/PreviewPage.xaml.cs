using content_management_system.Models;
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

        public PreviewPage(Obrenovic obrenovic, NavigationService navigationService)
        {
            InitializeComponent();
            this.DataContext = obrenovic;
            this.navService = navigationService;

            LoadRtfDescription(obrenovic.RtfPath);
        }

        private void LoadRtfDescription(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    TextRange textRange = new TextRange(rtbDescription.Document.ContentStart, rtbDescription.Document.ContentEnd);
                    textRange.Load(fileStream, DataFormats.Rtf);
                }
                catch
                {
                    MessageBox.Show("Failed to load the description.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            navService.GoBack();
        }
    }
}
