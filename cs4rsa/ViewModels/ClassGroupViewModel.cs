using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Messages;
using cs4rsa.Models;
using LightMessageBus;
using LightMessageBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace cs4rsa.ViewModels
{
    public class ClassGroupViewModel : ViewModelBase, 
        IMessageHandler<SelectedSubjectChangeMessage>,
        IMessageHandler<DeleteClassGroupChoiceMessage>,
        IMessageHandler<DeleteSubjectMessage>
    {
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
                OnPropertyChanged();
            }
        }

        private ClassGroupModel _selectedClassGroup;
        public ClassGroupModel SelectedClassGroup
        {
            get
            {
                return _selectedClassGroup;
            }
            set
            {
                _selectedClassGroup = value;
                OnPropertyChanged();
                if (value != null)
                {
                    MessageBus.Default.Publish(new ClassGroupAddedMessage(value));
                }
            }
        }

        private ObservableCollection<Models.Teacher> teachers = new ObservableCollection<Models.Teacher>();
        public ObservableCollection<Models.Teacher> Teachers
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

        private Models.Teacher selectedTeacher;
        public Models.Teacher SelectedTeacher
        {
            get
            {
                return selectedTeacher;
            }
            set
            {
                selectedTeacher = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand GotoCourseCommand { get; set; }

        public ClassGroupViewModel()
        {
            MessageBus.Default.FromAny().Where<SelectedSubjectChangeMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteClassGroupChoiceMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteSubjectMessage>().Notify(this);
            GotoCourseCommand = new RelayCommand(OnGotoCourse);
        }

        private void OnGotoCourse(object obj)
        {
            string url = _selectedClassGroup.ClassGroup.GetUrl();
            Process.Start(url);
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
                BasicData.Teacher teacherAll = new BasicData.Teacher("TẤT CẢ")
                {
                    Id = "0"
                };
                teachers.Add(new Models.Teacher((BasicData.Teacher)teacherAll));

                List<string> tempTeachers = subjectModel.TempTeachers;
                foreach (Models.Teacher teacher in subjectModel.Teachers)
                {
                    if (!teachers.Contains(teacher) && teacher != null)
                    {
                        teachers.Add(teacher);
                        tempTeachers.Remove(teacher.Name);
                    }
                }

                // Giảng viên còn sót lại trong danh sách temp
                // mà không có detail page được xem là giảng viên thỉnh giảng
                if (tempTeachers.Count > 0)
                {
                    for (int i = 0; i < tempTeachers.Count; i++)
                    {
                        BasicData.Teacher guestLecturer = new BasicData.Teacher(tempTeachers[i])
                        {
                            Id = (i + 1).ToString()
                        };
                        teachers.Add(new Models.Teacher((BasicData.Teacher)guestLecturer));
                    }
                }

                SelectedTeacher = teachers[0];
            }
        }

        public void Handle(DeleteClassGroupChoiceMessage message)
        {
            SelectedClassGroup = null;
        }

        public void Handle(DeleteSubjectMessage message)
        {
            classGroupModels.Clear();
        }
    }
}
