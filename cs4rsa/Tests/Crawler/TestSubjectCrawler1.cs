using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using cs4rsa.Crawler;
using cs4rsa.BasicData;

namespace cs4rsa.Tests.Crawler
{
    [TestFixture]
    class TestSubjectCrawler1
    {
        public SubjectCrawler subjectCrawler;

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
            Assert.AreEqual(24, sj.GetListTrTagInCalendar().Count());
        }

        [Test]
        public void GetListTrTagClassGroup()
        {
            Subject sj = subjectCrawler.ToSubject();
            Assert.AreEqual(8, sj.GetClassGroupNames().Count());
        }

        [Test]
        public void FirstItemClassGroupName()
        {
            Subject sj = subjectCrawler.ToSubject();
            string classGroupName = sj.GetClassGroupNames()[0];
            Assert.AreEqual("CS 414 B", classGroupName);
        }

        [Test]
        public void GetTrTagsWithClassLop()
        {
            Subject sj = subjectCrawler.ToSubject();
            var schoolClasses = sj.GetTrTagsWithClassLop();
            Assert.AreEqual(16, schoolClasses.Count());
        }

        [Test]
        public void GetSchoolClassesSecondItem()
        {
            Subject subject = subjectCrawler.ToSubject();
            List<SchoolClass> schoolClasses = subject.GetSchoolClasses();
            Assert.AreEqual("CS 414 B1", schoolClasses[1].ClassGroupName);
        }
    }
}
