using cs4rsa_core.Services.CourseSearchSvc.Crawlers.Interfaces;
using cs4rsa_core.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;
using cs4rsa_core.Services.SubjectCrawlerSvc.Utils;
using cs4rsa_core.Utils;

using HtmlAgilityPack;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace cs4rsa_core.Services.SubjectCrawlerSvc.Crawlers
{
    /// <summary>
    /// Bộ cào này xác định môn tiên quyết và môn song hành.
    /// </summary>
    public class PreParSubjectCrawler : IPreParSubjectCrawler
    {
        public bool IsAvailableSubject { get; set; }

        private readonly ICourseCrawler _courseCrawler;
        private readonly HtmlWeb _htmlWeb;
        public PreParSubjectCrawler(ICourseCrawler courseCrawler, HtmlWeb htmlWeb)
        {
            _courseCrawler = courseCrawler;
            _htmlWeb = htmlWeb;
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của môn học.
        /// </summary>
        /// <param name="doc">HtmlDocument chứa thông tin của một trang môn học.</param>
        /// <returns></returns>
        private static bool IsExistSubject(HtmlDocument doc)
        {
            HtmlNode span = doc.DocumentNode.SelectSingleNode("div[2]/span");
            return span == null;
        }

        public async Task<PreParContainer> Run(string courseId)
        {
            string semesterId = _courseCrawler.GetCurrentSemesterValue();

            string url = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={courseId}&semesterid={semesterId}&timespan={semesterId}";
            HtmlDocument htmlDocument = await _htmlWeb.LoadFromWebAsync(url);

            if (IsExistSubject(htmlDocument))
            {
                IsAvailableSubject = true;
                HtmlNode prerequisite = htmlDocument.DocumentNode.SelectSingleNode("(//font[@color='#548DDB' or @color='green'])[1]");
                HtmlNode parallel = htmlDocument.DocumentNode.SelectSingleNode("(//font[@color='#548DDB' or @color='green'])[2]");
                List<string> pre = SubjectCodeParser.GetSubjectCodes(prerequisite.InnerText, GetFrom.Course);
                List<string> par = SubjectCodeParser.GetSubjectCodes(parallel.InnerText, GetFrom.Course);
                return new PreParContainer()
                {
                    ParallelSubjects = par,
                    PrerequisiteSubjects = pre
                };
            }
            else
            {
                IsAvailableSubject = false;
                return new PreParContainer()
                {
                    ParallelSubjects = new List<string>(),
                    PrerequisiteSubjects = new List<string>()
                };
            }
        }

        public async Task<PreParContainer> Run(string courseId, string sessionId)
        {
            string url = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/CourseClassResultForStudent.aspx?courseid={courseId}";
            string html = await DtuPageCrawler.GetHtml(sessionId, url);
            HtmlDocument document = new();
            document.LoadHtml(html);
            HtmlNode prerequisite = document.DocumentNode.SelectSingleNode("//tr[4]/td[2]/font");
            HtmlNode parallel = document.DocumentNode.SelectSingleNode("//tr[5]/td[2]/font");
            List<string> pre = SubjectCodeParser.GetSubjectCodes(prerequisite.InnerText, GetFrom.MyDTU);
            List<string> par = SubjectCodeParser.GetSubjectCodes(parallel.InnerText, GetFrom.MyDTU);
            return new PreParContainer() { ParallelSubjects = par, PrerequisiteSubjects = pre };
        }
    }
}
