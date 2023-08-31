namespace CwebizAPI.DTOs.Responses;

public class DtoRpSubject
{
    /// <summary>
    /// Danh sách tên giảng viên được lấy từ Temp Teacher của Subject.
    /// </summary>
    public List<string> TeacherNames { get; set; }
    /// <summary>
    /// Danh sách các nhóm lớp
    /// </summary>
    public DtoRpClassGroup[] ClassGroups { get; set; }
    /// <summary>
    /// Tên môn học
    /// </summary>
    public string SubjectName { get; set; }
    /// <summary>
    /// Mã môn
    /// </summary>
    public string SubjectCode { get; set; }
    /// <summary>
    /// Mã Course
    /// </summary>
    public string CourseId { get; set; }
    /// <summary>
    /// Loại đơn vị học tập
    /// </summary>
    public string StudyUnitType { get; set; }
    /// <summary>
    /// Hình thức học
    /// </summary>
    public string StudyType { get; set; }
    /// <summary>
    /// Học kỳ
    /// </summary>
    public string Semester  { get; set; }
    /// <summary>
    /// Mô tả
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Môn tiên quyết
    /// </summary>
    public string PrerequisiteSubjectAsString { get; set; }

    /// <summary>
    /// Môn song hành
    /// </summary>
    public string ParallelSubjectAsString { get; set; }

    /// <summary>
    /// Số tín chỉ
    /// </summary>
    public int StudyUnit { get; set; }

    /// <summary>
    /// Môn học đặc biệt là môn mà một lớp của nó có thể có nhiều mã đăng ký.
    /// </summary>
    public bool IsSpecialSubject { get; set; }

    /// <summary>
    /// Mã màu
    /// </summary>
    public string Color { get; set; }
}