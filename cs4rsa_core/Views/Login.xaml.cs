using cs4rsa_core.ViewModels;

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.Views
{
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            menu.DataContext = DataContext;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoginViewModel loginViewModel = DataContext as LoginViewModel;
            await loginViewModel.LoadStudentInfos();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.ToString(),
                UseShellExecute = true
            });
        }
    }
}
