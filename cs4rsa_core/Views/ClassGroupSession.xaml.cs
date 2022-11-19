using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Views
{
    public partial class ClassGroupSession : UserControl
    {
        public ClassGroupSession()
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