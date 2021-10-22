using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Khởi tạo trước các View trong Runtime
        private readonly Home _homeView = new();
        private readonly LoginView _loginView = new();
        private readonly MainScheduling _mainScheduling = new();
        //private readonly AutoSchedule _autoScheduling = new AutoSchedule();
        private readonly Info _infoView = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListViewMenu.SelectedIndex;
            MoveCursorMenu(index);

            switch (index)
            {
                case 0:
                    MainArea.Content = _homeView;
                    break;
                case 1:
                    MainArea.Content = _loginView;
                    break;
                case 2:
                    MainArea.Content = _mainScheduling;
                    break;
                case 3:
                    // fixed
                    MainArea.Content = _infoView;
                    break;
                case 4:
                    MainArea.Content = _infoView;
                    break;
            }
        }

        private void MoveCursorMenu(int index)
        {
            TrainsitionigContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, (100 + (60 * index)), 0, 0);
        }
    }
}
