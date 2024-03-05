using System.Collections.Generic;
using System.Text;
using Cs4rsa.Database.DataProviders;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Implements
{
    public class KeywordTeacherRepository : IKeywordTeacherRepository
    {
        private readonly RawSql _rawSql;

        public KeywordTeacherRepository(RawSql rawSql)
        {
            _rawSql = rawSql;
        }
        
        public void Add(KeywordTeacher kt)
        {
            var currentId = _rawSql.ExecScalar("SELECT COUNT(*) + 1 FROM KeywordTeachers", 0L);
            var sb = new StringBuilder()
                .AppendLine("INSERT INTO KeywordTeachers")
                .AppendLine("VALUES")
                .AppendLine("(@Id, @CourseId, @TeacherId)");
            var param = new Dictionary<string, object>()
            {
                { "@Id", currentId},
                { "@CourseId", kt.CourseId},
                { "@TeacherId", kt.TeacherId},
            };
            _rawSql.ExecNonQuery(sb.ToString(), param);
        }

        public bool ExistByTeacherIdAndCourseId(int teacherId, int courseId)
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT COUNT(*) ")
                .AppendLine("FROM KeywordTeachers ")
                .AppendLine("WHERE TeacherId = @teacherId ")
                .AppendLine("AND CourseId = @courseId");
            return _rawSql.ExecScalar(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@teacherId", teacherId },
                    { "@courseId", courseId },
                },
                0
                ) > 0;
        }

        public bool Exists(int teacherId, int courseId)
        {
            return _rawSql.ExecScalar(
                "SELECT COUNT(*) FROM KeywordTeachers WHERE CourseId = @CourseId AND TeacherId = @TeacherId"
                , new Dictionary<string, object>()
                {
                    { "@CourseId", courseId },
                    { "@TeacherId", teacherId }
                }, 0L
                ) == 1;
        }
    }
}
