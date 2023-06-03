using Cs4rsa.BaseClasses;

namespace Cs4rsa.Views.Profile
{
    public partial class Profile : ScreenAbstract
    {
        public Profile() : base()
        {
            InitializeComponent();
            RegisterComponent(TeacherComponent);
            RegisterComponent(StudentComponent);
        }
    }
}
