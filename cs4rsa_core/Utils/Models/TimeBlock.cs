using System;

namespace Cs4rsa.Utils.Models
{
    /// <summary>
    /// Đại diện cho một ô trong ScheduleControl.
    /// </summary>
    public class TimeBlock
    {
        public string Id { get; set; }
        /// <summary>
        /// Màu nền
        /// </summary>
        public string Background { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }

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

        /// <summary>
        /// Tên class group nếu có - mặc định Empty
        /// </summary>
        public string ClassGroupName { get; set; }

        /// <summary>
        /// Tên class 1 nếu có xung đột - mặc đinh Empty
        /// </summary>
        public string Class1 { get; set; }

        /// <summary>
        /// Tên class 2 nếu có xung đột - mặc đinh Empty
        /// </summary>
        public string Class2 { get; set; }

        public ScheduleTableItemType ScheduleTableItemType { get; set; }
    }
}
