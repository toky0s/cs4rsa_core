using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.Helpers;
using cs4rsa.BasicData;

namespace cs4rsa_test.Helpers
{
    [TestFixture]
    class TestBasicDataConverter
    {
        [Test]
        public void TestConvertPlace1()
        {
            Place output = BasicDataConverter.ToPlace("03 Quang Trung");
            Assert.AreEqual(Place.QUANGTRUNG, output);
        }
        [Test]
        public void TestConvertPlace2()
        {
            Place output = BasicDataConverter.ToPlace("334/4 Nguyễn Văn Linh");
            Assert.AreEqual(Place.VIETTIN, output);
        }
        [Test]
        public void TestConvertPlace3()
        {
            Place output = BasicDataConverter.ToPlace("254 Nguyễn Văn Linh");
            Assert.AreEqual(Place.NVL_254, output);
        }
        [Test]
        public void TestConvertPlace4()
        {
            Place output = BasicDataConverter.ToPlace("Hoà Khánh Nam - Toà Nhà A");
            Assert.AreEqual(Place.HOAKHANH, output);
        }
    }
}
