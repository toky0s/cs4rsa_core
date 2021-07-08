using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.Helpers;

namespace cs4rsa_test.Helpers
{
    [TestFixture]
    class TestShortedTimeConverter
    {
        [Test]
        public void TestConvertTime1()
        {
            DateTime now = DateTime.Now;
            DateTime time1 = new DateTime(now.Year, now.Month, now.Day, 7, 15, 0);
            DateTime converted = new DateTime(now.Year, now.Month, now.Day, 7, 0, 0);
            ShortedTimeConverter shortedTimeCv = ShortedTimeConverter.GetInstance();
            ShortedTime shortedTime = shortedTimeCv.Convert(time1);
            Assert.AreEqual(converted, shortedTime.NewTime);
        }
    }
}
