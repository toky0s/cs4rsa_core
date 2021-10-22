using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Cs4rsaDbContext _context;
        public UnitOfWork(Cs4rsaDbContext context)
        {
            _context = context;
            Curriculums = new CurriculumRepository(_context);
            Disciplines = new DisciplineRepository(_context);
            Keywords = new KeywordRepository(_context);
            Sessions = new SessionRepository(_context);
            Students = new StudentRepository(_context);
            Teachers = new TeacherRepository(_context);
        }

        public ICurriculumRepository Curriculums { get; private set; }

        public IDisciplineRepository Disciplines { get; private set; }

        public IKeywordRepository Keywords { get; private set; }

        public ISessionRepository Sessions { get; private set; }

        public IStudentRepository Students { get; private set; }

        public ITeacherRepository Teachers { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
