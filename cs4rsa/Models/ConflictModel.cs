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
        public readonly ClassGroup ClassGroup1;
        public readonly ClassGroup ClassGroup2;
        public readonly ConflictTime ConflictTime;
        public ConflictModel(Conflict conflict)
        {
            ClassGroup1 = conflict.ClassGroupFirst;
            ClassGroup2 = conflict.ClassGroupSecond;
            ConflictTime = conflict.GetConflictTime();
        }
    }
}
