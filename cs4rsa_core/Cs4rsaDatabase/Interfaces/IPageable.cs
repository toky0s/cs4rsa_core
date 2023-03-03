using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IPageable<T>
    {
        /// <summary>
        /// Đếm số lượng trang.
        /// </summary>
        /// <param name="limit">Giới hạn số phần tử.</param>
        /// <returns>Số lượng trang.</returns>
        Task<int> CountPageAsync(int limit);
        /// <summary>
        /// Lấy ra danh sách <typeparamref name="T"/>.
        /// </summary>
        /// <param name="page">Số trang.</param>
        /// <param name="limit">Giới hạn mỗi trang.</param>
        /// <returns></returns>
        IAsyncEnumerable<T> Get(int page, int limit);
    }
}
