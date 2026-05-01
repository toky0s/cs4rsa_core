using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Service.DisciplineCrawler
{
    public interface IDisciplineCrawler
    {
        List<Discipline> GetDisciplineAndKeyword(string currSemesterValue);
    }
}
