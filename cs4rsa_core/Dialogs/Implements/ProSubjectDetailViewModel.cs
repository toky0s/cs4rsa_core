using CommunityToolkit.Mvvm.Input;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Models;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Utils;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Cs4rsa.Dialogs.Implements
{
    internal class ProSubjectDetailViewModel : ViewModelBase
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

        public Action AddCallback { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public ProgramDiagram ProgramDiagram { get; set; }

        #region Services
        private readonly ColorGenerator _colorGenerator;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        public ProSubjectDetailViewModel(ColorGenerator colorGenerator, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _colorGenerator = colorGenerator;

            AddCommand = new RelayCommand(OnAdd);

            PreProgramSubjectModels = new();
            ParProgramSubjectModels = new();
        }

        private void OnAdd()
        {
            Console.WriteLine("Run Add in ProSubjectDetail VM");
        }

        public async Task LoadPreProSubjectModels()
        {
            if (_programSubjectModel != null)
            {
                IEnumerable<ProgramSubject> preProgramSubjects = ProgramDiagram.GetPreProgramSubjects(_programSubjectModel.ProgramSubject);
                foreach (ProgramSubject programSubject in preProgramSubjects)
                {
                    if (programSubject != null)
                    {
                        ProgramSubjectModel programSubjectModel = await ProgramSubjectModel.CreateAsync(programSubject, _colorGenerator, _unitOfWork);
                        PreProgramSubjectModels.Add(programSubjectModel);
                    }
                }
            }
        }

        public async Task LoadParProSubjectModels()
        {
            if (_programSubjectModel != null)
            {
                IEnumerable<ProgramSubject> parProgramSubject = await ProgramDiagram.GetParProgramSubject(_programSubjectModel.ProgramSubject);
                foreach (ProgramSubject programSubject in parProgramSubject)
                {
                    ProgramSubjectModel programSubjectModel = await ProgramSubjectModel.CreateAsync(programSubject, _colorGenerator, _unitOfWork);
                    ParProgramSubjectModels.Add(programSubjectModel);
                }
            }
        }
    }
}
