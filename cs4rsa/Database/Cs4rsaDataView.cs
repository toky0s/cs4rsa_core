using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using cs4rsa.Models;

namespace cs4rsa.Database
{
    /// <summary>
    /// new Cs4rsa Data
    /// </summary>
    class Cs4rsaDataView
    {
        /// <summary>
        /// Lấy ra danh sách mã ngành.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDisciplines()
        {
            using (SQLiteConnection connection = new SQLiteConnection(Cs4rsaData.ConnectString))
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

        public static List<DisciplineKeywordModel> GetDisciplineKeywordModels(string discipline)
        {
            using (SQLiteConnection connection = new SQLiteConnection(Cs4rsaData.ConnectString))
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

        public static DisciplineKeywordModel GetSingleDisciplineKeywordModel(string discipline, string keyword1)
        {
            using (SQLiteConnection connection = new SQLiteConnection(Cs4rsaData.ConnectString))
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
