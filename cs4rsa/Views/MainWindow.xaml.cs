using System.Windows;
using cs4rsa.ViewModels;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using cs4rsa.Helpers;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Views;
using MaterialDesignThemes.Wpf;

namespace cs4rsa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Khởi tạo trước các View trong Runtime
        private HomeView _homeView = new HomeView();
        private LoginView _loginView = new LoginView();
        private MainScheduling _mainScheduling = new MainScheduling();
        private AutoSchedule _autoScheduling = new AutoSchedule();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
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
                    MainArea.Content = _autoScheduling;
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
