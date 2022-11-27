using Cs4rsa.Services.ConflictSvc.Utils;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;

namespace Cs4rsaTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod("PhaseManipulation Case 1")]
        public void TestMethod1()
        {
            StudyWeek studyWeekF = new("1--8");
            StudyWeek studyWeekS = new("1--8");
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(studyWeekF, studyWeekS);
            Assert.AreEqual(phaseIntersect.StartWeek, 1);
            Assert.AreEqual(phaseIntersect.EndWeek, 8);
        }

        [TestMethod("PhaseManipulation Case 2")]
        public void TestMethod2()
        {
            StudyWeek studyWeekF = new("4--10");
            StudyWeek studyWeekS = new("2--17");
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(studyWeekF, studyWeekS);
            Assert.AreEqual(phaseIntersect.StartWeek, 4);
            Assert.AreEqual(phaseIntersect.EndWeek, 10);
        }

        [TestMethod("PhaseManipulation Case 3")]
        public void TestMethod3()
        {
            StudyWeek studyWeekF = new("2--17");
            StudyWeek studyWeekS = new("4--10");
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(studyWeekF, studyWeekS);
            Assert.AreEqual(phaseIntersect.StartWeek, 4);
            Assert.AreEqual(phaseIntersect.EndWeek, 10);
        }

        [TestMethod("PhaseManipulation Case 4")]
        public void TestMethod4()
        {
            StudyWeek studyWeekF = new("2--10");
            StudyWeek studyWeekS = new("2--18");
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(studyWeekF, studyWeekS);
            Assert.AreEqual(phaseIntersect.StartWeek, 2);
            Assert.AreEqual(phaseIntersect.EndWeek, 10);
        }

        [TestMethod("PhaseManipulation Case 5")]
        public void TestMethod5()
        {
            StudyWeek studyWeekF = new("2--18");
            StudyWeek studyWeekS = new("2--10");
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(studyWeekF, studyWeekS);
            Assert.AreEqual(phaseIntersect.StartWeek, 2);
            Assert.AreEqual(phaseIntersect.EndWeek, 10);
        }

        [TestMethod("PhaseManipulation Case 6")]
        public void TestMethod6()
        {
            StudyWeek studyWeekF = new("1--18");
            StudyWeek studyWeekS = new("5--18");
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(studyWeekF, studyWeekS);
            Assert.AreEqual(phaseIntersect.StartWeek, 5);
            Assert.AreEqual(phaseIntersect.EndWeek, 18);
        }

        [TestMethod("PhaseManipulation Case 7")]
        public void TestMethod7()
        {
            StudyWeek studyWeekF = new("5--11");
            StudyWeek studyWeekS = new("1--11");
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(studyWeekF, studyWeekS);
            Assert.AreEqual(phaseIntersect.StartWeek, 5);
            Assert.AreEqual(phaseIntersect.EndWeek, 11);
        }

        [TestMethod("PhaseManipulation Case 8")]
        public void TestMethod8()
        {
            StudyWeek studyWeekF = new("8--11");
            StudyWeek studyWeekS = new("1--7");
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(studyWeekF, studyWeekS);
            Assert.AreEqual(phaseIntersect.StartWeek, 0);
            Assert.AreEqual(phaseIntersect.EndWeek, 0);
        }

        [TestMethod("PhaseManipulation Case 9")]
        public void TestMethod9()
        {
            StudyWeek studyWeekF = new("1--7");
            StudyWeek studyWeekS = new("9--17");
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(studyWeekF, studyWeekS);
            Assert.AreEqual(phaseIntersect.StartWeek, 0);
            Assert.AreEqual(phaseIntersect.EndWeek, 0);
        }

        [TestMethod("PhaseManipulation Case 10")]
        public void TestMethod10()
        {
            StudyWeek studyWeekF = new("5--18");
            StudyWeek studyWeekS = new("2--10");
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(studyWeekF, studyWeekS);
            Assert.AreEqual(phaseIntersect.StartWeek, 5);
            Assert.AreEqual(phaseIntersect.EndWeek, 10);
        }

        [TestMethod("PhaseManipulation Case 11")]
        public void TestMethod11()
        {
            StudyWeek studyWeekF = new("2--10");
            StudyWeek studyWeekS = new("7--18");
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(studyWeekF, studyWeekS);
            Assert.AreEqual(phaseIntersect.StartWeek, 7);
            Assert.AreEqual(phaseIntersect.EndWeek, 10);
        }

        [TestMethod("PhaseManipulation Case 12")]
        public void TestMethod12()
        {
            StudyWeek studyWeekF = new("8--10");
            StudyWeek studyWeekS = new("1--8");
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(studyWeekF, studyWeekS);
            Assert.AreEqual(phaseIntersect.StartWeek, 8);
            Assert.AreEqual(phaseIntersect.EndWeek, 8);
        }

        [TestMethod("PhaseManipulation Case 13")]
        public void TestMethod13()
        {
            StudyWeek studyWeekF = new("1--10");
            StudyWeek studyWeekS = new("10--18");
            PhaseIntersect phaseIntersect = PhaseManipulation.GetPhaseIntersect(studyWeekF, studyWeekS);
            Assert.AreEqual(phaseIntersect.StartWeek, 10);
            Assert.AreEqual(phaseIntersect.EndWeek, 10);
        }
    }
}