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
            MoveCursorMenu(1);
        }

        private void MoveCursorMenu(int index)
        {
            if (index != 0)
            {
                TrainsitionigContentSlide.OnApplyTemplate();
                GridCursor.Margin = new Thickness(0, 100 + (60 * index), 0, 0);
            }
        }

        private void ListViewMenu_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListView listView = sender as ListView;
            int index = ListViewMenu.SelectedIndex;
            MainWindowViewModel mainWindowViewModel = DataContext as MainWindowViewModel;
            if (index > 0 && mainWindowViewModel.SelectedIndex != listView.SelectedIndex - 1)
            {
                mainWindowViewModel.SelectedIndex = index - 1;
                MoveCursorMenu(index);
            }
            else if (index == 0)
            {
                listView.SelectedIndex = 1;
                mainWindowViewModel.IsExpanded = !mainWindowViewModel.IsExpanded;
            }
        }
    }
}
