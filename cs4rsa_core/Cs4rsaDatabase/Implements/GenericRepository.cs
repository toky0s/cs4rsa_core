using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly Cs4rsaDbContext _context;
        protected readonly RawSql _rawSql;
        protected readonly string _tableName;
        public GenericRepository(Cs4rsaDbContext context)
        {
            _context = context;
            _tableName = typeof(T).Name + "s";
            _rawSql = context.RSql;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public long Count()
        {
            string sql = $@"SELECT COUNT(*) FROM {_tableName};";
            return _rawSql.ExecScalar(sql, null, 0L);
        }

        public long CountPage(int limit)
        {
            string sql = $@"SELECT CAST(ROUND(COUNT(*) / {limit} + 0.5, 0) AS INT) FROM {_tableName};";
            return _rawSql.ExecScalar(sql, null, 0L);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }

        public IAsyncEnumerable<T> Get(int page, int limit)
        {
            page = page == 0 ? 1 : page;
            limit = limit == 0 ? int.MaxValue : limit;
            int skip = (page - 1) * limit;
            return _context
                .Set<T>()
                .Skip(skip)
                .Take(limit)
                .AsAsyncEnumerable();
        }

        public IAsyncEnumerable<T> GetAll()
        {
            return _context.Set<T>().AsAsyncEnumerable();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public Task<int> RemoveAll()
        {
            string tableName = typeof(T).Name;
            return _context.Database.ExecuteSqlRawAsync($@"DELETE FROM {tableName}s");
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
