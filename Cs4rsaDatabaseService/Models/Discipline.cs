using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Models
{
    public class Discipline
    {
        public int DisciplineId { get; set; }
        public string Name { get; set; }
        public List<Keyword> Keywords { get; set; }
    }
}
