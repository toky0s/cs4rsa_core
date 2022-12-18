using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Utils;
using Cs4rsa.Utils;

using HtmlAgilityPack;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsa.Services.SubjectCrawlerSvc.Crawlers
{
    /// <summary>
    /// Bộ cào này xác định môn tiên quyết và môn song hành.
    /// </summary>
    public class PreParSubjectCrawler : IPreParSubjectCrawler
    {
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

        /// <summary>
        /// Lấy thông tin môn tiên quyết và song hành thông qua
        /// việc cào dữ liệu từ trang course.
        /// </summary>
        /// <param name="courseId">Course ID của môn học.</param>
        public async Task<PreParContainer> Run(string courseId, bool isUseCache)
        {
            string semesterId = _courseCrawler.GetCurrentSemesterValue();

            string url = $"http://courses.duytan.edu.vn/Modules/academicprogram/CourseClassResult.aspx?courseid={courseId}&semesterid={semesterId}&timespan={semesterId}";
            HtmlDocument htmlDocument = await _htmlWeb.LoadFromWebAsync(url);

            if (IsExistSubject(htmlDocument))
            {
                HtmlNode prerequisite = htmlDocument.DocumentNode.SelectSingleNode("(//font[@color='#548DDB' or @color='green'])[1]");
                HtmlNode parallel = htmlDocument.DocumentNode.SelectSingleNode("(//font[@color='#548DDB' or @color='green'])[2]");
                IEnumerable<string> pre = SubjectCodeParser.GetSubjectCodes(prerequisite.InnerText, GetFrom.Course);
                IEnumerable<string> par = SubjectCodeParser.GetSubjectCodes(parallel.InnerText, GetFrom.Course);
                return new PreParContainer()
                {
                    ParallelSubjects = par,
                    PrerequisiteSubjects = pre
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// <b>Deprecated</b>
        /// 
        /// <para>
        /// Tìm môn học song hành và môn học tiên quyết thông qua
        /// việc cào dữ liệu từ MyDTU sử dụng Session ID. Phải biết
        /// chính xác Session ID tại runtime, người dùng phải giữ
        /// được session này sống trong lúc lấy dữ liệu.
        /// </para>
        /// <para>
        /// Phương thức này có độ chính xác cao hơn vì được lấy từ MyDTU, nhưng
        /// bù lại tính sẵn sàng của chúng không cao đồng thời tốc
        /// độ không nhanh do phải thực hiện đăng nhập.
        /// </para>
        /// </summary>
        /// <param name="courseId">Course ID môn học.</param>
        /// <param name="sessionId">Session ID đăng nhập hiện tại của người dùng trong runtime.</param>
        public async Task<PreParContainer> Run(string courseId, string sessionId)
        {
            string url = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/CourseClassResultForStudent.aspx?courseid={courseId}";
            string html = await DtuPageCrawler.GetHtml(sessionId, url);
            HtmlDocument document = new();
            document.LoadHtml(html);
            HtmlNode prerequisite = document.DocumentNode.SelectSingleNode("//tr[4]/td[2]/font");
            HtmlNode parallel = document.DocumentNode.SelectSingleNode("//tr[5]/td[2]/font");
            IEnumerable<string> pre = SubjectCodeParser.GetSubjectCodes(prerequisite.InnerText, GetFrom.MyDTU);
            IEnumerable<string> par = SubjectCodeParser.GetSubjectCodes(parallel.InnerText, GetFrom.MyDTU);
            return new PreParContainer() { ParallelSubjects = par, PrerequisiteSubjects = pre };
        }
    }
}
