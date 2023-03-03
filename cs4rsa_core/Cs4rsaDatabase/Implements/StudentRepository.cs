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
        public StudentRepository(Cs4rsaDbContext context) : base(context)
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

        public async Task<Student> GetBySpecialStringAsync(string specialString)
        {
            return await _context.Set<Student>()
                .Where(st => st.SpecialString.Equals(specialString))
                .FirstOrDefaultAsync();
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
