using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Settings;
using NUnit.Framework;

namespace cs4rsa_test.Settings
{
    [TestFixture]
    class TestSettingReader
    {
        [Test]
        public void GetSetting1()
        {
            string result = SettingReader.GetSetting("SpecialString");
            Assert.AreEqual("NaN", result);
        }

        [Test]
        public void GetSetting2()
        {
            string result = SettingReader.GetSetting("DynamicSchedule");
            Assert.AreEqual("0", result);
        }

        [Test]
        public void GetSetting3()
        {
            string result = SettingReader.GetSetting("ShowPlaceColor");
            Assert.AreEqual("0", result);
        }
        [Test]
        public void GetSetting4()
        {
            string result = SettingReader.GetSetting("MyNam");
            Assert.AreEqual("truong A Xin", result);
        }
    }
}
