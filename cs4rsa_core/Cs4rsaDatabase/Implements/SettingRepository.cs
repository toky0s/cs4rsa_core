using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class SettingRepository : GenericRepository<Setting>, ISettingRepository
    {
        public SettingRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        public string GetBykey(string key)
        {
            string sql = $"SELECT Value FROM Settings WHERE Key = \"{key}\" LIMIT 1";
            string result = string.Empty;
            return _rawSql.ExecReader(sql, (reader) =>
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    result = reader.GetString(0);
                }
                return result;
            });
        }

        public void UpdateSemesterSetting(string yearInf, string yearVl, string semesterInf, string semesterVl)
        {
            string sql =
                  $"UPDATE Settings SET Value = \"{semesterInf}\" WHERE Key = \"{VmConstants.StCurrentSemesterInfo}\";"
                + $"UPDATE Settings SET Value = \"{semesterVl}\" WHERE Key = \"{VmConstants.StCurrentSemesterValue}\";"
                + $"UPDATE Settings SET Value = \"{yearInf}\" WHERE Key = \"{VmConstants.StCurrentYearInfo}\";"
                + $"UPDATE Settings SET Value = \"{yearVl}\" WHERE Key = \"{VmConstants.StCurrentYearValue}\";";
            _ = _rawSql.ExecScalar(sql);
        }

        public void InsertSemesterSetting(string yearInf, string yearVl, string semesterInf, string semesterVl)
        {
            string sql =
                   "INSERT INTO Settings(Key, Value) VALUES"
                + $"  (\"{VmConstants.StCurrentSemesterInfo}\", \"{semesterInf}\")"
                + $", (\"{VmConstants.StCurrentSemesterValue}\", \"{semesterVl}\")"
                + $", (\"{VmConstants.StCurrentYearInfo}\", \"{yearInf}\")"
                + $", (\"{VmConstants.StCurrentYearValue}\", \"{yearVl}\");";
            _ = _rawSql.ExecScalar(sql);
        }
    }
}
