using Cs4rsa.Constants;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.TeacherCrawlerSvc.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Cs4rsa.Services.SubjectCrawlerSvc.Models
{
    /// <summary>
    /// ClassGroupModel và ClassGroup:
    /// - ClassGroup là nơi cung cấp dữ liệu.
    /// - ClassGroupModel là nơi cung cấp các phương thức và dữ liệu tuỳ chỉnh dựa trên core đã có.
    /// </summary>
    public class ClassGroupModel
    {
        /// <summary>
        /// Vì một ClassGroup có thể chứa nhiều SchoolClass với nhiều mã đăng ký
        /// Khi đó ClassGroupModel buộc phải chứa duy nhất một mã đăng ký.
        /// </summary>
        private readonly List<SchoolClassModel> _currentSchoolClassModels;
        public List<SchoolClassModel> CurrentSchoolClassModels
        {
            get { return _currentSchoolClassModels; }
        }

        public ClassGroup ClassGroup { get; }
        public int EmptySeat { get; }
        public string Name { get; }
        public bool HaveSchedule { get; }
        public ObservableCollection<Place> Places { get; }
        public IEnumerable<string> TempTeacher { get; }
        public string SubjectCode { get; }
        public List<string> RegisterCodes { get; }
        public Phase Phase { get => ClassGroup.GetPhase(); }

        private Schedule _schedule;
        public Schedule Schedule { get => _schedule; }
        public ImplementType ImplementType { get; }
        public RegistrationType RegistrationType { get; }
        public string Color { get; }
        public SchoolClassModel CompulsoryClass { get; }
        public SchoolClassModel CodeSchoolClass { get; }

        /// <summary>
        /// Với trường hợp Special SchoolClass, người dùng sẽ phải chọn thêm một SchoolClass.
        /// </summary>
        private SchoolClassModel _userSelectedSchoolClass;
        public SchoolClassModel UserSelectedSchoolClass { get => _userSelectedSchoolClass; }

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

            if (classGroup.SchoolClasses.Count > 0)
            {
                _schedule = classGroup.GetSchedule();
                _currentSchoolClassModels = new();

                Places = new ObservableCollection<Place>(ClassGroup.GetPlaces());
                EmptySeat = classGroup.GetEmptySeat();
                TempTeacher = classGroup.GetTempTeachers();
                ImplementType = classGroup.GetImplementType();
                RegistrationType = classGroup.GetRegistrationType();
                RegisterCodes = classGroup.RegisterCodes;
                ClassSuffix = GetClassSuffix();
                CompulsoryClass = GetCompulsoryClass();
                IsSpecialClassGroup = EvaluateIsSpecialClassGroup(classGroup.SchoolClasses);

                // Class Group thường với duy nhất một SchoolClass
                if (classGroup.SchoolClasses.Count == 1)
                {
                    CodeSchoolClass = CompulsoryClass;
                    CurrentSchoolClassModels.Add(CompulsoryClass);
                }
                // Class Group thường với một SchoolClass bắt buộc và một SchoolClass chứa mã (hoặc không)
                else if (classGroup.SchoolClasses.Count >= 2 && !IsSpecialClassGroup)
                {
                    SchoolClass schoolClass = classGroup.SchoolClasses
                        .Where(sc => !sc.SchoolClassName.Equals(CompulsoryClass.SchoolClassName))
                        .FirstOrDefault();
                    if (schoolClass != null)
                    {
                        CodeSchoolClass = new SchoolClassModel(schoolClass);
                        CurrentSchoolClassModels.Add(CompulsoryClass);
                        CurrentSchoolClassModels.Add(CodeSchoolClass);
                    }
                }
            }
        }

        public string GetClassSuffix()
        {
            return Name[(ClassGroup.SubjectCode.Length + 1)..];
        }

        public bool EvaluateIsSpecialClassGroup(IEnumerable<SchoolClass> schoolClasses)
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

        public IEnumerable<SchoolClassModel> GetSchoolClassModels()
        {
            return ClassGroup.SchoolClasses.Select(sc => new SchoolClassModel(sc));
        }

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
        public void PickSchoolClass(string schoolClassName)
        {
            if (IsBelongSpecialSubject)
            {
                bool isValidSchoolClassName = ClassGroup.SchoolClasses
                    .Any(schoolClass => schoolClass.SchoolClassName == schoolClassName);
                if (isValidSchoolClassName)
                {
                    ReRenderScheduleRequest(schoolClassName);
                }
                else
                {
                    throw new Exception(VmConstants.InvalidRegisterCodeExcepion);
                }
            }
            else
            {
                string message = $"SchoolClass with code {schoolClassName} is not belong special subject!";
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
                return new SchoolClassModel(ClassGroup.SchoolClasses[0]);
            }

            // Trường hợp có nhiều hơn 1 school class,
            // chọn school class nào không có mã đăng ký.
            foreach (SchoolClass schoolClass in ClassGroup.SchoolClasses)
            {
                if (schoolClass.RegisterCode == string.Empty)
                {
                    return new SchoolClassModel(schoolClass);
                }
            }

            MessageBox.Show(VmConstants.NotFoundBaseSchoolClassModelException);
            return null;
        }

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
        public void ReRenderScheduleRequest(string schoolClassName)
        {
            _userSelectedSchoolClass = ClassGroup.SchoolClasses
                .Where(schoolClass => schoolClass.SchoolClassName == schoolClassName)
                .Select(schoolClass => new SchoolClassModel(schoolClass))
                .FirstOrDefault();

            _currentSchoolClassModels.Clear();
            _currentSchoolClassModels.Add(CompulsoryClass);
            _currentSchoolClassModels.Add(_userSelectedSchoolClass);
            _schedule = ClassGroup.GetSchedule(_currentSchoolClassModels.Select(scm => scm.SchoolClass));
        }
    }
}
