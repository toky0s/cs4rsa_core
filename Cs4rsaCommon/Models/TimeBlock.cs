using Cs4rsaCommon.Enums;

namespace Cs4rsaCommon.Models
{
    /// <summary>
    /// Đại diện cho một ô trong ScheduleControl.
    /// </summary>
    public struct TimeBlock
    {
        public string? Background { get; set; }
        public string? Desciption { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public BlockType BlockType { get; set; }
    }
}
