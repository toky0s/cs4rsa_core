using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;

using HtmlAgilityPack;

using System.Threading.Tasks;

namespace Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces
{
    public interface ISubjectCrawler
    {
        Task<Subject> Crawl(string discipline, string keyword1, bool isUseCache, bool withTeacher);
        Task<Subject> Crawl(int courseId, bool isUseCache, bool withTeacher);
        Task<Subject> Crawl(HtmlDocument htmlDocument, int courseId, bool withTeacher);

        /// <summary>
        /// Tải Cache của một Subject.
        /// </summary>
        /// <remarks>
        /// Nếu subject không tồn tại trả về null.
        /// Nếu quá trình tải gặp lỗi trả về null.
        /// </remarks>
        /// <param name="courseId">Course ID của Subject.</param>
        /// <returns>Outer Html</returns>
        Task<string> CrawlCache(string courseId);
    }
}
