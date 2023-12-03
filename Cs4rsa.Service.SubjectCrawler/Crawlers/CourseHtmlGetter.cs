using System.Threading.Tasks;
using Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces;
using HtmlAgilityPack;

namespace Cs4rsa.Service.SubjectCrawler.Crawlers
{
    public class CourseHtmlGetter: ICourseHtmlGetter
    {
        public async Task<HtmlDocument> GetHtmlDocument(string courseId, string semesterId)
        {
            var htmlWeb = new HtmlWeb();
            var url = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={courseId}&semesterid={semesterId}&timespan={semesterId}";
            return await htmlWeb.LoadFromWebAsync(url);
        }
    }
}