using Cs4rsa.Database.Models;

using System.Collections.Generic;
using System.Data;

namespace Cs4rsa.Database.Interfaces
{
    public interface ISettingRepository
    {
        void UpdateSemesterSetting(string yearInf, string yearVl, string semesterInf, string semesterVl);
        void InsertSemesterSetting(string yearInf, string yearVl, string semesterInf, string semesterVl);
        void InsertOrUpdateLastOfScreenIndex(string idx);
        string GetByKey(string key);
        IDictionary<string, string> GetSettings();
    }
}
