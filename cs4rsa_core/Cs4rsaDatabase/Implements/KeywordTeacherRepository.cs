using cs4rsa_core.Cs4rsaDatabase.DataProviders;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Cs4rsaDatabase.Implements
{
    public class KeywordTeacherRepository : GenericRepository<KeywordTeacher>, IKeywordTeacherRepository
    {
        public KeywordTeacherRepository(Cs4rsaDbContext context) : base(context)
        {
        }
    }
}
