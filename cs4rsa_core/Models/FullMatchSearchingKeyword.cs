using Cs4rsa.Cs4rsaDatabase.Models;

namespace Cs4rsa.Models
{
    public class FullMatchSearchingKeyword
    {
        public Discipline Discipline { get; set; }
        public Keyword Keyword { get; set; }

        public override string ToString()
        {
            return Keyword.SubjectName;
        }
    }
}
