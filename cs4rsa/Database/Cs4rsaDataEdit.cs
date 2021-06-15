using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using cs4rsa.Database;
using cs4rsa.Models;
using cs4rsa.Crawler;

namespace cs4rsa.Database
{
    class Cs4rsaDataEdit
    {
        public static void AddDiscipline(string discipline)
        {
            string sqlCommand = $@"INSERT INTO discipline (name)
                                  VALUES('{discipline}');";
            using (SQLiteConnection connection = new SQLiteConnection(Cs4rsaData.ConnectString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(sqlCommand, connection);
                command.ExecuteNonQuery();
            }
        }

        public static void AddKeyword(string keyword1, string courseId, int discipline_id, string subjectName, string color=null)
        {
            string sqlCommand = $@"insert into keyword (keyword1, course_id, discipline_id, subject_name, color)
                                    values ('{keyword1}',{courseId},{discipline_id},'{subjectName}', '{color}')";
            using (SQLiteConnection connection = new SQLiteConnection(Cs4rsaData.ConnectString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(sqlCommand, connection);
                command.ExecuteNonQuery();
            }
        }

        public static void AddSession(string name, string saveDate, List<ClassGroupModel> classGroupModels)
        {
            Cs4rsaDatabase cs4RsaDatabase = new Cs4rsaDatabase(Cs4rsaData.ConnectString);
            string semesterValue = HomeCourseSearch.GetInstance().CurrentSemesterValue;
            string yearValue = HomeCourseSearch.GetInstance().CurrentYearValue;
            string sql = $@"insert into sessionx(name, save_date, semester, year) values ('{name}', '{saveDate}','{semesterValue}',{yearValue})";
            cs4RsaDatabase.DoSomething(sql);

            string sqlGetId = "select id from sessionx ORDER by id desc LIMIT 1";
            long id = cs4RsaDatabase.GetScalar<long>(sqlGetId);
            foreach (ClassGroupModel item in classGroupModels)
            {
                sql = $@"insert into session_detail(session_id, subject_code, class_group) 
                        VALUES ({id}, '{item.SubjectCode}', '{item.Name}')";
                cs4RsaDatabase.DoSomething(sql);
            }
        }
    }
}
