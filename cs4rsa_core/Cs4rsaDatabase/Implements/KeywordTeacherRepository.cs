using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;

namespace Cs4rsa.Cs4rsaDatabase.Implements
{
    public class KeywordTeacherRepository : GenericRepository<KeywordTeacher>, IKeywordTeacherRepository
    {
        public KeywordTeacherRepository(Cs4rsaDbContext context, RawSql rawSql) : base(context, rawSql)
        {
        }
    }
}
