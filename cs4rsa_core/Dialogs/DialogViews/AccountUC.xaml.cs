using Cs4rsa.BaseClasses;
using Cs4rsa.Dialogs.Implements;

using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Dialogs.DialogViews
{
    public partial class AccountUC : UserControl, IDialog
    {
        public AccountUC()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AccountViewModel accountVm = DataContext as AccountViewModel;
            accountVm.LoadStudent();
        }

        public bool IsCloseOnClickAway() => true;
    }
}
