using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cs4rsa.Module.Shared;

namespace UnitTestProject.Shared_Utils
{
    [TestClass]
    public class OverlapTests
    {
        [TestMethod]
        public void Test_Overlap_NormalCase()
        {
            var start1 = new DateTime(2026, 5, 15, 10, 0, 0);
            var end1 = new DateTime(2026, 5, 15, 12, 0, 0);

            var start2 = new DateTime(2026, 5, 15, 11, 0, 0);
            var end2 = new DateTime(2026, 5, 15, 13, 0, 0);

            Assert.IsTrue(Utils.IsOverlap(start1, end1, start2, end2));
        }

        [TestMethod]
        public void Test_Overlap_TouchingAtEnd()
        {
            var start1 = new DateTime(2026, 5, 15, 10, 0, 0);
            var end1 = new DateTime(2026, 5, 15, 12, 0, 0);

            var start2 = new DateTime(2026, 5, 15, 12, 0, 0);
            var end2 = new DateTime(2026, 5, 15, 14, 0, 0);

            Assert.IsTrue(Utils.IsOverlap(start1, end1, start2, end2));
        }

        [TestMethod]
        public void Test_NoOverlap()
        {
            var start1 = new DateTime(2026, 5, 15, 10, 0, 0);
            var end1 = new DateTime(2026, 5, 15, 12, 0, 0);

            var start2 = new DateTime(2026, 5, 15, 13, 0, 0);
            var end2 = new DateTime(2026, 5, 15, 15, 0, 0);

            Assert.IsFalse(Utils.IsOverlap(start1, end1, start2, end2));
        }
    }
}
