using cs4rsa_core.BaseClasses;
using cs4rsa_core.Models;
using Cs4rsaDatabaseService.DataProviders;
using HelperService;
using Microsoft.Toolkit.Mvvm.Input;
using ProgramSubjectCrawlerService.DataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace cs4rsa_core.Dialogs.Implements
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
        private ColorGenerator _colorGenerator;
        private Cs4rsaDbContext _cs4rsaDbContext;
        public ProSubjectDetailVM(ColorGenerator colorGenerator, Cs4rsaDbContext cs4rsaDbContext)
        {
            _colorGenerator = colorGenerator;
            _cs4rsaDbContext = cs4rsaDbContext;
            CloseDialogCommand = new RelayCommand(OnCloseDialog);
            AddCommand = new RelayCommand(OnAdd);
            PreProgramSubjectModels = new ObservableCollection<ProgramSubjectModel>();
            ParProgramSubjectModels = new ObservableCollection<ProgramSubjectModel>();
        }

        private void OnAdd()
        {
            Console.WriteLine("Run Add in ProSubjectDetail VM");
            //AddCallback.Invoke(obj);
        }

        public void LoadPreProSubjectModels()
        {
            if (_programSubjectModel!=null)
            {
                List<ProgramSubject> preProgramSubject = ProgramDiagram.GetPreProgramSubject(_programSubjectModel.ProgramSubject);
                foreach (ProgramSubject programSubject in preProgramSubject)
                {
                    if (programSubject != null)
                        PreProgramSubjectModels.Add(new ProgramSubjectModel(programSubject, _colorGenerator, _cs4rsaDbContext));
                }
            }
        }

        public void LoadParProSubjectModels()
        {
            if (_programSubjectModel != null)
            {
                List<ProgramSubject> parProgramSubject = ProgramDiagram.GetParProgramSubject(_programSubjectModel.ProgramSubject);
                foreach (ProgramSubject programSubject in parProgramSubject)
                {
                    ParProgramSubjectModels.Add(new ProgramSubjectModel(programSubject, _colorGenerator, _cs4rsaDbContext));
                }
            }
        }

        private void OnCloseDialog()
        {
            CloseDialogCallback.Invoke();
        }
    }
}
