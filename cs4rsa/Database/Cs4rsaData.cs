using cs4rsa.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace cs4rsa.Database
{
    public class Cs4rsaData
    {
        private static readonly string databaseName = "cs4rsadb.db";
        public static readonly string connectString = $"Data Source={databaseName};Version=3;";

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

        private readonly string SQL_TEACHER_TABLE =
            @"create table if not exists teacher (
                id text primary key,
                name text,
                sex text,
                place text,
                academic_rank text,
                unit text,
                position text,
                subject text,
                form text
            )";

        private readonly string SQL_TEACHER_DETAIL_TABLE =
            @"create table if not exists teacher_detail (
                id integer primary key,
                teacher_id text,
                subject_name text,
                FOREIGN KEY (teacher_id) REFERENCES teacher (id)
            )";

        private readonly string SQL_SESSION =
            @"create table if not exists session (
                id integer primary key AUTOINCREMENT,
                name text,
                save_date text
            )";

        private readonly string SQL_SESSION_DETAIL =
            @"create table if not exists session_detail (
                id integer primary key AUTOINCREMENT,
                session_id integer,
                subject_code text,
                class_group text,
                foreign key (session_id) references session(id)
            )";

        public Cs4rsaData()
        {
            if (!File.Exists(databaseName))
            {
                SQLiteConnection.CreateFile(databaseName);
            }
            CreateTable(SQL_DISCIPLINE_TABLE);
            CreateTable(SQL_KEYWORD_TABLE);
            CreateTable(SQL_TEACHER_TABLE);
            CreateTable(SQL_TEACHER_DETAIL_TABLE);
            CreateTable(SQL_SESSION);
            CreateTable(SQL_SESSION_DETAIL);
        }

        public void CreateDatabaseIfNotExist()
        {
            if (IsDatabaseExist())
            {
                SQLiteConnection.CreateFile(databaseName);
                CreateTable(SQL_DISCIPLINE_TABLE);
                CreateTable(SQL_KEYWORD_TABLE);
            }
        }

        private bool IsDatabaseExist()
        {
            if (!File.Exists(databaseName))
            {
                return false;
            }
            return true;
        }

        private void CreateTable(string sqlString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(sqlString, connection);
                command.ExecuteNonQuery();
            }
        }

        public void AddDiscipline(string discipline)
        {
            string sqlCommand = $@"INSERT INTO discipline (name)
                                  VALUES('{discipline}');";
            using (SQLiteConnection connection = new SQLiteConnection(connectString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(sqlCommand, connection);
                command.ExecuteNonQuery();
            }
        }

        public void AddKeyword(string keyword1, string courseId, int discipline_id, string subjectName)
        {
            string sqlCommand = $@"insert into keyword (keyword1, course_id, discipline_id, subject_name)
                                    values ('{keyword1}',{courseId},{discipline_id},'{subjectName}')";
            using (SQLiteConnection connection = new SQLiteConnection(connectString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand(sqlCommand, connection);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Lấy ra danh sách mã ngành.
        /// </summary>
        /// <returns></returns>
        public List<string> GetDisciplines()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectString))
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
                return disciplines;
            }
        }

        public List<DisciplineKeywordModel> GetDisciplineKeywordModels(string discipline)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectString))
            {
                List<DisciplineKeywordModel> disciplineKeywordInfos = new List<DisciplineKeywordModel>();
                string sql = $@"SELECT course_id, subject_name, name, keyword1 from keyword, discipline
                                    where discipline_id = (select id from discipline WHERE name='{discipline}') AND
								    name = '{discipline}'";
                SQLiteCommand command = new SQLiteCommand(sql, connection);

                connection.Open();
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        disciplineKeywordInfos.Add(
                            new DisciplineKeywordModel(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3))
                        );
                    }
                }
                reader.Close();
                return disciplineKeywordInfos;
            }
        }

        public DisciplineKeywordModel GetSingleDisciplineKeywordModel(string discipline, string keyword1)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectString))
            {
                DisciplineKeywordModel disciplineKeywordModel = null;
                string sql = $@"SELECT course_id, subject_name, name, keyword1 from keyword, discipline
                            where keyword1 = '{keyword1}' and
                            name ='{discipline}'
                            and discipline_id = (SELECT id from discipline where name = '{discipline}') ";
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                connection.Open();
                SQLiteDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        disciplineKeywordModel = new DisciplineKeywordModel(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                        break;
                    }
                }
                reader.Close();
                return disciplineKeywordModel;
            }
        }
    }
}
