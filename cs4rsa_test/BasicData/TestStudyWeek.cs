using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.BasicData;

namespace cs4rsa_test.BasicData
{
    [TestFixture]
    class TestStudyWeek
    {
        public StudyWeek stw;

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Phase2Test1()
        {
            stw = new StudyWeek(36, 43);
            Assert.AreEqual(Phase.SECOND, stw.GetPhase());
        }

        [Test]
        public void Phase2Test2()
        {
            stw = new StudyWeek(37, 43);
            Assert.AreEqual(Phase.SECOND, stw.GetPhase());
        }

        [Test]
        public void Phase2Test3()
        {
            stw = new StudyWeek(11, 18);
            Assert.AreEqual(Phase.SECOND, stw.GetPhase());
        }

        [Test]
        public void Phase2Test4()
        {
            stw = new StudyWeek(11, 15);
            Assert.AreEqual(Phase.SECOND, stw.GetPhase());
        }

        [Test]
        public void Phase1Test1()
        {
            stw = new StudyWeek(26, 33);
            Assert.AreEqual(Phase.FIRST, stw.GetPhase());
        }

        [Test]
        public void Phase1Test2()
        {
            stw = new StudyWeek(1, 8);
            Assert.AreEqual(Phase.FIRST, stw.GetPhase());
        }

        [Test]
        public void Phase1Test3()
        {
            stw = new StudyWeek(1, 5);
            Assert.AreEqual(Phase.FIRST, stw.GetPhase());
        }

        [Test]
        public void PhaseAllTest1()
        {
            stw = new StudyWeek(2, 18);
            Assert.AreEqual(Phase.ALL, stw.GetPhase());
        }

        [Test]
        public void PhaseAllTest2()
        {
            stw = new StudyWeek(26, 39);
            Assert.AreEqual(Phase.ALL, stw.GetPhase());
        }

        [Test]
        public void PhaseAllStringTest1()
        {
            stw = new StudyWeek("26--39");
            Assert.AreEqual(Phase.ALL, stw.GetPhase());
        }

        [Test]
        public void GetStudyNumberOfWeeks()
        {
            stw = new StudyWeek("26--39");
            Assert.AreEqual(14, stw.GetStudyNumberOfWeeks());
        }
    }
}
