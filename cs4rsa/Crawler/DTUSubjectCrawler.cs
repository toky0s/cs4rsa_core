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
    public class DTUSubjectCrawler
    {
        private string _courseId;
        private string _sessionId;
        public List<string> PrerequisiteSubjects { get; private set; }
        public List<string> ParallelSubjects { get; private set; }

        public DTUSubjectCrawler(string sessionId, string courseId)
        {
            _courseId = courseId;
            _sessionId = sessionId;
            Run();
        }
        
        private void Run()
        {
            string url = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/CourseClassResultForStudent.aspx?courseid={_courseId}";
            string html = DTUPageCrawler.GetHtml(_sessionId, url);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            HtmlNode prerequisite = document.DocumentNode.SelectSingleNode("//tr[4]/td[2]/font");
            HtmlNode parallel = document.DocumentNode.SelectSingleNode("//tr[5]/td[2]/font");
            PrerequisiteSubjects = SubjectCodeParser.GetSubjectCodes(prerequisite.InnerText);
            ParallelSubjects = SubjectCodeParser.GetSubjectCodes(parallel.InnerText);
        }
    }
}
