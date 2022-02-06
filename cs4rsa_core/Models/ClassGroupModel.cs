using cs4rsa_core.Exceptions;
using Cs4rsaDatabaseService.Models;
using HelperService;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace cs4rsa_core.Models
{
    public class ClassGroupModel
    {
        private string _currentRegisterCode;

        public ClassGroup ClassGroup { get; }
        public int EmptySeat { get; }
        public string Name => ClassGroup.Name;
        public bool HaveSchedule { get; }
        public ObservableCollection<Place> Places { get; }
        public List<string> TempTeacher => ClassGroup.GetTempTeachers();
        public string SubjectCode => ClassGroup.SubjectCode;
        public string RegisterCode
        {
            get
            {
                return _currentRegisterCode ?? ClassGroup.GetRegisterCode();
            }
        }
        public Phase Phase => ClassGroup.GetPhase();
        public Schedule Schedule => ClassGroup.GetSchedule();
        public ImplementType ImplementType => ClassGroup.GetImplementType();
        public RegistrationType RegistrationType => ClassGroup.GetRegistrationType();
        public string Color { get; }

        /// <summary>
        /// Quyết định xem ClassGroupModel có thuộc một Multi Register Code Subject (SpecialSubject)
        /// hay không. Các vấn đề của môn CHE 101 hay BIO 101 dù đã được giải quyết nhưng tôi vẫn
        /// để comment này ở đây giúp bạn lưu ý về vấn đề này tương lai bạn refactor lại nó.
        /// </summary>
        public bool IsBelongSpecialSubject { get; }

        public ClassGroupModel(ClassGroup classGroup, bool isBelongSpecialSubject, ColorGenerator colorGenerator)
        {
            ClassGroup = classGroup;
            Places = new ObservableCollection<Place>(ClassGroup.GetPlaces());
            EmptySeat = classGroup.GetEmptySeat();
            HaveSchedule = IsHaveSchedule();
            Color = colorGenerator.GetColorWithSubjectCode(classGroup.SubjectCode);
            IsBelongSpecialSubject = isBelongSpecialSubject;
        }

        public List<Teacher> GetTeacherModels()
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

        public override bool Equals(object obj)
        {
            if (obj is ClassGroupModel other)
            {
                return Name.Equals(other.Name);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ClassGroup.Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public List<SchoolClassModel> GetSchoolClassModels()
        {
            return ClassGroup.SchoolClasses.Select(sc => new SchoolClassModel(sc)).ToList();
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
                throw new SchoolClassIsNotBelongSpecialSubjectException(message);
            }
        }

        /// <summary>
        /// Với các ClassGroup thuộc Special Subject thì ta có thể
        /// lấy ra Mã đăng ký hiện tại mà người dùng đã chọn.
        /// </summary>
        /// <returns>Mã đăng ký mà người dùng đã từng chọn cho ClassGroupModel này.</returns>
        public string GetCurrentRegisterCode()
        {
            return _currentRegisterCode;
        }
    }
}
