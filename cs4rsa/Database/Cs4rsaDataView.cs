using cs4rsa.BasicData;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;

namespace cs4rsa.Database
{
    class Cs4rsaDataView
    {
        private static Cs4rsaDatabase _cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
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

        public static bool IsStudentExists(string specialString)
        {
            string sql = $"SELECT count(*) from student WHERE specialString = '{specialString}'";
            long result = _cs4RsaDatabase.GetScalar<long>(sql);
            return result == 1 ? true : false;
        }

        public static List<StudentInfo> GetStudentInfos()
        {
            List<StudentInfo> result = new List<StudentInfo>();
            string sql = $@"SELECT * from student";
            DataTable table = _cs4RsaDatabase.GetDataTable(sql);
            foreach (DataRow item in table.Rows)
            {
                StudentInfo info = new StudentInfo()
                {
                    Name = item["name"].ToString(),
                    StudentId = item["studentID"].ToString(),
                    SpecialString = item["specialString"].ToString(),
                    Birthday = item["birthDay"].ToString(),
                    CMND = item["cmnd"].ToString(),
                    Email = item["email"].ToString(),
                    PhoneNumber = item["phoneNumber"].ToString(),
                    Address = item["address"].ToString(),
                    Image = item["image"].ToString(),
                };
                result.Add(info);
            }
            return result;
        }

        public static List<string> GetPreSubjects(string courseId)
        {
            string sql = $@"select pre_subject_code from program_subject, detail_pre, pre_par_subject
                            where program_subject.course_id = '{courseId}' AND
		                            program_subject.subject_code = detail_pre.pro_subject_code AND
		                            detail_pre.pre_subject_code = pre_par_subject.subject_code";
            DataTable table = _cs4RsaDatabase.GetDataTable(sql);
            List<string> result = new List<string>();
            foreach (DataRow item in table.Rows)
            {
                result.Add(item["pre_subject_code"].ToString());
            }
            return result;
        }

        public static List<string> GetParSubjects(string courseId)
        {
            string sql = $@"select par_subject_code from program_subject, detail_par, pre_par_subject
                            where program_subject.course_id = '{courseId}' AND
		                            program_subject.subject_code = detail_par.pro_subject_code AND
		                            detail_par.par_subject_code = pre_par_subject.subject_code";
            DataTable table = _cs4RsaDatabase.GetDataTable(sql);
            List<string> result = new List<string>();
            foreach (DataRow item in table.Rows)
            {
                result.Add(item["par_subject_code"].ToString());
            }
            return result;
        }

        public static bool IsExistsPreParSubject(string subjectCode)
        {
            string sql = $@"select count(*) from pre_par_subject where subject_code = '{subjectCode}'";
            long result = _cs4RsaDatabase.GetScalar<long>(sql);
            if (result == 1)
                return true;
            return false;
        }

        public static bool IsExistsProgramSubject(string subjectCode)
        {
            string sql = $@"select count(*) from program_subject where subject_code = '{subjectCode}'";
            long result = _cs4RsaDatabase.GetScalar<long>(sql);
            return result == 1;
        }

        public static bool IsExistsPreDetail(string proSubjectCode, string preSubjectCode)
        {
            string sql = $@"select count(*) 
                            from detail_pre 
                            where pro_subject_code = '{proSubjectCode}' and pre_subject_code = '{preSubjectCode}'";
            long result = _cs4RsaDatabase.GetScalar<long>(sql);
            return result == 1;
        }

        public static bool IsExistsParDetail(string proSubjectCode, string parSubjectCode)
        {
            string sql = $@"select count(*) 
                            from detail_par 
                            where pro_subject_code = '{proSubjectCode}' and par_subject_code = '{parSubjectCode}'";
            long result = _cs4RsaDatabase.GetScalar<long>(sql);
            return result == 1;
        }

        public static bool IsExistsProSubjectDetail(string studentId, string proSubjectCode)
        {
            string sql = $@"select count(*) 
                            from detail_program_subject 
                            where student_id='{studentId}' and program_subject_code = '{proSubjectCode}'";
            long result = _cs4RsaDatabase.GetScalar<long>(sql);
            return result == 1;
        }

        public static bool IsExistsCurriculum(string curId)
        {
            string sql = $@"select count(*) from curriculum where curid = '{curId}'";
            long result = _cs4RsaDatabase.GetScalar<long>(sql);
            return result == 1;
        }
    }
}
