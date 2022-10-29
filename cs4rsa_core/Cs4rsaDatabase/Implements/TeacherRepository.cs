using cs4rsa_core.Cs4rsaDatabase.DataProviders;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;

namespace cs4rsa_core.Cs4rsaDatabase.Implements
{
    public class TeacherRepository : GenericRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(Cs4rsaDbContext context) : base(context)
        {
        }

        public IEnumerable<Teacher> GetTeacherByNameOrId(string nameOrId)
        {
            return _context.Teachers
                    .AsEnumerable()
                    .Where(teacher => teacher.Name.ToLower().Contains(nameOrId)
                        || teacher.TeacherId.ToString().Contains(nameOrId));
        }

        public IAsyncEnumerable<Teacher> GetTeachersAsync(int page, int limit)
        {
            page = page == 0 ? 1 : page;
            limit = limit == 0 ? int.MaxValue : limit;
            int skip = (page - 1) * limit;
            return _context.Teachers
                .Skip(skip)
                .Take(limit)
                .AsAsyncEnumerable();
        }
    }
}
