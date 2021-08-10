using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Configuration;
using LightMessageBus;
using cs4rsa.Messages;
using cs4rsa.ViewModels;

namespace cs4rsa.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// Không triển khai MVVM
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            MessageBus.Default.Publish<Cs4rsaSnackbarMessage>(new Cs4rsaSnackbarMessage("Chào mừng đến với CS4RSA"));
        }

        private void GotoSource(object sender, RoutedEventArgs e)
        {
            string url = ConfigurationManager.AppSettings.Get("Cs4rsaGitHub");
            Process.Start(url);
        }

        private void GotoWeb(object sender, RoutedEventArgs e)
        {
            string url = ConfigurationManager.AppSettings.Get("Cs4rsa");
            Process.Start(url);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string url = ConfigurationManager.AppSettings.Get("Cs4rsaOfDTU");
            Process.Start(url);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string url = ConfigurationManager.AppSettings.Get("Author");
            Process.Start(url);
        }
    }
}
