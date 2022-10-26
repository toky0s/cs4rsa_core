using cs4rsa_core.Cs4rsaDatabase.Models;

namespace cs4rsa_core.Models
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
