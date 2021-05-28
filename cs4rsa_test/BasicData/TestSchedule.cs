using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Helpers;


namespace cs4rsa.Tests.BasicData
{
    [TestFixture]
    class TestSchedule
    {
        public Subject subject;
        public SubjectCrawler subjectCrawler;

        [SetUp]
        public void Setup()
        {
            subjectCrawler = new SubjectCrawler("CS", "414");
            subject = subjectCrawler.ToSubject();
        }

    }
}
