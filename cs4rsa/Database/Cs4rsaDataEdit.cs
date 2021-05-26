using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using cs4rsa.Database;

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
    }
}
