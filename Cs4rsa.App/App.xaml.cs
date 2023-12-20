using Cs4rsa.App.Views;
using Cs4rsa.Common;
using Cs4rsa.Common.Interfaces;
using Cs4rsa.Database.DataProviders;
using Cs4rsa.Database.Implements;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Module.ManuallySchedule;
using Cs4rsa.Service.CourseCrawler.Crawlers;
using Cs4rsa.Service.CourseCrawler.Interfaces;
using Cs4rsa.Service.DisciplineCrawler;
using Cs4rsa.Module.ManuallySchedule.Utils;
using DryIoc;

using MaterialDesignThemes.Wpf;

using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Prism.Services.Dialogs;
using Cs4rsa.Service.Dialog.Interfaces;
using Cs4rsa.Service.Dialog;

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

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Khỏi tạo xong");
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

            containerRegistry.RegisterSingleton<ISemesterHtmlGetter, SemesterHtmlGetter>();
            containerRegistry.RegisterSingleton<IDisciplineHtmlGetter, DisciplineHtmlGetter>();
            containerRegistry.RegisterSingleton<ICourseCrawler, CourseCrawler>();
            containerRegistry.RegisterSingleton<DisciplineCrawler>();
            
            containerRegistry.RegisterSingleton<IUnitOfWork, UnitOfWork>();
            containerRegistry.RegisterSingleton<IOpenInBrowser, OpenInBrowser>();
            containerRegistry.RegisterSingleton<IFolderManager, FolderManager>();
        }

        protected override Window CreateShell()
        {
            //BackgroundWorker backgroundWorker = new BackgroundWorker();
            //backgroundWorker.DoWork += BackgroundWorker_DoWork;
            //backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            //backgroundWorker.RunWorkerAsync();

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
            return w;
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ManuallyScheduleModule>();
        }
    }
}
