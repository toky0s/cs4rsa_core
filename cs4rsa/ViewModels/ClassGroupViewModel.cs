using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using cs4rsa.BasicData;
using cs4rsa.Models;
using cs4rsa.BaseClasses;
using cs4rsa.ViewModels;
using System.Runtime.CompilerServices;

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

        public ClassGroupViewModel()
        {
            SelectedSubjectChanged += OnSelectedSubjectChanged;
        }

        private void OnSelectedSubjectChanged(object sender, EventArgs e)
        {
            SubjectModel subjectModel = (SubjectModel)sender;
            classGroupModels.Clear();
            foreach(ClassGroupModel classGroupModel in subjectModel.ClassGroupModels)
            {
                classGroupModels.Add(classGroupModel);
            }
        }
    }
}
