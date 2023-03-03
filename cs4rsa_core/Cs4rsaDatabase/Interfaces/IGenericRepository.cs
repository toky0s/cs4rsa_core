using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IGenericRepository<T> : IPageable<T> where T : class
    {
        /// <summary>
        /// Lấy ra tổng số lượng <typeparamref name="T"/> bản ghi
        /// </summary>
        Task<long> Count();
        Task<T> GetByIdAsync(int id);
        IAsyncEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        void Add(T entity);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Update(T entity);
        /// <summary>
        /// Xoá mọi dữ liệu của bảng <typeparamref name="T"/>
        /// </summary>
        /// <returns>Số lượng bảng ghi đã xoá</returns>
        Task<int> RemoveAll();
    }
}
