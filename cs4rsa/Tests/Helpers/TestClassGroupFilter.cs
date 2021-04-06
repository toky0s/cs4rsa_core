using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Helpers;
using cs4rsa.Crawler;
using NUnit.Framework;


namespace cs4rsa.Tests.Helpers
{
    [TestFixture]
    class TestClassGroupFilter
    {
        public List<ClassGroup> classGroups = new List<ClassGroup>();

        [SetUp]
        public void Setup()
        {
            SubjectCrawler subjectCrawler = new SubjectCrawler("CS", "414");
            Subject subject = subjectCrawler.ToSubject();
            classGroups.AddRange(subject.ClassGroups);
            subjectCrawler = new SubjectCrawler("CS", "201");
            subject = subjectCrawler.ToSubject();
            classGroups.AddRange(subject.ClassGroups);
            subjectCrawler = new SubjectCrawler("CS", "347");
            subject = subjectCrawler.ToSubject();
            classGroups.AddRange(subject.ClassGroups);
            subjectCrawler = new SubjectCrawler("CS", "447");
            subject = subjectCrawler.ToSubject();
            classGroups.AddRange(subject.ClassGroups);
        }
        
        [Test]
        public void TestSoLuong()
        {
            Assert.AreEqual(34, classGroups.Count);
        }
    }
}
