using Cs4rsaDatabaseService.Models;
using System.Threading.Tasks;

namespace TeacherCrawlerService1.Crawlers.Interfaces
{
    public interface ITeacherCrawler
    {
        Task<Teacher> Crawl(string url);
    }
}
