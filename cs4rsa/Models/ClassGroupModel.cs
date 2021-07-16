using cs4rsa.BasicData;
using cs4rsa.Database;
using cs4rsa.Enums;
using System.Collections.Generic;

namespace cs4rsa.Models
{
    public class ClassGroupModel
    {
        private ClassGroup _classGroup;
        public ClassGroup ClassGroup => _classGroup;

        private int _emptySeat;
        public int EmptySeat
        {
            get { return _emptySeat; }
            set { _emptySeat = value; }
        }

        private string _name;
        public string Name
        {
            get { return _classGroup.Name; }
            set { _name = value; }
        }

        private bool _haveSchedule;
        public bool HaveSchedule
        {
            get { return _haveSchedule; }
            set { _haveSchedule = value; }
        }

        public string SubjectCode => _classGroup.SubjectCode;
        public string RegisterCode => _classGroup.GetRegisterCode();
        public Phase Phase => _classGroup.GetPhase();
        public Schedule Schedule => _classGroup.GetSchedule();
        public List<Place> Places => _classGroup.GetPlaces();
        public ImplementType ImplementType => _classGroup.GetImplementType();
        public RegistrationType RegistrationType => _classGroup.GetRegistrationType();
        public string Color { get; set; }

        public ClassGroupModel(ClassGroup classGroup)
        {
            _classGroup = classGroup;
            _emptySeat = classGroup.GetEmptySeat();
            _haveSchedule = IsHaveSchedule();
            Color = ColorGenerator.GetColorWithSubjectCode(classGroup.SubjectCode);
        }

        public List<TeacherModel> GetTeacherModels()
        {
            List<TeacherModel> teacherModels = new List<TeacherModel>();
            foreach (Teacher teacher in _classGroup.GetTeachers())
                teacherModels.Add(new TeacherModel(teacher));
            return teacherModels;
        }

        /// <summary>
        /// Kiểm tra xem nhóm lớp này có Lịch hay không. Đôi khi sau giai đoạn đăng ký tín
        /// chỉ một số class group sẽ không còn lịch hiển thị hoặc chỉ hiển thị lịch bổ sung
        /// mà không có lịch chính.
        /// </summary>
        public bool IsHaveSchedule()
        {
            return _classGroup.GetSchedule().ScheduleTime.Count > 0 ? true : false;
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
            return _classGroup.Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
