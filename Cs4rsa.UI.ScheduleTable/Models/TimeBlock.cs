using Cs4rsa.UI.ScheduleTable.Interfaces;

using System;

namespace Cs4rsa.UI.ScheduleTable.Models
{
    /// <summary>
    /// Đại diện cho một ô trong ScheduleControl.
    /// </summary>
    public abstract class TimeBlock
    {
        public string Name { get; set; }
        public TimeBlock(
            string id
            , string background
            , string content
            , DayOfWeek dayOfWeek
            , DateTime start
            , DateTime end
            , ScheduleTableItemType scheduleTableItemType)
        {
            Id = id;
            Background = background;
            Content = content;
            DayOfWeek = dayOfWeek;
            Start = start;
            End = end;
            ScheduleTableItemType = scheduleTableItemType;
            Name = "TimeBlock";
        }

        public string Id { get; }
        /// <summary>
        /// Màu nền
        /// </summary>
        public string Background { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Thứ trong tuần
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        public DateTime End { get; set; }
        public ScheduleTableItemType ScheduleTableItemType { get; set; }
    }
}
