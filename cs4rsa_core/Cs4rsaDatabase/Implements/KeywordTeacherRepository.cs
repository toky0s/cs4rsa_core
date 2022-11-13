using cs4rsa_core.Cs4rsaDatabase.DataProviders;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;

namespace cs4rsa_core.Cs4rsaDatabase.Implements
{
    public class KeywordTeacherRepository : GenericRepository<KeywordTeacher>, IKeywordTeacherRepository
    {
        public KeywordTeacherRepository(Cs4rsaDbContext context) : base(context)
        {
        }
    }
}
