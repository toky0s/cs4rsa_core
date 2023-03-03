using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.ViewModels.Profile;

using MaterialDesignThemes.Wpf;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cs4rsa.Views.Profile
{
    public partial class Teacher : UserControl, IComponent
    {
        public Teacher()
        {
            InitializeComponent();
        }

        public async Task LoadData()
        {
            await (DataContext as TeacherViewModel).LoadTeachers();
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
