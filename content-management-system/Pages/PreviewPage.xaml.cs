using content_management_system.Models;
using System;
using System.Collections.Generic;
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
        }

        private void Back_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            navService.GoBack();
        }
    }
}
