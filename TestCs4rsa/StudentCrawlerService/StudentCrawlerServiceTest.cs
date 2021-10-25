using NUnit.Framework;
using StudentCrawlerService.Crawlers;
using Cs4rsaDatabaseService.DataProviders;
using Autofac;
using Cs4rsaDatabaseService.Models;
using StudentCrawlerService.Crawlers.Interfaces;
using System.Threading.Tasks;

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

        [Test, Category("TestInLocal")]
        public async Task CrawlStudentInfo1()
        {
            using (_container.BeginLifetimeScope())
            {
                Cs4rsaDbContext cs4RsaDbContext = _container.Resolve<Cs4rsaDbContext>();
                if (cs4RsaDbContext.Database.EnsureCreated() == true)
                {
                    IDtuStudentInfoCrawler dtuStudentInfoCrawler = _container.Resolve<IDtuStudentInfoCrawler>();
                    Student student = await dtuStudentInfoCrawler.Crawl("ppxdPtQCkOX2 rc5tqBFhg==");
                    Assert.AreEqual("Trương A Xin", student.Name);
                }
            }

        }
        
        [Test, Category("TestInLocal")]
        public async Task CrawlStudentInfo2()
        {
            Cs4rsaDbContext cs4RsaDbContext = _container.Resolve<Cs4rsaDbContext>();
            if (cs4RsaDbContext.Database.EnsureCreated() == true)
            {
                IDtuStudentInfoCrawler dtuStudentInfoCrawler = _container.Resolve<IDtuStudentInfoCrawler>();
                Student student = await dtuStudentInfoCrawler.Crawl("ppxdPtQCkOX2 rc5tqBFhg==");
                Assert.AreEqual("truongaxin@dtu.edu.vn", student.Email);
            }

        }
        
        [Test, Category("TestInLocal")]
        public async Task CrawlStudentInfo3()
        {
            Cs4rsaDbContext cs4RsaDbContext = _container.Resolve<Cs4rsaDbContext>();
            if (cs4RsaDbContext.Database.EnsureCreated() == true)
            {
                IDtuStudentInfoCrawler dtuStudentInfoCrawler = _container.Resolve<IDtuStudentInfoCrawler>();
                Student student = await dtuStudentInfoCrawler.Crawl("ppxdPtQCkOX2 rc5tqBFhg==");
                Assert.AreEqual("", student.PhoneNumber);
            }
        }
    }
}
