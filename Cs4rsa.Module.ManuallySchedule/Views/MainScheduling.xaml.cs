using Prism.Regions;

namespace Cs4rsa.Module.ManuallySchedule.Views
{
    public partial class MainScheduling
    {
        public MainScheduling(IRegionManager regionManager)
        {
            InitializeComponent();

            regionManager.RegisterViewWithRegion(RegionInfo.Search, typeof(Search));
            regionManager.RegisterViewWithRegion(RegionInfo.ClassGroup, typeof(Clg));
            regionManager.RegisterViewWithRegion(RegionInfo.Chose, typeof(Chose));
            regionManager.RegisterViewWithRegion(RegionInfo.Scheduler, typeof(Scheduler));
        }
    }
}
