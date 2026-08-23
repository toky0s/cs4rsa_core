using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.UI.ScheduleTable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Services
{
    public interface ITimeBlockGenerator
    {
        /// <summary>
        /// Sinh ra một khối thời gian để vẽ trên ScheduleTable.
        /// </summary>
        /// <returns><see cref="TimeBlock"/></returns>
        TimeBlock[] Generate(Conflict conflict);
        TimeBlock[] Generate(PlaceConflict conflict);
        TimeBlock[] Generate(SchoolClassModel schoolClassModel);
    }
}
