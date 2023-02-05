using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Utils;
using Cs4rsa.Utils;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsa.Services.SubjectCrawlerSvc.Crawlers
{
    /// <summary>
    /// Bộ cào này xác định môn tiên quyết và môn song hành.
    /// </summary>
    public class PreParSubjectCrawler : BaseCrawler, IPreParSubjectCrawler
    {
        private readonly ICourseCrawler _courseCrawler;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HtmlWeb _htmlWeb;

        public PreParSubjectCrawler(
            ICourseCrawler courseCrawler,
            IUnitOfWork unitOfWork,
            HtmlWeb htmlWeb)
        {
            _courseCrawler = courseCrawler;
            _unitOfWork = unitOfWork;
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
        /// 
        /// Sử dụng Cache sẽ kiểm tra DB trong lần cào trước đó.
        /// Nếu tồn tại thì lấy luôn từ DB. Nếu không thì cào lại.
        /// </summary>
        /// <param name="courseId">Course ID của môn học.</param>
        /// <param name="isUseCache">
        /// True: Sử dụng thông tin của DB.
        /// False: Cào thông tin từ web.
        /// </param>
        /// <returns><see cref="Tuple{T1, T2}"/>
        /// T1: Danh sách môn song hành.
        /// T2: Danh sách môn tiên quyết.
        /// </returns>
        public async Task<Tuple<IEnumerable<string>, IEnumerable<string>>> Run(string courseId, bool isUseCache)
        {
            bool exists = await _unitOfWork.ProgramSubjects.ExistsByCourseId(courseId);
            if (isUseCache && exists)
            {
                IEnumerable<string> par = _unitOfWork.ProgramSubjects.GetParByCourseId(courseId);
                IEnumerable<string> pre = _unitOfWork.ProgramSubjects.GetPreByCourseId(courseId);
                return new Tuple<IEnumerable<string>, IEnumerable<string>>(par, pre);
            }
            else
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
                    return new Tuple<IEnumerable<string>, IEnumerable<string>>(par, pre);
                }
                else
                {
                    return new Tuple<IEnumerable<string>, IEnumerable<string>>(new List<string>(), new List<string>());
                }
            }
        }

        /// <summary>
        /// <b>Deprecated</b>
        /// 
        /// <para>
        /// Tìm môn học song hành và môn học tiên quyết thông qua
        /// việc cào dữ liệu từ MyDTU sử dụng Session ID. 
        /// <br></br>
        /// Phải biết chính xác Session ID tại runtime, người dùng phải giữ
        /// được session này sống trong lúc lấy dữ liệu.
        /// <br></br>
        /// Phương thức này có độ chính xác cao hơn vì được lấy từ MyDTU, nhưng
        /// bù lại tính sẵn sàng của chúng không cao đồng thời tốc
        /// độ không nhanh do phải thực hiện đăng nhập.
        /// <br></br>
        /// Thông thường hệ thống MyDTU sẽ bảo trì lúc 11h. Các thao tác trên hệ
        /// thống sẽ không thực hiện được dẫn tới fail. Trong trường hợp này, method
        /// sẽ trả về một <see cref="Tuple{T1, T2}"/> chứa hai List rỗng.
        /// </para>
        /// </summary>
        /// <param name="courseId">Course ID môn học.</param>
        /// <param name="sessionId">Session ID đăng nhập hiện tại của người dùng trong runtime.</param>
        /// <returns><see cref="Tuple{T1, T2}"/>
        /// T1: Danh sách môn song hành.
        /// T2: Danh sách môn tiên quyết.
        /// </returns>
        public async Task<Tuple<IEnumerable<string>, IEnumerable<string>>> Run(string courseId, string sessionId)
        {
            string url = $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/CourseClassResultForStudent.aspx?courseid={courseId}";

            try
            {
                // Nếu cảm thấy việc lấy thông tin này là không cần thiết hãy tạm thời
                // bỏ qua để đỡ tốn effort.
                string html = await DtuPageCrawler.GetHtml(sessionId, url);
                HtmlDocument document = new();
                document.LoadHtml(html);
                HtmlNode prerequisite = document.DocumentNode.SelectSingleNode("//tr[4]/td[2]/font");
                HtmlNode parallel = document.DocumentNode.SelectSingleNode("//tr[5]/td[2]/font");
                IEnumerable<string> pre = SubjectCodeParser.GetSubjectCodes(prerequisite.InnerText, GetFrom.MyDTU);
                IEnumerable<string> par = SubjectCodeParser.GetSubjectCodes(parallel.InnerText, GetFrom.MyDTU);
                if (pre != null)
                {
                    pre = new List<string>();
                }
                if (par != null)
                {
                    par = new List<string>();
                }
                return new Tuple<IEnumerable<string>, IEnumerable<string>>(par, pre);
            }
            catch
            {
                return new Tuple<IEnumerable<string>, IEnumerable<string>>(new List<string>(), new List<string>());
            }
        }
    }
}
