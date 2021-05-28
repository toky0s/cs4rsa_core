using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.Crawler;
using cs4rsa.BasicData;

namespace cs4rsa.Tests.BasicData
{
    [TestFixture]
    class TestConflict
    {
        [SetUp]
        public void Setup()
        {
            SubjectCrawler sjc1 = new SubjectCrawler("CS", "414");
            SubjectCrawler sjc2 = new SubjectCrawler("CR", "250");
        }
    }
}
