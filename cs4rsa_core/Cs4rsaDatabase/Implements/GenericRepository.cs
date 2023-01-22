using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly Cs4rsaDbContext _context;
        public GenericRepository(Cs4rsaDbContext context)
        {
            _context = context;
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

        public async Task<int> CountPageAsync(int limit, Expression<Func<T, bool>> expression)
        {
            return (await _context.Set<T>().Where(expression).CountAsync() + limit - 1) / limit;
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
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
