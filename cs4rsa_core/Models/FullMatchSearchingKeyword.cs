using Cs4rsaDatabaseService.Models;

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
