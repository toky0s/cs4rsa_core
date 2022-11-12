namespace cs4rsa_core.Commons.Enums
{
    /// <summary>
    /// Chỉ mục màn hình
    /// 
    /// Các bước thêm một màn hình vào menu
    /// - Thêm mới một ScreenIndex
    /// - Tạo View tương ứng và đặt vào MainWindow
    /// - Tạo ViewModel tương ứng cho View được kế thừa từ ViewModelBase
    /// </summary>
    public enum ScreenIndex
    {
        /// <summary>
        /// Trang chủ
        /// </summary>
        HOME,

        /// <summary>
        /// Màn hình tài khoản
        /// </summary>
        ACCOUNT,

        /// <summary>
        /// Màn hình xếp lịch thủ công
        /// </summary>
        HAND,

        /// <summary>
        /// Màn hình xếp lịch tự động
        /// </summary>
        AUTO,

        /// <summary>
        /// Màn hình danh sách giảng viên
        /// </summary>
        LECTURE,

        /// <summary>
        /// Màn hình thông tin ứng dụng
        /// </summary>
        INFO,
    }
}
