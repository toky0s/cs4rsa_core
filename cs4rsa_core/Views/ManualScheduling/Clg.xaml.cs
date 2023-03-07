using Cs4rsa.ViewModels.ManualScheduling;

using System.Windows.Controls;

namespace Cs4rsa.Views.ManualScheduling
{
    public partial class Clg : UserControl
    {
        private ClgViewModel Vm;
        public Clg()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Vm = (ClgViewModel)DataContext;
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Vm.SelectedClassGroup == null || Vm.SelectedClassGroup.IsBelongSpecialSubject == false) return;
            Vm.ShowDetailsSchoolClassesCommand.Execute(null);
        }
    }
}