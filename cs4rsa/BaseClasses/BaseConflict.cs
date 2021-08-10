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
        protected SchoolClass _schoolClass1;
        protected SchoolClass _schoolClass2;

        public SchoolClass FirstSchoolClass => _schoolClass1;
        public SchoolClass SecondSchoolClass => _schoolClass2;

        public BaseConflict(SchoolClass schoolClass1, SchoolClass schoolClass2)
        {
            _schoolClass1 = schoolClass1;
            _schoolClass2 = schoolClass2;
        }

        public BaseConflict(SchoolClassModel schoolClassModel1, SchoolClassModel schoolClassModel2)
        {
            _schoolClass1 = schoolClassModel1.SchoolClass;
            _schoolClass2 = schoolClassModel2.SchoolClass;
        }

        protected static bool CanConflictPhase(Phase phase1, Phase phase2)
        {
            if (phase1 == Phase.ALL || phase2 == Phase.ALL) return true;
            return phase1 == phase2;
        }
    }
}
