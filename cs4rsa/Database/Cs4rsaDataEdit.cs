using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using cs4rsa.Database;
using cs4rsa.Models;
using cs4rsa.Crawler;
using cs4rsa.BasicData;

namespace cs4rsa.Database
{
    class Cs4rsaDataEdit
    {
        private static Cs4rsaDatabase _cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
        public static void AddDiscipline(string discipline)
        {
            string sqlCommand = $@"INSERT INTO discipline (name)
                                  VALUES('{discipline}');";
            _cs4RsaDatabase.DoSomething(sqlCommand);
        }

        public static void AddKeyword(string keyword1, string courseId, int discipline_id, string subjectName, string color=null)
        {
            string sqlCommand = $@"insert into keyword (keyword1, course_id, discipline_id, subject_name, color)
                                    values ('{keyword1}',{courseId},{discipline_id},'{subjectName}', '{color}')";
            _cs4RsaDatabase.DoSomething(sqlCommand);
        }

        public static void AddSession(string name, string saveDate, List<ClassGroupModel> classGroupModels)
        {
            string semesterValue = HomeCourseSearch.GetInstance().CurrentSemesterValue;
            string yearValue = HomeCourseSearch.GetInstance().CurrentYearValue;
            string sql = $@"insert into session(name, save_date, semester, year) values ('{name}', '{saveDate}','{semesterValue}',{yearValue})";
            _cs4RsaDatabase.DoSomething(sql);

            string sqlGetId = "select id from session ORDER by id desc LIMIT 1";
            long id = _cs4RsaDatabase.GetScalar<long>(sqlGetId);
            foreach (ClassGroupModel item in classGroupModels)
            {
                sql = $@"insert into session_detail(session_id, subject_code, class_group) 
                        VALUES ({id}, '{item.SubjectCode}', '{item.Name}')";
                _cs4RsaDatabase.DoSomething(sql);
            }
        }

        public static int DeleteSession(string id)
        {
            string sqlSessionDetail = $@"delete from session_detail
                            where session_id = {id}";
            string sqlsession = $@"delete from session
                            where id = {id}";
            _cs4RsaDatabase.DoSomething(sqlSessionDetail);
            int result = _cs4RsaDatabase.DoSomething(sqlsession);
            return result;
        }

        private static int DeleteTable(string tableName)
        {
            string sql = $"delete from {tableName}";
            return _cs4RsaDatabase.DoSomething(sql);
        }

        public static int DeleteDataInTableDiscipline()
        {
            return DeleteTable("discipline");
        }

        public static int DeleteDataInTableKeyword()
        {
            return DeleteTable("keyword");
        }

        public static int AddStudent(StudentInfo studentInfo)
        {
            string sql = $@"insert into student
                           VALUES('{studentInfo.StudentId}', '{studentInfo.SpecialString}', '{studentInfo.Name}',
                           '{studentInfo.Birthday}', '{studentInfo.CMND}', 
                           '{studentInfo.Email}', '{studentInfo.PhoneNumber}', '{studentInfo.Address}', '{studentInfo.Image}', '{studentInfo.Curriculum.CurId}')";
            return _cs4RsaDatabase.DoSomething(sql);
        }

        public static int UpdateStudent(StudentInfo studentInfo)
        {
            string sql = $@"update student
                            SET name = '{studentInfo.Name}',
                            birthDay = '{studentInfo.Birthday}',
                            cmnd = '{studentInfo.CMND}',
                            email = '{studentInfo.Email}',
                            phoneNumber = '{studentInfo.PhoneNumber}',
                            address = '{studentInfo.Address}',
                            image = '{studentInfo.Image}',
                            curid = '{studentInfo.Curriculum.CurId}'
                            WHERE specialString = '{studentInfo.SpecialString}'";
            return _cs4RsaDatabase.DoSomething(sql);
        }

        public static int AddProgramSubject(ProgramSubject subject)
        {
            string sql = $@"insert into program_subject
                            values('{subject.SubjectCode}', '{subject.CourseId}','{subject.SubjectName}', {subject.StudyUnit})";
            return _cs4RsaDatabase.DoSomething(sql);
        }

        public static int UpdateProgramSubject(ProgramSubject subject)
        {
            string sql = $@"update program_subject
                            set subject_name = {subject.SubjectName}, 
                            credit = {subject.StudyUnit}
                            where subject_code = '{subject.SubjectCode}'";
            return _cs4RsaDatabase.DoSomething(sql);
        }

        /// <summary>
        /// Thêm mã môn và bảng môn tiên quyết và song hành
        /// </summary>
        public static int AddPreParSubject(string subjectCode)
        {
            string sql = $@"insert into pre_par_subject
                            values('{subjectCode}')";
            return _cs4RsaDatabase.DoSomething(sql);
        }

        public static int AddPreSubjectDetail(string proSubjectCode, string preSubjectCode)
        {
            string sql = $@"insert into detail_pre
                            values ('{proSubjectCode}', '{preSubjectCode}')";
            return _cs4RsaDatabase.DoSomething(sql);
        }

        public static int AddParSubjectDetail(string proSubjectCode, string parSubjectCode)
        {
            string sql = $@"insert into detail_par
                            values ('{proSubjectCode}', '{parSubjectCode}')";
            return _cs4RsaDatabase.DoSomething(sql);
        }

        public static int AddProSubjectDetail(string studentId, string proSubjectCode)
        {
            string sql = $@"insert into detail_program_subject values ('{studentId}', '{proSubjectCode}')";
            return _cs4RsaDatabase.DoSomething(sql);
        }

        public static int AddCurriculum(Curriculum curriculum)
        {
            string sql = $@"insert into curriculum values ('{curriculum.CurId}','{curriculum.Name}')";
            return _cs4RsaDatabase.DoSomething(sql);
        }
    }
}
