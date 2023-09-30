using System;

namespace ExamListExcelReader.Models
{
    public class Student
    {
        public string Code { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string @Class { get; set; }
        public DateTime BirthDate { get; set; }
        public string BornPlace { get; set; }
        public string Sex { get; set; }
    }
}