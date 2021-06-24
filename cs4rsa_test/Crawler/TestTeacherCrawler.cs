using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.Crawler;
using cs4rsa.BasicData;

namespace cs4rsa_test.Crawler
{
    [TestFixture]
    class TestTeacherCrawler
    {
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
