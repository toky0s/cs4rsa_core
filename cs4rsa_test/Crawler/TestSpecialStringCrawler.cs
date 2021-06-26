using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Crawler;
using NUnit.Framework;
using cs4rsa_test.Crawler;

namespace cs4rsa_test.Crawler
{
    [TestFixture]
    class TestSpecialStringCrawler
    {
        [Test]
        public void GetMyName()
        {
            SpecialStringCrawler specialStringCrawler = new SpecialStringCrawler(InfoForTest.sessionId);
            string result = SpecialStringCrawler.GetSpecialString();
            Assert.AreEqual("ppxdPtQCkOX2+rc5tqBFhg==", result);
        }
    }
}
