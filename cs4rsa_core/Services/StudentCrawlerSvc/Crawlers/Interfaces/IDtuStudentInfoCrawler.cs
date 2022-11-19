using Cs4rsa.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace Cs4rsa.Services.StudentCrawlerSvc.Crawlers.Interfaces
{
    public interface IDtuStudentInfoCrawler
    {
        Task<Student> Crawl(string specialString);
    }
}
