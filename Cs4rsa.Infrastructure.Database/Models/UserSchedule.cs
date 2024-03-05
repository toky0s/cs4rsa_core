using System;
using System.Collections.Generic;

namespace Cs4rsa.Database.Models
{
    /// <summary>
    /// Thông tin tổng quan bộ lịch của người dùng đã lưu
    /// 
    ///     UserScheduleId  :Id
    ///     Name            :Tên bộ lịch
    ///     SaveDate        :Ngày lưu
    ///     SemesterValue   :Giá trị học kỳ
    ///     YearValue       :Giá trị năm học
    ///     Semester        :Thông tin học kỳ
    ///     Year            :Thông tin năm học
    ///     
    /// Modified date:
    ///     20240203 - Thêm hai properties Semester và Year
    /// </summary>
    public class UserSchedule
    {
        public int UserScheduleId { get; set; }
        public string Name { get; set; }
        public DateTime SaveDate { get; set; }
        public string SemesterValue { get; set; }
        public string YearValue { get; set; }
        /// <summary>
        /// Thông tin học kỳ, ví dụ: Học kỳ I
        /// </summary>
        public string Semester { get; set; }
        /// <summary>
        /// Thông tin năm học, ví dụ: Năm học 2023 - 2024
        /// </summary>
        public string Year { get; set; }
        public List<ScheduleDetail> SessionDetails { get; set; }
    }
}
