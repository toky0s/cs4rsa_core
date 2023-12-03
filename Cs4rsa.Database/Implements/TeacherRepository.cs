using System.Collections.Generic;
using System.Text;
using Cs4rsa.Database.DataProviders;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Implements
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly RawSql _rawSql;

        public TeacherRepository(RawSql rawSql)
        {
            _rawSql = rawSql;
        }
        public void Add(Teacher teacher)
        {
            var sb = new StringBuilder()
                .AppendLine("INSERT INTO Teachers VALUES")
                .AppendLine("(")
                .AppendLine("  @TeacherId")
                .AppendLine(", @Name")
                .AppendLine(", @Sex")
                .AppendLine(", @Place")
                .AppendLine(", @Degree")
                .AppendLine(", @WorkUnit")
                .AppendLine(", @Position")
                .AppendLine(", @Subject")
                .AppendLine(", @Form")
                .AppendLine(", @TeachedSubjects")
                .AppendLine(", @Path")
                .AppendLine(", @Url")
                .AppendLine(")");

            var param = new Dictionary<string, object>
            {
                { "@TeacherId", teacher.TeacherId},
                { "@Name", teacher.Name},
                { "@Sex", teacher.Sex},
                { "@Place", teacher.Place},
                { "@Degree", teacher.Degree},
                { "@WorkUnit", teacher.WorkUnit},
                { "@Position", teacher.Position},
                { "@Subject", teacher.Subject},
                { "@Form", teacher.Form},
                { "@Path", teacher.Path},
                { "@TeachedSubjects", teacher.TeachedSubjects},
                { "@Url", teacher.Url},
            };

            _rawSql.ExecNonQuery(sb.ToString(), param);
        }

        public bool ExistById(int id)
        {
            return _rawSql.ExecScalar(
                "SELECT COUNT(*) FROM Teachers WHERE TeacherId = @id"
                , new Dictionary<string, object>
                {
                    {"@id", id }
                }
                , 0L
                
            ) > 0L;
        }

        public Teacher GetTeacherById(int id)
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Teacher>(_rawSql.CnnStr)
                .AppendLine("FROM Teachers")
                .AppendLine("WHERE")
                .AppendLine("TeacherId = @TeacherId");
            return _rawSql.ExecReaderGetFirstOrDefault(
                sb.ToString()
                , new Dictionary<string, object>
                {
                    {"@TeacherId", id}
                }
                , r => new Teacher
                {
                      TeacherId= r.GetInt32(0)
                    , Name= r.GetString(1)
                    , Sex= r.GetString(2)
                    , Place= r.GetString(3)
                    , Degree= r.GetString(4)
                    , WorkUnit= r.GetString(5)
                    , Position= r.GetString(6)
                    , Subject= r.IsDBNull(7) ? string.Empty : r.GetString(7)
                    , Form= r.GetString(8)
                    , TeachedSubjects= r.GetString(9)
                    , Path= r.GetString(10)
                    , Url = r.GetString(11)
                }
                
            );
        }

        public Teacher GetTeacherByName(string name)
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Teacher>(_rawSql.CnnStr)
                .AppendLine("FROM Teachers")
                .AppendLine("WHERE")
                .AppendLine("Name = @name");
            return _rawSql.ExecReaderGetFirstOrDefault(
                sb.ToString()
                , new Dictionary<string, object>
                {
                    {"@name", name}
                }
                , r => new Teacher
                {
                      TeacherId= r.GetInt32(0)
                    , Name= r.GetString(1)
                    , Sex= r.GetString(2)
                    , Place= r.GetString(3)
                    , Degree= r.GetString(4)
                    , WorkUnit= r.GetString(5)
                    , Position= r.GetString(6)
                    , Subject= r.IsDBNull(7) ? string.Empty : r.GetString(7)
                    , Form= r.GetString(8)
                    , TeachedSubjects= r.GetString(9)
                    , Path= r.GetString(10)
                    , Url = r.GetString(11)
                }
                
            );
        }

        public List<Teacher> GetTeacherByNameOrId(string nameOrId)
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Teacher>(_rawSql.CnnStr)
                .AppendLine("FROM Teachers")
                .AppendLine("WHERE")
                .AppendLine("UPPER(Name) LIKE UPPER('%' || @nameOrId || '%')")
                .AppendLine("OR TeacherId LIKE '%' || @nameOrId || '%'");
            return _rawSql.ExecReader(
                sb.ToString()
                , new Dictionary<string, object>
                {
                    {"@nameOrId", nameOrId}
                }
                , r => new Teacher
                {
                      TeacherId= r.GetInt32(0)
                    , Name= r.GetString(1)
                    , Sex= r.GetString(2)
                    , Place= r.GetString(3)
                    , Degree= r.GetString(4)
                    , WorkUnit= r.GetString(5)
                    , Position= r.GetString(6)
                    , Subject= r.IsDBNull(7) ? string.Empty : r.GetString(7)
                    , Form= r.GetString(8)
                    , TeachedSubjects= r.GetString(9)
                    , Path= r.GetString(10)
                    , Url = r.GetString(11)
                }
                
            );
        }

        public List<Teacher> GetTeachers(int page, int limit)
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Teacher>(_rawSql.CnnStr)
                .AppendLine("FROM Teachers")
                .AppendLine("LIMIT @limit")
                .AppendLine("OFFSET @page * @limit");
            return _rawSql.ExecReader(
                sb.ToString()
                , new Dictionary<string, object>
                {
                    { "@page", page - 1},
                    { "@limit", limit}
                }
                , r => new Teacher
                {
                      TeacherId= r.GetInt32(0)
                    , Name= r.GetString(1)
                    , Sex= r.GetString(2)
                    , Place= r.GetString(3)
                    , Degree= r.GetString(4)
                    , WorkUnit= r.GetString(5)
                    , Position= r.GetString(6)
                    , Subject= r.IsDBNull(7) ? string.Empty : r.GetString(7)
                    , Form= r.GetString(8)
                    , TeachedSubjects= r.GetString(9)
                    , Path= r.GetString(10)
                    , Url = r.GetString(11)
                }
                
            );
        }

        public int Update(Teacher teacher)
        {
            var sb = new StringBuilder()
                .AppendLine("UPDATE Teachers")
                .AppendLine("SET")
                .AppendLine("  Name               = @Name")
                .AppendLine(", Sex                = @Sex")
                .AppendLine(", Place              = @Place")
                .AppendLine(", Degree             = @Degree")
                .AppendLine(", WorkUnit           = @WorkUnit")
                .AppendLine(", Position           = @Position")
                .AppendLine(", Subject            = @Subject")
                .AppendLine(", Form               = @Form")
                .AppendLine(", TeachedSubjects    = @TeachedSubjects")
                .AppendLine(", Path               = @Path")
                .AppendLine(", Url                = @Url")
                .AppendLine("WHERE")
                .AppendLine("TeacherId = @TeacherId");

            var param = new Dictionary<string, object>
            {
                { "@TeacherId", teacher.TeacherId},
                { "@Name", teacher.Name},
                { "@Sex", teacher.Sex},
                { "@Place", teacher.Place},
                { "@Degree", teacher.Degree},
                { "@WorkUnit", teacher.WorkUnit},
                { "@Position", teacher.Position},
                { "@Subject", teacher.Subject},
                { "@Form", teacher.Form},
                { "@TeachedSubjects", teacher.TeachedSubjects},
                { "@Path", teacher.Path},
                { "@Url", teacher.Url},
            };

            return _rawSql.ExecNonQuery(sb.ToString(), param);
        }

        public long CountPage(int size)
        {
            return _rawSql.ExecScalar($"SELECT CAST(ROUND(COUNT(*) / {size} + 0.5, 0) AS INT) FROM Teachers", 0L);
        }

        public List<Teacher> GetTeachers()
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Teacher>(_rawSql.CnnStr)
                .AppendLine("FROM Teachers");
            return _rawSql.ExecReader(
                sb.ToString()
                , r => new Teacher
                {
                      TeacherId= r.GetInt32(0)
                    , Name= r.GetString(1)
                    , Sex= r.GetString(2)
                    , Place= r.GetString(3)
                    , Degree= r.GetString(4)
                    , WorkUnit= r.GetString(5)
                    , Position= r.GetString(6)
                    , Subject= r.IsDBNull(7) ? string.Empty : r.GetString(7)
                    , Form= r.GetString(8)
                    , TeachedSubjects= r.GetString(9)
                    , Path= r.GetString(10)
                    , Url = r.GetString(11)
                }
                
            );
        }
    }
}
