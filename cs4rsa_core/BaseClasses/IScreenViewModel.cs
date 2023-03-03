using System.Threading.Tasks;

namespace Cs4rsa.BaseClasses
{
    internal interface IScreenViewModel
    {
        /// <summary>
        /// Khởi tạo thông tin ViewModel của màn hình.
        /// </summary>
        void InitData();

        /// <summary>
        /// Khởi tạo thông tin ViewModel của màn hình.
        /// </summary>
        Task InitDataAsync();
    }
}
