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
    public class TestCheckVersion
    {
        [Test]
        public void GetVersion()
        {
            string ver = Cs4rsaVersion.CheckVer();
            Assert.AreEqual("1.0.0", ver);
        }
    }
}
