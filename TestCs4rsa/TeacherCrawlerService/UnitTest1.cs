using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Models;
using NUnit.Framework;
using TeacherCrawlerService1.Crawlers;

namespace TestCs4rsa.TeacherCrawlerService
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetTeacherInfo()
        {
            using(Cs4rsaDbContext db = new Cs4rsaDbContext())
            {
                db.Database.EnsureCreated();
                string url = @"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_lecturerdetail&timespan=71&intructorid=221111108&classid=132070&academicleveltypeid=&curriculumid=";
                TeacherCrawler teacherCrawler = new TeacherCrawler(db);
                Teacher teacher = teacherCrawler.Crawl(url);
                Assert.AreEqual("HỒ LÊ VIẾT NIN", teacher.Name);
            }
        }
    }
}