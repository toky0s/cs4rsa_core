using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Dialogs.DialogResults
{
    public class AutoSortResult
    {
        private List<List<ClassGroupModel>> _classGroupModelCombinations;
        public List<List<ClassGroupModel>> ClassGroupModelCombinations
        {
            get
            {
                return _classGroupModelCombinations;
            }
            set
            {
                _classGroupModelCombinations = value;

            }
        }

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
            _classGroupModelCombinations = classGroupModelCombination;
        }
    }
}
