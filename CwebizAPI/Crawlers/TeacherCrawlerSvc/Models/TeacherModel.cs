using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Crawlers.TeacherCrawlerSvc.Models
{
    public class TeacherModel
    {
        public string TeacherId { get; }
        public string Name { get; }
        public string Sex { get; }
        public string Place { get; }
        public string Degree { get; }
        public string WorkUnit { get; }
        public string Position { get; }
        public string Subject { get; }
        public string Form { get; }
        /// <summary>
        /// Đường dẫn tới hình ảnh giảng viên.
        /// </summary>
        private string Path;
        public string Url { get; }

        public TeacherModel(Teacher teacher)
        {
            TeacherId = teacher.TeacherId;
            Name = teacher.Name;
            Sex = teacher.Sex;
            Place = teacher.Place;
            Degree = teacher.Degree;
            WorkUnit = teacher.WorkUnit;
            Position = teacher.Position;
            Subject = teacher.Subject;
            Form = teacher.Form;
        }

        public TeacherModel(
            string teacherId,
            string name,
            string sex,
            string place,
            string degree,
            string workUnit,
            string position,
            string subject,
            string form,
            string path,
            string url)
        {
            TeacherId = teacherId;
            Name = name;
            Sex = sex;
            Place = place;
            Degree = degree;
            WorkUnit = workUnit;
            Position = position;
            Subject = subject;
            Form = form;
            Path = path;
            Url = url;
        }

        public TeacherModel(string teacherId, string name)
        {
            TeacherId = teacherId;
            Name = name;
        }
    }
}
