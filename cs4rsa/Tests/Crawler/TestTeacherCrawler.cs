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
        [Test]
        public void GetID()
        {
            string url = @"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_lecturerdetail&timespan=71&intructorid=010132007&classid=139631&academicleveltypeid=&curriculumid=";
            TeacherCrawler teacherCrawler = new TeacherCrawler(url);
            Assert.AreEqual("010132007", teacherCrawler.ToTeacher().Id);
        }

        [Test]
        public void GetTeacher1()
        {
            string intructionId = "171111012";
            string semesterValue = "70";
            TeacherCrawler teacherCrawler = new TeacherCrawler(semesterValue, intructionId);
            Teacher teacher = teacherCrawler.ToTeacher();
            Assert.AreEqual("NGUYỄN KIM TUẤN", teacher.Name);
        }
    }
}
