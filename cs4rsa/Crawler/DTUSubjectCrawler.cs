using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Helpers;
using HtmlAgilityPack;

namespace cs4rsa.Crawler
{
    /// <summary>
    /// Bộ cào này xác định môn tiên quyết và môn song hành
    /// của một môn nào đó.
    /// *Chỉ dùng trong tính năng tự động xếp lịch*.
    /// </summary>
    public class DtuSubjectCrawler
    {
        private string _courseId;
        private string _sessionId;
        private List<string> _prerequisiteSubjects = new List<string>();
        private List<string> _parallelSubjects = new List<string>();
        public List<string> PrerequisiteSubjects => _prerequisiteSubjects;
        public List<string> ParallelSubjects => _parallelSubjects;

        public DtuSubjectCrawler(string sessionId, string courseId)
        {
            _courseId = courseId;
            _sessionId = sessionId;
            Run();
        }
        
        private void Run()
        {
            string url = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/CourseClassResultForStudent.aspx?courseid={_courseId}";
            string html = DtuPageCrawler.GetHtml(_sessionId, url);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            HtmlNode prerequisite = document.DocumentNode.SelectSingleNode("//tr[4]/td[2]/font");
            HtmlNode parallel = document.DocumentNode.SelectSingleNode("//tr[5]/td[2]/font");
            var pre = SubjectCodeParser.GetSubjectCodes(prerequisite.InnerText);
            var par = SubjectCodeParser.GetSubjectCodes(parallel.InnerText);
            if (pre != null)
                _prerequisiteSubjects = pre;
            if (par != null)
                _parallelSubjects = par;
        }
    }
}
