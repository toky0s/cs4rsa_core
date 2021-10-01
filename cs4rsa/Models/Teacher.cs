using cs4rsa.BasicData;
using System.Collections.Generic;

namespace cs4rsa.Models
{
    public class Teacher
    {
        public string TeacherId { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Place { get; set; }
        public string Degree { get; set; }
        public string WorkUnit { get; set; }
        public string Position { get; set; }
        public string Subject { get; set; }
        public string Form { get; set; }
        public List<string> Subjects { get; set; }
    }
}
