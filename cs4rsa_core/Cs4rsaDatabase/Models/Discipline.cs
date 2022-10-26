using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cs4rsa_core.Cs4rsaDatabase.Models
{
    public class Discipline
    {
        [Key]
        public int DisciplineId { get; set; }
        public string Name { get; set; }
        public List<Keyword> Keywords { get; set; }
    }
}
