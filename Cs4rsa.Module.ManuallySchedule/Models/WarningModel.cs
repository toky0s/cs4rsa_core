using Cs4rsa.Service.Conflict.Interfaces;
using Cs4rsa.UI.ScheduleTable.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.Module.ManuallySchedule.Models
{
    public enum WarningType
    {
        TimeConflict,
        PlaceConflict,
        EmptySeat
    }

    public class WarningModel
    {
        public string WarningTitle { get; set; }
        public string Description { get; set; }

        public WarningModel(WarningType type, string description)
        {
            WarningTitle = ConvertTypeToTitle(type);
            Description = description;
        }

        private string ConvertTypeToTitle(WarningType type)
        {
            switch (type)
            {
                case WarningType.TimeConflict:
                    return "Trùng lịch";
                case WarningType.PlaceConflict:
                    return "Vị trí xa";
                case WarningType.EmptySeat:
                    return "Hết chỗ";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
