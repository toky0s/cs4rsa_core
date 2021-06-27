using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa;
using NUnit.Framework;
using cs4rsa_test.Crawler;
using cs4rsa.BasicData;
using cs4rsa.Crawler;

namespace cs4rsa_test.Crawler
{
    [TestFixture]
    class TestStudentInfoCrawler
    {
        [Test]
        public void GetStudentInfo()
        {
            string specialString = SpecialStringCrawler.GetSpecialString(InfoForTest.sessionId);
            StudentInfo info = DtuStudentInfoCrawler.ToStudentInfo(specialString, new FakeStudentSaver());
            Assert.AreEqual("Trương A Xin", info.Name);
            Assert.AreEqual("24211200265", info.StudentId);
            Assert.AreEqual("215564568", info.CMND);
        }
    }
}
