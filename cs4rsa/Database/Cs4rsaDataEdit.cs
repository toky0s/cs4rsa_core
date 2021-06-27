﻿using System;
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
        private static Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
        public static void AddDiscipline(string discipline)
        {
            string sqlCommand = $@"INSERT INTO discipline (name)
                                  VALUES('{discipline}');";
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            cs4RsaDatabase.DoSomething(sqlCommand);
        }

        public static void AddKeyword(string keyword1, string courseId, int discipline_id, string subjectName, string color=null)
        {
            string sqlCommand = $@"insert into keyword (keyword1, course_id, discipline_id, subject_name, color)
                                    values ('{keyword1}',{courseId},{discipline_id},'{subjectName}', '{color}')";
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            cs4RsaDatabase.DoSomething(sqlCommand);
        }

        public static void AddSession(string name, string saveDate, List<ClassGroupModel> classGroupModels)
        {
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
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

        public static int DeleteSession(string id)
        {
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            string sqlSessionDetail = $@"delete from session_detail
                            where session_id = {id}";
            string sqlSessionx = $@"delete from sessionx
                            where id = {id}";
            cs4RsaDatabase.DoSomething(sqlSessionDetail);
            int result = cs4RsaDatabase.DoSomething(sqlSessionx);
            return result;
        }

        private static int DeleteTable(string tableName)
        {
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            string sql = $"delete from {tableName}";
            return cs4RsaDatabase.DoSomething(sql);
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
                           '{studentInfo.Email}', '{studentInfo.PhoneNumber}', '{studentInfo.Address}', '{studentInfo.Image}')";
            return cs4RsaDatabase.DoSomething(sql);
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
                            image = '{studentInfo.Image}'
                            WHERE specialString = '{studentInfo.SpecialString}'";
            return cs4RsaDatabase.DoSomething(sql);
        }
    }
}
