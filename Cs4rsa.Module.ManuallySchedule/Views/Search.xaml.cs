using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Module.ManuallySchedule.ViewModels;

namespace Cs4rsa.Module.ManuallySchedule.Views
{
    public partial class Search: UserControl
    {
        private readonly SearchViewModel Vm;
        public Search()
        {
            InitializeComponent();
            Vm = (SearchViewModel)DataContext;
        }

        private void SearchingTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Popup_Recommend.IsOpen = false;
                Keyboard.ClearFocus();
            }
            else
            {
                Popup_Recommend.IsOpen = true;
            }
        }

        private void SearchingTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Popup_Recommend.IsOpen = false;
        }

        private void DownloadSubjects_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                var url = (string)e.Data.GetData(DataFormats.UnicodeText);
                var uri = new UriBuilder(url).Uri;
                Vm.OnAddSubjectFromUriAsync(uri);
            }
        }

        private void SearchingTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Popup_Recommend.IsOpen = SearchingTextBox.Text.Trim().Length > 0;
        }

        private void BtnReDonwload_Click(object sender, RoutedEventArgs e)
        {
            var subjectModel = (SubjectModel)((Button)sender).DataContext;
            Vm.ReloadCommand.Execute(subjectModel);
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            var subjectModel = (SubjectModel)((MenuItem)sender).DataContext;
            Vm.DeleteCommand.Execute(subjectModel);
        }

        private void Btn_Details_Click(object sender, RoutedEventArgs e)
        {
            var subjectModel = (SubjectModel)((MenuItem)sender).DataContext;
            Vm.DetailCommand.Execute(subjectModel);
        }

        private void OpenInCourse_Click(object sender, RoutedEventArgs e)
        {
            var subjectModel = (SubjectModel)((MenuItem)sender).DataContext;
            Vm.GotoCourseCommand.Execute(subjectModel);
        }

        private void CopyError_Click(object sender, RoutedEventArgs e)
        {
            var subjectModel = (SubjectModel)((MenuItem)sender).DataContext;
            Clipboard.SetText(subjectModel.ErrorMessage);
        }
    }
}