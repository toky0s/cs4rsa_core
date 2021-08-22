using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Models;
using cs4rsa.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace cs4rsa.Views
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

        public void Load()
        {
            AutoScheduleViewModel vm = DataContext as AutoScheduleViewModel;
            vm.LoadProgramSubject();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void ReloadSubjects(object sender, RoutedEventArgs e)
        {
            
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            menu.DataContext = DataContext;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window  = sender as Window;
            window.Topmost = true;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            (DataContext as AutoScheduleViewModel).SelectedProSubject = e.NewValue as ProgramSubjectModel;
        }
    }
}
