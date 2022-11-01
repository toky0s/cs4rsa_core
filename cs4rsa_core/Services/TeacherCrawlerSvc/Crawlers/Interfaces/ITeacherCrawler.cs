using cs4rsa_core.Services.TeacherCrawlerSvc.Models;

using System.Threading.Tasks;

namespace cs4rsa_core.Services.TeacherCrawlerSvc.Crawlers.Interfaces
{
    public interface ITeacherCrawler
    {
        Task<TeacherModel> Crawl(string url, int courseId);
    }
}
