using HtmlAgilityPack;

using SubjectCrawlService1.DataTypes;

using System.Threading.Tasks;

namespace SubjectCrawlService1.Crawlers.Interfaces
{
    public interface ISubjectCrawler
    {
        Task<Subject> Crawl(string discipline, string keyword1, bool isUseCache = true);
        Task<Subject> Crawl(int courseId, bool isUseCache = true);
        Task<Subject> Crawl(HtmlDocument htmlDocument, int courseId);

        /// <summary>
        /// Lưu tạm thông tin môn học đã lưu vào DB.
        /// </summary>
        /// <param name="keywordId">Keyword ID</param>
        /// <param name="HtmlRaw">Đoạn HTML cần lưu</param>
        Task SaveCache(int keywordId, string HtmlRaw);
    }
}
