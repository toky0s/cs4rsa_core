using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;

namespace Cs4rsa.Services.ConflictSvc.DataTypes
{
    /// <summary>
    /// Mọi Conflict đều phải kế thừa từ lớp này
    /// </summary>
    public abstract class BaseConflict
    {
        protected SchoolClassModel _schoolClass1;
        protected SchoolClassModel _schoolClass2;

        public SchoolClassModel FirstSchoolClass => _schoolClass1;
        public SchoolClassModel SecondSchoolClass => _schoolClass2;

        public BaseConflict(SchoolClassModel schoolClass1, SchoolClassModel schoolClass2)
        {
            _schoolClass1 = schoolClass1;
            _schoolClass2 = schoolClass2;
        }
    }
}
