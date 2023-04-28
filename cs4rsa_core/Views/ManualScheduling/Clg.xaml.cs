using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Messages.Publishers.UIs;
using Cs4rsa.ViewModels.ManualScheduling;

using System.Linq;
using System.Windows.Controls;


namespace Cs4rsa.Views.ManualScheduling
{
    public partial class Clg : BaseUserControl
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
            Vm.ShowDetailsSchoolClassesCommand.Execute(null);
        }
    }
}