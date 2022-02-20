using cs4rsa_core.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MoveCursorMenu(0);
        }

        private void MoveCursorMenu(byte index)
        {
            TrainsitionigContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, 0 + (60 * index), 0, 0);
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            MainWindowViewModel mainWindowViewModel = DataContext as MainWindowViewModel;
            mainWindowViewModel.SelectedIndex = (byte)listView.SelectedIndex;
            MoveCursorMenu((byte)listView.SelectedIndex);
            MaterialDesignThemes.Wpf.DrawerHost.CloseDrawerCommand.Execute(null, null);
        }
    }
}
