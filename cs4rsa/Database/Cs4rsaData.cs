using cs4rsa.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace cs4rsa.Database
{
    public class Cs4rsaData
    {
        private SQLiteConnection connection;
        private readonly string databaseName = "cs4rsadb.db";
        private readonly string connectString;

        private readonly string SQL_DISCIPLINE_TABLE =
@"create table IF NOT EXISTS discipline (
    id integer primary key,
    name text
);";

        private readonly string SQL_KEYWORD_TABLE =
@"create table IF NOT EXISTS keyword (
    id integer primary key,
    keyword1 text,
    course_id text,
    discipline_id interger,
    subject_name text,
    FOREIGN KEY (discipline_id) REFERENCES discipline (id)
);";


        public Cs4rsaData()
        {
            if (!File.Exists(databaseName))
            {
                SQLiteConnection.CreateFile(databaseName);
                connectString = $"Data Source={databaseName};Version=3;";
                connection = new SQLiteConnection(connectString);
                CreateTable(SQL_DISCIPLINE_TABLE);
                CreateTable(SQL_KEYWORD_TABLE);
            }
            else
            {
                connectString = $"Data Source={databaseName};Version=3;";
                connection = new SQLiteConnection(connectString);
            }
        }

        private void CreateTable(string sqlString)
        {
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(sqlString, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void AddDiscipline(string discipline)
        {
            string sqlCommand = $@"INSERT INTO discipline (name)
                                  VALUES('{discipline}');";
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(sqlCommand, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void AddKeyword(string keyword1, string courseId, int discipline_id, string subjectName)
        {
            string sqlCommand = $@"insert into keyword (keyword1, course_id, discipline_id, subject_name)
                                    values ('{keyword1}',{courseId},{discipline_id},'{subjectName}')";
            connection.Open();
            SQLiteCommand command = new SQLiteCommand(sqlCommand, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }


        /// <summary>
        /// Lấy ra danh sách mã ngành.
        /// </summary>
        /// <returns></returns>
        public List<string> GetDisciplines()
        {
            List<string> disciplines = new List<string>();
            SQLiteCommand command = new SQLiteCommand
            {
                CommandText = "select name from discipline",
                Connection = connection
            };
            connection.Open();
            SQLiteDataReader reader = command.ExecuteReader();
            
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    disciplines.Add(reader.GetString(0));
                }
            }
            
            reader.Close();
            connection.Close();
            return disciplines;
        }

        public List<DisciplineKeywordModel> GetDisciplineKeywordModels(string discipline)
        {
            List<DisciplineKeywordModel> disciplineKeywordInfos = new List<DisciplineKeywordModel>();
            SQLiteCommand command = new SQLiteCommand
            {
                CommandText = $@"SELECT course_id, subject_name, keyword1 from keyword
                                where discipline_id = (select id from discipline WHERE name='{discipline}')",
                Connection = connection
            };
            connection.Open();
            SQLiteDataReader reader = command.ExecuteReader();
            
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    disciplineKeywordInfos.Add(
                        new DisciplineKeywordModel(reader.GetString(0), reader.GetString(1), reader.GetString(2))
                        );
                }
            }
            reader.Close();
            connection.Close();
            return disciplineKeywordInfos;
        }
    }
}
