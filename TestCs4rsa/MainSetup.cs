using Microsoft.Extensions.DependencyInjection;
using StudentCrawlerService.Crawlers;
using Cs4rsaDatabaseService.DataProviders;
using StudentCrawlerService.Crawlers.Interfaces;
using Cs4rsaDatabaseService.Implements;
using Cs4rsaDatabaseService.Interfaces;
using CourseSearchService.Crawlers.Interfaces;
using CurriculumCrawlerService.Crawlers.Interfaces;
using TeacherCrawlerService1.Crawlers.Interfaces;
using SubjectCrawlService1.Crawlers.Interfaces;
using ProgramSubjectCrawlerService.Crawlers;
using DisciplineCrawlerService.Crawlers;
using CourseSearchService.Crawlers;
using CurriculumCrawlerService.Crawlers;
using TeacherCrawlerService1.Crawlers;
using SubjectCrawlService1.Crawlers;
using HelperService;

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
