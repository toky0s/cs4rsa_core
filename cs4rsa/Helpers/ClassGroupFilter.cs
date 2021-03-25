using cs4rsa.BasicData;
using System.Collections.Generic;

namespace cs4rsa.Helpers
{
    class ClassGroupFilter
    {
        private readonly List<ClassGroup> baseClassGroups;

        private Teacher teacher;
        private List<WeekDate> weekDates = new List<WeekDate>();
        private List<Session> sessions = new List<Session>();
        private List<Phase> phases = new List<Phase>();
        private List<Place> places = new List<Place>();

        //public ClassGroupFilter(List<ClassGroup> classGroups)
        //{
        //    baseClassGroup = classGroups;
        //}

        public void AddTeacher(Teacher teacher) => this.teacher = teacher;

        public void AddWeekDate(WeekDate weekDate) => weekDates.Add(weekDate);

        public void AddSession(Session session) => sessions.Add(session);

        public void AddPhase(Phase phase) => phases.Add(phase);

        public void AddPlace(Place place) => places.Add(place);

        public void RemoveTeacher()
        {
            teacher = null;
        }

        public void RemoveWeekDate(WeekDate weekDate)
        {
            weekDates.Remove(weekDate);
        }

        public void RemoveSession(Session session)
        {
            sessions.Remove(session);
        }

        public void RemovePhase(Phase phase)
        {
            phases.Remove(phase);
        }

        public void RemovePlace(Place place)
        {
            places.Remove(place);
        }

        //public List<ClassGroup> Run()
        //{
        //    List<ClassGroup> classGroups = new List<ClassGroup>(baseClassGroups);
        //    //Teacher
        //    if (teacher != null)
        //    {
        //        classGroups = classGroups.Where(item => item.)
        //    }

        //}

        //public List<ClassGroup> Reset()
        //{
        //    return baseClassGroup;
        //}

    }
}
