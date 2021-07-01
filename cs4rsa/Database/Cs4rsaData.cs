using cs4rsa.Crawler;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace cs4rsa.Database
{
    /// <summary>
    /// Class này chịu trách nhiệm khởi tạo database và các bảng.
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

        private readonly string SQL_USER_SETTINGS =
            @"create table if not exists user_settings (
                key text primary key,
                value text
            )";

        private readonly string SQL_curriculum =
            @"CREATE table curriculum (
                curid text PRIMARY key,
                name text
                )";

        private readonly string SQL_STUDENT =
            @"create table if not exists student (
                studentId text,
                specialString text,
                name text,
                birthDay text,
                cmnd text,
                email text,
                phoneNumber text,
                address text,
                image text,
                curid text,
                foreign key (curid) references curriculum(curid)
            )";

        private readonly string SQL_pre_par_subject =
            @"create table pre_par_subject (
                subject_code text primary key
                )";

        private readonly string SQL_program_subject =
            @"create table program_subject (
                subject_code text primary key,
                course_id text,
                name text,
                credit int
                )";

        private readonly string SQL_detail_pre =
            @"create table detail_pre(
                pro_subject_code TEXT,
                pre_subject_code text,
                PRIMARY KEY(pro_subject_code, pre_subject_code),
                FOREIGN key (pro_subject_code) REFERENCES program_subject(subject_code),
                FOREIGN key (pre_subject_code) REFERENCES pre_par_subject(subject_code)
                )";

        private readonly string SQL_detail_par =
            @"create table detail_par(
                pro_subject_code TEXT,
                par_subject_code text,
                PRIMARY KEY(pro_subject_code,par_subject_code),
                FOREIGN key (pro_subject_code) REFERENCES program_subject(subject_code),
                FOREIGN key (par_subject_code) REFERENCES pre_par_subject(subject_code)
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
                CreateTable(SQL_USER_SETTINGS);
                CreateTable(SQL_curriculum);
                CreateTable(SQL_STUDENT);
                CreateTable(SQL_pre_par_subject);
                CreateTable(SQL_detail_pre);
                CreateTable(SQL_detail_par);
                CreateTable(SQL_program_subject);
                DumpData();
            }
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
