﻿using System;
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
    class TestDTUSubjectCrawler
    {
        [Test]
        public void Run()
        {
            DtuSubjectCrawler crawler = new DtuSubjectCrawler(InfoForTest.sessionId, "95");
            string real = crawler.PrerequisiteSubjects[0];
            Assert.AreEqual("CS 211", real);
        }
    }
}