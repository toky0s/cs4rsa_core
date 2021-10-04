using Autofac;
using CourseSearchService.Crawlers;
using CourseSearchService.Crawlers.Interfaces;
using Cs4rsaDatabaseService.DataProviders;
using DisciplineCrawlerService.Crawlers;
using HelperService;
using NUnit.Framework;
using SubjectCrawlService1.BasicDatas;
using SubjectCrawlService1.Crawlers;
using SubjectCrawlService1.Crawlers.Interfaces;
using TeacherCrawlerService1.Crawlers;
using TeacherCrawlerService1.Crawlers.Interfaces;

namespace TestCs4rsa.SubjectCrawlService
{
    public class Tests
    {
        public IContainer _container;
        [SetUp]
        public void Setup()
        {
            ContainerBuilder builder = new();
            builder.RegisterInstance(new CourseCrawler()).As<ICourseCrawler>().SingleInstance();
            builder.RegisterInstance(new Cs4rsaDbContext()).AsSelf();
            builder.RegisterType<SubjectCrawler>().As<ISubjectCrawler>();
            builder.RegisterType<TeacherCrawler>().As<ITeacherCrawler>();
            builder.RegisterType<ColorGenerator>().AsSelf().SingleInstance();
            builder.RegisterType<DisciplineCrawler>().AsSelf();
            _container = builder.Build();
        }

        [Test]
        public void GetSubject1()
        {
            using (_container.BeginLifetimeScope())
            {
                Cs4rsaDbContext cs4RsaDbContext = _container.Resolve<Cs4rsaDbContext>();
                cs4RsaDbContext.Database.EnsureCreated();

                ISubjectCrawler subjectCrawler = _container.Resolve<ISubjectCrawler>();
                Subject subject = subjectCrawler.Crawl("CS", "414");
                Assert.AreEqual(3, subject.StudyUnit);
            }
        }
    }
}