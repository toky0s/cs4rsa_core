using System;
using System.Collections.Generic;

namespace Cs4rsa.Cs4rsaDatabase.Models
{
    /// <summary>
    /// Thông tin tổng quan bộ lịch của người dùng đã lưu
    /// 
    ///     UserScheduleId: Id
    ///     Name:           Tên bộ lịch
    ///     SaveDate:       Ngày lưu
    ///     SemesterValue:  Giá trị học kỳ
    ///     YearValue:      Giá trị năm học
    /// </summary>
    public class UserSchedule
    {
        public int UserScheduleId { get; set; }
        public string Name { get; set; }
        public DateTime SaveDate { get; set; }
        public string SemesterValue { get; set; }
        public string YearValue { get; set; }
        public List<ScheduleDetail> SessionDetails { get; set; }
    }
}
