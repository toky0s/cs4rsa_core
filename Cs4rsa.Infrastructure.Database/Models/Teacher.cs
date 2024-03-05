namespace Cs4rsa.Database.Models
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

        /// <summary>
        /// Các môn đã giảng dạy
        /// 
        /// Khúc này tính tạo thêm một bảng, nhưng suy đi xét lại về
        /// performance thì điều này vẫn thật sự chưa cần thiết.
        /// </summary>
        public string TeachedSubjects { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
    }
}
