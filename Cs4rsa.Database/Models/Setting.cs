using System.ComponentModel.DataAnnotations;

namespace Cs4rsa.Database.Models
{
    public class Setting
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
