using System.Collections.ObjectModel;
using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes;
using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes.Enums;

namespace CwebizAPI.DTOs.Responses;

/// <summary>
/// ClassGroupModel và ClassGroup:
/// - ClassGroup là nơi cung cấp dữ liệu.
/// - ClassGroupModel là nơi cung cấp các phương thức và dữ liệu tuỳ chỉnh dựa trên core đã có.
/// - DtoRpClassGroup DTO ClassGroup được trả về bởi API.
/// 
/// 31/03/2023 A Xin
/// - Thêm Color cho các thành phần phân cấp của ClassGroupModel
/// bao gồm SchoolClassModel và các thành phần khác.
/// 21/07/2023 A Xin
/// - Update tài liệu.
/// </summary>
public class DtoRpClassGroup
{
    /// <summary>
    /// Vì một ClassGroup có thể chứa nhiều SchoolClass với nhiều mã đăng ký
    /// Khi đó ClassGroupModel buộc phải chứa duy nhất một mã đăng ký.
    /// </summary>
    public List<DtoRpSchoolClass> CurrentSchoolClassModels { get; set; }

    public IEnumerable<DtoRpSchoolClass> NormalSchoolClassModels { get; set; }
    public int EmptySeat { get; set; }
    public string Name { get; set; }
    public bool HaveSchedule { get; set; }
    public ObservableCollection<Place> Places { get; set; }
    public IEnumerable<string> TempTeachers { get; set; }
    public string SubjectCode { get; set; }
    public List<string> RegisterCodes { get; set; }

    public Phase Phase { get; set; }
    
    public Schedule Schedule { get; set; }

    public ImplementType ImplementType { get; set; }
    public RegistrationType RegistrationType { get; set; }
    public string Color { get; set; }
    public DtoRpSchoolClass? CompulsoryClass { get; set; }
    public DtoRpSchoolClass CodeSchoolClass { get; set; }

    /// <summary>
    /// Với trường hợp Special SchoolClass, người dùng sẽ phải chọn thêm một SchoolClass.
    /// </summary>
    public DtoRpSchoolClass UserSelectedSchoolClass { get; set; }

    /// <summary>
    /// Hậu tố tên lớp, nếu môn Class Name là: ACC 001 B thì hậu tố là B.
    /// </summary>
    public string ClassSuffix { get; set; }

    /// <summary>
    /// Quyết định xem ClassGroupModel có thuộc một Multi Register ClassGroupName Subject (SpecialSubject)
    /// hay không. Các vấn đề của môn CHE 101 hay BIO 101 dù đã được giải quyết nhưng tôi vẫn
    /// để comment này ở đây giúp bạn lưu ý về vấn đề này tương lai bạn refactor lại nó.
    /// 
    /// 25/08/2022 A Xin
    /// Với môn ART 161 (Special Subject) solution hiện tại là check xem đâu là Compulsory Class
    /// của một ClassGroup và check xem class group nào có nhiều hơn 1 mã đăng ký.
    /// </summary>
    public bool IsBelongSpecialSubject { get; set; }

    /// <summary>
    /// 27/08/2022 A Xin
    /// Với môn ART 161 (Special Subject)
    /// - Có class group có nhiều mã đăng ký
    /// - Và cũng có class group chỉ có một mã đăng ký
    /// 
    /// Nên thuộc tính này sẽ ảnh hưởng tới việc render trên mô phỏng.
    /// </summary>
    public bool IsSpecialClassGroup { get; set; }

    // public IEnumerable<TeacherModel> GetTeacherModels()
    // {
    //     return ClassGroup.GetTeachers();
    // }

    /// <summary>
    /// Kiểm tra xem nhóm lớp này có Lịch hay không. Đôi khi sau giai đoạn đăng ký tín
    /// chỉ một số class group sẽ không còn lịch hiển thị hoặc chỉ hiển thị lịch bổ sung
    /// mà không có lịch chính.
    /// </summary>
    // public bool IsHaveSchedule()
    // {
    //     return ClassGroup.GetSchedule().ScheduleTime.Count > 0;
    // }

    /**
     * Mô tả:
     *      Chọn school class cho một class group thuộc special subject.
     *      
     *      
     * Tham số:
     *      schoolClassName:
     *          Với class group chứa nhiều mã đăng ký (special class group), 
     *          school class name là duy nhất sẽ được quét để tìm kiếm 
     *          school class mà người dùng đã chọn.
     */
    // public void PickSchoolClass(string schoolClassName)
    // {
    //     if (IsBelongSpecialSubject)
    //     {
    //         bool isValidSchoolClassName = ClassGroup.SchoolClasses
    //             .Any(schoolClass => schoolClass.SchoolClassName == schoolClassName);
    //         if (isValidSchoolClassName)
    //         {
    //             ReRenderScheduleRequest(schoolClassName);
    //         }
    //         else
    //         {
    //             throw new Exception(VmConstants.InvalidRegisterCodeExcepion);
    //         }
    //     }
    //     else
    //     {
    //         string message = $"SchoolClass with code {schoolClassName} is not belong special subject!";
    //         throw new Exception(message);
    //     }
    // }


    /**
         * Mô tả:
         *      Trừ school class bắt buộc, các special class group 
         *      cần một school class đi kèm (có thể có mã đăng ký hoặc không).
         *      
         *      1. ClassGroupModel là một đại diện duy nhất của một ClassGroup
         *      2. ClassGroup có thể có nhiều hình thức vì nó có nhiều mã đăng ký
         * 
         * Tham số:
         *      schoolClassName:
         *          Trường hợp registerCode là null hoặc empty, 
         *          schoolClassName sẽ được sử dụng để tìm kiếm school class tương ứng.
         */
    // private void ReRenderScheduleRequest(string schoolClassName)
    // {
    //     _userSelectedSchoolClass = ClassGroup.SchoolClasses
    //         .Where(schoolClass => schoolClass.SchoolClassName.Equals(schoolClassName))
    //         .Select(schoolClass => new SchoolClassModel(schoolClass, Color))
    //         .FirstOrDefault();
    //
    //     _currentSchoolClassModels.Clear();
    //     _currentSchoolClassModels.Add(CompulsoryClass);
    //     _currentSchoolClassModels.Add(_userSelectedSchoolClass);
    //     _schedule = ClassGroup.GetSchedule(_currentSchoolClassModels.Select(scm => scm.SchoolClass));
    // }
}