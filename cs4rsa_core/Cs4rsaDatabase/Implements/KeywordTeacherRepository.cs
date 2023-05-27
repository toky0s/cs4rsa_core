using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Text;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class KeywordTeacherRepository : IKeywordTeacherRepository
    {
        public void Add(KeywordTeacher kt)
        {
            long currentId = RawSql.ExecScalar("SELECT COUNT(*) + 1 FROM KeywordTeachers", 0L);
            StringBuilder sb = new StringBuilder()
                .AppendLine("INSERT INTO KeywordTeachers")
                .AppendLine("VALUES")
                .AppendLine("(@Id, @CourseId, @TeacherId)");
            Dictionary<string, object> param = new()
            {
                { "@Id", currentId},
                { "@CourseId", kt.CourseId},
                { "@TeacherId", kt.TeacherId},
            };
            RawSql.ExecNonQuery(sb.ToString(), param);
        }

        public bool ExistByTeacherIdAndCourseId(int teacherId, int courseId)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT COUNT(*) ")
                .AppendLine("FROM KeywordTeachers ")
                .AppendLine("WHERE TeacherId = @teacherId ")
                .AppendLine("AND CourseId = @courseId");
            return RawSql.ExecScalar(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@teacherId", teacherId },
                    { "@courseId", courseId },
                },
                0) > 0;
        }

        public bool Exists(int teacherId, int courseId)
        {
            return RawSql.ExecScalar(
                "SELECT COUNT(*) FROM KeywordTeachers WHERE CourseId = @CourseId AND TeacherId = @TeacherId"
                , new Dictionary<string, object>()
                {
                    { "@CourseId", courseId },
                    { "@TeacherId", teacherId }
                }, 0L) == 1;
        }
    }
}
