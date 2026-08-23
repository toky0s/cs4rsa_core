using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.UI.ScheduleTable;

using System.Collections.Generic;

namespace Cs4rsa.UI.ScheduleTable
{
    public enum TimeBlockType
    {
        SchoolClass,
        TimeConflict,
        PlaceConflict,
    }

    public class TimeBlockName
    {
        public const string SchoolClass = "SchoolClassBlock";
        public const string TimeConflict = "TimeConflictBlock";
        public const string PlaceConflict = "PlaceConflictBlock"; 
    }
}
