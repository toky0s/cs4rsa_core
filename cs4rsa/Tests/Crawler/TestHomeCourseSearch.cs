using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Crawler;
using cs4rsa.BasicData;
using NUnit.Framework;

namespace cs4rsaTest.Crawler
{
    [TestFixture]
    class TestHomeCourseSearch
    {
        public HomeCourseSearch hcs;

        [SetUp]
        public void Setup()
        {
            hcs = new HomeCourseSearch();
        }

        [Test]
        public void GetCurrentSemesterValue()
        {
            Assert.AreEqual("71", hcs.CurrentSemesterValue);
        }

        [Test]
        public void GetCurrentYearValue()
        {
            Assert.AreEqual("69", hcs.CurrentYearValue);
        }

        [Test]
        public void GetCurrentSemesterInfo()
        {
            Assert.AreEqual("Học Kỳ II", hcs.CurrentSemesterInfo);
        }

        [Test]
        public void GetCurrentYearInfo()
        {
            Assert.AreEqual("Năm Học 2020-2021", hcs.CurrentYearInfo);
        }
    }
}
