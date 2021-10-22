using Cs4rsaDatabaseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsaDatabaseService.Interfaces
{
    public interface ISessionRepository: IGenericRepository<Session>
    {
        List<SessionDetail> GetSessionDetails(int sessionId);
    }
}
