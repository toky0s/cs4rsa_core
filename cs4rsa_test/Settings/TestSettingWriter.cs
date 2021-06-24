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
    class TestSettingWriter
    {
        [Test]
        public void WriteASetting1()
        {
            SettingWriter.EditSetting("ShowPlaceColor", "1");
        }
    }
}
