using cs4rsa_core.Cs4rsaDatabase.DataProviders;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace cs4rsa_core.Cs4rsaDatabase.Implements
{
    public class DisciplineRepository : GenericRepository<Discipline>, IDisciplineRepository
    {
        public DisciplineRepository(Cs4rsaDbContext context) : base(context)
        {

        }

        public async Task<List<Discipline>> GetAllIncludeKeywordAsync()
        {
            return await _context.Disciplines.Include(discipline => discipline.Keywords).ToListAsync();
        }
    }
}
