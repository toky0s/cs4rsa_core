using System;
using System.IO;

namespace Cs4rsa.Database.Models
{
    public class Student
    {
        /// <summary>
        /// Mã sinh viên
        /// </summary>
        public string StudentId { get; set; }
        public string SpecialString { get; set; }
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public string Cmnd { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string AvatarImgPath { get; set; }
        public int? CurriculumId { get; set; }
    }
}
