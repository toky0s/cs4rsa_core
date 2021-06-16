using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using cs4rsa.Models;
using System.Data;

namespace cs4rsa.Database
{
    class Cs4rsaDataView
    {
        /// <summary>
        /// Lấy ra danh sách mã ngành.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDisciplines()
        {
            using (SQLiteConnection connection = new SQLiteConnection(Cs4rsaDatabase.ConnectString))
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
            using (SQLiteConnection connection = new SQLiteConnection(Cs4rsaDatabase.ConnectString))
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
            using (SQLiteConnection connection = new SQLiteConnection(Cs4rsaDatabase.ConnectString))
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

        public static DisciplineKeywordModel GetSingleDisciplineKeywordModel(string courseId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(Cs4rsaDatabase.ConnectString))
            {
                DisciplineKeywordModel disciplineKeywordModel = null;
                string sql = $@"SELECT course_id, subject_name, name, keyword1 from keyword k, discipline d
                            where course_id = '{courseId}' and d.id = k.discipline_id";
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

        public static string GetColorWithCourseId(string courseId)
        {
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            string sqlString = $@"select color from keyword where course_id = {courseId}";
            return cs4RsaDatabase.GetScalar<string>(sqlString);
        }

        public static List<ScheduleSession> GetScheduleSessions()
        {
            List<ScheduleSession> scheduleSessions = new List<ScheduleSession>();
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            string sql = $@"select id, name, save_date, semester, year from sessionx";
            DataTable table = cs4RsaDatabase.GetDataTable(sql);
            foreach (DataRow item in table.Rows)
            {
                string id = item["id"].ToString();
                string name = item["name"].ToString();
                DateTime saveDate = DateTime.Parse(item["save_date"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                string semester = item["semester"].ToString();
                string year = item["year"].ToString();
                ScheduleSession session = new ScheduleSession()
                {
                    ID = id,
                    Name = name,
                    SaveDate = saveDate,
                    Semester = semester,
                    Year = year
                };
                scheduleSessions.Add(session);
            }
            return scheduleSessions;
        }

        public static List<ScheduleSessionDetail> GetSessionDetail(string id)
        {
            List<ScheduleSessionDetail> result = new List<ScheduleSessionDetail>();
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            string sql = $@"select s.subject_code, k.subject_name, s.class_group
                            from discipline as d, keyword as k, session_detail as s 
                            WHERE k.discipline_id = d.id and s.subject_code = d.name || ' ' ||k.keyword1 and session_id ={id}";
            DataTable table = cs4RsaDatabase.GetDataTable(sql);
            foreach (DataRow item in table.Rows)
            {
                ScheduleSessionDetail detail = new ScheduleSessionDetail()
                {
                    SubjectCode = item["subject_code"].ToString(),
                    SubjectName = item["subject_name"].ToString(),
                    ClassGroup = item["class_group"].ToString(),
                };
                result.Add(detail);
            }
            return result;
        }

        public static string GetCourseId(string subjectCode)
        {
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            string sql = $@"select course_id from discipline d, keyword k
                            where d.id = k.discipline_id and d.name || ' ' || k.keyword1 = '{subjectCode}'";
            return cs4RsaDatabase.GetScalar<string>(sql);
        }
    }
}
