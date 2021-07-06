﻿using System.Collections.Generic;
using cs4rsa.BasicData;
using cs4rsa.Enums;
using cs4rsa.Models;

namespace cs4rsa.Models
{
    public class ClassGroupModel
    {
        public ClassGroup classGroup;

        private int _emptySeat;
        public int EmptySeat
        {
            get
            {
                return _emptySeat;
            }
            set
            {
                _emptySeat = value;
            }
        }
        public string Name => classGroup.Name;
        public string SubjectCode => classGroup.SubjectCode;
        public string RegisterCode => classGroup.GetRegisterCode();
        public Phase Phase => classGroup.GetPhase();
        public Schedule Schedule => classGroup.GetSchedule();
        public List<Place> Places => classGroup.GetPlaces();
        public ImplementType ImplementType => classGroup.GetImplementType();
        public RegistrationType RegistrationType => classGroup.GetRegistrationType();
        public string Color { get; set; }

        public ClassGroupModel(ClassGroup classGroup)
        {
            this.classGroup = classGroup;
            _emptySeat = classGroup.GetEmptySeat();
        }

        public List<TeacherModel> GetTeacherModels()
        {
            List<TeacherModel> teacherModels = new List<TeacherModel>();
            foreach (Teacher teacher in classGroup.GetTeachers())
                teacherModels.Add(new TeacherModel(teacher));
            return teacherModels;
        }
    }
}
