using System.Collections.Generic;
using System.Text;
using Cs4rsa.Database.DataProviders;
using Cs4rsa.Database.Interfaces;

namespace Cs4rsa.Database.Implements
{
    public class SettingRepository : ISettingRepository
    {
        private readonly RawSql _rawSql;

        public SettingRepository(RawSql rawSql)
        {
            _rawSql = rawSql;
        }
        public string GetByKey(string key)
        {
            var sql = $"SELECT Value FROM Settings WHERE Key = @Key LIMIT 1";
            var param = new Dictionary<string, object>()
            {
                {"@Key", key}
            };
            return _rawSql.ExecScalar(sql, param, "");
        }

        public void UpdateSemesterSetting(
              string yearInf
            , string yearVl
            , string semesterInf
            , string semesterVl
        )
        {
            var sb = new StringBuilder()
                .AppendLine("UPDATE Settings SET Value = @semesterInf WHERE Key = @StCurrentSemesterInfo;")
                .AppendLine("UPDATE Settings SET Value = @semesterVl  WHERE Key = @StCurrentSemesterValue;")
                .AppendLine("UPDATE Settings SET Value = @yearInf     WHERE Key = @StCurrentYearInfo;")
                .AppendLine("UPDATE Settings SET Value = @yearVl      WHERE Key = @StCurrentYearValue;");
            var param = new Dictionary<string, object>()
            {
                { "@semesterInf", semesterInf},
                { "@semesterVl", semesterVl},
                { "@yearInf", yearInf},
                { "@yearVl", yearVl},
                { "@StCurrentSemesterInfo", DbConsts.StCurrentSemesterInfo},
                { "@StCurrentSemesterValue", DbConsts.StCurrentSemesterValue},
                { "@StCurrentYearInfo", DbConsts.StCurrentYearInfo},
                { "@StCurrentYearValue", DbConsts.StCurrentYearValue},
            };
            _rawSql.ExecNonQuery(sb.ToString(), param);
        }

        public void InsertSemesterSetting(
              string yearInf
            , string yearVl
            , string semesterInf
            , string semesterVl
        )
        {
            var sb = new StringBuilder()
                .AppendLine("INSERT INTO Settings(Key, Value) VALUES")
                .AppendLine("  (@StCurrentSemesterInfo, @semesterInf)")
                .AppendLine(", (@StCurrentSemesterValue, @semesterVl)")
                .AppendLine(", (@StCurrentYearInfo, @yearInf)")
                .AppendLine(", (@StCurrentYearValue, @yearVl)");
            var param = new Dictionary<string, object>()
            {
                { "@StCurrentSemesterInfo", DbConsts.StCurrentSemesterInfo },
                { "@StCurrentSemesterValue", DbConsts.StCurrentSemesterValue },
                { "@StCurrentYearInfo", DbConsts.StCurrentYearInfo },
                { "@StCurrentYearValue", DbConsts.StCurrentYearValue },
                { "@semesterInf", semesterInf},
                { "@semesterVl", semesterVl},
                { "@yearInf", yearInf },
                { "@yearVl", yearVl}
            };
            _rawSql.ExecNonQuery(sb.ToString(), param);
        }

        public void InsertOrUpdateLastOfScreenIndex(string idx)
        {
            const string countSettingSql = "SELECT COUNT(*) FROM Settings WHERE Key = @key LIMIT 1;";
            var param = new Dictionary<string, object>() 
            { 
                { "@idx", idx },
                { "@key", DbConsts.StLastOfScreenIdx },
            };
            var countResult = _rawSql.ExecScalar(countSettingSql, param, 0L);
            if (countResult == 0)
            {
                var insertSettingSql = $"INSERT INTO Settings(Key, Value) VALUES (@key, @idx);";
                _rawSql.ExecNonQuery(insertSettingSql, param);
            }
            else
            {
                const string updateStSql = "UPDATE Settings SET Value = @idx WHERE Key = @key";
                _rawSql.ExecNonQuery(updateStSql, param);
            }
        }

        public IDictionary<string, string> GetSettings()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            _rawSql.ExecReader("SELECT * FROM Settings", reader =>
            {
                settings.Add(reader.GetString(0), reader.GetString(1));
            });
            return settings;
        }
    }
}
