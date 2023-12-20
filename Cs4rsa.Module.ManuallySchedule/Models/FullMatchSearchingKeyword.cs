using Cs4rsa.Database.Models;

namespace Cs4rsa.Module.ManuallySchedule.Models
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
