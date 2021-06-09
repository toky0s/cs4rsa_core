using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.Crawler;
using cs4rsa.BasicData;

namespace cs4rsa_test.Crawler
{
    [TestFixture]
    class TestMetaDataCrawler
    {
        [Test]
        public void TestToMetaData1()
        {
            string url = "http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listclassdetail&timespan=73&semesterid=73&classid=140631&academicleveltypeid=&curriculumid=";
            MetaDataCrawler metaDataCrawler = new MetaDataCrawler(url);
            DayPlaceMetaData dayPlaceMetaData = metaDataCrawler.ToDayPlaceMetaData();
            Assert.AreEqual(Place.NVL_137, dayPlaceMetaData.GetPlaceAddDay(DayOfWeek.Tuesday));
        }

        [Test]
        public void TestToMetaData2()
        {
            string url = "http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listclassdetail&timespan=73&semesterid=73&classid=140631&academicleveltypeid=&curriculumid=";
            MetaDataCrawler metaDataCrawler = new MetaDataCrawler(url);
            DayPlaceMetaData dayPlaceMetaData = metaDataCrawler.ToDayPlaceMetaData();
            Assert.AreEqual(Place.NVL_137, dayPlaceMetaData.GetPlaceAddDay(DayOfWeek.Thursday));
        }

        [Test]
        public void TestToMetaData3()
        {
            string url = "http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listclassdetail&timespan=73&semesterid=73&classid=140216&academicleveltypeid=&curriculumid=";
            MetaDataCrawler metaDataCrawler = new MetaDataCrawler(url);
            DayPlaceMetaData dayPlaceMetaData = metaDataCrawler.ToDayPlaceMetaData();
            Assert.AreEqual(Place.QUANGTRUNG, dayPlaceMetaData.GetPlaceAddDay(DayOfWeek.Tuesday));
        }

        [Test]
        public void TestToMetaData4()
        {
            string url = "http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listclassdetail&timespan=73&semesterid=73&classid=140216&academicleveltypeid=&curriculumid=";
            MetaDataCrawler metaDataCrawler = new MetaDataCrawler(url);
            DayPlaceMetaData dayPlaceMetaData = metaDataCrawler.ToDayPlaceMetaData();
            Assert.AreEqual(Place.QUANGTRUNG, dayPlaceMetaData.GetPlaceAddDay(DayOfWeek.Friday));
        }


        [Test]
        public void TestToMetaData5()
        {
            string url = "http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listclassdetail&timespan=73&semesterid=73&classid=140603&academicleveltypeid=&curriculumid=";
            MetaDataCrawler metaDataCrawler = new MetaDataCrawler(url);
            DayPlaceMetaData dayPlaceMetaData = metaDataCrawler.ToDayPlaceMetaData();
            Assert.AreEqual(Place.HOAKHANH, dayPlaceMetaData.GetPlaceAddDay(DayOfWeek.Wednesday));
        }


        [Test]
        public void TestToMetaData6()
        {
            string url = "http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listclassdetail&timespan=73&semesterid=73&classid=140603&academicleveltypeid=&curriculumid=";
            MetaDataCrawler metaDataCrawler = new MetaDataCrawler(url);
            DayPlaceMetaData dayPlaceMetaData = metaDataCrawler.ToDayPlaceMetaData();
            Assert.AreEqual(Place.VIETTIN, dayPlaceMetaData.GetPlaceAddDay(DayOfWeek.Saturday));
        }

    }
}
