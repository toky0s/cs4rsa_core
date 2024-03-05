using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cs4rsa.Database.Models
{
    /// <summary>
    /// Lưu thông tin Teacher mỗi lần cào Subject
    /// - Dùng thực hiện dự đoán mã môn học mà một
    /// giảng viên đã/đang giảng dạy trong trường hợp
    /// một tên môn có nhiều hơn một mã môn.
    /// - Tại sao bảng này không quan hệ với Keywords Table ?
    /// Vì mục đích của bảng này là để dự đoán mã môn giảng dạy.
    /// Trong trường hợp có quan hệ với Keyword thì khi cập nhật
    /// các thông tin này sẽ bị xoá hết.
    /// </summary>
    public class KeywordTeacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int TeacherId { get; set; }
    }
}
