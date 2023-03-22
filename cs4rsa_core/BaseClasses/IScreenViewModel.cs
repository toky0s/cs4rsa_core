namespace Cs4rsa.BaseClasses
{
    public interface IScreenViewModel
    {
        /// <summary>
        /// Khởi tạo thông tin ViewModel của màn hình.
        /// </summary>
        /// <remarks>
        /// Phương thức này sẽ được GỌI LẠI mỗi khi màn
        /// hình được gọi tới bởi <see cref="Views.MainWindow.Goto"/>.
        /// Triển khai của phương thức này nên chỉ khởi tạo các
        /// thông tin có khả năng thay đổi liên tục trong ứng dụng.
        /// Các thông tin khác như các hằng số. Chỉ nên được khởi tạo
        /// ngay lần đầu đối tượng được inject.
        /// <br/>
        /// Update Date:
        /// <br/>
        /// XinTA - 15/03/2023: Chỉnh sửa tài liệu.
        /// </remarks>
        void InitData();
    }
}
