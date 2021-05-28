using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using cs4rsa.Crawler;
using cs4rsa.BasicData;
using HtmlAgilityPack;

namespace cs4rsa.Tests.Crawler
{
    [TestFixture]
    class TestScheduleParser
    {
        public string path1 = @"C:\Users\truon\source\repos\cs4rsa\cs4rsa_test\Tests\Crawler\HtmlTdTags\HTMLPagetdTag1.html";
        public string path2 = @"C:\Users\truon\source\repos\cs4rsa\cs4rsa_test\Tests\Crawler\HtmlTdTags\HTMLPagetdTag2.html";
        public string path3 = @"C:\Users\truon\source\repos\cs4rsa\cs4rsa_test\Tests\Crawler\HtmlTdTags\HTMLPagetdTag3.html";
        public string path4 = @"C:\Users\truon\source\repos\cs4rsa\cs4rsa_test\Tests\Crawler\HtmlTdTags\HTMLPagetdTag4.html";

        public HtmlDocument docs1 = new HtmlDocument();
        public HtmlDocument docs2 = new HtmlDocument();
        public HtmlDocument docs3 = new HtmlDocument();
        public HtmlDocument docs4 = new HtmlDocument();

        public HtmlNode td1;
        public HtmlNode td2;
        public HtmlNode td3;
        public HtmlNode td4;

        public ScheduleParser scheduleParser1;
        public ScheduleParser scheduleParser2;
        public ScheduleParser scheduleParser3;
        public ScheduleParser scheduleParser4;

        [SetUp]
        public void Setup()
        {
            docs1.Load(path1);
            docs2.Load(path2);
            docs3.Load(path3);
            docs4.Load(path4);

            td1 = docs1.DocumentNode;
            td2 = docs2.DocumentNode;
            td3 = docs3.DocumentNode;
            td4 = docs4.DocumentNode;

            scheduleParser1 = new ScheduleParser(td1);
            scheduleParser2 = new ScheduleParser(td2);
            scheduleParser3 = new ScheduleParser(td3);
            scheduleParser4 = new ScheduleParser(td4);
        }


        [Test]
        public void TestGetStudyTime1()
        {
            Schedule sd1 = scheduleParser1.ToSchedule();
            StudyTime std1 = sd1.GetStudyTimesAtDay(DayOfWeek.Monday)[0];
            Assert.AreEqual("13:00", std1.StartAsString);
            Assert.AreEqual("15:00", std1.EndAsString);
        }

        [Test]
        public void TestGetStudyTime2()
        {
            Schedule sd2 = scheduleParser2.ToSchedule();
            StudyTime std2 = sd2.GetStudyTimesAtDay(DayOfWeek.Thursday)[0];
            Assert.AreEqual("15:15", std2.StartAsString);
            Assert.AreEqual("17:15", std2.EndAsString);
        }

        [Test]
        public void TestGetStudyTime3()
        {
            Schedule sd3 = scheduleParser3.ToSchedule();
            StudyTime std3 = sd3.GetStudyTimesAtDay(DayOfWeek.Wednesday)[0];
            Assert.AreEqual("15:15", std3.StartAsString);
            Assert.AreEqual("17:15", std3.EndAsString);
        }

        [Test]
        public void TestGetStudyTime4Start()
        {
            Schedule sd4 = scheduleParser4.ToSchedule();
            StudyTime std4 = sd4.GetStudyTimesAtDay(DayOfWeek.Wednesday)[0];
            Assert.AreEqual("13:00", std4.StartAsString);
        }

        [Test]
        public void TestGetStudyTime4End()
        {
            Schedule sd4 = scheduleParser4.ToSchedule();
            StudyTime std4 = sd4.GetStudyTimesAtDay(DayOfWeek.Wednesday)[0];
            Assert.AreEqual("15:00", std4.EndAsString);
        }
    }
}
