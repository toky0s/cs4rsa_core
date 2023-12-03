using System.IO;
using Cs4rsa.Service.CourseCrawler.Crawlers;
using Cs4rsa.Service.CourseCrawler.Interfaces;
using HtmlAgilityPack;
using Moq;
using NUnit.Framework;

namespace Test_CourseCrawler
{
    [TestFixture]
    public class CourseCrawler_20231112
    {
        private Mock<ISemesterHtmlGetter> _mockSemesterHtmlGetter;
        private ICourseCrawler _courseCrawler;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockSemesterHtmlGetter = new Mock<ISemesterHtmlGetter>(MockBehavior.Strict);
        }
        
        [Test]
        public void GivenUrl_WhenCrawlCourse_ThenGetInfo()
        {
            _mockSemesterHtmlGetter
                .SetupSequence(semesterHtmlGetter => semesterHtmlGetter.GetHtmlDocument(It.IsAny<string>()))
                .Returns(() =>
                {
                    var text = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "20231112_82.htm"));
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return htmlDoc;
                })
                .Returns(() =>
                {
                    var text = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "20231112_year82.htm"));
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return htmlDoc;
                });
            
            _courseCrawler = new CourseCrawler(_mockSemesterHtmlGetter.Object);
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
        
        [Test]
        public void GivenUrl_WhenCrawlCourse_ThenGetLatestSemester()
        {
            _mockSemesterHtmlGetter
                .SetupSequence(semesterHtmlGetter => semesterHtmlGetter.GetHtmlDocument(It.IsAny<string>()))
                .Returns(() =>
                {
                    var text = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "20231112_78.htm"));
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return htmlDoc;
                })
                .Returns(() =>
                {
                    var text = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", "20231112_year78.htm"));
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return htmlDoc;
                });
            
            _courseCrawler = new CourseCrawler(_mockSemesterHtmlGetter.Object);
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