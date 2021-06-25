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
    class TestSpecialStringCrawler
    {
        private string sessionId = "mjtb13tex0zcguf4iyrlcpur";
        [Test]
        public void GetMyName()
        {
            SpecialStringCrawler specialStringCrawler = new SpecialStringCrawler(sessionId);
            string result = SpecialStringCrawler.GetSpecialString();
            Assert.AreEqual("ppxdPtQCkOX2+rc5tqBFhg==", result);
        }
    }
}
