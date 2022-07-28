using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cs4rsaDatabaseService.Models
{
    public class StudentImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        /// <summary>
        /// Mã sinh viên
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Tên sinh viên
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Họ sinh viên
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Lớp sinh hoạt
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Ngành
        /// </summary>
        public string Major { get; set; }

        /// <summary>
        /// Đường dẫn tới hình ảnh
        /// </summary>
        public string Path { get; set; }
    }
}
