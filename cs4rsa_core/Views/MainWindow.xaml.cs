using cs4rsa_core.ViewModels;
using System.Windows;

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
            int index = ListViewMenu.SelectedIndex;
            MainWindowViewModel mainWindowViewModel = DataContext as MainWindowViewModel;
            if (index > 0)
            {
                mainWindowViewModel.SelectedIndex = index - 1;
                MoveCursorMenu(index);
            }
            else
            {
                mainWindowViewModel.IsExpanded = !mainWindowViewModel.IsExpanded;
            }
        }
    }
}
