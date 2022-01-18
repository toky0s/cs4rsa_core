using DisciplineCrawlerDLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisciplineCrawlerDLL.Interfaces
{
    public interface IDisciplineCrawler
    {
        Task<List<Discipline>> GetDisciplines(string currentSemesterValue);
    }
}
