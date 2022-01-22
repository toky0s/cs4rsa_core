using DisciplineCrawlerDLL.Models;

namespace DisciplineCrawlerDLL.Interfaces
{
    public interface IDisciplineCrawler
    {
        Task<List<Discipline>> GetDisciplines(string currentSemesterValue);
    }
}
