using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Helpers;
using NUnit.Framework;

namespace cs4rsa_test.Helpers
{
    [TestFixture]
    class TestSubjectCodeParser
    {
        [Test]
        public void ParseSubjectCode1()
        {
            List<string> subjectCodes = SubjectCodeParser.GetSubjectCodes("CS 211 - Lập Trình Cơ Sở, IS 301 - Cơ Sở Dữ Liệu");
            Assert.AreEqual("CS 211", subjectCodes[0]);
            Assert.AreEqual("IS 301", subjectCodes[1]);
        }

        [Test]
        public void ParseSubjectCode2()
        {
            List<string> test = SubjectCodeParser.GetSubjectCodes("(Không có môn song hành)");
            Assert.AreEqual(null, test);
        }

        [Test]
        public void ParseSubjectCode3()
        {
            List<string> test = SubjectCodeParser.GetSubjectCodes("JAP 202 - Nhật Ngữ Trung Cấp 2");
            Assert.AreEqual("JAP 202", test[0]);
        }
    }
}
