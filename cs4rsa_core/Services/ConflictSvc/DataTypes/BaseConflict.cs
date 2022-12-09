using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;

namespace Cs4rsa.Services.ConflictSvc.DataTypes
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
    }
}
