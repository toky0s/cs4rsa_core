using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.Text;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class TeacherRepository : ITeacherRepository
    {
        public void Add(Teacher teacher)
        {
            StringBuilder sb = new StringBuilder()
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

            Dictionary<string, object> param = new()
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

            RawSql.ExecNonQuery(sb.ToString(), param);
        }

        public bool ExistByID(int id)
        {
            return RawSql.ExecScalar(
                "SELECT COUNT(*) FROM Teachers WHERE TeacherId = @id"
                , new Dictionary<string, object>()
                {
                    {"@id", id }
                }
                , 0L
            ) > 0L;
        }

        public Teacher GetTeacherById(int id)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Teacher>()
                .AppendLine("FROM Teachers")
                .AppendLine("WHERE")
                .AppendLine("TeacherId = @TeacherId");
            return RawSql.ExecReaderGetFirstOrDefault(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    {"@TeacherId", id}
                }
                , r => new Teacher()
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
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Teacher>()
                .AppendLine("FROM Teachers")
                .AppendLine("WHERE")
                .AppendLine("Name = @name");
            return RawSql.ExecReaderGetFirstOrDefault(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    {"@name", name}
                }
                , r => new Teacher()
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
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Teacher>()
                .AppendLine("FROM Teachers")
                .AppendLine("WHERE")
                .AppendLine("UPPER(Name) LIKE UPPER('%' || @nameOrId || '%')")
                .AppendLine("OR TeacherId LIKE '%' || @nameOrId || '%'");
            return RawSql.ExecReader(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    {"@nameOrId", nameOrId}
                }
                , r => new Teacher()
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
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Teacher>()
                .AppendLine("FROM Teachers")
                .AppendLine("LIMIT @limit")
                .AppendLine("OFFSET @page * @limit");
            return RawSql.ExecReader(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@page", page - 1},
                    { "@limit", limit}
                }
                , r => new Teacher()
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
            StringBuilder sb = new StringBuilder()
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

            Dictionary<string, object> param = new()
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

            return RawSql.ExecNonQuery(sb.ToString(), param);
        }

        public long CountPage(int size)
        {
            return RawSql.ExecScalar($"SELECT CAST(ROUND(COUNT(*) / {size} + 0.5, 0) AS INT) FROM Teachers", 0L);
        }

        public List<Teacher> GetTeachers()
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Teacher>()
                .AppendLine("FROM Teachers");
            return RawSql.ExecReader(
                sb.ToString()
                , r => new Teacher()
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
