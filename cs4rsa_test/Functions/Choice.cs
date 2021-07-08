using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.Dialogs.Implements;

namespace cs4rsa_test.Functions
{
    [TestFixture]
    public class Choice
    {
        [Test]
        public void Test1()
        {
            List<string> col = new List<string>()
            {
                "A",
                "B",
                "C",
                "D",
                "E",
                "F",
                "G",
            };
            List<List<string>> result = AutoSortViewModel.Gen(3, col);
            Assert.AreEqual(3, result.Last().ToList().Count);
            Assert.AreEqual("G", result.Last().ToList().Last());
        }
    }
}
