﻿using cs4rsa.BasicData;
using cs4rsa.Database;
using cs4rsa.Interfaces;

namespace cs4rsa.Implements
{
    class TeacherSaver : ITeacherSaver
    {
        private Cs4rsaDatabase cs4RsaDatabase;
        public TeacherSaver()
        {
            string connectString = Cs4rsaData.ConnectString;
            cs4RsaDatabase = new Cs4rsaDatabase(connectString);
        }

        private bool IsExist(Teacher teacher)
        {

            string countQueryString = $@"SELECT count(id) from teacher
                                    WHERE id like {teacher.Id}";
            long result = cs4RsaDatabase.CountSomething(countQueryString);
            if (result > 0)
                return true;
            return false;
        }

        private bool IsExist(string teacherId)
        {
            string countQueryString = $@"SELECT count(id) from teacher
                                    WHERE id like {teacherId}";
            long result = cs4RsaDatabase.CountSomething(countQueryString);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjectName"></param>
        /// <returns></returns>
        private bool IsHaveOnlyOneCourseIdWithSubject(string subjectName)
        {
            string countString = $@"select count(course_id) FROM keyword where subject_name like '{subjectName}'";
            long count = cs4RsaDatabase.CountSomething(countString);
            if (count == 1)
                return true;
            return false;
        }

        private void SaveDetail(Teacher teacher)
        {
            foreach (string subjectName in teacher.TeachedSubjects)
            {
                long currentId = cs4RsaDatabase.CountSomething("select count(id) from teacher_detail")+1;
                string updateDetailSQLString = $@"insert into teacher_detail values('{currentId}','{teacher.Id}', '{subjectName}')";
                cs4RsaDatabase.DoSomething(updateDetailSQLString);
            }

        }
    }
}
