using Microsoft.EntityFrameworkCore.Storage;

using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface IUnitOfWork
    {
        ICurriculumRepository Curriculums { get; }
        IDisciplineRepository Disciplines { get; }
        IKeywordRepository Keywords { get; }
        ISessionRepository Sessions { get; }
        IStudentRepository Students { get; }
        IStudentImageRepository StudentImages { get; }
        ITeacherRepository Teachers { get; }
        IProgramSubjectRepository ProgramSubjects { get; }
        IPreParSubjectRepository PreParSubjects { get; }
        IPreProDetailsRepository PreProDetails { get; }
        IParProDetailsRepository ParProDetails { get; }
        ISessionSchoolClassRepository SessionSchoolClassRepository { get; }
        int Complete();
        Task<int> CompleteAsync();
        IDbContextTransaction BeginTrans();
        Task<IDbContextTransaction> BeginTransAsync();
        void Commit();
        Task CommitAsync();
        void Rollback();
        Task RollbackAsync();
    }
}
