using Cs4rsa.Constants;
using Cs4rsa.ViewModels;

using MaterialDesignThemes.Wpf;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cs4rsa.Views
{
    public partial class Teacher : UserControl
    {
        public Teacher()
        {
            InitializeComponent();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int teacherId = (DataContext as TeacherViewModel).SelectedTeacher.TeacherId;
            Clipboard.SetData(DataFormats.Text, teacherId);
            ISnackbarMessageQueue snackbar = (ISnackbarMessageQueue)((App)Application.Current).Container.GetService(typeof(ISnackbarMessageQueue));
            snackbar.Enqueue(CredizText.TeacherMsg001(teacherId));
        }
    }
}
