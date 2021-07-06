using cs4rsa.Dialogs.DialogResults;
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
    public partial class AutoSchedule : Window
    {
        public AutoSchedule(LoginResult result)
        {
            InitializeComponent();
            AutoScheduleViewModel autoScheduleViewModel = new AutoScheduleViewModel(result.StudentModel);
            DataContext = autoScheduleViewModel;
            ListViewSubjects.ItemsSource = autoScheduleViewModel.ProgramSubjectModels;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewSubjects.ItemsSource);
            view.Filter = Filter;
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
                    (subject.SubjectCode.IndexOf(TextBoxSubjectName.Text, StringComparison.OrdinalIgnoreCase) >= 0);                    
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
    }
}
