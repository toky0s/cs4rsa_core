using cs4rsa_core.Constants;
using cs4rsa_core.ViewModels;

using MaterialDesignThemes.Wpf;

using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.Views
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
