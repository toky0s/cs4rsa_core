using System.ComponentModel.DataAnnotations;

namespace Cs4rsa.Cs4rsaDatabase.Models
{
    public class Setting
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
