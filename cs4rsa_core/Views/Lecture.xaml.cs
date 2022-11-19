using Cs4rsa.ViewModels;

using MaterialDesignThemes.Wpf;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cs4rsa.Views
{
    public partial class Lecture : UserControl
    {
        public Lecture()
        {
            InitializeComponent();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int teacherId = (DataContext as LectureViewModel).SelectedTeacher.TeacherId;
            Clipboard.SetData(DataFormats.Text, teacherId);
            ISnackbarMessageQueue snackbar = (ISnackbarMessageQueue)((App)Application.Current).Container.GetService(typeof(ISnackbarMessageQueue));
            snackbar.Enqueue($"Đã sao chép mã {teacherId} vào Clipboard");
        }
    }
}
