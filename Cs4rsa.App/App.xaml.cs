using Cs4rsa.App.Services;
using Cs4rsa.App.ViewModels;
using Cs4rsa.App.Views;
using Cs4rsa.App.Views.UserControls;
using Cs4rsa.Common;
using Cs4rsa.Common.Interfaces;
using Cs4rsa.Database.DataProviders;
using Cs4rsa.Database.Implements;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Module.ManuallySchedule;
using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Views;
using Cs4rsa.Module.ManuallySchedule.Views;
using Cs4rsa.Service.CourseCrawler.Crawlers;
using Cs4rsa.Service.CourseCrawler.Interfaces;
using Cs4rsa.Service.Dialog;
using Cs4rsa.Service.Dialog.Interfaces;
using Cs4rsa.Service.DisciplineCrawler;
using Cs4rsa.Service.SubjectCrawler.Crawlers;
using Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces;

using DryIoc;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.Logging;

using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using Serilog;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;

using Velopack;

namespace Cs4rsa.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        [STAThread]
        private static void Main(string[] args)
        {
            VelopackApp.Build().Run();
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        public static Rules DefaultRules => Rules.Default.WithConcreteTypeDynamicRegistrations(reuse: Reuse.Transient)
                                                        .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
                                                        .WithFuncAndLazyWithoutRegistration()
                                                        .WithTrackingDisposableTransients()
                                                        .WithFactorySelector(Rules.SelectLastRegisteredFactory());

        protected override Rules CreateContainerRules()
        {
            return DefaultRules;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            RawSql rawSql = Container.Resolve<RawSql>();
            IUnitOfWork unitOfWork = Container.Resolve<IUnitOfWork>();
            DisciplineCrawler disciplineCrawler = Container.Resolve<DisciplineCrawler>();
            IFolderManager folderManager = Container.Resolve<IFolderManager>();
            ICourseCrawler courseCrawler = Container.Resolve<ICourseCrawler>();
            var logger = Container.Resolve<ILogger<App>>();

            courseCrawler.GetInfo(out string yearInfo, out string yearValue, out string semesterInfo, out string semesterValue);
            folderManager.CreateFoldersAtStartUp();
            List<Discipline> disciplines = disciplineCrawler.GetDisciplineAndKeyword(semesterValue);

            var dbPath = Cs4rsa.App.Properties.Resources.DbPath;
            var migratePath = Cs4rsa.App.Properties.Resources.MigratePath;
            if (!File.Exists(dbPath))
            {
                rawSql.CreateDbIfNotExist(dbPath, migratePath);

                // Seed Settings
                unitOfWork.Settings.InsertSemesterSetting(yearInfo, yearValue, semesterInfo, semesterValue);
                string sql = BulkInsertDisciplines.GetBulkInsertSql(disciplines);
                logger.LogInformation("Executing bulk insert for disciplines and keywords.\n{Sql}", sql);
                rawSql.ExecNonQuery(sql);
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register logging
            const string LogFolderPath = "logs";
            if (!Directory.Exists(LogFolderPath))
            {
                Directory.CreateDirectory(LogFolderPath);
            }
            var logFileName = $"{LogFolderPath}/log-{DateTime.Now:yyyyMMdd-HHmmss}.txt";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logFileName)
                .CreateLogger();

            // Hook vào Microsoft ILogger
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });

            containerRegistry.RegisterInstance(loggerFactory);
            containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

            string cnnStr = Cs4rsa.App.Properties.Resources.DbConn;
            containerRegistry.Register<RawSql>(provider => new RawSql(cnnStr, provider.Resolve<ILogger<RawSql>>()));

            containerRegistry.RegisterSingleton<ISemesterHtmlGetter, SemesterHtmlGetter>();
            containerRegistry.RegisterSingleton<IDisciplineHtmlGetter, DisciplineHtmlGetter>();
            containerRegistry.RegisterSingleton<ICourseCrawler, CourseCrawler>();
            containerRegistry.RegisterSingleton<DisciplineCrawler>();

            containerRegistry.RegisterSingleton<IUnitOfWork, UnitOfWork>();
            containerRegistry.RegisterSingleton<ISubjectCrawler, SubjectCrawler>();
            containerRegistry.RegisterSingleton<IOpenInBrowser, OpenInBrowser>();
            containerRegistry.RegisterSingleton<IFolderManager, FolderManager>();
            containerRegistry.RegisterSingleton<ISnackbarMessageQueue, SnackbarMessageQueue>();

            containerRegistry.RegisterDialog<DownloadUpdatesDialog, DownloadUpdatesDialogViewModel>();

#if DEBUG
            containerRegistry.RegisterSingleton<IUpdateService, DummyUpdateService>();
#else
            containerRegistry.RegisterSingleton<IUpdateService, GithubUpdateService>();
#endif
        }

        protected override Window CreateShell()
        {
            IFolderManager folderManager = Container.Resolve<IFolderManager>();
            folderManager.CreateFoldersAtStartUp();

            var dbPath = Cs4rsa.App.Properties.Resources.DbPath;
            if (!File.Exists(dbPath))
            {
                RawSql rawSql = Container.Resolve<RawSql>();
                IUnitOfWork unitOfWork = Container.Resolve<IUnitOfWork>();
                ICourseCrawler courseCrawler = Container.Resolve<ICourseCrawler>();
                DisciplineCrawler disciplineCrawler = Container.Resolve<DisciplineCrawler>();

                rawSql.CreateDbIfNotExist(dbPath, Cs4rsa.App.Properties.Resources.MigratePath);
                courseCrawler.GetInfo(out string yearInfo, out string yearValue, out string semesterInfo, out string semesterValue);
                List<Discipline> disciplines = disciplineCrawler.GetDisciplineAndKeyword(semesterValue);
                // Seed Settings
                unitOfWork.Settings.InsertSemesterSetting(yearInfo, yearValue, semesterInfo, semesterValue);
                string sql = BulkInsertDisciplines.GetBulkInsertSql(disciplines);
                rawSql.ExecNonQuery(sql);
            }

            /**
             * Don't forget to remove the StartupUri property from the PrismApplication tag. 
             * Otherwise, you will end up with two window instances.
             */
            var w = Container.Resolve<MainWindow>();
            // https://prismlibrary.github.io/docs/wpf/dialog-service.html

            return w;
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ManuallyScheduleModule>();
        }


        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var viewModelName = $"{viewName}ViewModel, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }
    }
}
