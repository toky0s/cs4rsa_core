using CwebizAPI.Crawlers.SubjectCrawlerSvc.Crawlers.Interfaces;
using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes;
using CwebizAPI.Db.Interfaces;
using CwebizAPI.Share.Database.Models;
using HtmlAgilityPack;

namespace CwebizAPI.Crawlers.SubjectCrawlerSvc.Crawlers
{
    public class SubjectCrawler : ISubjectCrawler
    {
        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly HtmlWeb _htmlWeb;
        #endregion

        /// <summary>
        /// Bộ cào Subject từ Course DTU
        /// </summary>
        /// <remarks>
        /// discipline: Hai chữ cái đầu của mã môn (CS).
        /// keyword1: Các chữ số đằng sau (414).
        /// </remarks>
        /// <param name="unitOfWork"></param>
        /// <param name="htmlWeb"></param>
        public SubjectCrawler(
            IUnitOfWork unitOfWork,
            HtmlWeb htmlWeb
        )
        {
            _unitOfWork = unitOfWork;
            _htmlWeb = htmlWeb;
        }

        public async Task<Subject?> Crawl(string discipline, string keyword1)
        {
            Keyword? keyword = await _unitOfWork.DisciplineRepository.GetKeyword(discipline, keyword1);
            if (keyword is null) throw new ArgumentException($"Không tìm thấy Keyword với mã môn là {discipline} {keyword1}");
            Course? course = await _unitOfWork.DisciplineRepository.GetLatestCourse();
            if (course is null) throw new BadHttpRequestException("Không tim thấy course mới nhất");
            string semesterId = course.SemesterValue!;
            string url = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={keyword.CourseId}&semesterid={semesterId}&timespan={semesterId}";
            HtmlDocument htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
            Subject? subject = await Crawl(htmlDocument, keyword.CourseId);
            return subject;
        }

        public async Task<Subject?> Crawl(string courseId)
        {
            Keyword? keyword = await _unitOfWork.DisciplineRepository.GetKeywordByCourseId(courseId);
            if (keyword is null)
            {
                throw new ArgumentException($"Không tìm thấy Keyword với Course ID là {courseId}");
            }

            Course? course = await _unitOfWork.DisciplineRepository.GetLatestCourse();
            if (course is null) throw new BadHttpRequestException("Không tim thấy course mới nhất");
            string semesterId = course.SemesterValue!;
            
            string url = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={courseId}&semesterid={semesterId}&timespan={semesterId}";
            HtmlDocument htmlDocument = await _htmlWeb.LoadFromWebAsync(url);
            Subject? subject = await Crawl(htmlDocument, keyword.CourseId);
            return subject;
        }

        public async Task<Subject?> Crawl(HtmlDocument htmlDocument, string courseId)
        {
            if (!IsSubjectExists(htmlDocument)) return null;

            HtmlNode table = htmlDocument.DocumentNode.Descendants("table").ToArray()[2];
            HtmlNode[] trTags = table.Descendants("tr").ToArray();
            string subjectCode = trTags[0].Elements("td").ToArray()[1].InnerText.Trim();

            Keyword? kw = await _unitOfWork.DisciplineRepository.GetKeywordByCourseId(courseId);
            if (kw is null)
            {
                throw new ArgumentException($"Không tìm thấy Keyword với Course ID là {courseId}");
            }
            string name = kw.SubjectName;

            string studyUnit = trTags[1].Elements("td").ToArray()[1].GetDirectInnerText().Split(' ')[24];
            string studyUnitType = trTags[2].Elements("td").ToArray()[1].InnerText.Trim();
            string studyType = trTags[3].Elements("td").ToArray()[1].InnerText.Trim();
            string semester = trTags[4].Elements("td").ToArray()[1].InnerText.Trim();

            // Môn tiên quyết
            string mustStudySubject = trTags[5].Elements("td").ToArray()[1].InnerText.Trim();
            
            // Môn song hành
            string parallelSubject = trTags[6].Elements("td").ToArray()[1].InnerText.Trim();

            string description = trTags[7].Elements("td").ToArray()[1].InnerText.Trim();

            string rawSoup = htmlDocument.DocumentNode.OuterHtml;
            return await Subject.CreateAsync(
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
                _htmlWeb
            );
        }

        /// <summary>
        /// Kiểm tra môn học có tồn tại hay không.
        /// </summary>
        /// <param name="htmlDocument">HtmlDocument</param>
        /// <returns>True nếu tồn tại, ngược lại trả về False.</returns>
        private static bool IsSubjectExists(HtmlDocument htmlDocument)
        {
            HtmlNode span = htmlDocument.DocumentNode.SelectSingleNode("div[2]/span");
            if (span is not null)
            {
                return false;
            }
            IEnumerable<HtmlNode> tables = htmlDocument.DocumentNode.Descendants("table");
            return tables.Any();
        }
    }
}
