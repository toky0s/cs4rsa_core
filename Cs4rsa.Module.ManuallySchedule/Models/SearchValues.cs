using System.Collections.Generic;

namespace Cs4rsa.Module.ManuallySchedule.Models
{
    public class SearchValues
    {
        internal class UndoDelValue
        {
            public SubjectModel SubjectModel { get; set; }
            public ClassGroupModel SeletedClassGroupModel { get; set; }
            public int Index { get; set; }
        }

        internal class UndoDelAllValue
        {
            public IEnumerable<SubjectModel> SubjectModels { get; set; }
            public IEnumerable<ClassGroupModel> ClassGroupModels { get; set; }
        }
    }
}
