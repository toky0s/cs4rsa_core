using Cs4rsaDatabaseService.Models;
using HelperService;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace cs4rsa_core.Models
{
    public class ClassGroupModel: ICloneable
    {
        public ClassGroup ClassGroup { get; }

        public int EmptySeat { get; set; }

        private string _name;
        public string Name
        {
            get => ClassGroup.Name;
            set => _name = value;
        }
        public bool HaveSchedule { get; set; }

        private ObservableCollection<Place> _places;
        public ObservableCollection<Place> Places
        {
            get => new ObservableCollection<Place>(ClassGroup.GetPlaces());
            set => _places = value;
        }

        private List<string> _tempTeacher;

        public List<string> TempTeacher
        {
            get => ClassGroup.GetTempTeachers();
            set => _tempTeacher = value;
        }

        public string SubjectCode => ClassGroup.SubjectCode;
        public string RegisterCode => ClassGroup.GetRegisterCode();
        public Phase Phase => ClassGroup.GetPhase();
        public Schedule Schedule => ClassGroup.GetSchedule();
        public ImplementType ImplementType => ClassGroup.GetImplementType();
        public RegistrationType RegistrationType => ClassGroup.GetRegistrationType();
        public string Color { get; set; }

        private ColorGenerator _colorGenerator;

        public ClassGroupModel(ClassGroup classGroup, ColorGenerator colorGenerator)
        {
            ClassGroup = classGroup;
            EmptySeat = classGroup.GetEmptySeat();
            HaveSchedule = IsHaveSchedule();
            _colorGenerator = colorGenerator;
            Color = _colorGenerator.GetColorWithSubjectCode(classGroup.SubjectCode);
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
            return ClassGroup.GetSchedule().ScheduleTime.Count > 0 ? true : false;
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

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
