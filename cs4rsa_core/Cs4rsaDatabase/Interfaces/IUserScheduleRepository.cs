using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IUserScheduleRepository : IGenericRepository<UserSchedule>
    {
        IEnumerable<ScheduleDetail> GetSessionDetails(int sessionId);
    }
}
