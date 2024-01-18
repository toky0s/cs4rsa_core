using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Interfaces
{
    public interface ISettingRepository
    {
        void UpdateSemesterSetting(string yearInf, string yearVl, string semesterInf, string semesterVl);
        void InsertSemesterSetting(string yearInf, string yearVl, string semesterInf, string semesterVl);
        void InsertOrUpdateLastOfScreenIndex(string idx);
        string GetByKey(string key);
    }
}
