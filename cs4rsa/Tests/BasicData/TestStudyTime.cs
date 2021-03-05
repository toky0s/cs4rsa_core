using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.BasicData;


namespace cs4rsa.Tests.BasicData
{
    [TestFixture]
    class TestStudyTime
    {
        public StudyTime studyTime1;
        public StudyTime studyTime2;
        public StudyTime studyTime3;
        public StudyTime studyTime4;
        public StudyTime studyTime5;
        public StudyTime studyTime6;
        public StudyTime studyTime7;
        List<StudyTime> studyTimes;

        [SetUp]
        public void Setup()
        {
            studyTime1 = new StudyTime("15:15", "17:15");
            studyTime2 = new StudyTime("07:00", "09:00");
            studyTime3 = new StudyTime("09:15", "11:15");
            studyTime4 = new StudyTime("13:00", "15:00");
            studyTime5 = new StudyTime("17:45", "21:00");
            studyTime6 = new StudyTime("07:00", "10:15");
            studyTime7 = new StudyTime("07:00", "11:15");

            studyTimes = new List<StudyTime>
            {
                studyTime1,
                studyTime2,
                studyTime3,
                studyTime4,
                studyTime5,
                studyTime6,
                studyTime7
            };
        }

        [Test]
        public void TestGetStudyTimeIntersect1()
        {
            StudyTimeIntersect studyTimeIntersect = StudyTimeManipulation.GetStudyTimeIntersect(studyTime2, studyTime6);
            Assert.AreEqual("07:00",studyTimeIntersect.Start);
            Assert.AreEqual("09:00",studyTimeIntersect.End);
        }

        [Test]
        public void TestGetStudyTimeIntersect2()
        {
            StudyTimeIntersect studyTimeIntersect = StudyTimeManipulation.GetStudyTimeIntersect(studyTime7, studyTime6);
            Assert.AreEqual("07:00", studyTimeIntersect.Start);
            Assert.AreEqual("10:15", studyTimeIntersect.End);
        }

        [Test]
        public void TestGetStudyTimeIntersect3()
        {
            StudyTimeIntersect studyTimeIntersect = StudyTimeManipulation.GetStudyTimeIntersect(studyTime6, studyTime7);
            Assert.AreEqual("07:00", studyTimeIntersect.Start);
            Assert.AreEqual("10:15", studyTimeIntersect.End);
        }

        [Test]
        public void TestPairStudyTime()
        {
            List<Tuple<StudyTime, StudyTime>> tuples = StudyTimeManipulation.PairStudyTimes(studyTimes);
            Assert.AreEqual(21, tuples.Count());
        }

        [Test]
        public void TestGetStudyTimeIntersects()
        {
            List<Tuple<StudyTime, StudyTime>> tuples = StudyTimeManipulation.PairStudyTimes(studyTimes);
            List<StudyTimeIntersect> timeIntersects = StudyTimeManipulation.GetStudyTimeIntersects(tuples);
            Assert.AreEqual(5, timeIntersects.Count());
        }
    }
}
