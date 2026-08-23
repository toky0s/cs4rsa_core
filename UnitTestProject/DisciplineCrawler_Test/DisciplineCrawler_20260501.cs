using Cs4rsa.Service.DisciplineCrawler;

using HtmlAgilityPack;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using System;
using System.IO;
using System.Linq;

namespace UnitTestProject.DisciplineCrawler_Test
{
    [TestClass]
    public class DisciplineCrawler_20260501
    {
        private IDisciplineCrawler _disciplineCrawler;
        private IDisciplineHtmlGetter _disciplineHtmlGetter;

        [TestInitialize]
        public void ClassInitialize_v()
        {
            _disciplineHtmlGetter = Substitute.For<IDisciplineHtmlGetter>();
            _disciplineHtmlGetter
                .GetHtmlDocument(Arg.Any<string>())
                .Returns(_ => {
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DisciplineCrawler_Test/Resources", "CourseResultSearch.htm");
                    var text = File.ReadAllText(path);
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(text);
                    return htmlDoc;
                });
            _disciplineCrawler = new DisciplineCrawler(_disciplineHtmlGetter);
        }

        [TestMethod]
        public void CountTotalDiscipline()
        {
            var disciplines = _disciplineCrawler.GetDisciplineAndKeyword("83");
            Assert.AreEqual(116, disciplines.Count);
        }
        
        [TestMethod]
        public void CountTotalDisciplineKeyword_ACC()
        {
            var disciplines = _disciplineCrawler.GetDisciplineAndKeyword("83");
            var count = disciplines.Where(item => item.Name == "ACC").First().Keywords.Count();
            Assert.AreEqual(14, count);
        }
    }
}
