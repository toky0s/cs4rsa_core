using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        IAsyncEnumerable<T> Get(int page, int limit);
        IAsyncEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        void Add(T entity);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Update(T entity);
        /// <summary>
        /// Lấy ra tổng số lượng <typeparamref name="T"/> record
        /// </summary>
        Task<long> Count();
        Task<int> CountPageAsync(int limit);

        /// <summary>
        /// Xoá mọi dữ liệu của bảng <see cref="T"/>
        /// </summary>
        /// <returns>Số lượng bảng ghi đã xoá</returns>
        Task<int> RemoveAll();
    }
}
