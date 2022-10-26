using cs4rsa_core.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace cs4rsa_core.Services.TeacherCrawlerSvc.Crawlers.Interfaces
{
    public interface ITeacherCrawler
    {
        Task<Teacher> Crawl(string url);
    }
}
