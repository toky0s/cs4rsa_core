using System.Collections.Generic;

namespace Cs4rsa.Service.TeacherCrawler.Models
{
    public class TeacherModel
    {
        public int TeacherId { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Place { get; set; }
        public string Degree { get; set; }
        public string WorkUnit { get; set; }
        public string Position { get; set; }
        public string Subject { get; set; }
        public string Form { get; set; }
        /// <summary>
        /// Các môn học đã và đang giảng dạy
        /// </summary>
        public string[] TaughtSubjects { get; set; }
        public string Url { get; set; }

        public TeacherModel()
        {
            
        }
        public TeacherModel(int teacherId, string name)
        {
            TeacherId = teacherId;
            Name = name;
        }
    }
}
