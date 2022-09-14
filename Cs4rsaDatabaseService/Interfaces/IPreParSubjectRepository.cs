using Cs4rsaDatabaseService.Models;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface IPreParSubjectRepository : IGenericRepository<PreParSubject>
    {
        public PreParSubject GetById(int id);
    }
}
