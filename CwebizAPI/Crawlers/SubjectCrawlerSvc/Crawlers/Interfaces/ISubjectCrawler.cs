using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes;
using HtmlAgilityPack;

namespace CwebizAPI.Crawlers.SubjectCrawlerSvc.Crawlers.Interfaces
{
    public interface ISubjectCrawler
    {
        Task<Subject?> Crawl(string discipline, string keyword1);
        Task<Subject?> Crawl(string courseId);
        Task<Subject?> Crawl(HtmlDocument htmlDocument, string courseId);
    }
}
