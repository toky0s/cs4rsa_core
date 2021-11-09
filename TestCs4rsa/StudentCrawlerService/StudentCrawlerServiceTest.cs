using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using StudentCrawlerService.Crawlers.Interfaces;
using System.Threading.Tasks;

namespace TestCs4rsa.StudentCrawlerService
{
    class StudentCrawlerServiceTest
    {
        private ServiceProvider _serviceProvider;
        [SetUp]
        public void Setup()
        {
            _serviceProvider = MainSetup.GetServiceContainer();
        }

        [Test, Category("TestInLocal")]
        public async Task CrawlStudentInfo1()
        {
            Cs4rsaDbContext cs4RsaDbContext = _serviceProvider.GetService<Cs4rsaDbContext>();
            if (cs4RsaDbContext.Database.EnsureCreated() == true)
            {
                IDtuStudentInfoCrawler dtuStudentInfoCrawler = _serviceProvider.GetService<IDtuStudentInfoCrawler>();
                Student student = await dtuStudentInfoCrawler.Crawl("ppxdPtQCkOX2 rc5tqBFhg==");
                Assert.AreEqual("Trương A Xin", student.Name);
            }
        }

        [Test, Category("TestInLocal")]
        public async Task CrawlStudentInfo2()
        {
            Cs4rsaDbContext cs4RsaDbContext = _serviceProvider.GetService<Cs4rsaDbContext>();
            if (cs4RsaDbContext.Database.EnsureCreated() == true)
            {
                IDtuStudentInfoCrawler dtuStudentInfoCrawler = _serviceProvider.GetService<IDtuStudentInfoCrawler>();
                Student student = await dtuStudentInfoCrawler.Crawl("ppxdPtQCkOX2 rc5tqBFhg==");
                Assert.AreEqual("truongaxin@dtu.edu.vn", student.Email);
            }

        }

        [Test, Category("TestInLocal")]
        public async Task CrawlStudentInfo3()
        {
            Cs4rsaDbContext cs4RsaDbContext = _serviceProvider.GetService<Cs4rsaDbContext>();
            if (cs4RsaDbContext.Database.EnsureCreated() == true)
            {
                IDtuStudentInfoCrawler dtuStudentInfoCrawler = _serviceProvider.GetService<IDtuStudentInfoCrawler>();
                Student student = await dtuStudentInfoCrawler.Crawl("ppxdPtQCkOX2 rc5tqBFhg==");
                Assert.AreEqual("", student.PhoneNumber);
            }
        }
    }
}
