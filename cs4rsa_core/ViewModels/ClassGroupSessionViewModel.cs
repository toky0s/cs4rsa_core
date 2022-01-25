using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.DialogViews;
using cs4rsa_core.Dialogs.Implements;
using cs4rsa_core.Interfaces;
using cs4rsa_core.Messages;
using cs4rsa_core.Models;
using Cs4rsaDatabaseService.Models;
using LightMessageBus;
using LightMessageBus.Interfaces;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;

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
                    if (value.IsBelongSpecialSubject)
                    {
                        OnShowDetailsSchoolClasses();
                    }
                    else
                    {
                        MessageBus.Default.Publish(new ClassGroupAddedMessage(value));
                    }
                }
            }
        }
        public ObservableCollection<Teacher> Teachers { get; set; } = new();
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
        public RelayCommand ShowDetailsSchoolClassesCommand { get; set; }
        private readonly IOpenInBrowser _openInBrowser;
        public ClassGroupSessionViewModel(IOpenInBrowser openInBrowser)
        {
            _openInBrowser = openInBrowser;
            MessageBus.Default.FromAny().Where<SelectedSubjectChangeMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteClassGroupChoiceMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteSubjectMessage>().Notify(this);
            GotoCourseCommand = new RelayCommand(OnGotoCourse);
            ShowDetailsSchoolClassesCommand = new RelayCommand(OnShowDetailsSchoolClasses);
        }

        public void OnShowDetailsSchoolClasses()
        {
            ShowDetailsSchoolClassesUC showDetailsSchoolClassesUC = new();
            ShowDetailsSchoolClassesViewModel vm = new();
            vm.ClassGroupModel = _selectedClassGroup;
            vm.CloseDialogCallback = CloseDialogAndHandleClassGroupResult;
            _selectedClassGroup.GetSchoolClassModels()
                .ForEach(scm =>
                {
                    if (scm.Type == "LAB")
                    {
                        vm.SchoolClassModels.Add(scm);
                    }
                });
            showDetailsSchoolClassesUC.DataContext = vm;
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(showDetailsSchoolClassesUC);
        }

        private void CloseDialogAndHandleClassGroupResult(ClassGroupResult classGroupResult)
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            // Pick current register code here
            ClassGroupModel classGroupModel = classGroupResult.ClassGroupModel;
            string registerCode = classGroupResult.SelectedRegisterCode;
            foreach (var classGroupMD in ClassGroupModels.Where(classGroupMD => classGroupMD.Name.Equals(classGroupModel.Name)))
            {
                classGroupMD.PickASchoolClass(registerCode);
                MessageBus.Default.Publish(new ClassGroupAddedMessage(classGroupMD));
            }
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
                Teacher allTeacher = new() { TeacherId = 0, Name = "TẤT CẢ" };
                Teachers.Add(allTeacher);

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
