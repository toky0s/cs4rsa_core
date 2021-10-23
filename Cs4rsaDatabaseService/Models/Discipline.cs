using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cs4rsaDatabaseService.Models
{
    public class Discipline
    {
        [Key]
        public int DisciplineId { get; set; }
        public string Name { get; set; }
        public List<Keyword> Keywords { get; set; }
    }
}
