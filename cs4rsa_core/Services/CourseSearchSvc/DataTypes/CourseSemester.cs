namespace cs4rsa_core.Services.CourseSearchSvc.DataTypes
{
    /// <summary>
    /// Class này đại diện cho thông tin của một học kỳ, đóng vai trò
    /// quan trọng trong việc xác định được course cần thiết trong các
    /// crawler liên quan tới cào thông tin môn học và bảng điểm sinh viên.
    /// </summary>
    public class CourseSemester
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}