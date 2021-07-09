using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.DialogViews;
using cs4rsa.Dialogs.Implements;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using LightMessageBus;
using cs4rsa.Messages;

namespace cs4rsa.ViewModels
{
    class AutoScheduleViewModel : NotifyPropertyChangedBase
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
                AddCommand.RaiseCanExecuteChanged();
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

        private ProgramSubjectModel _selectedProSubjectInChoiced;
        public ProgramSubjectModel SelectedProSubjectInChoiced
        {
            get
            {
                return _selectedProSubjectInChoiced;
            }
            set
            {
                _selectedProSubjectInChoiced = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<CombinationModel> _combinationModels = new ObservableCollection<CombinationModel>();
        public ObservableCollection<CombinationModel> CombinationModels
        {
            get
            {
                return _combinationModels;
            }
            set
            {
                _combinationModels = value;
            }
        }

        private CombinationModel _selectedCombinationModel;
        public CombinationModel SelectedCombinationModel
        {
            get
            {
                return _selectedCombinationModel;
            }
            set
            {
                _selectedCombinationModel = value;
                RaisePropertyChanged();
            }
        }

        private int _combinationCount = 0;
        public int CombinationCount
        {
            get
            {
                return _combinationCount;
            }
            set
            {
                _combinationCount = value;
                RaisePropertyChanged();
            }
        }

        private ProgramDiagram _programDiagram;
        private IMessageBox _messageBox;
        private Window _window;

        public RelayCommand AddCommand { get; set; }
        public RelayCommand SortCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand GotoCourseCommand { get; set; }
        public RelayCommand WatchDetailCommand { get; set; }
        public RelayCommand ShowOnSimuCommand { get; set; }

        public AutoScheduleViewModel(StudentModel studentModel, IMessageBox messageBox, Window window)
        {
            AddCommand = new RelayCommand(OnAddSubject, CanAdd);
            SortCommand = new RelayCommand(OnSort, CanSort);
            DeleteCommand = new RelayCommand(OnDelete);
            GotoCourseCommand = new RelayCommand(OnGoToCourse);
            WatchDetailCommand = new RelayCommand(OnWatchDetail);
            ShowOnSimuCommand = new RelayCommand(OnShowOnSimu, CanShowOnSimu);
            _studentModel = studentModel;
            _messageBox = messageBox;
            _window = window;
        }

        public void LoadProgramSubject()
        {
            ProSubjectLoadWindow proSubjectLoadWindow = new ProSubjectLoadWindow();
            ProSubjectLoadViewModel proSubjectLoadViewModel = new ProSubjectLoadViewModel(_studentModel.StudentInfo.SpecialString);
            ProSubjectLoadResult result = DialogService<ProSubjectLoadResult>.OpenDialog(proSubjectLoadViewModel, proSubjectLoadWindow, _window); ;

            _programDiagram = result.ProgramDiagram;
            List<ProgramSubject> programSubjects = _programDiagram.GetAllProSubject();
            foreach (ProgramSubject subject in programSubjects)
            {
                ProgramSubjectModel proSubjectModel = new ProgramSubjectModel(subject, ref _programDiagram);
                _programSubjectModels.Add(proSubjectModel);
            }
        }

        private bool CanAdd()
        {
            if (_selectedProSubject == null || _choicedProSubjectModels.Contains(_selectedProSubject))
                return false;
            return _selectedProSubject.IsCanChoice;
        }

        private bool CanShowOnSimu()
        {
            return true;
        }

        private void OnShowOnSimu(object obj)
        {
            ShowOnSimuMessage showOnSimuMessage = new ShowOnSimuMessage(_selectedCombinationModel);
            MessageBus.Default.Publish<ShowOnSimuMessage>(showOnSimuMessage);
        }

        private bool CanSort()
        {
            return _choicedProSubjectModels.Count > 0;
        }

        private void OnWatchDetail(object obj)
        {
            _messageBox.ShowMessage("Mở xem chi tiết");
        }

        private void OnGoToCourse(object obj)
        {
            if (_selectedProSubjectInChoiced != null)
            {
                string courseId = _selectedProSubjectInChoiced.CourseId;
                string semesterValue = HomeCourseSearch.GetInstance().CurrentSemesterValue;
                string url = $@"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid={courseId}&timespan={semesterValue}&t=s";
                Process.Start(url);
            }
        }

        private void OnDelete(object obj)
        {
            _choicedProSubjectModels.Remove(_selectedProSubjectInChoiced);
            SortCommand.RaiseCanExecuteChanged();
        }

        private void OnSort(object obj)
        {
            AutoSortDialogWindow autoSortDialogWindow = new AutoSortDialogWindow();
            AutoSortViewModel autoSortViewModel = new AutoSortViewModel(_choicedProSubjectModels.ToList());
            AutoSortResult result = DialogService<AutoSortResult>.OpenDialog(autoSortViewModel, autoSortDialogWindow, obj as Window);
            List<CombinationModel> combinationModels = result.ClassGroupModelCombinations
                .Select(item => new CombinationModel(result.SubjectModels,item))
                .ToList();
            foreach (CombinationModel combination in combinationModels)
            {
                if (combination.IsValid() && !combination.IsHaveTimeConflicts())
                    _combinationModels.Add(combination);
            }
            UpdateCombinationCount();
        }

        private void UpdateCombinationCount()
        {
            CombinationCount = _combinationModels.Count;
        }

        private void OnAddSubject(object obj)
        {
            if (_selectedProSubject != null)
            {
                _choicedProSubjectModels.Add(_selectedProSubject);
                SortCommand.RaiseCanExecuteChanged();
                AddCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
