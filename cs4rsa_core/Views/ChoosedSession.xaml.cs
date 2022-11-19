using Cs4rsa.Constants;
using Cs4rsa.ViewModels;

using MaterialDesignThemes.Wpf;

using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Views
{
    public partial class ChoosedSession : UserControl
    {
        public ChoosedSession()
        {
            InitializeComponent();
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            menu.DataContext = DataContext;
        }
    }
}
