using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisciplineCrawlerDLL.Models
{
    public class Discipline
    {
        public int DisciplineId { get; }
        public string Name { get; }
        public List<Keyword> Keywords { get; set; }

        public Discipline(int disciplineId, string name)
        {
            DisciplineId = disciplineId;
            Name = name;
            Keywords = new();
        }
    }
}
