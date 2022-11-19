using Cs4rsa.BaseClasses;

using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Dialogs.DialogViews
{
    public partial class StudentInputUC : UserControl, IDialog
    {
        public StudentInputUC()
        {
            InitializeComponent();
        }

        public bool IsCloseOnClickAway()
        {
            return true;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            menu.DataContext = DataContext;
        }
    }
}
