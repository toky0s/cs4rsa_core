using Cs4rsa.BaseClasses;

namespace Cs4rsa.Views.AutoScheduling
{
    public partial class Auto : ScreenAbstract
    {
        public Auto()
        {
            InitializeComponent();
        }

        private void ScreenAbstract_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RegisterComponent(ProgramTreeComponent);
        }
    }
}
