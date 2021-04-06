using cs4rsa.BasicData;
using System;
using System.Collections.Generic;
using System.Linq;
using cs4rsa.Helpers;

namespace cs4rsa.Helpers
{
    class ClassGroupFilter
    {
        private readonly List<ClassGroup> baseClassGroups;

        private Teacher teacher;
        private Phase phase = Phase.NON;
        private List<WeekDate> weekDates = new List<WeekDate>();
        private List<Session> sessions = new List<Session>();
        private List<Place> places = new List<Place>();

        public ClassGroupFilter(List<ClassGroup> classGroups)
        {
            baseClassGroups = classGroups;
        }

        public void AddTeacher(Teacher teacher) => this.teacher = teacher;

        public void AddWeekDate(WeekDate weekDate) => weekDates.Add(weekDate);

        public void AddSession(Session session) => sessions.Add(session);

        public void AddPhase(Phase phase) => this.phase = phase;

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
            this.phase = Phase.NON;
        }

        public void RemovePlace(Place place)
        {
            places.Remove(place);
        }

        public List<ClassGroup> Run()
        {
            List<ClassGroup> classGroups = new List<ClassGroup>(baseClassGroups);
            //Teacher
            if (teacher != null)
            {
                classGroups = classGroups.Where(classGroup => classGroup.GetTeachers().Contains(teacher)).ToList();
            }
            //Phase
            if (phase != Phase.NON)
            {
                classGroups = classGroups.Where(classGroup => classGroup.GetPhase() == phase).ToList();
            }
            //WeekDate
            if (weekDates.Count != 0)
            {
                classGroups = classGroups.Where(
                    classGroup => 
                    Checker.ThisSetInThatSet<WeekDate>(classGroup.GetWeekDates(), weekDates)
                )
                .ToList();
            }
            if (sessions.Count != 0)
            {
                classGroups = classGroups.Where(
                    classGroup =>
                    Checker.ThisSetInThatSet<Session>(classGroup.GetSession(), sessions)
                    ).ToList();
            }
            if (sessions.Count != 0)
            {
                classGroups = classGroups.Where(
                    classGroup =>
                    Checker.ThisSetInThatSet<Place>(classGroup.GetPlaces(), places)
                    ).ToList();
            }

            return classGroups;
        }

        public List<ClassGroup> Reset()
        {
            return baseClassGroups;
        }

    }
}
