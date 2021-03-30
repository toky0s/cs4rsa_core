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
            List<WeekDate> weekDates = new List<WeekDate>
            {
                WeekDate.FRIDAY,
                WeekDate.MONDAY
            };
            List<WeekDate> weekDates2 = new List<WeekDate>
            {
                WeekDate.MONDAY,
                WeekDate.THURDAY,
                WeekDate.TUSEDAY,
                WeekDate.WEDNESDAY,
            };
            Assert.AreEqual(false, Checker.ThisSetInThatSet<WeekDate>(weekDates, weekDates2));
        }

        [Test]
        public void TestThisSetInThatSet2()
        {
            List<WeekDate> weekDates = new List<WeekDate>
            {
                WeekDate.FRIDAY,
                WeekDate.MONDAY
            };
            List<WeekDate> weekDates2 = new List<WeekDate>
            {
                WeekDate.MONDAY,
                WeekDate.THURDAY,
                WeekDate.TUSEDAY,
                WeekDate.WEDNESDAY,
                WeekDate.FRIDAY
            };
            Assert.AreEqual(false, Checker.ThisSetInThatSet<WeekDate>(weekDates, weekDates2));
        }

        [Test]
        public void TestThisSetInThatSet3()
        {
            List<WeekDate> weekDates = new List<WeekDate>
            {
                WeekDate.TUSEDAY,
                WeekDate.WEDNESDAY
            };
            List<WeekDate> weekDates2 = new List<WeekDate>
            {
                WeekDate.MONDAY,
                WeekDate.THURDAY,
                WeekDate.FRIDAY
            };
            Assert.AreEqual(false, Checker.ThisSetInThatSet<WeekDate>(weekDates, weekDates2));
        }

        [Test]
        public void TestThisSetInThatSet4()
        {
            List<WeekDate> weekDates = new List<WeekDate>
            {
                WeekDate.TUSEDAY,
                WeekDate.WEDNESDAY,
                WeekDate.THURDAY
            };
            List<WeekDate> weekDates2 = new List<WeekDate>
            {
                WeekDate.THURDAY,
                WeekDate.WEDNESDAY
            };
            Assert.AreEqual(true, Checker.ThisSetInThatSet<WeekDate>(weekDates, weekDates2));
        }

        [Test]
        public void TestThisSetInThatSet5()
        {
            List<WeekDate> weekDates = new List<WeekDate>
            {
                WeekDate.TUSEDAY,
                WeekDate.WEDNESDAY,
                WeekDate.THURDAY
            };
            List<WeekDate> weekDates2 = new List<WeekDate>();
            Assert.AreEqual(true, Checker.ThisSetInThatSet<WeekDate>(weekDates, weekDates2));
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
            List<WeekDate> weekDates = new List<WeekDate>
            {
                WeekDate.TUSEDAY,
                WeekDate.WEDNESDAY,
                WeekDate.THURDAY
            };
            List<WeekDate> weekDates2 = new List<WeekDate>();
            Assert.AreEqual(true, Checker.ThisSetInThatSet<WeekDate>(weekDates, weekDates2));
        }

        [Test]
        public void TestThisSetInThatSet8()
        {
            List<WeekDate> weekDates = new List<WeekDate>
            {
                WeekDate.TUSEDAY,
                WeekDate.WEDNESDAY,
                WeekDate.THURDAY
            };
            List<WeekDate> weekDates2 = new List<WeekDate>();
            Assert.AreEqual(true, Checker.ThisSetInThatSet<WeekDate>(weekDates, weekDates2));
        }

        [Test]
        public void TestThisSetInThatSet9()
        {
            List<WeekDate> weekDates = new List<WeekDate>
            {
                WeekDate.TUSEDAY,
                WeekDate.WEDNESDAY,
                WeekDate.THURDAY
            };
            List<WeekDate> weekDates2 = new List<WeekDate>();
            Assert.AreEqual(true, Checker.ThisSetInThatSet<WeekDate>(weekDates, weekDates2));
        }
    }
}
