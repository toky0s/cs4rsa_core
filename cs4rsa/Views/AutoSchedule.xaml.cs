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
        private AutoScheduleViewModel _autoScheduleViewModel;
        public AutoSchedule()
        {
            InitializeComponent();
            Cs4rsaMessageBox cs4RsaMessageBox = new Cs4rsaMessageBox();
            _autoScheduleViewModel = new AutoScheduleViewModel(cs4RsaMessageBox);
            DataContext = _autoScheduleViewModel;
            ListViewSubjects.ItemsSource = _autoScheduleViewModel.ProgramSubjectModels;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewSubjects.ItemsSource);
            view.Filter = Filter;
        }

        public void Load()
        {
            _autoScheduleViewModel.LoadProgramSubject();
        }

        private bool Filter(object item)
        {
            ProgramSubjectModel subject = item as ProgramSubjectModel;
            return SubjectFilter(item)
                && FilterShowing(subject);
        }

        private bool SubjectFilter(object item)
        {
            if (string.IsNullOrEmpty(TextBoxSubjectName.Text))
                return true;
            else
            {
                ProgramSubjectModel subject = item as ProgramSubjectModel;
                return (subject.SubjectName.IndexOf(TextBoxSubjectName.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (subject.SubjectCode.IndexOf(TextBoxSubjectName.Text, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (subject.FolderName.IndexOf(TextBoxSubjectName.Text, StringComparison.OrdinalIgnoreCase) >= 0);                    
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewSubjects.ItemsSource).Refresh();
        }

        private bool FilterShowing(ProgramSubjectModel subject)
        {
            if (RadioButtonCanChoice.IsChecked.Value == true)
            {
                return subject.IsCanChoice;
            }
            if (RadioButtonDoneSubject.IsChecked.Value == true)
            {
                return subject.IsDone;
            }
            if (RadioButtonUnCompletedFolder.IsChecked.Value == true)
            {
                return !subject.IsFolderCompleted;
            }
            if (RadioButtonAvailableSubject.IsChecked.Value == true)
            {
                return subject.IsAvaiableInThisSemester();
            }
            return true;
        }

        private void ReloadSubjects(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewSubjects.ItemsSource).Refresh();
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
    }
}
