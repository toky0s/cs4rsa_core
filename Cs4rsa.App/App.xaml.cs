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
using Cs4rsa.Module.Shared;
using Cs4rsa.Service.CourseCrawler.Crawlers;
using Cs4rsa.Service.CourseCrawler.Interfaces;
using Cs4rsa.Service.Dialog;
using Cs4rsa.Service.Dialog.Interfaces;
using Cs4rsa.Service.DisciplineCrawler;
using Cs4rsa.Service.SubjectCrawler.Crawlers;
using Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces;

using DryIoc;


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

using Xeplich.Service.Search;

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
            containerRegistry.Register<IndexBuilder>(provider => {
                var rawsql = provider.Resolve<RawSql>();
                return new IndexBuilder(rawsql);
            });

            containerRegistry.RegisterSingleton<ISemesterHtmlGetter, SemesterHtmlGetter>();
            containerRegistry.RegisterSingleton<IDisciplineHtmlGetter, DisciplineHtmlGetter>();
            containerRegistry.RegisterSingleton<ICourseCrawler, CourseCrawler>();
            containerRegistry.RegisterSingleton<DisciplineCrawler>();

            containerRegistry.RegisterSingleton<IUnitOfWork, UnitOfWork>();
            containerRegistry.RegisterSingleton<ISubjectCrawler, SubjectCrawler>();
            containerRegistry.RegisterSingleton<IOpenInBrowser, OpenInBrowser>();
            containerRegistry.RegisterSingleton<IFolderManager, FolderManager>();
            containerRegistry.RegisterSingleton<NetworkMonitor>();

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

            // Quét db, tạo index để search subject
            var indexBuilder = Container.Resolve<IndexBuilder>();
            indexBuilder.BuildIndex();

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(async () =>
            {
                NetworkMonitor networkMonitor = Container.Resolve<NetworkMonitor>();
                await networkMonitor.CheckInternetAsync();
            }));

            /**
            * Don't forget to remove the StartupUri property from the PrismApplication tag. 
            * Otherwise, you will end up with two window instances.
            */
            var w = Container.Resolve<MainWindow>();
            // https://prismlibrary.github.io/docs/wpf/dialog-service.html

            // Đăng ký MainWindow làm owner để toast bám vào cửa sổ
            w.Loaded += (a, b) =>
                Cs4rsa.Module.Shared.ToastService.Instance.SetOwner(Application.Current.MainWindow);

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
