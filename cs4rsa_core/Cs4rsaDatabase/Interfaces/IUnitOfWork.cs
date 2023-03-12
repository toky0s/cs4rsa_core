using Cs4rsa.Cs4rsaDatabase.DataProviders;

using Microsoft.EntityFrameworkCore.Storage;

using System.Threading.Tasks;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IUnitOfWork
    {
        ICurriculumRepository Curriculums { get; }
        IDisciplineRepository Disciplines { get; }
        IKeywordRepository Keywords { get; }
        IUserScheduleRepository UserSchedules { get; }
        IStudentRepository Students { get; }
        ITeacherRepository Teachers { get; }
        IProgramSubjectRepository ProgramSubjects { get; }
        IPreParSubjectRepository PreParSubjects { get; }
        IPreProDetailsRepository PreProDetails { get; }
        IParProDetailsRepository ParProDetails { get; }
        IKeywordTeacherRepository KeywordTeachers { get; }
        ISettingRepository Settings { get; }
        RawSql GetRawSql();
        int Complete();
        Task<int> CompleteAsync();
        Task<IDbContextTransaction> BeginTransAsync();
        Task CommitAsync();
        void Commit();
        Task RollbackAsync();
        void Rollback();
        IDbContextTransaction BeginTrans();
    }
}
