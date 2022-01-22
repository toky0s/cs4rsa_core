using CourseSearchService.Crawlers;
using CourseSearchService.Crawlers.Interfaces;
using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Implements;
using Cs4rsaDatabaseService.Interfaces;
using CurriculumCrawlerService.Crawlers;
using CurriculumCrawlerService.Crawlers.Interfaces;
using DisciplineCrawlerService.Crawlers;
using HelperService;
using Microsoft.Extensions.DependencyInjection;
using ProgramSubjectCrawlerService.Crawlers;
using StudentCrawlerService.Crawlers;
using StudentCrawlerService.Crawlers.Interfaces;
using SubjectCrawlService1.Crawlers;
using SubjectCrawlService1.Crawlers.Interfaces;
using TeacherCrawlerService1.Crawlers;
using TeacherCrawlerService1.Crawlers.Interfaces;

namespace TestCs4rsa
{
    internal class MainSetup
    {
        public static ServiceProvider GetServiceContainer()
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
            services.AddSingleton<ColorGenerator>();
            return services.BuildServiceProvider();
        }
    }
}
