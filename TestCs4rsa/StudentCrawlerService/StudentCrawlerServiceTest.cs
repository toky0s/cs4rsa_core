using NUnit.Framework;
using StudentCrawlerService.Crawlers;
using StudentCrawlerService.Interfaces;
using Cs4rsaDatabaseService.DataProviders;
using Autofac;
using Cs4rsaDatabaseService.Models;

namespace TestCs4rsa.StudentCrawlerService
{
    class StudentCrawlerServiceTest
    {
        public IContainer _container;
        [SetUp]
        public void Setup()
        {
            ContainerBuilder builder = new();
            builder.RegisterInstance(new Cs4rsaDbContext()).AsSelf();
            builder.RegisterType<DtuStudentInfoCrawler>().As<IDtuStudentInfoCrawler>().SingleInstance();
            _container = builder.Build();
        }

        [Test]
        public void CrawlStudentInfo1()
        {
            using (_container.BeginLifetimeScope())
            {
                Cs4rsaDbContext cs4RsaDbContext = _container.Resolve<Cs4rsaDbContext>();
                if (cs4RsaDbContext.Database.EnsureCreated() == true)
                {
                    IDtuStudentInfoCrawler dtuStudentInfoCrawler = _container.Resolve<IDtuStudentInfoCrawler>();
                    Student student = dtuStudentInfoCrawler.Crawl("ppxdPtQCkOX2 rc5tqBFhg==");
                    Assert.AreEqual("Trương A Xin", student.Name);
                }
            }

        }
        
        [Test]
        public void CrawlStudentInfo2()
        {
            Cs4rsaDbContext cs4RsaDbContext = _container.Resolve<Cs4rsaDbContext>();
            if (cs4RsaDbContext.Database.EnsureCreated() == true)
            {
                IDtuStudentInfoCrawler dtuStudentInfoCrawler = _container.Resolve<IDtuStudentInfoCrawler>();
                Student student = dtuStudentInfoCrawler.Crawl("ppxdPtQCkOX2 rc5tqBFhg==");
                Assert.AreEqual("truongaxin@dtu.edu.vn", student.Email);
            }

        }
        
        [Test]
        public void CrawlStudentInfo3()
        {
            Cs4rsaDbContext cs4RsaDbContext = _container.Resolve<Cs4rsaDbContext>();
            if (cs4RsaDbContext.Database.EnsureCreated() == true)
            {
                IDtuStudentInfoCrawler dtuStudentInfoCrawler = _container.Resolve<IDtuStudentInfoCrawler>();
                Student student = dtuStudentInfoCrawler.Crawl("ppxdPtQCkOX2 rc5tqBFhg==");
                Assert.AreEqual("", student.PhoneNumber);
            }
        }
    }
}
