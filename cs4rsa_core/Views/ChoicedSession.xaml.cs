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
using cs4rsa_core.ViewModels;

namespace cs4rsa_core.Views
{
    /// <summary>
    /// Interaction logic for ChoicedSession.xaml
    /// </summary>
    public partial class ChoicedSession : UserControl
    {
        public ChoicedSession()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ChoicedSessionViewModel _choiceSessionViewModel = DataContext as ChoicedSessionViewModel;
            string registerCode = _choiceSessionViewModel.ClassGroupModels[Listbox_Choiced.SelectedIndex].RegisterCode;
            Clipboard.SetData(DataFormats.Text, registerCode);
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            menu.DataContext = DataContext;
        }
    }
}
