using CourseSearchService.Crawlers;
using CourseSearchService.Crawlers.Interfaces;
using CurriculumCrawlerService.Crawlers;
using DisciplineCrawlerService.Crawlers;
using ProgramSubjectCrawlerService.Crawlers;
using StudentCrawlerService.Crawlers;
using SubjectCrawlService1.Crawlers;
using SubjectCrawlService1.Crawlers.Interfaces;
using Cs4rsaDatabaseService.Implements;
using Cs4rsaDatabaseService.Interfaces;
using TeacherCrawlerService1.Crawlers;
using TeacherCrawlerService1.Crawlers.Interfaces;
using Cs4rsaDatabaseService.DataProviders;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using cs4rsa_core.ViewModels;
using cs4rsa_core.Utils;
using HelperService;
using cs4rsa_core.Settings;
using cs4rsa_core.Settings.Interfaces;
using cs4rsa_core.ModelExtensions;
using cs4rsa_core.Dialogs.Implements;
using cs4rsa_core.Dialogs.MessageBoxService;
using CurriculumCrawlerService.Crawlers.Interfaces;
using StudentCrawlerService.Crawlers.Interfaces;
using System.Threading.Tasks;

namespace cs4rsa_core
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Container { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetupExceptionHandling();
            Container = CreateServiceProvider();
            ISetting setting = Container.GetRequiredService<ISetting>();
            string isDatabaseCreated = setting.Read("IsDatabaseCreated");
            if (isDatabaseCreated == "false")
            {
                Container.GetRequiredService<Cs4rsaDbContext>().Database.EnsureCreated();
                Container.GetService<DisciplineCrawler>().GetDisciplineAndKeyword();
                setting.CurrentSetting.IsDatabaseCreated = "true";
                setting.Save();
            }
        }
        private IServiceProvider CreateServiceProvider()
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
            services.AddSingleton<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<ICourseCrawler, CourseCrawler>();
            services.AddSingleton<ICurriculumCrawler, CurriculumCrawler>();
            services.AddSingleton<ITeacherCrawler, TeacherCrawler>();
            services.AddSingleton<ISubjectCrawler, SubjectCrawler>();
            services.AddSingleton<IPreParSubjectCrawler, PreParSubjectCrawler>();
            services.AddSingleton<IDtuStudentInfoCrawler, DtuStudentInfoCrawler>();
            services.AddSingleton<ProgramDiagramCrawler>();
            services.AddSingleton<StudentProgramCrawler>();
            services.AddSingleton<DisciplineCrawler>();
            services.AddSingleton<ShareString>();
            services.AddSingleton<ColorGenerator>();
            services.AddSingleton<IMessageBox, Cs4rsaMessageBox>();
            services.AddSingleton<ISetting, Setting>();
            services.AddSingleton<ISetting, Setting>();
            services.AddSingleton<SessionExtension>();

            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<SearchSessionViewModel>();
            services.AddScoped<ClassGroupSessionViewModel>();
            services.AddScoped<ChoiceSessionViewModel>();
            services.AddScoped<ScheduleTableViewModel>();
            services.AddScoped<MainSchedulingViewModel>();
            services.AddScoped<LoginViewModel>();
            services.AddScoped<StudentInputViewModel>();

            services.AddScoped<SaveSessionViewModel>();
            services.AddScoped<ImportSessionViewModel>();

            return services.BuildServiceProvider();
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            string message = $"Unhandled exception ({source})";
            try
            {
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in LogUnhandledException");
            }
            finally
            {
                Console.WriteLine(message);
            }
        }
    }
}
