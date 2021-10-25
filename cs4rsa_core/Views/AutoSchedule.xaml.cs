using cs4rsa_core.Models;
using cs4rsa_core.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace cs4rsa_core.Views
{
    /// <summary>
    /// Interaction logic for AutoSchedule.xaml
    /// </summary>
    public partial class AutoSchedule : UserControl
    {
        public AutoSchedule()
        {
            InitializeComponent();
            treeView.ItemsSource = (DataContext as AutoScheduleViewModel).ProgramFolderModels;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            menu.DataContext = DataContext;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window window = sender as Window;
            window.Topmost = true;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is ProgramSubjectModel)
            {
                (DataContext as AutoScheduleViewModel).SelectedProSubject = e.NewValue as ProgramSubjectModel;
            }
            else
            {
                (DataContext as AutoScheduleViewModel).SelectedProSubject = null;
            }
        }


        // Chống scroll auto đưa item vào trung tâm khi focus
        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}
