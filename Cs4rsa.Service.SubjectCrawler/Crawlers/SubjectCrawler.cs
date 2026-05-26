using Cs4rsa.Infrastructure.Common;
using Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces;
using Cs4rsa.Service.SubjectCrawler.DataTypes;

using HtmlAgilityPack;

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cs4rsa.Service.SubjectCrawler.Crawlers
{
    public class SubjectCrawler : ISubjectCrawler
    {
        private readonly ICourseHtmlGetter _courseHtmlGetter;

        /// <summary>
        /// Bộ cào Subject từ Course DTU
        /// </summary>
        /// <param name="courseHtmlGetter"></param>
        public SubjectCrawler(ICourseHtmlGetter courseHtmlGetter)
        {
            _courseHtmlGetter = courseHtmlGetter;
        }

        public async Task<(Subject, string)> Crawl(string courseId, string semesterId)
        {
            var htmlDocument = await _courseHtmlGetter.GetHtmlDocument(courseId, semesterId);
            return (
                InternalCrawl(htmlDocument, courseId, semesterId),
                OptimizeHTMLCache(htmlDocument.DocumentNode.InnerHtml)
            );
        }

        private string OptimizeHTMLCache(string innerHtml)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(innerHtml);
            var styleNodes = doc.DocumentNode.SelectNodes("//style");
            if (styleNodes != null)
            {
                foreach (var node in styleNodes)
                {
                    node.Remove(); // Xóa thẻ <style>
                }
            }
            return MinifyHtml(doc.DocumentNode.OuterHtml);
        }

        public static string MinifyHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Remove comment nodes
            var comments = doc.DocumentNode.SelectNodes("//comment()");
            if (comments != null)
                foreach (var comment in comments.ToList())
                    comment.Remove();

            // Remove all style= attributes from every element
            var nodesWithStyle = doc.DocumentNode.SelectNodes("//*[@style]");
            if (nodesWithStyle != null)
                foreach (var node in nodesWithStyle)
                    node.Attributes["style"].Remove();

            // Normalize text nodes: collapse all whitespace sequences to a single space
            var textNodes = doc.DocumentNode.SelectNodes("//text()");
            if (textNodes != null)
                foreach (var textNode in textNodes)
                    textNode.InnerHtml = Regex.Replace(textNode.InnerText, @"[\r\n\t]+", " ");

            var sb = new StringBuilder();
            using (var writer = new System.IO.StringWriter(sb))
                doc.Save(writer);
            return sb.ToString(); 
        }

        /// <summary>
        /// Kiểm tra môn học có tồn tại hay không.
        /// </summary>
        /// <param name="htmlDocument">HtmlDocument</param>
        /// <returns>True nếu tồn tại, ngược lại trả về False.</returns>
        private static bool IsSubjectExists(HtmlDocument htmlDocument)
        {
            var tables = htmlDocument.DocumentNode.Descendants("table");
            return tables.Any();
        }

        public Subject CrawlFromCache(string cache, string courseId, string semesterId)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(cache);
            return InternalCrawl(htmlDocument, courseId, semesterId);
        }

        private Subject InternalCrawl(HtmlDocument htmlDocument, string courseId, string semesterId)
        {
            if (!IsSubjectExists(htmlDocument)) return null;

            var table = htmlDocument.DocumentNode.Descendants("table").ToArray()[2];
            var trTags = table.Descendants("tr").ToArray();

            var xpathNames = new string[2]
            {
                "//div[1]/table/tr/td/span",
                "//div[1]/table/tbody/tr/td/span"
            };
            var xpathName = xpathNames.Where(item => htmlDocument.DocumentNode.SelectSingleNode(item) != null).First();
            var name = htmlDocument.DocumentNode
                .SelectSingleNode(xpathName).InnerText.Trim();
            var subjectCode = trTags[0].Elements("td").ToArray()[1].InnerText.Trim();
            var studyUnit = trTags[1].Elements("td").ToArray()[1].GetDirectInnerText().Trim();
            var studyUnitType = trTags[2].Elements("td").ToArray()[1].InnerText.Trim();
            var studyType = trTags[3].Elements("td").ToArray()[1].InnerText.Trim();
            var semester = trTags[4].Elements("td").ToArray()[1].InnerText.Trim();
            // Môn tiên quyết
            var mustStudySubject = trTags[5].Elements("td").ToArray()[1].InnerText.Trim();
            // Môn song hành
            var parallelSubject = trTags[6].Elements("td").ToArray()[1].InnerText.Trim();
            var description = trTags[7].Elements("td").ToArray()[1].InnerText.Trim();
            var rawSoup = htmlDocument.DocumentNode.OuterHtml;
            return new Subject(
                name,
                subjectCode,
                studyUnit,
                studyUnitType,
                studyType,
                semester,
                mustStudySubject,
                parallelSubject,
                description,
                rawSoup,
                courseId,
                semesterId
            );
        }
    }
}
