using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using NUnit.Framework;

namespace cs4rsa_test.BasicData
{
    [TestFixture]
    public class TestPlaceMap
    {
        [Test]
        public void Test1()
        {
            List<PlaceMap> placeMaps = new List<PlaceMap>()
            {
                new PlaceMap(new StudyTime("07:00", "09:00"), Place.HOAKHANH),
                new PlaceMap(new StudyTime("09:15", "11:00"), Place.QUANGTRUNG)
            };
            placeMaps.Sort();
            Assert.AreEqual(Place.HOAKHANH, placeMaps[0].Place);
            Assert.AreEqual(Place.QUANGTRUNG, placeMaps[1].Place);
        }

        [Test]
        public void Test2()
        {
            List<PlaceMap> placeMaps = new List<PlaceMap>()
            {
                new PlaceMap(new StudyTime("09:15", "11:00"), Place.QUANGTRUNG),
                new PlaceMap(new StudyTime("07:00", "09:00"), Place.HOAKHANH)
            };
            placeMaps.Sort();
            Assert.AreEqual(Place.HOAKHANH, placeMaps[0].Place);
            Assert.AreEqual(Place.QUANGTRUNG, placeMaps[1].Place);
        }
    }
}
