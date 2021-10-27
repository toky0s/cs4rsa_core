using cs4rsa_core.BaseClasses;
using cs4rsa_core.Interfaces;
using cs4rsa_core.Messages;
using cs4rsa_core.Models;
using Cs4rsaDatabaseService.Models;
using LightMessageBus;
using LightMessageBus.Interfaces;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace cs4rsa_core.ViewModels
{
    public class ClassGroupSessionViewModel : ViewModelBase,
        IMessageHandler<SelectedSubjectChangeMessage>,
        IMessageHandler<DeleteClassGroupChoiceMessage>,
        IMessageHandler<DeleteSubjectMessage>
    {
        public ObservableCollection<ClassGroupModel> ClassGroupModels { get; set; } = new();

        private ClassGroupModel _selectedClassGroup;
        public ClassGroupModel SelectedClassGroup
        {
            get => _selectedClassGroup;
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

        public ObservableCollection<Teacher> Teachers { get; set; } = new ObservableCollection<Teacher>();

        private Teacher selectedTeacher;
        public Teacher SelectedTeacher
        {
            get => selectedTeacher;
            set
            {
                selectedTeacher = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand GotoCourseCommand { get; set; }
        private readonly IOpenInBrowser _openInBrowser;
        public ClassGroupSessionViewModel(IOpenInBrowser openInBrowser)
        {
            _openInBrowser = openInBrowser;
            MessageBus.Default.FromAny().Where<SelectedSubjectChangeMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteClassGroupChoiceMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteSubjectMessage>().Notify(this);
            GotoCourseCommand = new RelayCommand(OnGotoCourse);
        }

        private void OnGotoCourse()
        {
            string url = _selectedClassGroup.ClassGroup.GetUrl();
            _openInBrowser.Open(url);
        }

        public void Handle(SelectedSubjectChangeMessage message)
        {
            SubjectModel subjectModel = message.Source;
            ClassGroupModels.Clear();
            Teachers.Clear();
            if (subjectModel != null)
            {
                foreach (ClassGroupModel classGroupModel in subjectModel.ClassGroupModels)
                {
                    ClassGroupModels.Add(classGroupModel);
                }
                Teacher ALL_TEACHER = new() { TeacherId = 0, Name = "TẤT CẢ" };
                Teachers.Add(ALL_TEACHER);

                List<string> tempTeachers = subjectModel.TempTeachers;
                foreach (Teacher teacher in subjectModel.Teachers)
                {
                    if (!Teachers.Contains(teacher) && teacher != null)
                    {
                        Teachers.Add(teacher);
                        tempTeachers.Remove(teacher.Name);
                    }
                }

                // Giảng viên còn sót lại trong danh sách temp
                // mà không có detail page được xem là giảng viên thỉnh giảng
                if (tempTeachers.Count > 0)
                {
                    for (int i = 0; i < tempTeachers.Count; i++)
                    {
                        Teacher guestLecturer = new Teacher() { TeacherId = i + 1, Name = tempTeachers[i] };
                        Teachers.Add(guestLecturer);
                    }
                }

                SelectedTeacher = Teachers[0];
            }
        }

        public void Handle(DeleteClassGroupChoiceMessage message)
        {
            SelectedClassGroup = null;
        }

        public void Handle(DeleteSubjectMessage message)
        {
            ClassGroupModels.Clear();
        }
    }
}
