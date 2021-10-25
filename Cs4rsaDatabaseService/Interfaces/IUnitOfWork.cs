using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICurriculumRepository Curriculums { get; }
        IDisciplineRepository Disciplines { get; }
        IKeywordRepository Keywords { get; }
        ISessionRepository Sessions { get; }
        IStudentRepository Students { get; }
        ITeacherRepository Teachers { get; }
        IProgramSubjectRepository ProgramSubjects { get; }
        IPreParSubjectRepository PreParSubjects { get; }
        IPreProDetailsRepository PreProDetails { get; }
        IParProDetailsRepository ParProDetails { get; }
        int Complete();
        Task<int> CompleteAsync();
    }
}
