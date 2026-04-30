using Cs4rsa.Database.Models;
using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Views;
using Cs4rsa.Module.ManuallySchedule.ViewModels;
using Cs4rsa.Module.Shared;

using Prism.Regions;

using System.Windows.Controls;

namespace Cs4rsa.Module.ManuallySchedule.Views
{
    public partial class MainScheduling
    {
        public MainScheduling(
            IRegionManager regionManager
        )
        {
            InitializeComponent();


            regionManager.RegisterViewWithRegion(RegionInfo.Manual_ClassGroup, typeof(Clg));
            regionManager.RegisterViewWithRegion(RegionInfo.Manual_Choose, typeof(Choose));
            regionManager.RegisterViewWithRegion(RegionInfo.Manual_Scheduler, typeof(Scheduler));
        }
    }
}
