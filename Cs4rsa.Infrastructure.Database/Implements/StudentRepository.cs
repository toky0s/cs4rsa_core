using System;
using System.Collections.Generic;
using System.Text;
using Cs4rsa.Database.DataProviders;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Implements
{
    public class StudentRepository : IStudentRepository
    {
        private readonly RawSql _rawSql;

        public StudentRepository(RawSql rawSql)
        {
            _rawSql = rawSql;
        }
        
        public long CountByContainsId(string studentId)
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT COUNT(*)")
                .AppendLine("FROM Students")
                .AppendLine("WHERE StudentId LIKE '%' || @studentId || '%'");

            return _rawSql.ExecScalar(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@studentId", studentId }
                }
                , 0L
            );
        }

        public bool ExistsBySpecialString(string specialString)
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT COUNT(*)")
                .AppendLine("FROM Students")
                .AppendLine("WHERE SpecialString = @specialString");

            return _rawSql.ExecScalar(
                sb.ToString()
                , new Dictionary<string, object>
                {
                    { "@specialString", specialString }
                }
                , 0L
            ) > 0L;
        }

        public List<Student> GetAllBySpecialStringNotNull()
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Student>(_rawSql.CnnStr)
                .AppendLine("FROM Students")
                .AppendLine("WHERE SpecialString IS NOT NULL")
                .AppendLine("AND LENGTH(SpecialString) > 0");
            return _rawSql.ExecReader(
                sb.ToString()
                , r => new Student
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
            var sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Student>(_rawSql.CnnStr)
                .AppendLine("FROM Students")
                .AppendLine("WHERE StudentId = @id");
            return _rawSql.ExecReaderGetFirstOrDefault(
                sb.ToString()
                , new Dictionary<string, object>
                {
                    { "@id", id }
                }
                , r => new Student
                {
                      StudentId     = r.GetString(0)
                    , SpecialString = r.IsDBNull(1) ? null : r.GetString(1)
                    , Name          = r.IsDBNull(2) ? null : r.GetString(2)
                    , BirthDay      = DateTime.Parse(r.GetString(3))
                    , Cmnd          = r.IsDBNull(4) ? null : r.GetString(4)
                    , Email         = r.IsDBNull(5) ? null : r.GetString(5)
                    , PhoneNumber   = r.IsDBNull(6) ? null : r.GetString(6)
                    , Address       = r.IsDBNull(7) ? null : r.GetString(7)
                    , AvatarImgPath = r.GetString(8)
                    , CurriculumId  = r.IsDBNull(9) ? 0 : r.GetInt32(9)
                }
            );
        }

        public List<Student> GetStudentsByContainsId(
            string studentId
            , int limit
            , int page
        )
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Student>(_rawSql.CnnStr)
                .AppendLine("FROM Students")
                .AppendLine("WHERE StudentId LIKE '%' || @studentId || '%'")
                .AppendLine("LIMIT @limit")
                .AppendLine("OFFSET @page * @limit");
            return _rawSql.ExecReader(
                sb.ToString()
                , new Dictionary<string, object>
                {
                    { "@page", page},
                    { "@limit", limit},
                    { "@studentId", studentId}
                }
                , r => new Student
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
            var sb = new StringBuilder()
                .AppendLine("DELETE FROM Students")
                .AppendLine("WHERE StudentId = @StudentId");
            return _rawSql.ExecNonQuery(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@StudentId", student.StudentId }
                }
            );
        }

        public int Update(Student student)
        {
            var sb = new StringBuilder()
                .AppendLine("UPDATE Students SET")
                .AppendLine("  SpecialString  = @SpecialString")
                .AppendLine(", Name           = @Name")
                .AppendLine(", BirthDay       = @BirthDay")
                .AppendLine(", Cmnd           = @Cmnd")
                .AppendLine(", Email          = @Email")
                .AppendLine(", PhoneNumber    = @PhoneNumber")
                .AppendLine(", Address        = @Address")
                .AppendLine(", AvatarImgPath  = @AvatarImgPath")
                .AppendLine(", CurriculumId   = @CurriculumId")
                .AppendLine("WHERE")
                .AppendLine("StudentId = @StudentId");

            var param = new Dictionary<string, object>
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

            return _rawSql.ExecNonQuery(sb.ToString(), param);
        }

        public void Add(Student student)
        {
            var sb = new StringBuilder()
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

            var param = new Dictionary<string, object>
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

            _rawSql.ExecNonQuery(sb.ToString(), param);
        }

        public Student GetBySpecialString(string specialString)
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Student>(_rawSql.CnnStr)
                .AppendLine("FROM Students")
                .AppendLine("WHERE SpecialString = @SpecialString");
            return _rawSql.ExecReaderGetFirstOrDefault(
                sb.ToString()
                , new Dictionary<string, object>
                {
                    { "@SpecialString", specialString}
                }
                , r => new Student
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

        public bool ExistsByStudentCode(string studentCode)
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT COUNT(*)")
                .AppendLine("FROM Students")
                .AppendLine("WHERE StudentId = @studentCode");

            return _rawSql.ExecScalar(
                sb.ToString()
                , new Dictionary<string, object>()
                {
                    { "@studentCode", studentCode }
                }
                , 0L
            ) > 0L;
        }

        public List<Student> GetAll()
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT")
                .AppendSelectColumns<Student>(_rawSql.CnnStr)
                .AppendLine("FROM Students");
            return _rawSql.ExecReader(
                sb.ToString()
                /* 
                 * Khởi tạo Student đã bao gồm xử lý cho việc
                 * nhập Student qua SessionId và qua chức năng
                 * tìm kiếm hình ảnh sinh viên.
                 * */
                , r => new Student()
                {
                      StudentId     = r.GetString(0)
                    , SpecialString = r.IsDBNull(1) ? null : r.GetString(1)
                    , Name          = r.IsDBNull(2) ? null : r.GetString(2)
                    /* 
                     * Với chức năng tìm kiếm hình ảnh sinh viên 
                     * BirthDay được set thành Min của DateTime.
                     */
                    , BirthDay      = DateTime.Parse(r.GetString(3))
                    , Cmnd          = r.IsDBNull(4) ? null : r.GetString(4)
                    , Email         = r.IsDBNull(5) ? null : r.GetString(5)
                    , PhoneNumber   = r.IsDBNull(6) ? null : r.GetString(6)
                    , Address       = r.IsDBNull(7) ? null : r.GetString(7)
                    , AvatarImgPath = r.GetString(8)
                    , CurriculumId  = r.IsDBNull(9) ? 0 : r.GetInt32(9)
                }
            );
        }
    }
}
