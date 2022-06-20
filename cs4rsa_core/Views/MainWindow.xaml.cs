using cs4rsa_core.ViewModels;

using Cs4rsaCommon.Enums;

using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Goto((int)ScreenIndex.HOME);
        }

        private void MoveCursorMenu(int index)
        {
            TrainsitionigContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, 0 + (60 * index), 0, 0);
        }

        private void Goto(int index)
        {
            if (ListViewMenu.SelectedIndex != index)
            {
                ListViewMenu.SelectedIndex = index;
            }
            MainWindowViewModel mainWindowViewModel = DataContext as MainWindowViewModel;
            mainWindowViewModel.SelectedIndex = index;
            MoveCursorMenu(index);
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            Goto(listView.SelectedIndex);
            MaterialDesignThemes.Wpf.DrawerHost.CloseDrawerCommand.Execute(null, null);
        }

        private void GotoHome(object sender, RoutedEventArgs e)
        {
            Goto((int)ScreenIndex.HOME);
        }

        private void GotoAccount(object sender, RoutedEventArgs e)
        {
            Goto((int)ScreenIndex.ACCOUNT);
        }

        private void GotoXepLichThuCong(object sender, RoutedEventArgs e)
        {
            Goto((int)ScreenIndex.HAND);
        }

        private void GotoXepLichTuDong(object sender, RoutedEventArgs e)
        {
            Goto((int)ScreenIndex.AUTO);
        }

        private void GotoThongTin(object sender, RoutedEventArgs e)
        {
            Goto((int)ScreenIndex.INFO);
        }
    }
}
