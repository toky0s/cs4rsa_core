using cs4rsa_core.Commons.Enums;

using System;

namespace cs4rsa_core.Commons.Models
{
    /// <summary>
    /// Đại diện cho một ô trong ScheduleControl.
    /// </summary>
    public class TimeBlock
    {
        /// <summary>
        /// Màu nền
        /// </summary>
        public readonly string Background;

        /// <summary>
        /// Mô tả
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// Nội dung
        /// </summary>
        public readonly string Content;

        /// <summary>
        /// Thứ trong tuần
        /// </summary>
        public readonly DayOfWeek DayOfWeek;

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public readonly DateTime Start;

        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        public readonly DateTime End;

        /// <summary>
        /// Loại block
        /// </summary>
        public readonly BlockType BlockType;

        /// <summary>
        /// Tên class group nếu có - mặc định Empty
        /// </summary>
        public readonly string Code;

        /// <summary>
        /// Tên class 1 nếu có xung đột - mặc đinh Empty
        /// </summary>
        public readonly string Class1;

        /// <summary>
        /// Tên class 2 nếu có xung đột - mặc đinh Empty
        /// </summary>
        public readonly string Class2;

        public TimeBlock(
            string background,
            string description,
            DayOfWeek dayOfWeek,
            DateTime start, DateTime end,
            BlockType blockType,
            string content,
            string code = "",
            string class1 = "",
            string class2 = "")
        {
            Background = background;
            Description = description;
            DayOfWeek = dayOfWeek;
            Start = start;
            End = end;
            BlockType = blockType;
            Content = content;
            Code = code;
            Class1 = class1;
            Class2 = class2;
        }
    }
}
