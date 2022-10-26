using cs4rsa_core.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace cs4rsa_core.Services.CurriculumCrawlerSvc.Crawlers.Interfaces
{
    public interface ICurriculumCrawler
    {
        Task<Curriculum> GetCurriculum(string specialString);
    }
}
