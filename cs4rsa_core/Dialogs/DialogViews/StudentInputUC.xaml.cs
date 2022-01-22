using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.Dialogs.DialogViews
{
    /// <summary>
    /// Interaction logic for LoginUC.xaml
    /// </summary>
    public partial class StudentInputUC : UserControl
    {
        public StudentInputUC()
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
