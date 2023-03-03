using Cs4rsa.BaseClasses;

using System.Windows;

namespace Cs4rsa.Views.Profile
{
    public partial class Profile : ScreenAbstract
    {
        public Profile() : base()
        {
            InitializeComponent();
        }

        private void ScreenAbstract_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterComponent(TeacherComponent);
            RegisterComponent(StudentComponent);
        }
    }
}
