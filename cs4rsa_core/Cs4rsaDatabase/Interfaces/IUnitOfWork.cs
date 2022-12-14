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
        int Complete();
        Task<int> CompleteAsync();
        Task<IDbContextTransaction> BeginTransAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
