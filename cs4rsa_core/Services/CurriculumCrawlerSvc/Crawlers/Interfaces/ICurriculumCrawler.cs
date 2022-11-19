using Cs4rsa.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace Cs4rsa.Services.CurriculumCrawlerSvc.Crawlers.Interfaces
{
    public interface ICurriculumCrawler
    {
        Task<Curriculum> GetCurriculum(string specialString);
    }
}
