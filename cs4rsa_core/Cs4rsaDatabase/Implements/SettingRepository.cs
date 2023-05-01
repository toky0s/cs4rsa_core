using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Diagnostics;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class SettingRepository : GenericRepository<Setting>, ISettingRepository
    {
        public SettingRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        public string GetBykey(string key)
        {
            string sql = $"SELECT Value FROM Settings WHERE Key = @Key LIMIT 1";
            Dictionary<string, object> param = new()
            {
                {"@Key", key}
            };
            return _rawSql.ExecScalar(sql, param, "");
        }

        public void UpdateSemesterSetting(string yearInf, string yearVl, string semesterInf, string semesterVl)
        {
            string sql =
                  $"UPDATE Settings SET Value = '{semesterInf}' WHERE Key = '{VmConstants.StCurrentSemesterInfo}';"
                + $"UPDATE Settings SET Value = '{semesterVl}'  WHERE Key = '{VmConstants.StCurrentSemesterValue}';"
                + $"UPDATE Settings SET Value = '{yearInf}'     WHERE Key = '{VmConstants.StCurrentYearInfo}';"
                + $"UPDATE Settings SET Value = '{yearVl}'      WHERE Key = '{VmConstants.StCurrentYearValue}\'";
            _rawSql.ExecNonQuery(sql, null);
        }

        public void InsertSemesterSetting(string yearInf, string yearVl, string semesterInf, string semesterVl)
        {
            string sql =
                   "INSERT INTO Settings(Key, Value) VALUES"
                + $"  ('{VmConstants.StCurrentSemesterInfo}', '{semesterInf}')"
                + $", ('{VmConstants.StCurrentSemesterValue}', '{semesterVl}')"
                + $", ('{VmConstants.StCurrentYearInfo}', '{yearInf}')"
                + $", ('{VmConstants.StCurrentYearValue}', '{yearVl}');";
            _rawSql.ExecNonQuery(sql, null);
        }

        public void InsertOrUpdateLastOfScreenIndex(string idx)
        {
            string countSettingSql = "SELECT COUNT(*) FROM Settings WHERE Key = @key LIMIT 1;";
            Dictionary<string, object> param = new() 
            { 
                { "@idx", idx },
                { "@key", VmConstants.StLastOfScreenIdx },
            };
            long countResult = _rawSql.ExecScalar(countSettingSql, param, 0L);
            if (countResult == 0)
            {
                string insertSettingSql = $"INSERT INTO Settings(Key, Value) VALUES (@key, @idx);";
                int insertResult = _rawSql.ExecNonQuery(insertSettingSql, param);
                Debug.Assert(insertResult > 0);
            }
            else
            {
                string updateStSql = "UPDATE Settings SET Value = @idx WHERE Key = @key";
                int insertResult = _rawSql.ExecNonQuery(updateStSql, param);
                Debug.Assert(insertResult > 0);
            }
        }
    }
}
