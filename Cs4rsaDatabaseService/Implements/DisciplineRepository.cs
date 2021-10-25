using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Implements
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
