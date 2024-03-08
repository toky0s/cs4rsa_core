using System.Collections.Generic;
using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Interfaces
{
    public interface IUserScheduleRepository
    {
        List<ScheduleDetail> GetSessionDetails(int sessionId);
        int Remove(UserSchedule userSchedule);
        int Remove(int userScheduleId);
        void Add(UserSchedule userSchedule);
        List<UserSchedule> GetAll();
    }
}
