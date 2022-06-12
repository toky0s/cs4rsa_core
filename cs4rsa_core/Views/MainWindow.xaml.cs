using cs4rsa_core.ViewModels;

using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.Views
{
    public partial class MainWindow : Window
    {
        #region Menu | Chỉ mục vị trí các màn hình
        private static readonly int HOME = 0;
        private static readonly int ACCOUNT = 1;
        private static readonly int HAND = 2;
        private static readonly int AUTO = 3;
        private static readonly int INFO = 4;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Goto(HOME);
        }

        private void MoveCursorMenu(int index)
        {
            TrainsitionigContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, 0 + (60 * index), 0, 0);
        }

        private void Goto(int index)
        {
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
            Goto(HOME);
        }

        private void GotoAccount(object sender, RoutedEventArgs e)
        {
            Goto(ACCOUNT);
        }

        private void GotoXepLichThuCong(object sender, RoutedEventArgs e)
        {
            Goto(HAND);
        }

        private void GotoXepLichTuDong(object sender, RoutedEventArgs e)
        {
            Goto(AUTO);
        }

        private void GotoThongTin(object sender, RoutedEventArgs e)
        {
            Goto(INFO);
        }
    }
}
