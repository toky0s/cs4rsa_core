using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Implements;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using NUnit.Framework;
using System.Threading.Tasks;
using TeacherCrawlerService1.Crawlers;

namespace TestCs4rsa.TeacherCrawlerService
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test, Category("TestInLocal")]
        public async Task GetTeacherInfo()
        {
            Cs4rsaDbContext context = new Cs4rsaDbContext();
            IUnitOfWork unitOfWork = new UnitOfWork(context);
            context.Database.EnsureCreated();
            string url = @"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_lecturerdetail&timespan=71&intructorid=221111108&classid=132070&academicleveltypeid=&curriculumid=";
            TeacherCrawler teacherCrawler = new TeacherCrawler(unitOfWork);
            Teacher teacher = await teacherCrawler.Crawl(url);
            Assert.AreEqual("HỒ LÊ VIẾT NIN", teacher.Name);
        }
    }
}