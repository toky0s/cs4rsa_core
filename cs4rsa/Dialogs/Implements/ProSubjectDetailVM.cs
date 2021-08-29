using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace cs4rsa.Dialogs.Implements
{
    public class ProSubjectDetailVM : ViewModelBase
    {
        private string _subjectName;
        public string SubjectName
        {
            get { return _subjectName; }
            set { _subjectName = value; OnPropertyChanged(); }
        }

        private string _subjectCode;
        public string SubjectCode
        {
            get { return _subjectCode; }
            set { _subjectCode = value; OnPropertyChanged(); }
        }

        private int _studyUnit;
        public int StudyUnit
        {
            get { return _studyUnit; }
            set { _studyUnit = value; OnPropertyChanged(); }
        }

        private string _folderName;
        public string FolderName
        {
            get { return _folderName; }
            set { _folderName = value; OnPropertyChanged(); }
        }

        private ProgramSubjectModel _programSubjectModel;
        public ProgramSubjectModel ProgramSubjectModel
        {
            get { return _programSubjectModel; }
            set
            {
                _programSubjectModel = value;
                if (value != null)
                {
                    SubjectName = value.SubjectName;
                    SubjectCode = value.SubjectCode;
                    StudyUnit = value.StudyUnit;
                    FolderName = value.FolderName;
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProgramSubjectModel> PreProgramSubjectModels { get; set; }
        public ObservableCollection<ProgramSubjectModel> ParProgramSubjectModels { get; set; }

        public Action CloseDialogCallback { get; set; }
        public Action<object> AddCallback { get; set; }

        public RelayCommand CloseDialogCommand { get; set; }
        public RelayCommand AddCommand { get; set; }


        public ProgramDiagram ProgramDiagram { get; set; }

        public ProSubjectDetailVM()
        {
            CloseDialogCommand = new RelayCommand(OnCloseDialog);
            AddCommand = new RelayCommand(OnAdd);
            PreProgramSubjectModels = new ObservableCollection<ProgramSubjectModel>();
            ParProgramSubjectModels = new ObservableCollection<ProgramSubjectModel>();
        }

        private void OnAdd(object obj)
        {
            AddCallback.Invoke(obj);
        }

        public void LoadPreProSubjectModels()
        {
            if (_programSubjectModel!=null)
            {
                List<ProgramSubject> preProgramSubject = ProgramDiagram.GetPreProgramSubject(_programSubjectModel.ProgramSubject);
                foreach (ProgramSubject programSubject in preProgramSubject)
                {
                    if (programSubject != null)
                        PreProgramSubjectModels.Add(new ProgramSubjectModel(programSubject));
                }
            }
        }

        public void LoadParProSubjectModels()
        {
            List<ProgramSubject> parProgramSubject = ProgramDiagram.GetParProgramSubject(_programSubjectModel.ProgramSubject);
            foreach (ProgramSubject programSubject in parProgramSubject)
            {
                ParProgramSubjectModels.Add(new ProgramSubjectModel(programSubject));
            }
        }

        private void OnCloseDialog(object obj)
        {
            CloseDialogCallback.Invoke();
        }
    }
}
