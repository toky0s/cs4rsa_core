using System;
using System.IO;
using Cs4rsa.Service.CourseCrawler.Crawlers;
using Cs4rsa.Service.CourseCrawler.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Test_CourseCrawler
{
    [TestClass]
    public class CourseCrawler20231112
    {
        private Mock<ISemesterHtmlGetter> _mockSemesterHtmlGetter;
        private Mock<ILogger<CourseCrawler>> _mockLogger;
        private ICourseCrawler _courseCrawler;
        private string _resourceDirectory;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockSemesterHtmlGetter = new Mock<ISemesterHtmlGetter>(MockBehavior.Loose);
            _mockLogger = new Mock<ILogger<CourseCrawler>>(MockBehavior.Loose);
            _resourceDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
        }

        [TestMethod]
        public void GivenUrl_WhenCrawlCourse_ThenGetInfo()
        {
            Console.WriteLine(typeof(HtmlDocument).Assembly.Location);
            _mockSemesterHtmlGetter
                .SetupSequence(semesterHtmlGetter => semesterHtmlGetter.GetHtmlDocument(It.IsAny<string>()))
                .Returns(() =>
                {
                    var text = File.ReadAllText(Path.Combine(_resourceDirectory,
                        "20231112_82.htm"));
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return htmlDoc;
                })
                .Returns(() =>
                {
                    var text = File.ReadAllText(Path.Combine(_resourceDirectory,
                        "20231112_year82.htm"));
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return htmlDoc;
                });

            _courseCrawler = new CourseCrawler(_mockSemesterHtmlGetter.Object, _mockLogger.Object);
            _courseCrawler.GetInfo(
                out var yearInfo
                , out var yearValue
                , out var semesterInfo
                , out var semesterValue);

            Assert.AreEqual("Năm Học 2023-2024", yearInfo);
            Assert.AreEqual("82", yearValue);
            Assert.AreEqual("Học Kỳ I", semesterInfo);
            Assert.AreEqual("83", semesterValue);
        }

        [TestMethod]
        public void GivenUrl_WhenCrawlCourse_ThenGetLatestSemester()
        {
            _mockSemesterHtmlGetter
                .SetupSequence(semesterHtmlGetter => semesterHtmlGetter.GetHtmlDocument(It.IsAny<string>()))
                .Returns(() =>
                {
                    var text = File.ReadAllText(Path.Combine(_resourceDirectory,
                        "20231112_78.htm"));
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return htmlDoc;
                })
                .Returns(() =>
                {
                    var text = File.ReadAllText(Path.Combine(_resourceDirectory,
                        "20231112_year78.htm"));
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return htmlDoc;
                });

            _courseCrawler = new CourseCrawler(_mockSemesterHtmlGetter.Object, _mockLogger.Object);
            _courseCrawler.GetInfo(
                out var yearInfo
                , out var yearValue
                , out var semesterInfo
                , out var semesterValue);

            Assert.AreEqual("Năm Học 2022-2023", yearInfo);
            Assert.AreEqual("78", yearValue);
            Assert.AreEqual("Học Kỳ Hè", semesterInfo);
            Assert.AreEqual("81", semesterValue);
        }
    }
}