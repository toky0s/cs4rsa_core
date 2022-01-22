using cs4rsa_core.ViewModels;
using System.Windows;
using System.Windows.Controls;

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
