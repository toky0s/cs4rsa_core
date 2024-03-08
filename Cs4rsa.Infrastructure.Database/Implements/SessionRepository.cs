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

            var userScheduleId = _rawSql.ExecScalar("SELECT COUNT(*) + 1 FROM UserSchedules", 0L);
            var sessionDetailId = _rawSql.ExecScalar("SELECT COUNT(*) + 1 FROM ScheduleDetails", 0L);
            var sb = new StringBuilder()
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
            foreach (var sd in userSchedule.SessionDetails)
            {
                sb
                    .Append('(')
                    .Append(sessionDetailId).Append(", ")
                    .Append('\'').Append(sd.SubjectCode).Append('\'').Append(", ")
                    .Append('\'').Append(sd.SubjectName).Append('\'').Append(", ")
                    .Append('\'').Append(sd.ClassGroup).Append('\'').Append(", ")
                    .Append('\'').Append(sd.RegisterCode).Append('\'').Append(", ")
                    .Append('\'').Append(sd.SelectedSchoolClass).Append('\'').Append(", ")
                    .Append(userScheduleId)
                    .AppendLine("),");
                sessionDetailId++;
            }
            sb.RemoveLastCharAfterAppendLine();

            var param = new Dictionary<string, object>()
            {
                { "@UserScheduleId", userScheduleId},
                { "@Name", userSchedule.Name},
                { "@SaveDate", userSchedule.SaveDate},
                { "@SemesterValue", userSchedule.SemesterValue},
                { "@YearValue", userSchedule.YearValue},
                { "@Semester", semester},
                { "@Year", year},
            };

            _rawSql.ExecNonQuery(sb.ToString(), param);
        }

        public List<UserSchedule> GetAll()
        {
            return _rawSql.ExecReader(
                $"SELECT * FROM UserSchedules",
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

        public int Remove(UserSchedule userSchedule)
        {
            if (userSchedule == null)
            {
                throw new NullReferenceException("UserSchedule was null");
            }
            var sb = new StringBuilder()
                .AppendLine("DELETE FROM UserSchedules")
                .AppendLine("WHERE UserScheduleId = @UserScheduleId");
            return _rawSql.ExecNonQuery(
                sb.ToString(),
                new Dictionary<string, object>()
                {
                    { "@UserScheduleId", userSchedule.UserScheduleId }
                });
        }

        public int Remove(int userScheduleId)
        {
            var sb = new StringBuilder()
                .AppendLine("DELETE FROM UserSchedules")
                .AppendLine("WHERE UserScheduleId = @UserScheduleId");
            return _rawSql.ExecNonQuery(
                sb.ToString(),
                new Dictionary<string, object>()
                {
                    { "@UserScheduleId", userScheduleId }
                });
        }
    }
}
