using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Models;

namespace cs4rsa.ViewModels
{
    class AutoScheduleViewModel: NotifyPropertyChangedBase
    {
        private StudentModel _studentModel;
        public StudentModel StudentModel
        {
            get
            {
                return _studentModel;
            }
            set
            {
                _studentModel = value;
            }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                _progressValue = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<ProgramSubjectModel> _programSubjectModels = new ObservableCollection<ProgramSubjectModel>();
        public ObservableCollection<ProgramSubjectModel> ProgramSubjectModels
        {
            get
            {
                return _programSubjectModels;
            }
            set
            {
                _programSubjectModels = value;
            }
        }

        private ProgramSubjectModel _selectedProSubject;
        public ProgramSubjectModel SelectedProSubject
        {
            get
            {
                return _selectedProSubject;
            }
            set
            {
                _selectedProSubject = value;
            }
        }

        private ObservableCollection<ProgramSubjectModel> _choicedProSubjectModels = new ObservableCollection<ProgramSubjectModel>();
        public ObservableCollection<ProgramSubjectModel> ChoicedProSubjectModels
        {
            get
            {
                return _choicedProSubjectModels;
            }
            set
            {
                _choicedProSubjectModels = value;
            }
        }

        private ProgramDiagram _programDiagram;
        public RelayCommand AddCommand { get; set; }
        public RelayCommand SortCommand { get; set; }

        public AutoScheduleViewModel(StudentModel studentModel)
        {
            AddCommand = new RelayCommand(OnAddSubject, () => true);
            SortCommand = new RelayCommand(OnSort, () => true);
            _studentModel = studentModel;
            GetProgramDiagram(studentModel.StudentInfo.SpecialString);
        }

        private void OnSort(object obj)
        {
            throw new NotImplementedException();
        }

        private void OnAddSubject(object obj)
        {
            if (_selectedProSubject != null)
            {
                _choicedProSubjectModels.Add(_selectedProSubject);
            }
        }

        private void GetProgramDiagram(string specialString)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync(specialString);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressValue = 0;
            _programDiagram = e.Result as ProgramDiagram;
            List<ProgramSubject> programSubjects = _programDiagram.GetAllProSubject();
            foreach(ProgramSubject subject in programSubjects)
            {
                ProgramSubjectModel proSubjectModel = new ProgramSubjectModel(subject, ref _programDiagram);
                _programSubjectModels.Add(proSubjectModel);
            }
            Analyze();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Analyze()
        {
            
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker backgroundWorker = sender as BackgroundWorker;
            string specialString = (string)e.Argument;
            backgroundWorker.ReportProgress(10);
            ProgramDiagramCrawler programDiagramCrawler = new ProgramDiagramCrawler(null, specialString, backgroundWorker);
            ProgramDiagram result = programDiagramCrawler.ToProgramDiagram();
            backgroundWorker.ReportProgress(100);
            e.Result = result;
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressValue = e.ProgressPercentage;
        }
    }
}
