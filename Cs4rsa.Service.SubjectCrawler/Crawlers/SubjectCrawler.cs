using System.Linq;
using System.Threading.Tasks;
using Cs4rsa.Common;
using Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using HtmlAgilityPack;

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
        
        public async Task<Subject> Crawl(string courseId, string semesterId)
        {
            var htmlDocument = await _courseHtmlGetter.GetHtmlDocument(courseId, semesterId);
            if (!IsSubjectExists(htmlDocument)) return null;

            var table = htmlDocument.DocumentNode.Descendants("table").ToArray()[2];
            var trTags = table.Descendants("tr").ToArray();
            
            var name = htmlDocument.DocumentNode
                .SelectSingleNode("//div[1]/table/tr/td/span").InnerText.SuperCleanString();
            var subjectCode = trTags[0].Elements("td").ToArray()[1].InnerText.Trim();
            var studyUnit = trTags[1].Elements("td").ToArray()[1].GetDirectInnerText().Split(' ')[24];
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
                courseId);
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
    }
}
