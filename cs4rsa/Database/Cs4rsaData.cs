using cs4rsa.Crawler;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace cs4rsa.Database
{
    /// <summary>
    /// Class này chịu trách nhiệm khởi tạo database
    /// </summary>
    public class Cs4rsaData
    {
        private static readonly string databaseName = "cs4rsadb.db";

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
                color text,
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
            @"create table if not exists sessionx (
                id integer primary key AUTOINCREMENT,
                name text,
                save_date text,
                semester text,
                year text
            )";

        private readonly string SQL_SESSION_DETAIL =
            @"create table if not exists session_detail (
                id integer primary key AUTOINCREMENT,
                session_id integer,
                subject_code text,
                class_group text,
                foreign key (session_id) references sessionx(id)
            )";

        public Cs4rsaData()
        {
            if (!File.Exists(databaseName))
            {
                SQLiteConnection.CreateFile(databaseName);
                CreateTable(SQL_DISCIPLINE_TABLE);
                CreateTable(SQL_KEYWORD_TABLE);
                CreateTable(SQL_TEACHER_TABLE);
                CreateTable(SQL_TEACHER_DETAIL_TABLE);
                CreateTable(SQL_SESSION);
                CreateTable(SQL_SESSION_DETAIL);
                DumpData();
            }
        }

        /// <summary>
        /// Thao tác update sẽ được người dùng chạy khi cơ sỡ dữ liệu đã lỗi thời.
        /// </summary>
        private void UpdateDatabase()
        {
            // TODO: Drop tất cả các record của discipline và keyword.
            // TODO: Thực hiện đổ lại data.
            Console.WriteLine("Update data");
        }

        private void DumpData()
        {
            DisciplineData disciplineData = new DisciplineData();
            disciplineData.GetDisciplineAndKeywordDatabase();
        }

        private void CreateTable(string sqlString)
        {
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            cs4RsaDatabase.DoSomething(sqlString);
        }
    }
}
