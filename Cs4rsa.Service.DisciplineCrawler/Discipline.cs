using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Service.DisciplineCrawler
{
    public class Discipline
    {
        public string Name { get; set; }
        public ICollection<Keyword> Keywords { get; set; }
    }
}
