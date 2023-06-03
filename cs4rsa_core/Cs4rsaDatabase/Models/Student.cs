using Cs4rsa.Constants;

using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Cs4rsa.Cs4rsaDatabase.Models
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
        public string AvatarImgPath
        {
            get => _imagePath;
            set
            {
                string actualPath = File.Exists(value)
                    ? value
                    : CredizText.PathDefaultStudentImage();
                _imagePath = value;
                using FileStream fileStream = new(actualPath, FileMode.Open);
                BitmapFrame = BitmapFrame.Create(
                      fileStream
                    , BitmapCreateOptions.None
                    , BitmapCacheOption.OnLoad
                );
            }
        }
        public int? CurriculumId { get; set; }

        #region Binding
        /// <summary>
        /// Để tránh các lỗi không mong muốn, thay vì Binding hình ảnh trực tiếp
        /// từ file, hình ảnh sẽ đưa về dạng BitmapImage.
        /// </summary>
        /// 
        private string _imagePath;
        public BitmapFrame BitmapFrame { get; set; }
        #endregion
    }
}
