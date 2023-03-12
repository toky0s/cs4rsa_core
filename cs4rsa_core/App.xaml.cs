using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Implements;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Dialogs.Implements;
using Cs4rsa.ModelExtensions;
using Cs4rsa.Services.CourseSearchSvc.Crawlers;
using Cs4rsa.Services.CurriculumCrawlerSvc.Crawlers;
using Cs4rsa.Services.CurriculumCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.DisciplineCrawlerSvc.Crawlers;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.Crawlers;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.Interfaces;
using Cs4rsa.Services.StudentCrawlerSvc.Crawlers;
using Cs4rsa.Services.StudentCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.Crawlers;
using Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.TeacherCrawlerSvc.Crawlers;
using Cs4rsa.Services.TeacherCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;
using Cs4rsa.ViewModels;
using Cs4rsa.ViewModels.AutoScheduling;
using Cs4rsa.ViewModels.Database;
using Cs4rsa.ViewModels.ManualScheduling;
using Cs4rsa.ViewModels.Profile;

using HtmlAgilityPack;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.DependencyInjection;

using Squirrel;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa
{
    public sealed partial class App
    {
        public IServiceProvider Container { get; private set; }
        public IMessenger Messenger { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            // Init Clowd.Squirrel
            SquirrelAwareApp.HandleEvents(
                onInitialInstall: OnAppInstall,
                onAppUninstall: OnAppUninstall,
                onEveryRun: OnAppRun
            );

            base.OnStartup(e);
            Messenger = WeakReferenceMessenger.Default;
            Container = CreateServiceProvider();

            // Database Init Data
            CourseCrawler courseCrawler = Container.GetService<CourseCrawler>();
            courseCrawler.InitInfor();
            if (!File.Exists(VmConstants.DbFilePath))
            {
                Container.GetRequiredService<Cs4rsaDbContext>().Database.EnsureCreated();

                // Seed Settings
                Container.GetService<IUnitOfWork>().Settings.InsertSemesterSetting(
                      courseCrawler.CurrentYearInfo
                    , courseCrawler.CurrentYearValue
                    , courseCrawler.CurrentSemesterInfo
                    , courseCrawler.CurrentSemesterValue
                );

                // Seed Data Discipline and Keyword
                Container.GetService<DisciplineCrawler>().GetDisciplineAndKeyword();
            }

            // Init Folder
            InitFolders();
        }

        private static void OnAppInstall(SemanticVersion version, IAppTools tools)
        {
            tools.CreateShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }

        private static void OnAppUninstall(SemanticVersion version, IAppTools tools)
        {
            tools.RemoveShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }

        private static void OnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
        {
            tools.SetProcessAppUserModelId();
            // show a welcome message when the app is first installed
            if (firstRun) MessageBox.Show("Thanks for installing my application!");
        }

        private static async Task UpdateMyApp()
        {
            using var mgr = new UpdateManager(VmConstants.LinkProjectPage);
            var newVersion = await mgr.UpdateApp();

            // optionally restart the app automatically, or ask the user if/when they want to restart
            if (newVersion != null)
            {
                UpdateManager.RestartApp();
            }
        }

        private static IServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDbContext<Cs4rsaDbContext>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<HttpClient>();

            services.AddSingleton<ICurriculumCrawler, CurriculumCrawler>();
            services.AddSingleton<ITeacherCrawler, TeacherCrawler>();
            services.AddSingleton<ISubjectCrawler, SubjectCrawler>();
            services.AddSingleton<IPreParSubjectCrawler, PreParSubjectCrawler>();
            services.AddSingleton<IDtuStudentInfoCrawler, DtuStudentInfoCrawlerV2>();
            services.AddSingleton<IStudentProgramCrawler, StudentProgramCrawler>();
            services.AddSingleton<IStudentPlanCrawler, StudentPlanCrawler>();
            services.AddSingleton<DisciplineCrawler>();

            services.AddSingleton<ImageDownloader>();
            services.AddSingleton<CourseCrawler>();
            services.AddSingleton<ShareString>();
            services.AddSingleton<ColorGenerator>();

            HtmlWeb htmlWeb = new()
            {
                PreRequest = delegate (HttpWebRequest wr)
                {
                    // Set timeout for HtmlWeb
                    wr.Timeout = 2000;
                    return true;
                }
            };
            services.AddSingleton(htmlWeb);
            services.AddSingleton<SessionExtension>();
            services.AddSingleton<IOpenInBrowser, OpenInBrowser>();
            services.AddSingleton<IFolderManager, FolderManager>();
            services.AddSingleton<ISnackbarMessageQueue>(new SnackbarMessageQueue(TimeSpan.FromSeconds(2)));

            services.AddSingleton<SaveSessionViewModel>();
            services.AddSingleton<ImportSessionViewModel>();
            services.AddSingleton<ShareStringViewModel>();
            services.AddSingleton<PhaseStore>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<SearchViewModel>();
            services.AddSingleton<ClgViewModel>();
            services.AddSingleton<ChoosedViewModel>();
            services.AddSingleton<SchedulerViewModel>();
            services.AddSingleton<MainSchedulingViewModel>();
            services.AddSingleton<AccountViewModel>();
            services.AddSingleton<ProgramTreeViewModel>();
            services.AddSingleton<ResultViewModel>();
            services.AddSingleton<DbViewModel>();
            services.AddSingleton<DclTabViewModel>();

            // Profile Screen
            services.AddSingleton<ProfileViewModel>();
            services.AddSingleton<TeacherViewModel>();
            services.AddSingleton<StudentPfViewModel>();

            return services.BuildServiceProvider();
        }

        private void InitFolders()
        {
            IFolderManager folderManager = Container.GetRequiredService<IFolderManager>();
            folderManager.CreateFoldersAtStartUp();
        }
    }
}
