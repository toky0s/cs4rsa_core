using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.DataTypes.Enums;

namespace ConflictService.DataTypes
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

        protected static bool CanConflictPhase(Phase phase1, Phase phase2)
        {
            if (phase1 == Phase.All || phase2 == Phase.All) return true;
            return phase1 == phase2;
        }
    }
}
