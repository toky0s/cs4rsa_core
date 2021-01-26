using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BasicData
{
    class Conflict
    {
        private ClassGroup classGroup1;
        private ClassGroup classGroup2;

        public Conflict(ClassGroup classGroup1, ClassGroup classGroup2)
        {
            this.classGroup1 = classGroup1;
            this.classGroup2 = classGroup2;
        }

        public bool isConflict()
        {
            return true;
        }
    }
}
