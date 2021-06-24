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
        [Test]
        public void GetMyName()
        {
            SpecialStringCrawler specialStringCrawler = new SpecialStringCrawler("4qsekl50amdo1dhyvrdq0kt1");
            string result = SpecialStringCrawler.GetSpecialString();
            Assert.AreEqual("ppxdPtQCkOX2+rc5tqBFhg==", result);
        }
    }
}
