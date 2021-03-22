using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using cs4rsa.BasicData;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// 
    /// </summary>
    public class ClassGroup
    {
        private readonly string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        private readonly string subjectCode;
        public string SubjectCode
        {
            get
            {
                return subjectCode;
            }
        }

        private readonly List<SchoolClass> schoolClasses = new List<SchoolClass>();
        public List<SchoolClass> SchoolClasses { get { return schoolClasses; } }

        public ClassGroup()
        {

        }

        public ClassGroup(string name, string subjectCode)
        {
            this.name = name;
            this.subjectCode = subjectCode;
        }

        public void AddSchoolClass(SchoolClass schoolClass)
        {
            schoolClasses.Add(schoolClass);
        }

        /// <summary>
        /// Hợp nhất và duy nhất các Schedule của các SchoolClass trong ClassGroup này.
        /// </summary>
        /// <returns>Trả về một Schedule.</returns>
        public Schedule GetSchedule()
        {
            Dictionary<WeekDate, List<StudyTime>> weekDateStudyTimePairs = new Dictionary<WeekDate, List<StudyTime>>();
            foreach(SchoolClass schoolClass in schoolClasses)
            {
                schoolClass.Schedule.ScheduleTime.ToList().ForEach(pair => weekDateStudyTimePairs.Add(pair.Key, pair.Value));
            }
            Schedule schedule = new Schedule(weekDateStudyTimePairs);
            return schedule;
        }

        public Phase GetPhase()
        {
            return schoolClasses[0].StudyWeek.GetPhase();
        }

        public List<string> GetTeachers()
        {
            List<string> teachers = new List<string>();
            foreach(SchoolClass schoolClass in schoolClasses)
            {
                teachers.Add(schoolClass.Teacher);
            }
            return teachers;
        }

        /// <summary>
        /// Hợp nhất hai StudyWeek của các SchoolClass trong ClassGroup này.
        /// </summary>
        /// <returns>Trả về StudyWeek của ClassGroup này.</returns>
        private StudyWeek GetStudyWeeks()
        {
            List<int> studyWeekValues = new List<int>();
            foreach(SchoolClass schoolClass in SchoolClasses)
            {
                studyWeekValues.Add(schoolClass.StudyWeek.StartWeek);
                studyWeekValues.Add(schoolClass.StudyWeek.EndWeek);
            }
            StudyWeek studyWeek = new StudyWeek(studyWeekValues.Min(), studyWeekValues.Max());
            return studyWeek;
        }
    }

    /// <summary>
    /// Class này bao gồm các phương thức thao tác với các ClassGroup.
    /// </summary>
    public class ClassGroupManipulation
    {

    }

    public class ClassGroupFilter
    {
        private WeekDate[] weekDates = new WeekDate[7];
        
        private ObservableCollection<ClassGroup> classGroups;
        public ClassGroupFilter(ObservableCollection<ClassGroup> classGroups)
        {
            this.classGroups = classGroups;
        }

        public void DayFilter(List<WeekDate> weekDates)
        {

        }

        public void TeacherFilter(List<Teacher> teachers)
        {

        }

        public void PhaseFilter(List<Phase> phases)
        {

        }

        public ObservableCollection<ClassGroup> GetClassGroupCollection()
        {
            return null;
        }
    }
}
