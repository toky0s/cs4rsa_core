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
        private List<ClassGroupModel> _classGroupModels;
        public List<ClassGroupModel> ClassGroupModels
        {
            get
            {
                return _classGroupModels;
            }
            set
            {
                _classGroupModels = value;

            }
        }

        public AutoSortResult(List<ClassGroupModel> classGroupModels)
        {
            _classGroupModels = classGroupModels;
        }
    }
}
