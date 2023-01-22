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
        /// Lưu tạm thông tin môn học đã lưu vào DB.
        /// </summary>
        /// <param name="keywordId">Keyword ID</param>
        /// <param name="HtmlRaw">Đoạn HTML cần lưu</param>
        Task SaveCache(int keywordId, string HtmlRaw);
    }
}
