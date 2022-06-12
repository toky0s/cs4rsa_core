using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;

using Microsoft.EntityFrameworkCore.Storage;

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
            ProgramSubjects = new ProgramSubjectRepository(_context);
            PreParSubjects = new PreParSubjectRepository(_context);
            PreProDetails = new PreProDetailRepository(_context);
            ParProDetails = new ParProDetailRepository(_context);
            SessionSchoolClassRepository = new SessionSchoolClassRepository(_context);
        }

        public ICurriculumRepository Curriculums { get; private set; }

        public IDisciplineRepository Disciplines { get; private set; }

        public IKeywordRepository Keywords { get; private set; }

        public ISessionRepository Sessions { get; private set; }

        public IStudentRepository Students { get; private set; }

        public ITeacherRepository Teachers { get; private set; }

        public IProgramSubjectRepository ProgramSubjects { get; private set; }

        public IPreParSubjectRepository PreParSubjects { get; private set; }

        public IPreProDetailsRepository PreProDetails { get; private set; }

        public IParProDetailsRepository ParProDetails { get; private set; }

        public ISessionSchoolClassRepository SessionSchoolClassRepository { get; private set; }

        public IDbContextTransaction BeginTrans()
        {
            return _context.Database.BeginTransaction();
        }

        public Task<IDbContextTransaction> BeginTransAsync()
        {
            return _context.Database.BeginTransactionAsync();
        }

        public void Commit()
        {
            _context.Database.CommitTransaction();
        }

        public Task CommitAsync()
        {
            return _context.Database.CommitTransactionAsync();
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Rollback()
        {
            _context.Database.RollbackTransaction();
        }

        public Task RollbackAsync()
        {
            return _context.Database.RollbackTransactionAsync();
        }
    }
}
