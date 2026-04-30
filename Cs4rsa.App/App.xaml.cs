using Cs4rsa.App.Views;
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
using Prism.Services.Dialogs;

using Serilog;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Cs4rsa.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
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

            courseCrawler.GetInfo(out string yearInfo, out string yearValue, out string semesterInfo, out string semesterValue);
            folderManager.CreateFoldersAtStartUp();
            List<Discipline> disciplines = disciplineCrawler.GetDisciplineAndKeyword(semesterValue);

            if (!File.Exists("cs4rsa.db"))
            {
                rawSql.CreateDbIfNotExist("cs4rsa.db", "DataProviders/cs4rsa.db.sql");

                // Seed Settings
                unitOfWork.Settings.InsertSemesterSetting(yearInfo, yearValue, semesterInfo, semesterValue);
                string sql = BulkInsertDisciplines.GetBulkInsertSql(disciplines);
                rawSql.ExecNonQuery(sql);
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            string cnnStr = ConfigurationManager.AppSettings["cnnStr"];
            containerRegistry.Register<RawSql>(provider => new RawSql(cnnStr));

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

            containerRegistry.RegisterSingleton<ISemesterHtmlGetter, SemesterHtmlGetter>();
            containerRegistry.RegisterSingleton<IDisciplineHtmlGetter, DisciplineHtmlGetter>();
            containerRegistry.RegisterSingleton<ICourseCrawler, CourseCrawler>();
            containerRegistry.RegisterSingleton<DisciplineCrawler>();
            
            containerRegistry.RegisterSingleton<IUnitOfWork, UnitOfWork>();
            containerRegistry.RegisterSingleton<ISubjectCrawler, SubjectCrawler>();
            containerRegistry.RegisterSingleton<IOpenInBrowser, OpenInBrowser>();
            containerRegistry.RegisterSingleton<IFolderManager, FolderManager>();
            containerRegistry.RegisterSingleton<ISnackbarMessageQueue, SnackbarMessageQueue>();

            containerRegistry.RegisterDialog<ScheduleDetailUC, ScheduleDetailUCViewModel>();
            containerRegistry.RegisterDialog<ShowDetailsSubjectUC, ShowDetailsSubjectUCViewModel>();
            containerRegistry.RegisterDialog<SaveSessionUC, SaveSessionUCViewModel>();
        }

        protected override Window CreateShell()
        {
            RawSql rawSql = Container.Resolve<RawSql>();
            IUnitOfWork unitOfWork = Container.Resolve<IUnitOfWork>();
            DisciplineCrawler disciplineCrawler = Container.Resolve<DisciplineCrawler>();
            IFolderManager folderManager = Container.Resolve<IFolderManager>();
            ICourseCrawler courseCrawler = Container.Resolve<ICourseCrawler>();

            courseCrawler.GetInfo(out string yearInfo, out string yearValue, out string semesterInfo, out string semesterValue);
            folderManager.CreateFoldersAtStartUp();
            List<Discipline> disciplines = disciplineCrawler.GetDisciplineAndKeyword(semesterValue);

            if (!File.Exists("cs4rsa.db"))
            {
                rawSql.CreateDbIfNotExist("cs4rsa.db", "DataProviders/cs4rsa.db.sql");

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
