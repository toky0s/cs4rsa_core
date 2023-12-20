using Cs4rsa.Common;
using Cs4rsa.Common.Interfaces;
using Cs4rsa.Database.Implements;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Module.ManuallySchedule.Utils;
using Cs4rsa.Module.ManuallySchedule.Views;
using Cs4rsa.Service.Dialog;
using Cs4rsa.Service.Dialog.Interfaces;
using Cs4rsa.Service.SubjectCrawler.Crawlers;
using Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces;
using Cs4rsa.Service.TeacherCrawler.Crawlers;
using Cs4rsa.Service.TeacherCrawler.Crawlers.Interfaces;

using MaterialDesignThemes.Wpf;

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
            containerRegistry.RegisterSingleton<IUnitOfWork, UnitOfWork>();
            containerRegistry.RegisterSingleton<ISnackbarMessageQueue, SnackbarMessageQueue>();
            containerRegistry.RegisterSingleton<ShareString>();
            containerRegistry.RegisterSingleton<IDialogService, DialogService>();
            containerRegistry.RegisterSingleton<ITeacherCrawler, TeacherCrawler>();
            containerRegistry.RegisterSingleton<ISubjectCrawler, SubjectCrawler>();
            containerRegistry.RegisterSingleton<ICourseHtmlGetter, CourseHtmlGetter>();
            containerRegistry.RegisterSingleton<IOpenInBrowser, OpenInBrowser>();
        }
    }
}
