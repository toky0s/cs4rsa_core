using System;
using System.Runtime.Remoting.Messaging;

namespace ExamListExcelReader.Models
{
    public class Student
    {
        public string Code { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string MainClass { get; set; }
        public string SubjectClass { get; set; }
        public DateTime BirthDate { get; set; }
        public string BornPlace { get; set; }
        public string Sex { get; set; }

        public override string ToString()
        {
            return $"Mã sinh viên: {Code}, Họ: {LastName}, Tên: {FirstName}, Tên đầy đủ: {FullName}, Lớp sinh hoạt: {MainClass}, Lớp môn học: {SubjectClass}, Giới tính: {Sex}";
        }
    }
}