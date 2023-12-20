using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Module.ManuallySchedule.ViewModels;

namespace Cs4rsa.Module.ManuallySchedule.Views
{
    public partial class Search: UserControl
    {
        private static readonly Key[] _userAllowedKeys = { Key.OemMinus, Key.Back, Key.Space };
        private SearchViewModel Vm;
        public Search()
        {
            InitializeComponent();
        }

        public static bool IsKeyAChar(Key key)
        {
            return key >= Key.A && key <= Key.Z;
        }

        public static bool IsKeyADigit(Key key)
        {
            return (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9);
        }

        private static bool IsUserAllowedKey(Key key)
        {
            return _userAllowedKeys.Contains(key);
        }

        private void SearchingTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Popup_Recommend.IsOpen = (IsKeyAChar(e.Key) || IsKeyADigit(e.Key) || IsUserAllowedKey(e.Key))
                && e.Key != Key.Escape;
            if (e.Key == Key.Escape)
            {
                Keyboard.ClearFocus();
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

        private void Btn_GotoCourse_Click(object sender, RoutedEventArgs e)
        {
            var subjectModel = (SubjectModel)((MenuItem)sender).DataContext;
            Vm.GotoCourseCommand.Execute(subjectModel);
        }

        private void Btn_Details_Click(object sender, RoutedEventArgs e)
        {
            var subjectModel = (SubjectModel)((MenuItem)sender).DataContext;
            Vm.DetailCommand.Execute(subjectModel);
        }

        public void LoadData()
        {
            Vm = (SearchViewModel)DataContext;
        }
    }
}