using System;

namespace ConflictService.DataTypes
{
    /// <summary>
    /// Đại điện cho một khoảng giao về thời gian giữa hai StudyTime. Phục vụ cho việc phát hiện xung đột.
    /// </summary>
    public class StudyTimeIntersect
    {
        private readonly DateTime _start;
        private readonly DateTime _end;

        public string StartString { get { return _start.ToString("HH:mm"); } }
        public string EndString { get { return _end.ToString("HH:mm"); } }

        public DateTime Start => _start;
        public DateTime End => _end;

        public StudyTimeIntersect(DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
        }
    }
}
