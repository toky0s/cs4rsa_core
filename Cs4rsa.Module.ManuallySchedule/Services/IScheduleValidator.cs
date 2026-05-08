using Cs4rsa.Module.ManuallySchedule.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Services
{
    public interface IScheduleValidator
    {
        List<WarningModel> ValidateSchedule(List<SchoolClassModel> schoolClasses);
    }
}
