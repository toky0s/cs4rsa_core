using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Implements
{
    public class PreParSubjectRepository : GenericRepository<PreParSubject>, IPreParSubjectRepository
    {
        public PreParSubjectRepository(Cs4rsaDbContext context) : base(context)
        {
        }
    }
}
