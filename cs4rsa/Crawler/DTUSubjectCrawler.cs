using cs4rsa.Enums;
using cs4rsa.Helpers;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace cs4rsa.Crawler
{
    /// <summary>
    /// Bộ cào này xác định môn tiên quyết và môn song hành của một môn nào đó.
    /// </summary>
    public class DtuSubjectCrawler
    {
        private readonly string _courseId;
        private readonly string _sessionId;
        private List<string> _prerequisiteSubjects = new List<string>();
        private List<string> _parallelSubjects = new List<string>();
        public List<string> PrerequisiteSubjects => _prerequisiteSubjects;
        public List<string> ParallelSubjects => _parallelSubjects;

        public bool IsAvailableSubject { get; internal set; }

        public DtuSubjectCrawler(string sessionId, string courseId)
        {
            _courseId = courseId;
            _sessionId = sessionId;
            Run();
        }

        public DtuSubjectCrawler(string courseId)
        {
            _courseId = courseId;
            RunOnCourse();
        }

        /// <summary>
        /// Phương thức này sẽ chạy trên Course của DTU
        /// và đưa danh sách các môn tiên quyết và song hành
        /// vào các thuộc tính tương ứng.
        /// </summary>
        private void RunOnCourse()
        {
            HomeCourseSearch homeCourseSearch = HomeCourseSearch.GetInstance();
            string semesterId = homeCourseSearch.CurrentSemesterValue;

            string url = string.Format("http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={0}&semesterid={1}&timespan={2}", _courseId, semesterId, semesterId);
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load(url);

            if (IsExistSubject(htmlDocument))
            {
                IsAvailableSubject = true;
                HtmlNode prerequisite = htmlDocument.DocumentNode.SelectSingleNode("(//font[@color='#548DDB' or @color='green'])[1]");
                HtmlNode parallel = htmlDocument.DocumentNode.SelectSingleNode("(//font[@color='#548DDB' or @color='green'])[2]");
                List<string> pre = SubjectCodeParser.GetSubjectCodes(prerequisite.InnerText, GetFrom.Course);
                List<string> par = SubjectCodeParser.GetSubjectCodes(parallel.InnerText, GetFrom.Course);
                _prerequisiteSubjects = pre;
                _parallelSubjects = par;
            }
            else
            {
                IsAvailableSubject = false;
            }
        }

        private void Run()
        {
            string url = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/CourseClassResultForStudent.aspx?courseid={_courseId}";
            string html = DtuPageCrawler.GetHtml(_sessionId, url);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            HtmlNode prerequisite = document.DocumentNode.SelectSingleNode("//tr[4]/td[2]/font");
            HtmlNode parallel = document.DocumentNode.SelectSingleNode("//tr[5]/td[2]/font");
            var pre = SubjectCodeParser.GetSubjectCodes(prerequisite.InnerText, GetFrom.MyDTU);
            var par = SubjectCodeParser.GetSubjectCodes(parallel.InnerText, GetFrom.MyDTU);
            _prerequisiteSubjects = pre;
            _parallelSubjects = par;
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của môn học.
        /// </summary>
        /// <param name="doc">HtmlDocument chứa thông tin của một trang môn học.</param>
        /// <returns></returns>
        private bool IsExistSubject(HtmlDocument doc)
        {
            HtmlNode span = doc.DocumentNode.SelectSingleNode("div[2]/span");
            return span == null;
        }
    }
}
