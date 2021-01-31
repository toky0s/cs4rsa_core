using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.Crawler;

namespace cs4rsaTest.Crawler
{
    [TestFixture]
    class TestScheduleParser
    {
        public ScheduleParser scheduleParser;
        

        [SetUp]
        public void Setup()
        {
            scheduleParser = new ScheduleParser();
        }

        [Test]
        public void ToSchedule()
        {
        }
    }
}
