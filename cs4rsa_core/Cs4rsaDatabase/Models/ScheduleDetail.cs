namespace cs4rsa_core.Cs4rsaDatabase.Models
{
    /// <summary>
    /// Thông tin chi tiết về bộ lịch đã lưu.
    ///     
    ///     ScheduleDetailId:       ID
    ///     UserScheduleId:         UserSchedule ID
    /// 
    ///     SubjectCode:            Mã môn (không cần thiết - có thể tính toán)
    ///     SubjectName:            Tên môn (không cần thiết - có thể tính toán)
    ///     ClassGroup:             Nhóm lớp (cần thiết để xác định lớp cần chọn)
    ///     RegisterCode:           Mã đăng ký (cần thiết khi mất mạng)
    ///     SelectedSchoolClass:    SchoolClass được chọn
    /// </summary>
    public class ScheduleDetail
    {
        public int ScheduleDetailId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string ClassGroup { get; set; }
        public string RegisterCode { get; set; }
        public string SelectedSchoolClass { get; set; }
        public int UserScheduleId { get; set; }
        public UserSchedule UserSchedule { get; set; }
    }
}
