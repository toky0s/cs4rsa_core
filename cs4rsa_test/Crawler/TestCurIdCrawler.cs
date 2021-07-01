using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Crawler;
using NUnit.Framework;

namespace cs4rsa_test.Crawler
{
    [TestFixture]
    class TestCurIdCrawler
    {
        [Test]
        public void GetCurId()
        {
            string result = CurriculumCrawler.GetCurriculum("ppxdPtQCkOX2 rc5tqBFhg==").CurId;
            Assert.AreEqual("605", result);
        }
    }
}
