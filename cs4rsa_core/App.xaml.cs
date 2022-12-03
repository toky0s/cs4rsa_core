
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Implements;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Dialogs.Implements;
using Cs4rsa.Dialogs.MessageBoxService;
using Cs4rsa.ModelExtensions;
using Cs4rsa.Services.CourseSearchSvc.Crawlers;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
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
using Cs4rsa.Settings;
using Cs4rsa.Settings.Interfaces;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;
using Cs4rsa.ViewModels;
using Cs4rsa.ViewModels.Interfaces;

using HtmlAgilityPack;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;

namespace Cs4rsa
{
    public sealed partial class App : Application
    {
        public IServiceProvider Container { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi-VN");
            Container = CreateServiceProvider();

            ISetting setting = Container.GetRequiredService<ISetting>();
            string isDatabaseCreated = setting.Read(VMConstants.STPROPS_IS_DATABASE_CREATED);
            if (isDatabaseCreated == "false")
            {
                Container.GetRequiredService<Cs4rsaDbContext>().Database.EnsureCreated();
                Container.GetService<DisciplineCrawler>().GetDisciplineAndKeyword();
                setting.CurrentSetting.IsDatabaseCreated = "true";
                setting.Save();
            }

            // Create folders
            IFolderManager folderManager = Container.GetRequiredService<IFolderManager>();
            string studentPlansPath = Path.Combine(AppContext.BaseDirectory, IFolderManager.FD_STUDENT_PLANS);
            folderManager.CreateFolderIfNotExists(studentPlansPath);
        }

        private static IServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDbContext<Cs4rsaDbContext>();
            services.AddSingleton<ICurriculumRepository, CurriculumRepository>();
            services.AddSingleton<IDisciplineRepository, DisciplineRepository>();
            services.AddSingleton<IKeywordRepository, KeywordRepository>();
            services.AddSingleton<IStudentRepository, StudentRepository>();
            services.AddSingleton<IProgramSubjectRepository, ProgramSubjectRepository>();
            services.AddSingleton<IPreParSubjectRepository, PreParSubjectRepository>();
            services.AddSingleton<IPreProDetailsRepository, PreProDetailRepository>();
            services.AddSingleton<IParProDetailsRepository, ParProDetailRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<ICourseCrawler, CourseCrawler>();
            services.AddSingleton<ICurriculumCrawler, CurriculumCrawler>();
            services.AddSingleton<ITeacherCrawler, TeacherCrawler>();
            services.AddSingleton<ISubjectCrawler, SubjectCrawler>();
            services.AddSingleton<IPreParSubjectCrawler, PreParSubjectCrawler>();
            services.AddSingleton<IDtuStudentInfoCrawler, DtuStudentInfoCrawlerV2>();
            services.AddSingleton<IStudentPlanCrawler, StudentPlanCrawler>();
            services.AddSingleton<DisciplineCrawler>();
            services.AddSingleton<ProgramDiagramCrawler>();

            services.AddTransient<StudentProgramCrawler>();

            services.AddSingleton<ShareString>();
            services.AddSingleton<ColorGenerator>();
            services.AddSingleton<ShareString>();
            services.AddSingleton(new HtmlWeb());
            services.AddSingleton<IMessageBox, Cs4rsaMessageBox>();
            services.AddSingleton<ISetting, Setting>();
            services.AddSingleton<SessionExtension>();
            services.AddSingleton<IOpenInBrowser, OpenInBrowser>();
            services.AddSingleton<IFolderManager, FolderManager>();
            services.AddSingleton<ISnackbarMessageQueue>(new SnackbarMessageQueue(TimeSpan.FromSeconds(2)));

            services.AddSingleton<SaveSessionViewModel>();
            services.AddSingleton<ImportSessionViewModel>();
            services.AddSingleton<ShareStringViewModel>();
            services.AddSingleton<IPhaseStore, PhaseStore>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<SearchViewModel>();
            services.AddSingleton<ClassGroupSessionViewModel>();
            services.AddSingleton<ChoosedSessionViewModel>();
            services.AddSingleton<SchedulerViewModel>();
            services.AddSingleton<MainSchedulingViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<AutoSortSubjectLoadViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
