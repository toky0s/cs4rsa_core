using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;

namespace cs4rsa.Models
{
    class ConflictModel
    {
        private ClassGroup _classGroup1;
        private ClassGroup _classGroup2;
        private ConflictTime _conflictTime;

        public ClassGroup ClassGroup1
        {
            get
            {
                return _classGroup1;
            }
            set
            {
                _classGroup1 = value;
            }
        }
        public ClassGroup ClassGroup2
        {
            get
            {
                return _classGroup2;
            }
            set
            {
                _classGroup2 = value;
            }
        }

        public ConflictTime ConflictTime
        {
            get
            {
                return _conflictTime;
            }
            set
            {
                _conflictTime = value;
            }
        }

        public ConflictModel(Conflict conflict)
        {
            _classGroup1 = conflict.ClassGroupFirst;
            _classGroup2 = conflict.ClassGroupSecond;
            _conflictTime = conflict.GetConflictTime();
        }

        public ConflictModel(Conflict conflict, ConflictTime conflictTime)
        {
            _classGroup1 = conflict.ClassGroupFirst;
            _classGroup2 = conflict.ClassGroupSecond;
            _conflictTime = conflictTime;
        }
    }
}
