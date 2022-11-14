using cs4rsa_core.Constants;
using cs4rsa_core.Cs4rsaDatabase.DataProviders;
using cs4rsa_core.Cs4rsaDatabase.Implements;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Dialogs.Implements;
using cs4rsa_core.Dialogs.MessageBoxService;
using cs4rsa_core.ModelExtensions;
using cs4rsa_core.Services.CourseSearchSvc.Crawlers;
using cs4rsa_core.Services.CourseSearchSvc.Crawlers.Interfaces;
using cs4rsa_core.Services.CurriculumCrawlerSvc.Crawlers;
using cs4rsa_core.Services.CurriculumCrawlerSvc.Crawlers.Interfaces;
using cs4rsa_core.Services.DisciplineCrawlerSvc.Crawlers;
using cs4rsa_core.Services.ProgramSubjectCrawlerSvc.Crawlers;
using cs4rsa_core.Services.StudentCrawlerSvc.Crawlers;
using cs4rsa_core.Services.StudentCrawlerSvc.Crawlers.Interfaces;
using cs4rsa_core.Services.SubjectCrawlerSvc.Crawlers;
using cs4rsa_core.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using cs4rsa_core.Services.TeacherCrawlerSvc.Crawlers;
using cs4rsa_core.Services.TeacherCrawlerSvc.Crawlers.Interfaces;
using cs4rsa_core.Settings;
using cs4rsa_core.Settings.Interfaces;
using cs4rsa_core.Utils;
using cs4rsa_core.Utils.Interfaces;
using cs4rsa_core.ViewModels;

using HtmlAgilityPack;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Windows;

namespace cs4rsa_core
{
    public sealed partial class App : Application
    {
        public IServiceProvider Container { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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
            services.AddSingleton<ProgramDiagramCrawler>();
            services.AddSingleton<StudentProgramCrawler>();
            services.AddSingleton<DisciplineCrawler>();

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
            services.AddSingleton<PhaseStore>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<SearchSessionViewModel>();
            services.AddSingleton<ClassGroupSessionViewModel>();
            services.AddSingleton<ChoosedSessionViewModel>();
            services.AddSingleton<ScheduleTableViewModel>();
            services.AddSingleton<MainSchedulingViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<StudentInputViewModel>();
            services.AddSingleton<AutoSortSubjectLoadViewModel>();
            services.AddSingleton<SubjectDownloadingViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
