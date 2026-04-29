using Cs4rsa.Database.Models;
using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Views;
using Cs4rsa.Module.ManuallySchedule.ViewModels;
using Cs4rsa.Module.Shared;
using Cs4rsa.Service.Dialog.Interfaces;

using Prism.Regions;

using System.Windows.Controls;

namespace Cs4rsa.Module.ManuallySchedule.Views
{
    public partial class MainScheduling
    {
        private readonly IDialogService _dialogService;
        public MainScheduling(
            IRegionManager regionManager,
            IDialogService dialogService
        )
        {
            InitializeComponent();

            _dialogService = dialogService;

            regionManager.RegisterViewWithRegion(RegionInfo.Manual_ClassGroup, typeof(Clg));
            regionManager.RegisterViewWithRegion(RegionInfo.Manual_Choose, typeof(Choose));
            regionManager.RegisterViewWithRegion(RegionInfo.Manual_Scheduler, typeof(Scheduler));
        }

        private void LoadUserScheduleButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UserSchedule userSchedule = ((Button)e.Source).DataContext as UserSchedule;
            if (userSchedule != null)
            {
                ScheduleDetailUC scheduleDetailView = new ScheduleDetailUC();
                ((ScheduleDetailUCViewModel)scheduleDetailView.DataContext).UserSchedule = userSchedule;
                _dialogService.OpenDialog(scheduleDetailView);
            }
        }

        private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var saveSessionUc = new SaveSessionUC();
            var vm = (SaveSessionUCViewModel)saveSessionUc.DataContext;
            vm.ClassGroupModels = ((MainSchedulingViewModel)DataContext).SelectedClassGroupModels;
            _dialogService.OpenDialog(saveSessionUc);
        }
    }
}
