using System.Linq;
using NUnit.Framework;
using cs4rsa.BasicData;
using cs4rsa.Crawler;

namespace cs4rsa.Tests.Crawler
{
    [TestFixture]
    class TestSubjectCrawler2
    {
        public SubjectCrawler subjectCrawler;

        [SetUp]
        public void Setup()
        {
            subjectCrawler = new SubjectCrawler("CR", "250");
        }

        [Test]
        public void GetSubject()
        {
            Subject sj = subjectCrawler.ToSubject();
            Assert.AreEqual("CR 250", sj.SubjectCode);
        }

        [Test]
        public void GetListTrTagInCalendar()
        {
            Subject sj = subjectCrawler.ToSubject();
            Assert.AreEqual(2, sj.GetListTrTagInCalendar().Count());
        }

        [Test]
        public void GetListTrTagClassGroup()
        {
            Subject sj = subjectCrawler.ToSubject();
            Assert.AreEqual(1, sj.GetClassGroupNames().Count());
        }

        [Test]
        public void FirstItemClassGroupName()
        {
            Subject sj = subjectCrawler.ToSubject();
            string classGroupName = sj.GetClassGroupNames()[0];
            Assert.AreEqual("CR 250 B", classGroupName);
        }

        [Test]
        public void GetTrTagsWithClassLop()
        {
            Subject sj = subjectCrawler.ToSubject();
            var schoolClasses = sj.GetTrTagsWithClassLop();
            Assert.AreEqual(1, schoolClasses.Count());
        }
    }
}
