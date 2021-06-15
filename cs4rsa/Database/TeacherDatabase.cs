using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Database;

namespace cs4rsa.Database
{
    class TeacherDatabase
    {
        private string _teacherId;
        public TeacherDatabase(string teacherId)
        {
            _teacherId = teacherId;
        }

        public Teacher ToTeacher()
        {
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            string sqlQuery = $@"select * from teacher where id like {_teacherId}";
            DataTable teacherTable = cs4RsaDatabase.GetDataTable(sqlQuery);
            string sqlTeacherDetail = $@"select subject_name from teacher_detail where teacher_id like {_teacherId}";

            List<string> teacherDetailSubjects = new List<string>();
            DataTable teacherDetailTable = cs4RsaDatabase.GetDataTable(sqlTeacherDetail);
            foreach (DataRow row in teacherDetailTable.Rows)
            {
                teacherDetailSubjects.Add(row["subject_name"].ToString());
            }

            Teacher teacher = new Teacher(
                teacherTable.Rows[0]["id"].ToString(),
                teacherTable.Rows[0]["name"].ToString(),
                teacherTable.Rows[0]["sex"].ToString(),
                teacherTable.Rows[0]["place"].ToString(),
                teacherTable.Rows[0]["academic_rank"].ToString(),
                teacherTable.Rows[0]["unit"].ToString(),
                teacherTable.Rows[0]["position"].ToString(),
                teacherTable.Rows[0]["subject"].ToString(),
                teacherTable.Rows[0]["form"].ToString(),
                teacherDetailSubjects.ToArray());
            return teacher;
        }
    }
}
