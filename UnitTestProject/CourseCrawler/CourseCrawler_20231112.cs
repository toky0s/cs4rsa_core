using Cs4rsa.Service.CourseCrawler.Crawlers;
using Cs4rsa.Service.CourseCrawler.Interfaces;

using HtmlAgilityPack;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.IO;


namespace Test_CourseCrawler
{
    [TestClass]
    public class CourseCrawler20231112
    {
        private ISemesterHtmlGetter _mockSemesterHtmlGetter;
        private ILogger<CourseCrawler> _mockLogger;
        private ICourseCrawler _courseCrawler;
        private string _resourceDirectory;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockSemesterHtmlGetter = Substitute.For<ISemesterHtmlGetter>();
            _mockLogger = Substitute.For<ILogger<CourseCrawler>>();
            _resourceDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CourseCrawler", "Resources");
        }

        [TestMethod]
        public void GivenUrl_WhenCrawlCourse_ThenGetInfo()
        {
            _mockSemesterHtmlGetter.GetHtmlDocument(Arg.Any<string>())
                .Returns(
                    _ => {
                        var text = File.ReadAllText(Path.Combine(_resourceDirectory, "20231112_82.htm"));
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(text);
                        return htmlDoc;
                    },
                    _ => {
                        var text = File.ReadAllText(Path.Combine(_resourceDirectory, "20231112_year82.htm"));
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(text);
                        return htmlDoc;
                    }
            );

            _courseCrawler = new CourseCrawler(_mockSemesterHtmlGetter, _mockLogger);
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
            // 1. Setup sequence using NSubstitute
            _mockSemesterHtmlGetter.GetHtmlDocument(Arg.Any<string>())
                .Returns(
                    _ => {
                        var text = File.ReadAllText(Path.Combine(_resourceDirectory, "20231112_78.htm"));
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(text);
                        return htmlDoc;
                    },
                    _ => {
                        var text = File.ReadAllText(Path.Combine(_resourceDirectory, "20231112_year78.htm"));
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(text);
                        return htmlDoc;
                    }
                );

            // 2. Initialize (Không cần .Object)
            _courseCrawler = new CourseCrawler(_mockSemesterHtmlGetter, _mockLogger);

            // 3. Execution
            _courseCrawler.GetInfo(
                out var yearInfo
                , out var yearValue
                , out var semesterInfo
                , out var semesterValue);

            // 4. Verification
            Assert.AreEqual("Năm Học 2022-2023", yearInfo);
            Assert.AreEqual("78", yearValue);
            Assert.AreEqual("Học Kỳ Hè", semesterInfo);
            Assert.AreEqual("81", semesterValue);
        }
    }
}