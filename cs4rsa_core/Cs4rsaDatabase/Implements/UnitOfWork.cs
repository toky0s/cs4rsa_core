using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;

using Microsoft.EntityFrameworkCore.Storage;

using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Cs4rsaDbContext _context;
        public UnitOfWork(Cs4rsaDbContext context, RawSql rawSql)
        {
            _context = context;
            Curriculums = new CurriculumRepository(_context, rawSql);
            Disciplines = new DisciplineRepository(_context, rawSql);
            Keywords = new KeywordRepository(_context, rawSql);
            UserSchedules = new SessionRepository(_context, rawSql);
            Students = new StudentRepository(_context, rawSql);
            Teachers = new TeacherRepository(_context, rawSql);
            ProgramSubjects = new ProgramSubjectRepository(_context, rawSql);
            PreParSubjects = new PreParSubjectRepository(_context, rawSql);
            PreProDetails = new PreProDetailRepository(_context, rawSql);
            ParProDetails = new ParProDetailRepository(_context, rawSql);
            KeywordTeachers = new KeywordTeacherRepository(_context, rawSql);
        }

        public ICurriculumRepository Curriculums { get; private set; }

        public IDisciplineRepository Disciplines { get; private set; }

        public IKeywordRepository Keywords { get; private set; }

        public IUserScheduleRepository UserSchedules { get; private set; }

        public IStudentRepository Students { get; private set; }

        public ITeacherRepository Teachers { get; private set; }

        public IProgramSubjectRepository ProgramSubjects { get; private set; }

        public IPreParSubjectRepository PreParSubjects { get; private set; }

        public IPreProDetailsRepository PreProDetails { get; private set; }

        public IParProDetailsRepository ParProDetails { get; private set; }

        public IKeywordTeacherRepository KeywordTeachers { get; private set; }

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
