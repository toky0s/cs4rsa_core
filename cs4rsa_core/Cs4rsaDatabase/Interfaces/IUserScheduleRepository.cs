using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IUserScheduleRepository
    {
        List<ScheduleDetail> GetSessionDetails(int sessionId);
        int Remove(UserSchedule userSchedule);
        void Add(UserSchedule userSchedule);
        List<UserSchedule> GetAll();
    }
}
