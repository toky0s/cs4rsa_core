﻿using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Messages;
using cs4rsa.Models;
using LightMessageBus;
using LightMessageBus.Interfaces;
using System;
using System.Collections.ObjectModel;

namespace cs4rsa.ViewModels
{
    public class ClassGroupViewModel : NotifyPropertyChangedBase, 
        IMessageHandler<SelectedSubjectChangeMessage>,
        IMessageHandler<DeleteClassGroupChoiceMessage>
    {
        public static EventHandler SelectedSubjectChanged;

        private ObservableCollection<ClassGroupModel> classGroupModels = new ObservableCollection<ClassGroupModel>();
        public ObservableCollection<ClassGroupModel> ClassGroupModels
        {
            get
            {
                return classGroupModels;
            }
            set
            {
                classGroupModels = value;
                RaisePropertyChanged();
            }
        }

        private ClassGroupModel selectedClassGroup;
        public ClassGroupModel SelectedClassGroup
        {
            get
            {
                return selectedClassGroup;
            }
            set
            {
                selectedClassGroup = value;
                RaisePropertyChanged();
                if (value != null)
                {
                    MessageBus.Default.Publish(new ClassGroupAddedMessage(value));
                }
            }
        }

        private ObservableCollection<TeacherModel> teachers = new ObservableCollection<TeacherModel>();
        public ObservableCollection<TeacherModel> Teachers
        {
            get
            {
                return teachers;
            }
            set
            {
                teachers = value;
            }
        }

        private TeacherModel selectedTeacher;
        public TeacherModel SelectedTeacher
        {
            get
            {
                return selectedTeacher;
            }
            set
            {
                selectedTeacher = value;
                RaisePropertyChanged();
            }
        }

        public ClassGroupViewModel()
        {
            MessageBus.Default.FromAny().Where<SelectedSubjectChangeMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteClassGroupChoiceMessage>().Notify(this);
        }

        public void Handle(SelectedSubjectChangeMessage message)
        {
            SubjectModel subjectModel = message.Source;
            classGroupModels.Clear();
            teachers.Clear();
            if (subjectModel != null)
            {
                foreach (ClassGroupModel classGroupModel in subjectModel.ClassGroupModels)
                {
                    classGroupModels.Add(classGroupModel);
                }
                Teacher teacherAll = new Teacher("TẤT CẢ")
                {
                    Id = "0"
                };
                teachers.Add(new TeacherModel(teacherAll));
                foreach (TeacherModel teacher in subjectModel.Teachers)
                {
                    if (!teachers.Contains(teacher) && teacher != null)
                        teachers.Add(teacher);
                }
                SelectedTeacher = teachers[0];
            }
        }

        public void Handle(DeleteClassGroupChoiceMessage message)
        {
            SelectedClassGroup = null;
        }
    }
}
