using System.Threading.Tasks;

namespace Cs4rsa.BaseClasses
{
    /// <summary>
    /// Các thành phần con là các <see cref="System.Windows.Controls.UserControl"/> 
    /// thuộc một màn hình sỡ hữu các ViewModel riêng biệt
    /// sẽ phải implement Interface này. Các <see cref="ScreenAbstract"/> sẽ
    /// gọi tới <see cref="LoadData"/> khi chúng được điều hướng tới bởi phương
    /// thức <see cref="Cs4rsa.Views.MainWindow.Goto(int)"/>.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Triển khai khởi tạo (hoặc tải lại) thông tin khi các Component của một Screen
        /// được điều hướng tới bằng <see cref="Cs4rsa.Views.MainWindow.Goto(int)"/>.
        /// </summary>
        /// <returns>Task</returns>
        Task LoadData();
    }
}
