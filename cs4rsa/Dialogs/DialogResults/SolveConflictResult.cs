using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Models;

namespace cs4rsa.Dialogs.DialogResults
{
    public class SolveConflictResult
    {
        private ClassGroupModel _classGroupModel;
        public ClassGroupModel ClassGroupModel
        {
            get { return _classGroupModel; }
            set { _classGroupModel = value; }
        }

        public SolveConflictResult(ClassGroupModel classGroupModel)
        {
            _classGroupModel = classGroupModel;
        }
    }
}
