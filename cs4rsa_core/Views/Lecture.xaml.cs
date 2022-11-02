using cs4rsa_core.ViewModels;

using MaterialDesignThemes.Wpf;

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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cs4rsa_core.Views
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
            ISnackbarMessageQueue snackbar = (ISnackbarMessageQueue) ((App)Application.Current).Container.GetService(typeof(ISnackbarMessageQueue));
            snackbar.Enqueue($"Đã sao chép mã {teacherId} vào Clipboard");
        }
    }
}
