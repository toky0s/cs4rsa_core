using Cs4rsa.Common;
using Cs4rsa.Common.Interfaces;
using Cs4rsa.Database.DataProviders;
using Cs4rsa.Database.Implements;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Views;
using Cs4rsa.Module.ManuallySchedule.Services;
using Cs4rsa.Module.ManuallySchedule.Utils;
using Cs4rsa.Module.ManuallySchedule.Views;
using Cs4rsa.Module.Shared;
using Cs4rsa.Service.Notification;
using Cs4rsa.Service.SubjectCrawler.Crawlers;
using Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

using Xeplich.Service.Search;

namespace Cs4rsa.Module.ManuallySchedule
{
    public class ManuallyScheduleModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionInfo.Manual, typeof(MainScheduling));
            var indexBuilder = containerProvider.Resolve<IndexBuilder>();
            indexBuilder.BuildIndex();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IShareStringService, ShareString>();
            containerRegistry.RegisterSingleton<INotificationService, NotificationService>();
            containerRegistry.RegisterSingleton<ISubjectCrawler, SubjectCrawler>();
            containerRegistry.RegisterSingleton<ICourseHtmlGetter, CourseHtmlGetter>();
            containerRegistry.RegisterSingleton<IOpenInBrowser, OpenInBrowser>();
            containerRegistry.RegisterSingleton<IUnitOfWork, UnitOfWork>();
            containerRegistry.RegisterSingleton<IScheduleValidator, ScheduleValidator>();
            containerRegistry.RegisterSingleton<ITimeBlockGenerator, TimeBlockGenerator>();
            containerRegistry.Register<IndexBuilder>(provider => {
                    var rawsql = provider.Resolve<RawSql>();
                    return new IndexBuilder(rawsql);
            });

            containerRegistry.RegisterDialog<ScheduleDetailUC, ScheduleDetailUCViewModel>();
            containerRegistry.RegisterDialog<ShowDetailsSubjectUC, ShowDetailsSubjectUCViewModel>();
            containerRegistry.RegisterDialog<SaveSessionUC, SaveSessionUCViewModel>();
            containerRegistry.RegisterDialog<ShareStringUC, ShareStringUCViewModel>();
            containerRegistry.RegisterDialog<SolveConflictUC, SolveConflictViewModel>();
            containerRegistry.RegisterDialog<SearchSubjectUC, SearchSubjectViewModel>();
            containerRegistry.RegisterDialog<ShowDetailsSchoolClassesUC, DetailsSchoolClassesViewModel>();
        }
    }
}
