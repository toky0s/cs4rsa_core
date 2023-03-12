using Cs4rsa.Cs4rsaDatabase.Models;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface ISettingRepository : IGenericRepository<Setting>
    {
        void UpdateSemesterSetting(string yearInf, string yearVl, string semesterInf, string semesterVl);
        void InsertSemesterSetting(string yearInf, string yearVl, string semesterInf, string semesterVl);
        string GetBykey(string key);
    }
}
