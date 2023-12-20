using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.Service.TeacherCrawler.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Cs4rsa.Module.ManuallySchedule.Models
{
    /// <summary>
    /// ClassGroupModel và ClassGroup:
    /// - ClassGroup là nơi cung cấp dữ liệu.
    /// - ClassGroupModel là nơi cung cấp các phương thức và dữ liệu tuỳ chỉnh dựa trên core đã có.
    /// 
    /// 31/03/2023 A Xin
    /// - Thêm Color cho các thành phần phân cấp của ClassGroupModel
    /// bao gồm SchoolClassModel và các thành phần khác.
    /// </summary>
    public class ClassGroupModel
    {
        /// <summary>
        /// Vì một ClassGroup có thể chứa nhiều SchoolClass với nhiều mã đăng ký
        /// Khi đó ClassGroupModel buộc phải chứa duy nhất một mã đăng ký.
        /// </summary>
        public List<SchoolClassModel> CurrentSchoolClassModels { get; }

        public readonly IEnumerable<SchoolClassModel> NormalSchoolClassModels;

        public ClassGroup ClassGroup { get; }
        public int EmptySeat { get; }
        public string Name { get; }
        public bool HaveSchedule { get; }
        public ObservableCollection<Place> Places { get; }
        public IEnumerable<string> TempTeacher { get; }
        public string SubjectCode { get; }
        public List<string> RegisterCodes { get; }
        public Phase Phase => ClassGroup.GetPhase();

        public Schedule Schedule { get; private set; }

        public string Color { get; }
        public SchoolClassModel CompulsoryClass { get; }
        public SchoolClassModel CodeSchoolClass { get; }

        /// <summary>
        /// Với trường hợp Special SchoolClass, người dùng sẽ phải chọn thêm một SchoolClass.
        /// </summary>
        public SchoolClassModel UserSelectedSchoolClass { get; private set; }

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
        public bool IsBelongSpecialSubject { get; }

        /// <summary>
        /// 27/08/2022 A Xin
        /// Với môn ART 161 (Special Subject)
        /// - Có class group có nhiều mã đăng ký
        /// - Và cũng có class group chỉ có một mã đăng ký
        /// 
        /// Nên thuộc tính này sẽ ảnh hưởng tới việc render trên mô phỏng.
        /// </summary>
        public bool IsSpecialClassGroup { get; }

        /// <summary>
        /// Dùng để Deep Clone, CẤM XOÁ
        /// </summary>
        public ClassGroupModel() { }

        public ClassGroupModel(
            ClassGroup classGroup,
            bool isBelongSpecialSubject,
            string color
        )
        {
            ClassGroup = classGroup;
            Name = classGroup.Name;
            SubjectCode = classGroup.SubjectCode;
            HaveSchedule = IsHaveSchedule();
            Color = color;
            IsBelongSpecialSubject = isBelongSpecialSubject;

            NormalSchoolClassModels = ClassGroup.SchoolClasses.Select(sc => new SchoolClassModel(sc, Color));

            if (classGroup.SchoolClasses.Count > 0)
            {
                Schedule = classGroup.GetSchedule();
                CurrentSchoolClassModels = new List<SchoolClassModel>();

                Places = new ObservableCollection<Place>(ClassGroup.GetPlaces());
                EmptySeat = classGroup.GetEmptySeat();
                TempTeacher = classGroup.GetTempTeachers();
                classGroup.GetImplementType();
                classGroup.GetRegistrationType();
                RegisterCodes = classGroup.RegisterCodes;
                ClassSuffix = GetClassSuffix();
                CompulsoryClass = GetCompulsoryClass();
                IsSpecialClassGroup = EvaluateIsSpecialClassGroup(classGroup.SchoolClasses);

                // ClassGroup thường với duy nhất một SchoolClass
                if (classGroup.SchoolClasses.Count == 1)
                {
                    CodeSchoolClass = CompulsoryClass;
                    CurrentSchoolClassModels.Add(CompulsoryClass);
                }
                // ClassGroup thường với một SchoolClass bắt buộc và một SchoolClass chứa mã (hoặc không)
                else if (classGroup.SchoolClasses.Count >= 2 && !IsSpecialClassGroup)
                {
                    var schoolClass = classGroup.SchoolClasses
                        .FirstOrDefault(sc => !sc.SchoolClassName.Equals(CompulsoryClass.SchoolClassName));
                    if (schoolClass == null) return;
                    CodeSchoolClass = new SchoolClassModel(schoolClass, color);
                    CurrentSchoolClassModels.Add(CompulsoryClass);
                    CurrentSchoolClassModels.Add(CodeSchoolClass);
                }
            }
        }

        private string GetClassSuffix()
        {
            return Name.Substring(ClassGroup.SubjectCode.Length + 1);
        }

        private bool EvaluateIsSpecialClassGroup(IEnumerable<SchoolClass> schoolClasses)
        {
            return schoolClasses.Where(sc => sc.SchoolClassName != ClassGroup.Name)
                .Distinct()
                .Count() >= 2;
        }

        public IEnumerable<TeacherModel> GetTeacherModels()
        {
            return ClassGroup.GetTeachers();
        }

        /// <summary>
        /// Kiểm tra xem nhóm lớp này có Lịch hay không. Đôi khi sau giai đoạn đăng ký tín
        /// chỉ một số class group sẽ không còn lịch hiển thị hoặc chỉ hiển thị lịch bổ sung
        /// mà không có lịch chính.
        /// </summary>
        public bool IsHaveSchedule()
        {
            return ClassGroup.GetSchedule().ScheduleTime.Count > 0;
        }

        /// <summary>
        /// Chọn school class cho một class group thuộc special subject.
        /// </summary>
        /// <param name="schoolClassName">Với class group chứa nhiều mã đăng ký (special class group), school class name
        /// là duy nhất sẽ được quét để tìm kiếm school class mà người dùng đã chọn.</param>
        /// <exception cref="Exception">Register code is invalid</exception>
        public void PickSchoolClass(string schoolClassName)
        {
            if (IsBelongSpecialSubject)
            {
                var isValidSchoolClassName = ClassGroup.SchoolClasses
                    .Any(schoolClass => schoolClass.SchoolClassName == schoolClassName);
                if (isValidSchoolClassName)
                {
                    ReRenderSchedule(schoolClassName);
                }
                else
                {
                    throw new Exception("Register code is invalid");
                }
            }
            else
            {
                var message = $"SchoolClass with code {schoolClassName} is not belong special subject!";
                throw new Exception(message);
            }
        }

        /**
         * Mô tả:
         *      Tìm kiếm base class, school class bắt buộc.
         *
         *
         * Trả về:
         *      Trả về Compulsory Class của một class group.
         *      1. Trường hợp có duy nhất một SchoolClass, nó sẽ là Compulsory Class.
         *      2. Trường hợp lớp có hai SchoolClass trở lên, 
         *         SchoolClass thằng không có mã đăng ký sẽ là Compulsory Class.
         */
        private SchoolClassModel GetCompulsoryClass()
        {
            // Early exit and safe check
            if (ClassGroup.SchoolClasses.Count == 0)
            {
                return null;
            }

            if (ClassGroup.SchoolClasses[0].SchoolClassName.Equals(ClassGroup.Name)
                || ClassGroup.SchoolClasses.Count == 1)
            {
                return new SchoolClassModel(ClassGroup.SchoolClasses[0], Color);
            }

            // Trường hợp có nhiều hơn 1 school class,
            // chọn school class nào không có mã đăng ký.
            foreach (var schoolClass in ClassGroup.SchoolClasses.Where(schoolClass => schoolClass.RegisterCode.Equals(string.Empty)))
            {
                return new SchoolClassModel(schoolClass, Color);
            }

            MessageBox.Show("Base school class model cound not be found");
            return null;
        }

        // TODO: Check here for render TimeBlock
        /// <summary>
        /// Trừ school class bắt buộc, các special class group cần một school class đi kèm (có thể có mã đăng ký hoặc không).
        /// 1. ClassGroupModel là một đại diện duy nhất của một ClassGroup.
        /// 2. ClassGroup có thể có nhiều hình thức vì nó có nhiều mã đăng ký.
        /// </summary>
        /// <param name="schoolClassName">Sẽ được sử dụng để tìm kiếm school class tương ứng.</param>
        public void ReRenderSchedule(string schoolClassName)
        {
            UserSelectedSchoolClass = ClassGroup.SchoolClasses
                .Where(schoolClass => schoolClass.SchoolClassName == schoolClassName)
                .Select(schoolClass => new SchoolClassModel(schoolClass, Color))
                .FirstOrDefault();

            CurrentSchoolClassModels.Clear();
            CurrentSchoolClassModels.Add(CompulsoryClass);
            CurrentSchoolClassModels.Add(UserSelectedSchoolClass);
            Schedule = ClassGroup.GetSchedule(CurrentSchoolClassModels.Select(scm => scm.SchoolClass));
        }
    }
}
