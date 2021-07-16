using cs4rsa.BasicData;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.BaseClasses
{
    /// <summary>
    /// Mọi Conflict đều phải kế thừa từ lớp này
    /// </summary>
    public abstract class BaseConflict
    {
        protected ClassGroup _classGroup1;
        protected ClassGroup _classGroup2;

        public ClassGroup FirstClassGroup { get => _classGroup1; private set => _classGroup1 = value; }
        public ClassGroup SecondClassGroup { get => _classGroup2; private set => _classGroup2 = value; }

        public BaseConflict(ClassGroup classGroup1, ClassGroup classGroup2)
        {
            _classGroup1 = classGroup1;
            _classGroup2 = classGroup2;
        }

        public BaseConflict(ClassGroupModel classGroup1, ClassGroupModel classGroup2)
        {
            _classGroup1 = classGroup1.ClassGroup;
            _classGroup2 = classGroup2.ClassGroup;
        }

        protected static bool CanConflictPhase(Phase phase1, Phase phase2)
        {
            if (phase1 == Phase.ALL || phase2 == Phase.ALL) return true;
            return phase1 == phase2;
        }
    }
}
