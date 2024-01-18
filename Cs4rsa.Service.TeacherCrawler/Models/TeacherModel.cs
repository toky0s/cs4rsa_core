namespace Cs4rsa.Service.TeacherCrawler.Models
{
    public class TeacherModel
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
        /// <summary>
        /// Các môn học đã và đang giảng dạy
        /// </summary>
        public string[] TaughtSubjects { get; set; }
        public string Url { get; set; }

        private static int _visitingLecturerIndex = 0;

        public TeacherModel()
        {
            
        }

        public TeacherModel(string teacherId, string name)
        {
            TeacherId = teacherId;
            Name = name;
        }
        
        /// <summary>
        /// Tạo một giảng viên đại diện cho Tất cả
        /// </summary>
        /// <returns></returns>
        public static TeacherModel CreateAsAll()
        {
            return new TeacherModel(string.Empty, "TẤT CẢ");
        }

        /// <summary>
        /// Tạo một giảng viên đại diện cho giảng viên thỉnh giảng
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TeacherModel CreateAsVisitingLecturer(string name)
        {
            return new TeacherModel((_visitingLecturerIndex++).ToString(), name);
        }
    }
}
