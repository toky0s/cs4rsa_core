using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.Crawler;
using cs4rsa.BasicData;

namespace cs4rsaTest.Crawler
{
    [TestFixture]
    class TestSubjectCrawler
    {
        SubjectCrawler subjectCrawler;

        [SetUp]
        public void Setup()
        {
            subjectCrawler = new SubjectCrawler("CS", "414");
        }

        [Test]
        public void GetSubject()
        {
            Subject sj = subjectCrawler.ToSubject();
            Assert.AreEqual("CS 414", sj.SubjectCode);
        }

        [Test]
        public void GetListTrTagInCalendar()
        {
            Subject sj = subjectCrawler.ToSubject();
            Assert.AreEqual(21, sj.GetListTrTagInCalendar().Count());
        }

        [Test]
        public void GetListTrTagClassGroup()
        {
            Subject sj = subjectCrawler.ToSubject();
            Assert.AreEqual(7, sj.GetClassGroupNames().Count());
        }

        [Test]
        public void FirstItemClassGroupName()
        {
            Subject sj = subjectCrawler.ToSubject();
            string classGroupName = sj.GetClassGroupNames()[0];
            Assert.AreEqual("CS 414 B", classGroupName);
        }
    }
}
