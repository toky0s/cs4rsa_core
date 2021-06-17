using cs4rsa.BasicData;
using cs4rsa.Database;
using cs4rsa.Interfaces;

namespace cs4rsa.Implements
{
    class TeacherSaver : ITeacherSaver
    {
        private Cs4rsaDatabase cs4RsaDatabase;
        public TeacherSaver()
        {
            cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
        }

        private bool IsExist(Teacher teacher)
        {

            string countQueryString = $@"SELECT count(id) from teacher
                                    WHERE id like {teacher.Id}";
            long result = cs4RsaDatabase.GetScalar<long>(countQueryString);
            if (result > 0)
                return true;
            return false;
        }

        private bool IsExist(string teacherId)
        {
            string countQueryString = $@"SELECT count(id) from teacher
                                    WHERE id like {teacherId}";
            long result = cs4RsaDatabase.GetScalar<long>(countQueryString);
            if (result > 0)
                return true;
            return false;
        }

        public void Save(Teacher teacher)
        {
            string doSQLString = $@"insert into teacher values ({teacher.Id},
                                    '{teacher.Name}',
                                    '{teacher.Sex}',
                                    '{teacher.Place}',
                                    '{teacher.Degree}',
                                    '{teacher.WorkUnit}',
                                    '{teacher.Position}',
                                    '{teacher.Subject}',
                                    '{teacher.Form}')";
            cs4RsaDatabase.DoSomething(doSQLString);
            SaveDetail(teacher);
        }

        private void SaveDetail(Teacher teacher)
        {
            foreach (string subjectName in teacher.TeachedSubjects)
            {
                long currentId = cs4RsaDatabase.GetScalar<long>("select count(id) from teacher_detail")+1;
                string updateDetailSQLString = $@"insert into teacher_detail values('{currentId}','{teacher.Id}', '{subjectName}')";
                cs4RsaDatabase.DoSomething(updateDetailSQLString);
            }

        }
    }
}
