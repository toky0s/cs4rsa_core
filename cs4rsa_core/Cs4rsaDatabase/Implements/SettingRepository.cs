using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class SettingRepository : ISettingRepository
    {
        public string GetBykey(string key)
        {
            string sql = $"SELECT Value FROM Settings WHERE Key = @Key LIMIT 1";
            Dictionary<string, object> param = new()
            {
                {"@Key", key}
            };
            return RawSql.ExecScalar(sql, param, "");
        }

        public void UpdateSemesterSetting(
              string yearInf
            , string yearVl
            , string semesterInf
            , string semesterVl
        )
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("UPDATE Settings SET Value = @semesterInf WHERE Key = @StCurrentSemesterInfo;")
                .AppendLine("UPDATE Settings SET Value = @semesterVl  WHERE Key = @StCurrentSemesterValue;")
                .AppendLine("UPDATE Settings SET Value = @yearInf     WHERE Key = @StCurrentYearInfo;")
                .AppendLine("UPDATE Settings SET Value = @yearVl      WHERE Key = @StCurrentYearValue;");
            Dictionary<string, object> param = new()
            {
                { "@semesterInf", semesterInf},
                { "@semesterVl", semesterVl},
                { "@yearInf", yearInf},
                { "@yearVl", yearVl},
                { "@StCurrentSemesterInfo", VmConstants.StCurrentSemesterInfo},
                { "@StCurrentSemesterValue", VmConstants.StCurrentSemesterValue},
                { "@StCurrentYearInfo", VmConstants.StCurrentYearInfo},
                { "@StCurrentYearValue", VmConstants.StCurrentYearValue},
            };
            RawSql.ExecNonQuery(sb.ToString(), param);
        }

        public void InsertSemesterSetting(
              string yearInf
            , string yearVl
            , string semesterInf
            , string semesterVl
        )
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("INSERT INTO Settings(Key, Value) VALUES")
                .AppendLine("  (@StCurrentSemesterInfo, @semesterInf)")
                .AppendLine(", (@StCurrentSemesterValue, @semesterVl)")
                .AppendLine(", (@StCurrentYearInfo, @yearInf)")
                .AppendLine(", (@StCurrentYearValue, @yearVl)");
            Dictionary<string, object> param = new()
            {
                { "@StCurrentSemesterInfo", VmConstants.StCurrentSemesterInfo },
                { "@StCurrentSemesterValue", VmConstants.StCurrentSemesterValue },
                { "@StCurrentYearInfo", VmConstants.StCurrentYearInfo },
                { "@StCurrentYearValue", VmConstants.StCurrentYearValue },
                { "@semesterInf", semesterInf},
                { "@semesterVl", semesterVl},
                { "@yearInf", yearInf },
                { "@yearVl", yearVl}
            };
            RawSql.ExecNonQuery(sb.ToString(), param);
        }

        public void InsertOrUpdateLastOfScreenIndex(string idx)
        {
            string countSettingSql = "SELECT COUNT(*) FROM Settings WHERE Key = @key LIMIT 1;";
            Dictionary<string, object> param = new() 
            { 
                { "@idx", idx },
                { "@key", VmConstants.StLastOfScreenIdx },
            };
            long countResult = RawSql.ExecScalar(countSettingSql, param, 0L);
            if (countResult == 0)
            {
                string insertSettingSql = $"INSERT INTO Settings(Key, Value) VALUES (@key, @idx);";
                int insertResult = RawSql.ExecNonQuery(insertSettingSql, param);
                Debug.Assert(insertResult > 0);
            }
            else
            {
                string updateStSql = "UPDATE Settings SET Value = @idx WHERE Key = @key";
                int insertResult = RawSql.ExecNonQuery(updateStSql, param);
                Debug.Assert(insertResult > 0);
            }
        }
    }
}
