﻿using CourseSearchService.Crawlers;
using CourseSearchService.Crawlers.Interfaces;
using CurriculumCrawlerService.Crawlers;
using DisciplineCrawlerService.Crawlers;
using ProgramSubjectCrawlerService.Crawlers;
using StudentCrawlerService.Crawlers;
using StudentCrawlerService.Interfaces;
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
            base.OnStartup(e);
        }
        private IServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDbContext<Cs4rsaDbContext>();
            services.AddSingleton<ICurriculumRepository, CurriculumRepository>();
            services.AddSingleton<IDisciplineRepository, DisciplineRepository>();
            services.AddSingleton<IKeywordRepository, KeywordRepository>();
            services.AddSingleton<IStudentRepository, StudentRepository>();
            services.AddSingleton<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<ICourseCrawler, CourseCrawler>();
            services.AddSingleton<CurriculumCrawler>();
            services.AddSingleton<ITeacherCrawler, TeacherCrawler>();
            services.AddSingleton<ISubjectCrawler, SubjectCrawler>();
            services.AddSingleton<IPreParSubjectCrawler, PreParSubjectCrawler>();
            services.AddSingleton<IDtuStudentInfoCrawler, DtuStudentInfoCrawler>();
            services.AddSingleton<ProgramDiagramCrawler>();
            services.AddSingleton<StudentProgramCrawler>();
            services.AddSingleton<DisciplineCrawler>();
            services.AddSingleton<ShareString>();
            services.AddSingleton<ColorGenerator>();

            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<SearchSessionViewModel>();
            services.AddScoped<ClassGroupSessionViewModel>();
            services.AddScoped<ChoiceSessionViewModel>();
            services.AddScoped<ScheduleTableViewModel>();
            services.AddScoped<MainSchedulingViewModel>();

            services.AddScoped<SaveSessionViewModel>();
            services.AddSingleton<ImportSessionViewModel>();

            services.AddScoped<ISetting, Setting>();
            // Model Extension
            services.AddSingleton<SessionExtension>();
            return services.BuildServiceProvider();
        }
    }
}
