using Cs4rsaDatabaseService.Models;
using HelperService;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SubjectCrawlService1.Models
{
    /// <summary>
    /// ClassGroupModel và ClassGroup:
    /// - ClassGroup là nơi cung cấp dữ liệu.
    /// - ClassGroupModel là nơi cung cấp các phương thức và dữ liệu tuỳ chỉnh dựa trên core đã có.
    /// </summary>
    public class ClassGroupModel
    {
        private string _currentRegisterCode;

        public ClassGroup ClassGroup { get; }
        public short EmptySeat { get; }
        public string Name { get; }
        public bool HaveSchedule { get; }
        public ObservableCollection<Place> Places { get; }
        public IEnumerable<string> TempTeacher { get; }
        public string SubjectCode { get; }
        public string RegisterCode
        {
            get
            {
                return _currentRegisterCode ?? ClassGroup.GetRegisterCode();
            }
        }
        public Phase Phase { get; }
        public Schedule Schedule { get; }
        public ImplementType ImplementType { get; }
        public RegistrationType RegistrationType { get; }
        public string Color { get; }

        /// <summary>
        /// Quyết định xem ClassGroupModel có thuộc một Multi Register Code Subject (SpecialSubject)
        /// hay không. Các vấn đề của môn CHE 101 hay BIO 101 dù đã được giải quyết nhưng tôi vẫn
        /// để comment này ở đây giúp bạn lưu ý về vấn đề này tương lai bạn refactor lại nó.
        /// </summary>
        public bool IsBelongSpecialSubject { get; }

        public ClassGroupModel() { }

        public ClassGroupModel(ClassGroup classGroup, bool isBelongSpecialSubject, ColorGenerator colorGenerator)
        {
            ClassGroup = classGroup;
            Name = classGroup.Name;
            SubjectCode = classGroup.SubjectCode;
            Places = new ObservableCollection<Place>(ClassGroup.GetPlaces());
            EmptySeat = classGroup.GetEmptySeat();
            HaveSchedule = IsHaveSchedule();
            TempTeacher = classGroup.GetTempTeachers();
            Color = colorGenerator.GetColorWithSubjectCode(classGroup.SubjectCode);
            Phase = classGroup.GetPhase();
            Schedule = classGroup.GetSchedule();
            ImplementType = classGroup.GetImplementType();
            RegistrationType = classGroup.GetRegistrationType();
            IsBelongSpecialSubject = isBelongSpecialSubject;
        }

        public IEnumerable<Teacher> GetTeacherModels()
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

        public void PickSchoolClass(string registerCode)
        {
            if (IsBelongSpecialSubject)
            {
                bool isValidRegisterCode = ClassGroup.SchoolClasses
                    .Any(schoolClass => schoolClass.RegisterCode == registerCode);
                if (isValidRegisterCode)
                {
                    _currentRegisterCode = registerCode;
                    ClassGroup.ReRenderScheduleRequest(registerCode);
                }
                else
                {
                    throw new Exception("Register code is invalid!");
                }
            }
            else
            {
                string message = $"SchoolClass with code {registerCode} is not belong special subject!";
                throw new ArgumentException(message);
            }
        }
    }
}
