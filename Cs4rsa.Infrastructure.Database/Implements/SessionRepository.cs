using System;
using System.Collections.Generic;
using System.Text;
using Cs4rsa.Database.DataProviders;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Implements
{
    public class SessionRepository : IUserScheduleRepository
    {
        private readonly RawSql _rawSql;

        public SessionRepository(RawSql rawSql)
        {
            _rawSql = rawSql;
        }

        /// <summary>
        /// Thêm bộ lịch đã sắp xếp vào cơ sở dữ liệu
        /// </summary>
        /// <param name="userSchedule">Thông tin bộ lịch</param>
        public void Add(UserSchedule userSchedule)
        {
            var semester = _rawSql.ExecScalar("SELECT Value FROM Settings WHERE Key = 'CurrentSemesterInfo'", "Chưa thể xác định");
            var year = _rawSql.ExecScalar("SELECT Value FROM Settings WHERE Key = 'CurrentYearInfo'", "Chưa thể xác định");
            var latestUserScheduleId = _rawSql.ExecScalar("SELECT UserScheduleId FROM UserSchedules ORDER BY UserScheduleId DESC LIMIT 1", 1);
            var sb = new StringBuilder()
                .BeginTransaction()
                .AppendLine("INSERT INTO UserSchedules VALUES")
                .AppendLine("(")
                .AppendLine("  @UserScheduleId")
                .AppendLine(", @Name")
                .AppendLine(", @SaveDate")
                .AppendLine(", @SemesterValue")
                .AppendLine(", @YearValue")
                .AppendLine(", @Semester") // Thông tin học kỳ
                .AppendLine(", @Year") // Thông tin năm học
                .AppendLine(");")
                .AppendLine("INSERT INTO ScheduleDetails VALUES");
            ScheduleDetail sd;
            for (int i = 0; i < userSchedule.SessionDetails.Count; i++)
            {
                sd = userSchedule.SessionDetails[i];
                sb
                    .Append('(')
                    .Append("NULL").Append(", ")
                    .Append('\'').Append(sd.SubjectCode).Append('\'').Append(", ")
                    .Append('\'').Append(sd.SubjectName).Append('\'').Append(", ")
                    .Append('\'').Append(sd.ClassGroup).Append('\'').Append(", ")
                    .Append('\'').Append(sd.RegisterCode).Append('\'').Append(", ")
                    .Append('\'').Append(sd.SelectedSchoolClass).Append('\'').Append(", ")
                    .Append("@UserScheduleId")
                    .AppendLine(")");

                // If is not last
                if (i != userSchedule.SessionDetails.Count - 1)
                {
                    sb.Append(", ");
                }
                else
                {
                    sb.Append(";");
                }
            }
            sb.CommitTransaction();

            var @params = new Dictionary<string, object>()
            {
                { "@UserScheduleId", latestUserScheduleId + 1},
                { "@Name", userSchedule.Name},
                { "@SaveDate", userSchedule.SaveDate},
                { "@SemesterValue", userSchedule.SemesterValue},
                { "@YearValue", userSchedule.YearValue},
                { "@Semester", semester},
                { "@Year", year},
            };

            _rawSql.ExecNonQuery(sb.ToString(), @params);
        }

        public List<UserSchedule> GetAll()
        {
            return _rawSql.ExecReader(
                "SELECT * FROM UserSchedules ORDER BY UserScheduleId DESC",
                record => new UserSchedule()
                {
                    UserScheduleId = record.GetInt32(0),
                    Name = record.GetString(1),
                    SaveDate = DateTime.Parse(record.GetString(2)),
                    SemesterValue = record.GetString(3),
                    YearValue = record.GetString(4),
                    Semester = record.GetString(5),
                    Year = record.GetString(6)
                }
            );
        }

        public List<ScheduleDetail> GetSessionDetails(int sessionId)
        {
            return _rawSql.ExecReader(
                $"SELECT * FROM ScheduleDetails WHERE UserScheduleId = {sessionId}"
                , record => new ScheduleDetail()
                {
                      ScheduleDetailId = record.GetInt32(0)
                    , SubjectCode = record.GetString(1)
                    , SubjectName = record.GetString(2)
                    , ClassGroup = record.GetString(3)
                    , RegisterCode = record.GetString(4)
                    , SelectedSchoolClass = record.GetString(5)
                    , UserScheduleId = record.GetInt32(6)
                }
            );
        }

        public int Remove(int userScheduleId)
        {
            var sb = new StringBuilder()
                .BeginTransaction()
                .PragmaForeignKeysOn()
                .AppendLine("DELETE FROM ScheduleDetails")
                .AppendLine("WHERE ScheduleDetails.ScheduleDetailId IN")
                .AppendLine("   (SELECT ScheduleDetails.ScheduleDetailId")
                .AppendLine("   FROM ScheduleDetails")
                .AppendLine("   WHERE ScheduleDetails.UserScheduleId = @UserScheduleId);")
                .AppendLine("DELETE FROM UserSchedules")
                .AppendLine("WHERE UserScheduleId = @UserScheduleId;")
                .CommitTransaction();
            return _rawSql.ExecNonQuery(
                sb.ToString(),
                new Dictionary<string, object>()
                {
                    { "@UserScheduleId", userScheduleId }
                });
        }
    }
}
