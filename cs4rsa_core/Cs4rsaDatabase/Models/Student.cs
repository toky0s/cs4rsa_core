using System;

namespace Cs4rsa.Cs4rsaDatabase.Models
{
    public class Student
    {
        public string StudentId { get; set; }
        public string SpecialString { get; set; }
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public string Cmnd { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string AvatarImage { get; set; }
        public int CurriculumId { get; set; }
        public Curriculum Curriculum { get; set; }
    }
}
