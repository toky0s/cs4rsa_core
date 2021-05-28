using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.BasicData;
using cs4rsa.Helpers;

namespace cs4rsa.Tests.Helpers
{
    [TestFixture]
    class TestHelpers
    {
        [Test]
        public void TestThisSetInThatSet1()
        {
            List<DayOfWeek> DayOfWeeks = new List<DayOfWeek>
            {
                DayOfWeek.Friday,
                DayOfWeek.Monday
            };
            List<DayOfWeek> DayOfWeeks2 = new List<DayOfWeek>
            {
                DayOfWeek.Monday,
                DayOfWeek.Thursday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
            };
            Assert.AreEqual(false, Checker.ThisSetInThatSet<DayOfWeek>(DayOfWeeks, DayOfWeeks2));
        }

        [Test]
        public void TestThisSetInThatSet2()
        {
            List<DayOfWeek> DayOfWeeks = new List<DayOfWeek>
            {
                DayOfWeek.Friday,
                DayOfWeek.Monday
            };
            List<DayOfWeek> DayOfWeeks2 = new List<DayOfWeek>
            {
                DayOfWeek.Monday,
                DayOfWeek.Thursday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Friday
            };
            Assert.AreEqual(false, Checker.ThisSetInThatSet<DayOfWeek>(DayOfWeeks, DayOfWeeks2));
        }

        [Test]
        public void TestThisSetInThatSet3()
        {
            List<DayOfWeek> DayOfWeeks = new List<DayOfWeek>
            {
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday
            };
            List<DayOfWeek> DayOfWeeks2 = new List<DayOfWeek>
            {
                DayOfWeek.Monday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday
            };
            Assert.AreEqual(false, Checker.ThisSetInThatSet<DayOfWeek>(DayOfWeeks, DayOfWeeks2));
        }

        [Test]
        public void TestThisSetInThatSet4()
        {
            List<DayOfWeek> DayOfWeeks = new List<DayOfWeek>
            {
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday
            };
            List<DayOfWeek> DayOfWeeks2 = new List<DayOfWeek>
            {
                DayOfWeek.Thursday,
                DayOfWeek.Wednesday
            };
            Assert.AreEqual(true, Checker.ThisSetInThatSet<DayOfWeek>(DayOfWeeks, DayOfWeeks2));
        }

        [Test]
        public void TestThisSetInThatSet5()
        {
            List<DayOfWeek> DayOfWeeks = new List<DayOfWeek>
            {
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday
            };
            List<DayOfWeek> DayOfWeeks2 = new List<DayOfWeek>();
            Assert.AreEqual(true, Checker.ThisSetInThatSet<DayOfWeek>(DayOfWeeks, DayOfWeeks2));
        }

        [Test]
        public void TestThisSetInThatSet6()
        {
            List<Phase> phases = new List<Phase>
            {
                Phase.FIRST
            };
            List<Phase> phases2 = new List<Phase>();
            Assert.AreEqual(true, Checker.ThisSetInThatSet<Phase>(phases, phases2));
        }

        [Test]
        public void TestThisSetInThatSet7()
        {
            List<DayOfWeek> DayOfWeeks = new List<DayOfWeek>
            {
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday
            };
            List<DayOfWeek> DayOfWeeks2 = new List<DayOfWeek>();
            Assert.AreEqual(true, Checker.ThisSetInThatSet<DayOfWeek>(DayOfWeeks, DayOfWeeks2));
        }

        [Test]
        public void TestThisSetInThatSet8()
        {
            List<DayOfWeek> DayOfWeeks = new List<DayOfWeek>
            {
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday
            };
            List<DayOfWeek> DayOfWeeks2 = new List<DayOfWeek>();
            Assert.AreEqual(true, Checker.ThisSetInThatSet<DayOfWeek>(DayOfWeeks, DayOfWeeks2));
        }

        [Test]
        public void TestThisSetInThatSet9()
        {
            List<DayOfWeek> DayOfWeeks = new List<DayOfWeek>
            {
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday
            };
            List<DayOfWeek> DayOfWeeks2 = new List<DayOfWeek>();
            Assert.AreEqual(true, Checker.ThisSetInThatSet<DayOfWeek>(DayOfWeeks, DayOfWeeks2));
        }
    }
}
