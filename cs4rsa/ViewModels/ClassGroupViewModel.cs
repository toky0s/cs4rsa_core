using cs4rsa.BaseClasses;
using cs4rsa.Models;
using LightMessageBus.Interfaces;
using System;
using System.Collections.ObjectModel;
using cs4rsa.Messages;
using LightMessageBus;

namespace cs4rsa.ViewModels
{
    public class ClassGroupViewModel : NotifyPropertyChangedBase, IMessageHandler<SelectedSubjectChangeMessage>
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
            //SelectedSubjectChanged += OnSelectedSubjectChanged;
            MessageBus.Default.FromAny().Where<SelectedSubjectChangeMessage>().Notify(this);
        }

        public void Handle(SelectedSubjectChangeMessage message)
        {
            SubjectModel subjectModel = (message.Source as DisciplinesViewModel).SelectedSubjectModel;
            classGroupModels.Clear();
            foreach (ClassGroupModel classGroupModel in subjectModel.ClassGroupModels)
            {
                classGroupModels.Add(classGroupModel);
            }

            teachers.Clear();
            foreach (TeacherModel teacher in subjectModel.Teachers)
            {
                teachers.Add(teacher);
            }
            SelectedTeacher = teachers[0];
        }
    }
}
