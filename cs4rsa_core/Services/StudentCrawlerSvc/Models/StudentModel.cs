using CommunityToolkit.Mvvm.ComponentModel;

namespace Cs4rsa.Services.StudentCrawlerSvc.Models
{
    /// <summary>
    /// Student Model
    /// chứa thông tin hồ sơ sinh viên.
    /// </summary>
    public partial class StudentModel : ObservableObject
    {
        /// <summary>
        /// Mã sinh viên
        /// </summary>
        [ObservableProperty]
        private string _studentId;

        /// <summary>
        /// Đường dẫn tới hình ảnh hồ sơ
        /// </summary>
        [ObservableProperty]
        private string _imgPath;

        /// <summary>
        /// Trạng thái tải: 
        /// True -  Thành công, 
        /// False - Thất bại
        /// </summary>
        [ObservableProperty]
        private bool _isSuccess;

        /// <summary>
        /// Đang tải
        /// </summary>
        /// 
        [ObservableProperty]
        private bool _isDownloading;

        /// <summary>
        /// Đã tải xong
        /// </summary>
        /// 
        [ObservableProperty]
        private bool _downloaded;
    }
}
