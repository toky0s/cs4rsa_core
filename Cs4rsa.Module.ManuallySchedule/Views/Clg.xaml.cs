using Cs4rsa.Module.ManuallySchedule.Models;

using System.Windows.Controls;
using Cs4rsa.Module.ManuallySchedule.ViewModels;

namespace Cs4rsa.Module.ManuallySchedule.Views
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
            //Messenger.Register<ScheduleBlockMsgs.SelectedMsg>(this, (recipient, message) =>
            //{
            //    int idx = ClassGroupListBox.SelectedIndex;
            //    ListBoxItem item = (ListBoxItem)ClassGroupListBox.ItemContainerGenerator.ContainerFromIndex(ClassGroupListBox.SelectedIndex);
            //    item.
            //});
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Vm.SelectedClassGroup == null || Vm.SelectedClassGroup.IsBelongSpecialSubject == false) return;
            Vm.ShowDetailsSchoolClassesCommand.Execute();
        }
    }
}