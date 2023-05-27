using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class SessionRepository : IUserScheduleRepository
    {
        public void Add(UserSchedule userSchedule)
        {
            long userScheduleId = RawSql.ExecScalar("SELECT COUNT(*) + 1 FROM UserSchedules", 0L);
            long sessionDetailId = RawSql.ExecScalar("SELECT COUNT(*) + 1 FROM ScheduleDetails", 0L);
            StringBuilder sb = new StringBuilder()
                .AppendLine("INSERT INTO UserSchedules VALUES")
                .AppendLine("(")
                .AppendLine("  @UserScheduleId")
                .AppendLine(", @Name")
                .AppendLine(", @SaveDate")
                .AppendLine(", @SemesterValue")
                .AppendLine(", @YearValue")
                .AppendLine(");")
                .AppendLine("INSERT INTO ScheduleDetails VALUES");
            foreach (ScheduleDetail sd in userSchedule.SessionDetails)
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

            Dictionary<string, object> param = new()
            {
                { "@UserScheduleId", userScheduleId},
                { "@Name", userSchedule.Name},
                { "@SaveDate", userSchedule.SaveDate},
                { "@SemesterValue", userSchedule.SemesterValue},
                { "@YearValue", userSchedule.YearValue},
            };

            RawSql.ExecNonQuery(sb.ToString(), param);
        }

        public List<UserSchedule> GetAll()
        {
            return RawSql.ExecReader(
                $"SELECT * FROM UserSchedules"
                , record => new UserSchedule()
                {
                      UserScheduleId = record.GetInt32(0)
                    , Name = record.GetString(1)
                    , SaveDate = DateTime.Parse(record.GetString(2))
                    , SemesterValue = record.GetString(3)
                    , YearValue = record.GetString(4)
                }
            );
        }

        public List<ScheduleDetail> GetSessionDetails(int sessionId)
        {
            return RawSql.ExecReader(
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
            StringBuilder sb = new StringBuilder()
                .AppendLine("DELETE FROM UserSchedules")
                .AppendLine("WHERE UserScheduleId = @UserScheduleId");
            return RawSql.ExecNonQuery(
                sb.ToString()
            , new Dictionary<string, object>()
                {
                    { "@UserScheduleId", userSchedule.UserScheduleId }
                });
        }
    }
}
