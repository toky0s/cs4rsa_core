using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace Cs4rsa.Services.TeacherCrawlerSvc.Models
{
    public class TeacherModel
    {
        public int TeacherId { get; }
        public string Name { get; }
        public string Sex { get; }
        public string Place { get; }
        public string Degree { get; }
        public string WorkUnit { get; }
        public string Position { get; }
        public string Subject { get; }
        public string Form { get; }
        public IEnumerable<string> TeachedSubjects { get; set; }
        /// <summary>
        /// Đường dẫn tới hình ảnh giảng viên.
        /// </summary>
        private string _imagePath;
        public string Path
        {
            get => _imagePath;
            set
            {
                string actualPath = string.IsNullOrWhiteSpace(value) 
                    ? CredizText.PathDefaultLectureImage() 
                    : value;
                _imagePath = value;
                using FileStream fileStream = new(actualPath, FileMode.Open);
                BitmapFrame = BitmapFrame.Create(
                      fileStream
                    , BitmapCreateOptions.None
                    , BitmapCacheOption.OnLoad
                );
            }
        }
        public string Url { get; }

        #region Binding
        /// <summary>
        /// Để tránh các lỗi không mong muốn, thay vì Binding hình ảnh trực tiếp
        /// từ file, hình ảnh sẽ đưa về dạng BitmapImage.
        /// </summary>
        public BitmapFrame BitmapFrame { get; set; }
        #endregion

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
            if (string.IsNullOrEmpty(teacher.TeachedSubjects))
            {
                TeachedSubjects = new List<string>();
            }
            else
            {
                TeachedSubjects = teacher.TeachedSubjects.Split(VmConstants.SeparatorTeacherSubject);
            }
            Path = teacher.Path;
            Url = teacher.Url;
        }

        public TeacherModel(
            int teacherId,
            string name,
            string sex,
            string place,
            string degree,
            string workUnit,
            string position,
            string subject,
            string form,
            IEnumerable<string> teachedSubjects,
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
            TeachedSubjects = teachedSubjects;
            Path = path;
            Url = url;
        }

        public TeacherModel(int teacherId, string name)
        {
            TeacherId = teacherId;
            Name = name;
        }
    }
}
