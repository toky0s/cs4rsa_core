using Cs4rsa.Module.Shared;

using Prism.Regions;

namespace Cs4rsa.Module.ManuallySchedule.Views
{
    public partial class MainScheduling
    {
        public MainScheduling(IRegionManager regionManager)
        {
            InitializeComponent();
            regionManager.RegisterViewWithRegion(RegionInfo.Manual_ClassGroup, typeof(Clg));
            regionManager.RegisterViewWithRegion(RegionInfo.Manual_Choose, typeof(Choose));
            regionManager.RegisterViewWithRegion(RegionInfo.Manual_Scheduler, typeof(Scheduler));
        }
    }
}
