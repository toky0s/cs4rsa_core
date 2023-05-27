using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class StudentRepository : IStudentRepository
    {
        public long CountByContainsId(string studentId)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT COUNT(*)")
                .AppendLine("FROM Students")
                .AppendLine("WHERE StudentId LIKE '%' || @studentId || '%'");

            return RawSql.ExecScalar(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@studentId", studentId }
                }
                , 0L);
        }

        public bool ExistsBySpecialString(string specialString)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT COUNT(*)")
                .AppendLine("FROM Students")
                .AppendLine("WHERE SpecialString = @specialString");

            return RawSql.ExecScalar(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@specialString", specialString }
                }
                , 0L) > 0L;
        }

        public List<Student> GetAllBySpecialStringNotNull()
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Student>()
                .AppendLine("FROM Students")
                .AppendLine("WHERE SpecialString IS NOT NULL")
                .AppendLine("AND LENGTH(SpecialString) > 0");
            return RawSql.ExecReader(
                sb.ToString()
                , r => new Student()
                {
                      StudentId = r.GetString(0)
                    , SpecialString = r.GetString(1)
                    , Name = r.GetString(2)
                    , BirthDay = DateTime.Parse(r.GetString(3))
                    , Cmnd = r.GetString(4)
                    , Email = r.GetString(5)
                    , PhoneNumber = r.GetString(6)
                    , Address = r.GetString(7)
                    , AvatarImgPath = r.GetString(8)
                    , CurriculumId = r.GetInt32(9)
                }
            );
        }

        public Student GetByStudentId(string id)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Student>()
                .AppendLine("FROM Students")
                .AppendLine("WHERE StudentId = @id");
            return RawSql.ExecReaderGetFirstOrDefault(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@id", id }
                }
                , r => new Student()
                {
                      StudentId = r.GetString(0)
                    , SpecialString = r.GetString(1)
                    , Name = r.GetString(2)
                    , BirthDay = DateTime.Parse(r.GetString(3))
                    , Cmnd = r.GetString(4)
                    , Email = r.GetString(5)
                    , PhoneNumber = r.GetString(6)
                    , Address = r.GetString(7)
                    , AvatarImgPath = r.GetString(8)
                    , CurriculumId = r.GetInt32(9)
                }
            );
        }

        public List<Student> GetStudentsByContainsId(
            string studentId
            , int limit
            , int page
        )
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Student>()
                .AppendLine("FROM Students")
                .AppendLine("WHERE StudentId LIKE '%' || @studentId || '%'")
                .AppendLine("LIMIT @limit")
                .AppendLine("OFFSET @page * @limit");
            return RawSql.ExecReader(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@page", page},
                    { "@limit", limit},
                    { "@studentId", studentId}
                }
                , r => new Student()
                {
                      StudentId = r.GetString(0)
                    , SpecialString = r.GetString(1)
                    , Name = r.GetString(2)
                    , BirthDay = DateTime.Parse(r.GetString(3))
                    , Cmnd = r.GetString(4)
                    , Email = r.GetString(5)
                    , PhoneNumber = r.GetString(6)
                    , Address = r.GetString(7)
                    , AvatarImgPath = r.GetString(8)
                    , CurriculumId = r.GetInt32(9)
                }
            );
        }

        public List<Student> GetByPaging(int limit, int page)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Student>()
                .AppendLine("FROM Students")
                .AppendLine("LIMIT @limit")
                .AppendLine("OFFSET @page * @limit");
            return RawSql.ExecReader(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@page", page},
                    { "@limit", limit}
                }
                , r => new Student()
                {
                      StudentId = r.GetString(0)
                    , SpecialString = r.GetString(1)
                    , Name = r.GetString(2)
                    , BirthDay = DateTime.Parse(r.GetString(3))
                    , Cmnd = r.GetString(4)
                    , Email = r.GetString(5)
                    , PhoneNumber = r.GetString(6)
                    , Address = r.GetString(7)
                    , AvatarImgPath = r.GetString(8)
                    , CurriculumId = r.GetInt32(9)
                }
            );
        }

        public int Remove(Student student)
        {
            if (student.StudentId == null)
            {
                throw new NullReferenceException("Student Id was null");
            }
            StringBuilder sb = new StringBuilder()
                .AppendLine("DELETE FROM Students")
                .AppendLine("WHERE StudentId = @StudentId");
            return RawSql.ExecNonQuery(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@StudentId", student.StudentId }
                });
        }

        public int Update(Student student)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("UPDATE Students")
                .AppendLine("SpecialString  = @SpecialString")
                .AppendLine("Name           = @Name")
                .AppendLine("BirthDay       = @BirthDay")
                .AppendLine("Cmnd           = @Cmnd")
                .AppendLine("Email          = @Email")
                .AppendLine("PhoneNumber    = @PhoneNumber")
                .AppendLine("Address        = @Address")
                .AppendLine("AvatarImgPath  = @AvatarImgPath")
                .AppendLine("CurriculumId   = @CurriculumId")
                .AppendLine("WHERE")
                .AppendLine("StudentId = @StudentId");

            Dictionary<string, object> param = new()
            {
                { "@StudentId", student.StudentId},
                { "@SpecialString", student.SpecialString},
                { "@Name", student.Name},
                { "@BirthDay", student.BirthDay},
                { "@Cmnd", student.Cmnd},
                { "@Email", student.Email},
                { "@PhoneNumber", student.PhoneNumber},
                { "@Address", student.Address},
                { "@AvatarImgPath", student.AvatarImgPath},
                { "@CurriculumId", student.CurriculumId},
            };

            return RawSql.ExecNonQuery(sb.ToString(), param);
        }

        public void Add(Student student)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("INSERT INTO Students VALUES (")
                .AppendLine("  @StudentId")
                .AppendLine(", @SpecialString")
                .AppendLine(", @Name")
                .AppendLine(", @BirthDay")
                .AppendLine(", @Cmnd")
                .AppendLine(", @Email")
                .AppendLine(", @PhoneNumber")
                .AppendLine(", @Address")
                .AppendLine(", @AvatarImgPath")
                .AppendLine(", @CurriculumId")
                .AppendLine(")");

            Dictionary<string, object> param = new()
            {
                { "@StudentId", student.StudentId},
                { "@SpecialString", student.SpecialString},
                { "@Name", student.Name},
                { "@BirthDay", student.BirthDay},
                { "@Cmnd", student.Cmnd},
                { "@Email", student.Email},
                { "@PhoneNumber", student.PhoneNumber},
                { "@Address", student.Address},
                { "@AvatarImgPath", student.AvatarImgPath},
                { "@CurriculumId", student.CurriculumId},
            };

            RawSql.ExecNonQuery(sb.ToString(), param);
        }

        public long CountPage(int limit)
        {
            return RawSql.ExecScalar($"SELECT CAST(ROUND(COUNT(*) / {limit} + 0.5, 0) AS INT) FROM Students", 0L);
        }

        public Student GetBySpecialString(string specialString)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Student>()
                .AppendLine("FROM Students")
                .AppendLine("WHERE SpecialString = @SpecialString");
            return RawSql.ExecReaderGetFirstOrDefault(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@SpecialString", specialString}
                }
                , r => new Student()
                {
                      StudentId = r.GetString(0)
                    , SpecialString = r.GetString(1)
                    , Name = r.GetString(2)
                    , BirthDay = DateTime.Parse(r.GetString(3))
                    , Cmnd = r.GetString(4)
                    , Email = r.GetString(5)
                    , PhoneNumber = r.GetString(6)
                    , Address = r.GetString(7)
                    , AvatarImgPath = r.GetString(8)
                    , CurriculumId = r.GetInt32(9)
                }
            );
        }
    }
}
