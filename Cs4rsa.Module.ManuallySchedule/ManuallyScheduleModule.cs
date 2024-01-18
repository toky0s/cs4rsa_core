using Cs4rsa.Common;
using Cs4rsa.Common.Interfaces;
using Cs4rsa.Module.ManuallySchedule.Utils;
using Cs4rsa.Module.ManuallySchedule.Views;
using Cs4rsa.Service.SubjectCrawler.Crawlers;
using Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Cs4rsa.Module.ManuallySchedule
{
    public class ManuallyScheduleModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ManualRegion", typeof(MainScheduling));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ShareString>();
            containerRegistry.RegisterSingleton<ISubjectCrawler, SubjectCrawler>();
            containerRegistry.RegisterSingleton<ICourseHtmlGetter, CourseHtmlGetter>();
            containerRegistry.RegisterSingleton<IOpenInBrowser, OpenInBrowser>();
        }
    }
}
