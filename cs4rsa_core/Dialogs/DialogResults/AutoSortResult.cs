using SubjectCrawlService1.Models;

using System.Collections.Generic;

namespace cs4rsa_core.Dialogs.DialogResults
{
    public class AutoSortResult
    {
        public List<List<ClassGroupModel>> ClassGroupModelCombinations { get; set; }

        private List<SubjectModel> _subjectModels;
        public List<SubjectModel> SubjectModels
        {
            get
            {
                return _subjectModels;
            }
            set
            {
                _subjectModels = value;
            }
        }

        public AutoSortResult(List<SubjectModel> subjectModels, List<List<ClassGroupModel>> classGroupModelCombination)
        {
            _subjectModels = subjectModels;
            ClassGroupModelCombinations = classGroupModelCombination;
        }
    }
}
