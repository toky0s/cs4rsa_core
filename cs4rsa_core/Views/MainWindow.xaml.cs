using Cs4rsa.Commons.Enums;
using Cs4rsa.ViewModels;

using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Views
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Goto((int)ScreenIndex.HAND);
        }

        public void LoadPage(ScreenIndex screenIndex)
        {
            Goto((int)screenIndex);
        }

        #region Util functions
        private void Goto(int index)
        {
            if (ListViewMenu.SelectedIndex != index)
            {
                ListViewMenu.SelectedIndex = index;
            }
            MainWindowViewModel mainWindowViewModel = DataContext as MainWindowViewModel;
            mainWindowViewModel.SelectedIndex = index;
        }
        #endregion

        #region Event handlers
        private async void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            Goto(listView.SelectedIndex);
            if (listView.SelectedIndex == (int)ScreenIndex.LECTURE)
            {
                await (Lecture.DataContext as TeacherViewModel).LoadTeachers();
            }
            // Close Drawer
            MaterialDesignThemes.Wpf.DrawerHost.CloseDrawerCommand.Execute(null, null);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPage(ScreenIndex.HOME);
        }
        #endregion

        #region Go to menus
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
        #endregion
    }
}
