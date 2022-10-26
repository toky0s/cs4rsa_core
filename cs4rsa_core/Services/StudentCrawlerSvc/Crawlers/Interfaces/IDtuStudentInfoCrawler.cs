using cs4rsa_core.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace cs4rsa_core.Services.StudentCrawlerSvc.Crawlers.Interfaces
{
    public interface IDtuStudentInfoCrawler
    {
        Task<Student> Crawl(string specialString);

        Task<Student> CrawlWithSessionId(string sessionId);
    }
}
