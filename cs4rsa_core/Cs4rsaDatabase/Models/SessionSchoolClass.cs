namespace cs4rsa_core.Cs4rsaDatabase.Models
{
    /// <summary>
    /// Thông tin school class của một class group. 
    /// Trừ lớp bắt buộc thì sẽ luôn có một lớp đi kèm nếu có.
    /// 
    ///     SessionSchoolClassId:   ID
    ///     Name:                   Tên
    ///     Type:                   Loại hình
    /// </summary>
    public class SessionSchoolClass
    {
        public int SessionSchoolClassId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public ScheduleDetail SessionDetail { get; set; }
    }
}
