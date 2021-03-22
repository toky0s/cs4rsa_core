using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.Crawler;
using cs4rsa.BasicData;

namespace cs4rsa.Tests.Crawler
{
    [TestFixture]
    class TestTeacherCrawler
    {
        public TeacherCrawler teacherCrawler;
        [SetUp]
        public void Setup()
        {
            teacherCrawler = new TeacherCrawler(@"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_lecturerdetail&timespan=71&intructorid=010132007&classid=139631&academicleveltypeid=&curriculumid=");
        }

        [Test]
        public void GetID()
        {
            Assert.AreEqual("010132007", teacherCrawler.ToTeacher().Id);
        }
    }
}
