using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;
using cs4rsa_core.Services.TeacherCrawlerSvc.Models;

using System;
using System.Collections.Generic;

namespace cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes
{
    public class SchoolClass
    {
        public string SubjectName { get; set; }
        public string ClassGroupName { get; set; }
        public string SchoolClassName { get; set; }
        public string RegisterCode { get; set; }
        public ClassForm Type { get; set; }
        public string RegistrationTermEnd { get; set; }
        public string RegistrationTermStart { get; set; }
        public string EmptySeat { get; set; }
        public Schedule Schedule { get; set; }
        public IEnumerable<string> Rooms { get; set; }
        public StudyWeek StudyWeek { get; set; }

        /**
         * Tôi thấy mỗi SchoolClass chỉ có một Teacher, tại sao
         * lại thiết kế một Tập hợp Teacher cho SchoolClass.
         * Theo đa số, SchoolClass chỉ do duy nhất một teacher
         * đảm nhiệm, nhưng trong một số trường hợp đặc biệt
         * một SchoolClass sẽ được tách với nhiều teacher đảm nhiệm
         * và chúng khác nhau ở thứ học trong tuần hoặc chả khác
         * nhau gì hết.
         * 
         * Tương ứng với TempTeachers.
         */
        public IEnumerable<TeacherModel> Teachers { get; set; }
        public IEnumerable<string> TempTeachers { get; set; }
        public IEnumerable<Place> Places { get; set; }
        public string RegistrationStatus { get; set; }
        public string ImplementationStatus { get; set; }
        public string Url { get; set; }
        public DayPlaceMetaData DayPlaceMetaData { get; set; }

        public string Color { get; set; }

        public SchoolClass(
            string schoolClassName, 
            string registerCode,
            string type,
            string emptySeat,
            string registrationTermEnd, 
            string registrationTermStart, 
            StudyWeek studyWeek, 
            Schedule schedule,
            IEnumerable<string> rooms,
            IEnumerable<Place> places,
            IEnumerable<TeacherModel> teachers, 
            IEnumerable<string> tempTeachers,
            string registrationStatus, 
            string implementationStatus,
            string url, 
            DayPlaceMetaData dayPlaceMetaData)
        {
            SchoolClassName = schoolClassName;
            RegisterCode = registerCode;
            Type = ClassForm.Find(type);
            EmptySeat = emptySeat;
            RegistrationTermStart = registrationTermStart;
            RegistrationTermEnd = registrationTermEnd;
            StudyWeek = studyWeek;
            Schedule = schedule;
            Rooms = rooms;
            Places = places;
            Teachers = teachers;
            TempTeachers = tempTeachers;
            RegistrationStatus = registrationStatus;
            ImplementationStatus = implementationStatus;
            Url = url;
            DayPlaceMetaData = dayPlaceMetaData;
        }

        public Phase GetPhase()
        {
            return StudyWeek.GetPhase();
        }

        public Cs4rsaMetaData GetMetaDataMap()
        {
            return new Cs4rsaMetaData(Schedule, DayPlaceMetaData, this);
        }

        /**
         * Vì bản thân một SchoolClass bao gồm nhiều tiết học được trải dài trong một Tuần.
         * Vì thế để thuận tiện cho việc hiển thị trên mô phỏng - Phương thức này cho phép
         * tách SchoolClass ra thành một tập các Unit tương ứng với các tiết học với
         * Giảng viên là duy nhất, Giờ học là duy nhất, Thứ học là duy nhất, Phòng học và địa
         * điểm học là duy nhất.
         * 
         * Với các trường hợp đặc biệt, một SchoolClass có thể có nhiều Giảng viên đảm nhiệm
         * Điển hình CS 252: Mạng Máy Tính 
         * http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid=65&timespan=66&t=s
         * 
         * CS 252 A1 do thầy Nguyễn Kim Tuấn và thầy Nguyễn Hoàng Nhật quản lý chả khác nhau
         * cái gì nhưng vẫn bị tách ra. Vì thế trường hợp này khi lấy ra ở mức UNIT thì teacher
         * vẫn là một tập hợp.
         */
        public IEnumerable<SchoolClassUnit> GetSchoolClassUnits()
        {
            foreach (DayOfWeek dayOfWeek in Schedule.GetSchoolDays())
            {
                IEnumerable<StudyTime> studyTimes = Schedule.GetStudyTimesAtDay(dayOfWeek);
                foreach (StudyTime studyTime in studyTimes)
                {
                    SchoolClassUnit schoolClassUnit = new()
                    {
                        ClassName = SchoolClassName,
                        DayOfWeek = dayOfWeek,
                        Start = studyTime.Start,
                        End = studyTime.End,
                        StudyWeek = StudyWeek,
                        Teachers = Teachers,
                        Room = DayPlaceMetaData.GetDayPlacePairAtDay(dayOfWeek).Room
                    };
                    yield return schoolClassUnit;
                }
            }
        }
    }
}
