using CourseSearchService.Crawlers;
using CourseSearchService.Crawlers.Interfaces;
using Cs4rsaDatabaseService.DataProviders;
using DisciplineCrawlerService.Crawlers;
using HelperService;
using SubjectCrawlService1.Crawlers;
using SubjectCrawlService1.Crawlers.Interfaces;
using SubjectCrawlService1.DataTypes;
using TeacherCrawlerService1.Crawlers;
using TeacherCrawlerService1.Crawlers.Interfaces;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Cs4rsaDatabaseService.Implements;
using Cs4rsaDatabaseService.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace TestCs4rsa.SubjectCrawlService
{
    public class Tests
    {
        private ServiceProvider _serviceProvider;
        [SetUp]
        public void Setup()
        {
            _serviceProvider = MainSetup.GetServiceContainer();
        }

        [Test]
        public async Task GetSubject1()
        {
            Cs4rsaDbContext cs4RsaDbContext = _serviceProvider.GetService<Cs4rsaDbContext>();
            if (cs4RsaDbContext.Database.EnsureCreated() == true)
            {
                DisciplineCrawler disciplineCrawler = _serviceProvider.GetService<DisciplineCrawler>();
                disciplineCrawler.GetDisciplineAndKeyword();
            }

            ISubjectCrawler subjectCrawler = _serviceProvider.GetService<ISubjectCrawler>();
            Subject subject = await subjectCrawler.Crawl("CS", "414");
            Assert.AreEqual(3, subject.StudyUnit);
        }

        public async Task GetSubject2()
        {
            Cs4rsaDbContext cs4RsaDbContext = _serviceProvider.GetService<Cs4rsaDbContext>();
            if (cs4RsaDbContext.Database.EnsureCreated() == true)
            {
                DisciplineCrawler disciplineCrawler = _serviceProvider.GetService<DisciplineCrawler>();
                disciplineCrawler.GetDisciplineAndKeyword();
            }

            ISubjectCrawler subjectCrawler = _serviceProvider.GetService<ISubjectCrawler>();
            Subject subject = await subjectCrawler.Crawl("AAA", "414");
        }

        [Test, Category("TestInLocal")]
        public void ThisWillPassIfExceptionThrownAsync()
        {
            Assert.ThrowsAsync<NullReferenceException>(async () => await GetSubject2());
        }

        [Test]
        public async Task GetSubject3()
        {
            Cs4rsaDbContext cs4RsaDbContext = _serviceProvider.GetService<Cs4rsaDbContext>();
            if (cs4RsaDbContext.Database.EnsureCreated() == true)
            {
                DisciplineCrawler disciplineCrawler = _serviceProvider.GetService<DisciplineCrawler>();
                disciplineCrawler.GetDisciplineAndKeyword();
            }

            ISubjectCrawler subjectCrawler = _serviceProvider.GetService<ISubjectCrawler>();
            Subject subject = await subjectCrawler.Crawl(95);
            Assert.AreEqual(3, subject.StudyUnit);
        }

        [Test]
        public async Task GetPreSubject1()
        {
            IPreParSubjectCrawler preParSubjectCrawler = _serviceProvider.GetService<IPreParSubjectCrawler>();
            PreParContainer preParContainer = await preParSubjectCrawler.Run("95");
            Assert.AreEqual(2, preParContainer.PrerequisiteSubjects.Count);
        }

        [Test]
        public async Task GetParSubject1()
        {
            IPreParSubjectCrawler preParSubjectCrawler = _serviceProvider.GetService<IPreParSubjectCrawler>();
            PreParContainer preParContainer = await preParSubjectCrawler.Run("95");
            Assert.AreEqual(0, preParContainer.ParallelSubjects.Count);
        }

        [Test]
        public async Task GetPreSubject2()
        {
            IPreParSubjectCrawler preParSubjectCrawler = _serviceProvider.GetService<IPreParSubjectCrawler>();
            PreParContainer preParContainer = await preParSubjectCrawler.Run("1231");
            Assert.AreEqual(0, preParContainer.PrerequisiteSubjects.Count);
        }

        [Test]
        public async Task GetParSubject2()
        {
            IPreParSubjectCrawler preParSubjectCrawler = _serviceProvider.GetService<IPreParSubjectCrawler>();
            PreParContainer preParContainer = await preParSubjectCrawler.Run("1231");
            Assert.AreEqual(0, preParContainer.ParallelSubjects.Count);
        }
    }
}