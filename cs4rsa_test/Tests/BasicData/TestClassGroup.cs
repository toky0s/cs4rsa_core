using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.BasicData;
using cs4rsa.Crawler;

namespace cs4rsa.Tests.BasicData
{
    [TestFixture]
    class TestClassGroup
    {
        [SetUp]
        public void Setup()
        {
            SubjectCrawler subjectCrawler = new SubjectCrawler("CS","414");
            Subject subject = subjectCrawler.ToSubject();
            List<ClassGroup> classGroups = subject.ClassGroups;
            ClassGroup classGroup1 = classGroups[0];
            ClassGroup classGroup2 = classGroups[1];
        }

        public void TestGetStudyWeek()
        {

        }

    }
}
