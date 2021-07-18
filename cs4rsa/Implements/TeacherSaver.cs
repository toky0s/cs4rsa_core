using cs4rsa.BasicData;
using cs4rsa.Database;
using cs4rsa.Interfaces;

namespace cs4rsa.Implements
{
    class TeacherSaver : ISaver<Teacher>
    {
        private Cs4rsaDatabase _cs4RsaDatabase;
        public TeacherSaver()
        {
            _cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
        }

        private bool IsExist(Teacher teacher)
        {

            string countQueryString = $@"SELECT count(id) from teacher
                                    WHERE id like {teacher.Id}";
            long result = _cs4RsaDatabase.GetScalar<long>(countQueryString);
            if (result > 0)
                return true;
            return false;
        }

        private bool IsExist(string teacherId)
        {
            string countQueryString = $@"SELECT count(id) from teacher
                                    WHERE id like {teacherId}";
            long result = _cs4RsaDatabase.GetScalar<long>(countQueryString);
            if (result > 0)
                return true;
            return false;
        }

        private void SaveDetail(Teacher teacher)
        {
            foreach (string subjectName in teacher.TeachedSubjects)
            {
                long currentId = _cs4RsaDatabase.GetScalar<long>("select count(id) from teacher_detail")+1;
                string updateDetailSQLString = $@"insert into teacher_detail values('{currentId}','{teacher.Id}', '{subjectName}')";
                _cs4RsaDatabase.DoSomething(updateDetailSQLString);
            }

        }

        public void Save(Teacher obj, object[] parameters = null)
        {
            string doSQLString = $@"insert into teacher values ({obj.Id},
                                    '{obj.Name}',
                                    '{obj.Sex}',
                                    '{obj.Place}',
                                    '{obj.Degree}',
                                    '{obj.WorkUnit}',
                                    '{obj.Position}',
                                    '{obj.Subject}',
                                    '{obj.Form}')";
            _cs4RsaDatabase.DoSomething(doSQLString);
            SaveDetail(obj);
        }
    }
}
