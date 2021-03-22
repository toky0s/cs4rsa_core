using cs4rsa.BaseClasses;
using cs4rsa.Models;
using System;
using System.Collections.ObjectModel;

namespace cs4rsa.ViewModels
{
    public class ClassGroupViewModel : NotifyPropertyChangedBase
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

        private ObservableCollection<string> teachers = new ObservableCollection<string>();
        public ObservableCollection<string> Teachers
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

        private string selectedTeacher;
        public string SelectedTeacher
        {
            get
            {
                return selectedTeacher;
            }
            set
            {
                selectedTeacher = value;
            }
        }

        public ClassGroupViewModel()
        {
            SelectedSubjectChanged += OnSelectedSubjectChanged;
        }

        private void OnSelectedSubjectChanged(object sender, EventArgs e)
        {
            SubjectModel subjectModel = (SubjectModel)sender;
            classGroupModels.Clear();
            foreach (ClassGroupModel classGroupModel in subjectModel.ClassGroupModels)
            {
                classGroupModels.Add(classGroupModel);
            }

            foreach (string teacher in subjectModel.Teachers)
            {
                if (!teachers.Contains(teacher) && teacher.Trim() != "")
                {
                    teachers.Add(teacher);
                }
            }
        }
    }
}
