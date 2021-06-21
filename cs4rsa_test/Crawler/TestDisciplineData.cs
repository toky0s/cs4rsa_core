using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.Crawler;

namespace cs4rsa.Tests.Crawler
{
    [TestFixture]
    class TestDisciplineData
    {
        /// <summary>
        /// Số lượng môn học có trong học kỳ hiện tại.
        /// Lúc thực hiện đoạn test này là học kỳ 73 thuộc kỳ Hè
        /// </summary>
        [Test]
        public void GetSoLuongMonHoc()
        {
            int expected = DisciplineData.GetNumberOfSubjects();
            Assert.AreEqual(expected, 397);
        }
    }
}
