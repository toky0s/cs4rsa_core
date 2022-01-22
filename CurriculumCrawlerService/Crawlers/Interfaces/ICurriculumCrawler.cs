using Cs4rsaDatabaseService.Models;
using System.Threading.Tasks;

namespace CurriculumCrawlerService.Crawlers.Interfaces
{
    public interface ICurriculumCrawler
    {
        Task<Curriculum> GetCurriculum(string specialString);
    }
}
