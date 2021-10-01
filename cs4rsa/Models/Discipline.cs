using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Models
{
    public class Discipline
    {
        public int DisciplineId { get; set; }
        public string Name { get; set; }

        public ICollection<Keyword> Keywords { get; set; }
    }
}
