using Cs4rsaDatabaseService.Models;

using System.Threading.Tasks;

namespace StudentCrawlerService.Crawlers.Interfaces
{
    public interface IDtuStudentInfoCrawler
    {
        Task<Student> Crawl(string specialString);

        Task<Student> CrawlWithSessionId(string sessionId);
    }
}
