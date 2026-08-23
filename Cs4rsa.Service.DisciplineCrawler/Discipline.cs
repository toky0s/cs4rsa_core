using System.Collections.Generic;

namespace Cs4rsa.Service.DisciplineCrawler
{
    public class Discipline
    {
        public string Name { get; set; }
        public ICollection<Keyword> Keywords { get; set; }
    }
}
