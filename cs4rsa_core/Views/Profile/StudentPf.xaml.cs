using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.StudentCrawlerSvc.Models;
using Cs4rsa.ViewModels.Profile;

using MaterialDesignThemes.Wpf;

using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Views.Profile
{
    public partial class StudentPf : BaseUserControl, IComponent
    {
        public StudentPf()
        {
            InitializeComponent();
        }

        private void Btn_OpenInFolder_Clicked(object sender, RoutedEventArgs e)
        {
            StudentModel stm = (StudentModel)((Button)sender).DataContext;
            ((StudentPfViewModel)DataContext).OpenInFolderCommand.Execute(stm.StudentId);
        }

        public void LoadData()
        {
            ((StudentPfViewModel)DataContext).OnInit();
        }

        private void Txt_StudentCodeForDownload_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            StudentPfViewModel studentPfViewModel = (StudentPfViewModel)DataContext;
            if (e.Key != System.Windows.Input.Key.Enter
                && e.Key != System.Windows.Input.Key.Space) return;
            string inputStudentId = Txt_StudentCodeForDownload.Text.Trim();
            studentPfViewModel.AddStudentIdToDownload(inputStudentId);
            Txt_StudentCodeForDownload.Text = string.Empty;
            e.Handled = true;
        }

        private void Btn_Remove_Clicked(object sender, RoutedEventArgs e)
        {
            int removeIdx = (int)((Button)sender).CommandParameter;
            ((StudentPfViewModel)DataContext).StudentModels.RemoveAt(removeIdx);
        }

        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string studentId = ((Student)((Label)sender).DataContext).StudentId;
            Clipboard.SetData(DataFormats.Text, studentId);
            ISnackbarMessageQueue snackbar = (ISnackbarMessageQueue)Container.GetService(typeof(ISnackbarMessageQueue));
            snackbar.Enqueue(CredizText.StudentMsg001(studentId));
        }
    }
}
