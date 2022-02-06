using CourseSearchService.Crawlers.Interfaces;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using HtmlAgilityPack;
using SubjectCrawlService1.Crawlers.Interfaces;
using SubjectCrawlService1.DataTypes;
using System.Linq;
using System.Threading.Tasks;
using TeacherCrawlerService1.Crawlers.Interfaces;

namespace SubjectCrawlService1.Crawlers
{
    public class SubjectCrawler : ISubjectCrawler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        private readonly ITeacherCrawler _teacherCrawler;

        /// <summary>
        /// Get a Subject of DTU.
        /// </summary>
        /// <param name="discipline">Hai chữ cái đầu của mã môn (CS).</param>
        /// <param name="keyword1">Các chữ số đằng sau (414).</param>
        public SubjectCrawler(ICourseCrawler courseCrawler, ITeacherCrawler teacherCrawler, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _teacherCrawler = teacherCrawler;
        }

        public async Task<Subject> Crawl(string discipline, string keyword1)
        {
            Keyword keyword = _unitOfWork.Keywords.GetKeyword(discipline, keyword1);
            int courseId = keyword.CourseId;

            string semesterId = _courseCrawler.GetCurrentSemesterValue();

            string url = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={courseId}&semesterid={semesterId}&timespan={semesterId}";
            HtmlWeb htmlWeb = new();
            HtmlDocument htmlDocument = await htmlWeb.LoadFromWebAsync(url);

            // kiểm tra sự tồn tại của môn học
            HtmlNode span = htmlDocument.DocumentNode.SelectSingleNode("div[2]/span");
            if (span == null)
            {
                HtmlNode table = htmlDocument.DocumentNode.Descendants("table").ToArray()[2];
                HtmlNode[] trTags = table.Descendants("tr").ToArray();
                string subjectCode = trTags[0].Elements("td").ToArray()[1].InnerText.Trim();

                string name = keyword.SubjectName;

                string studyUnit = trTags[1].Elements("td").ToArray()[1].GetDirectInnerText().Split(' ')[24];
                if (studyUnit == "\r\n")
                {
                    studyUnit = "0";
                }
                string studyUnitType = trTags[2].Elements("td").ToArray()[1].InnerText.Trim();
                string studyType = trTags[3].Elements("td").ToArray()[1].InnerText.Trim();
                string semester = trTags[4].Elements("td").ToArray()[1].InnerText.Trim();
                string mustStudySubject = trTags[5].Elements("td").ToArray()[1].InnerText.Trim();
                string parallelSubject = trTags[6].Elements("td").ToArray()[1].InnerText.Trim();
                string description = trTags[7].Elements("td").ToArray()[1].InnerText.Trim();

                string rawSoup = htmlDocument.DocumentNode.OuterHtml;
                return await Subject.CreateAsync(name, subjectCode, studyUnit, studyUnitType,
                           studyType, semester, mustStudySubject, parallelSubject, description, rawSoup, courseId, _teacherCrawler, _unitOfWork);
            }
            return null;
        }

        public async Task<Subject> Crawl(int courseId)
        {
            string semesterId = _courseCrawler.GetCurrentSemesterValue();

            string url = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={courseId}&semesterid={semesterId}&timespan={semesterId}";
            HtmlWeb htmlWeb = new();
            HtmlDocument htmlDocument = await htmlWeb.LoadFromWebAsync(url);

            // kiểm tra sự tồn tại của môn học
            HtmlNode span = htmlDocument.DocumentNode.SelectSingleNode("div[2]/span");
            if (span == null)
            {
                HtmlNode table = htmlDocument.DocumentNode.Descendants("table").ToArray()[2];
                HtmlNode[] trTags = table.Descendants("tr").ToArray();
                string subjectCode = trTags[0].Elements("td").ToArray()[1].InnerText.Trim();

                string name = _unitOfWork.Keywords.GetKeyword(courseId).SubjectName;

                string studyUnit = trTags[1].Elements("td").ToArray()[1].GetDirectInnerText().Split(' ')[24];
                string studyUnitType = trTags[2].Elements("td").ToArray()[1].InnerText.Trim();
                string studyType = trTags[3].Elements("td").ToArray()[1].InnerText.Trim();
                string semester = trTags[4].Elements("td").ToArray()[1].InnerText.Trim();

                string mustStudySubject = trTags[5].Elements("td").ToArray()[1].InnerText.Trim();
                string parallelSubject = trTags[6].Elements("td").ToArray()[1].InnerText.Trim();

                string description = trTags[7].Elements("td").ToArray()[1].InnerText.Trim();

                string rawSoup = htmlDocument.DocumentNode.OuterHtml;
                return await Subject.CreateAsync(name, subjectCode, studyUnit, studyUnitType,
                           studyType, semester, mustStudySubject, parallelSubject, description, rawSoup, courseId, _teacherCrawler, _unitOfWork);
            }
            return null;
        }
    }
}
