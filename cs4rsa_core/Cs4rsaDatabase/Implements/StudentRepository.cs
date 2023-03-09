using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(Cs4rsaDbContext context, RawSql rawSql) : base(context, rawSql)
        {
        }

        public async Task<int> CountByContainsId(string studentId)
        {
            return await _context.Students
                 .Where(s => s.StudentId.Contains(studentId))
                 .CountAsync();
        }

        public async Task<bool> ExistsBySpecialString(string specialString)
        {
            return await _context.Students.AnyAsync(st => st.SpecialString.Equals(specialString));
        }

        public IAsyncEnumerable<Student> GetAllBySpecialStringNotNull()
        {
            return _context.Students.Where(s => !string.IsNullOrEmpty(s.SpecialString)).AsAsyncEnumerable();
        }

        public async Task<Student> GetByStudentIdAsync(string id)
        {
            return await _context.Set<Student>().FindAsync(id);
        }

        public IAsyncEnumerable<Student> GetStudentsByContainsId(string studentId, int limit, int page)
        {
            page = page == 0 ? 1 : page;
            limit = limit == 0 ? int.MaxValue : limit;
            int skip = (page - 1) * limit;
            return _context.Students
                .Where(s => s.StudentId.Contains(studentId))
                .Skip(skip)
                .Take(limit)
                .AsAsyncEnumerable();
        }
    }
}
