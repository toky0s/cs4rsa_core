using Cs4rsa.BaseClasses;
using Cs4rsa.Dialogs.Implements;

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Cs4rsa.Dialogs.DialogViews
{
    public partial class AccountUC : UserControl, IDialog
    {
        public AccountUC()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AccountViewModel accountVm = DataContext as AccountViewModel;
            await accountVm.LoadStudent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.ToString(),
                UseShellExecute = true
            });
        }

        public bool IsCloseOnClickAway() => true;
    }
}
