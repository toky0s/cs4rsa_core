using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.DialogViews;
using cs4rsa.Dialogs.Implements;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Messages;
using cs4rsa.Models;
using cs4rsa.Views;
using LightMessageBus;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace cs4rsa.ViewModels
{
    class AutoScheduleViewModel : ViewModelBase
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                ShowOnSimuCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        private bool _isRemoveClassGroupInvalid;
        public bool IsRemoveClassGroupInvalid
        {
            get { return _isRemoveClassGroupInvalid; }
            set { _isRemoveClassGroupInvalid = value; OnPropertyChanged(); }
        }

        private bool _isAllowConflict;
        public bool IsAllowConflict
        {
            get { return _isAllowConflict; }
            set { _isAllowConflict = value; OnPropertyChanged(); }
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
                OnPropertyChanged();
            }
        }

        private int _choicedCount = 0;
        public int ChoicedCount
        {
            get
            {
                return _choicedCount;
            }
            set
            {
                _choicedCount = value;
                OnPropertyChanged();
            }
        }

        private int _creditCount = 0;
        public int CreditCount
        {
            get
            {
                return _creditCount;
            }
            set
            {
                _creditCount = value;
                OnPropertyChanged();
            }
        }

        private ProgramDiagram _programDiagram;
        private IMessageBox _messageBox;

        public RelayCommand ChoiceAccountCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand CannotAddReasonCommand { get; set; }
        public RelayCommand SortCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
        public RelayCommand GotoCourseCommand { get; set; }
        public RelayCommand WatchDetailCommand { get; set; }
        public RelayCommand ShowOnSimuCommand { get; set; }
        public RelayCommand OpenInNewWindowCommand { get; set; }


        public AutoScheduleViewModel(IMessageBox messageBox)
        {
            ChoiceAccountCommand = new RelayCommand(OnChoiceAccountCommand);
            AddCommand = new RelayCommand(OnAddSubject, CanAdd);
            SortCommand = new RelayCommand(OnSort, CanSort);
            DeleteCommand = new RelayCommand(OnDelete);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, CanDeleteAll);
            GotoCourseCommand = new RelayCommand(OnGoToCourse);
            WatchDetailCommand = new RelayCommand(OnWatchDetail);
            ShowOnSimuCommand = new RelayCommand(OnShowOnSimu, CanShowOnSimu);
            OpenInNewWindowCommand = new RelayCommand(OnOpenInNewWindow);
            _messageBox = messageBox;
            _isRemoveClassGroupInvalid = true;
            _isAllowConflict = false;
        }

        private void OnOpenInNewWindow(object obj)
        {
            CombinationContainerWindow combinationContainerWindow = new CombinationContainerWindow(_combinationModels.ToList(), this);
            combinationContainerWindow.Topmost = true;
            combinationContainerWindow.Show();
        }

        private void OnChoiceAccountCommand(object obj)
        {
            LoginDialogWindow loginWindow = new LoginDialogWindow();
            LoginDialogViewModel loginDialogViewModel = new LoginDialogViewModel();
            LoginResult result = DialogService<LoginResult>.OpenDialog(loginDialogViewModel, loginWindow, obj as Window);
            if (result != null)
            {
                _programSubjectModels.Clear();
                _choicedProSubjectModels.Clear();
                _combinationModels.Clear();
                StudentModel = result.StudentModel;
                LoadProgramSubject();
            }
        }

        private bool CanDeleteAll()
        {
            return _choicedProSubjectModels.Count > 0;
        }

        private void OnDeleteAll(object obj)
        {
            _choicedProSubjectModels.Clear();
            SortCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
        }

        public void LoadProgramSubject()
        {
            ProSubjectLoadWindow proSubjectLoadWindow = new ProSubjectLoadWindow();
            ProSubjectLoadViewModel proSubjectLoadViewModel = new ProSubjectLoadViewModel(_studentModel.StudentInfo.SpecialString);
            ProSubjectLoadResult result = DialogService<ProSubjectLoadResult>.OpenDialog(proSubjectLoadViewModel, proSubjectLoadWindow, null); ;

            if (result != null)
            {
                _programDiagram = result.ProgramDiagram;
                List<ProgramSubject> programSubjects = _programDiagram.GetAllProSubject();
                _programSubjectModels.Clear();
                foreach (ProgramSubject subject in programSubjects)
                {
                    ProgramSubjectModel proSubjectModel = new ProgramSubjectModel(subject, ref _programDiagram);
                    _programSubjectModels.Add(proSubjectModel);
                }
            }
        }

        private bool CanAdd()
        {
            if (_selectedProSubject == null
                || _choicedProSubjectModels.Contains(_selectedProSubject)
                || IsFolderWillCompleteInFuture(_programDiagram.GetFolder(_selectedProSubject.FolderName)))
                return false;
            return _selectedProSubject.IsCanChoice;
        }

        private bool CanShowOnSimu()
        {
            if (_selectedCombinationModel == null) return false;
            return !_selectedCombinationModel.HaveAClassGroupHaveNotSchedule &&
                    !_selectedCombinationModel.HaveAClassGroupHaveZeroEmptySeat;
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
            AddCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
            UpdateChoicedCount();
            UpdateCreditCount();
        }

        /// <summary>
        /// Thực hiện tại các Subject, lấy ra các ClassGroupModel, chạy thuật toán sinh tổ hợp chính tắc.
        /// Kết quả cuối cùng là một danh sách các CombinationModel đại diện cho một cách sắp xếp.
        /// </summary>
        /// <param name="obj"></param>
        private void OnSort(object obj)
        {
            AutoSortDialogWindow autoSortDialogWindow = new AutoSortDialogWindow();
            AutoSortViewModel autoSortViewModel = new AutoSortViewModel(_choicedProSubjectModels.ToList(),
                                                                        _isRemoveClassGroupInvalid);
            AutoSortResult result = DialogService<AutoSortResult>.OpenDialog(autoSortViewModel, autoSortDialogWindow, obj as Window);
            _combinationModels.Clear();
            List<CombinationModel> combinationModels = result.ClassGroupModelCombinations
                .Select(item => new CombinationModel(result.SubjectModels, item))
                .ToList();
            // filter
            foreach (CombinationModel combination in combinationModels)
            {
                if (_isAllowConflict)
                {
                    if (combination.IsValid())
                        _combinationModels.Add(combination);
                }
                else
                {
                    if (combination.IsValid() && !combination.IsHaveTimeConflicts() && !combination.IsHavePlaceConflicts())
                        _combinationModels.Add(combination);
                }
            }
            UpdateCombinationCount();

            string message = $"Hoàn tất tìm kiếm và sắp xếp";
            MessageBus.Default.Publish<Cs4rsaSnackbarMessage>(new Cs4rsaSnackbarMessage(message));
        }

        private void UpdateCombinationCount()
        {
            CombinationCount = _combinationModels.Count;
        }

        private void OnAddSubject(object obj)
        {
            if (obj != null)
            {
                ProgramSubjectModel programSubjectModel = obj as ProgramSubjectModel;
                _choicedProSubjectModels.Add(programSubjectModel);
            }
            else
            {
                if (_selectedProSubject != null)
                    _choicedProSubjectModels.Add(_selectedProSubject);
            }
            SortCommand.RaiseCanExecuteChanged();
            AddCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
            UpdateCreditCount();
            UpdateChoicedCount();
        }

        private void UpdateChoicedCount()
        {
            ChoicedCount = _choicedProSubjectModels.Count;
        }

        private void UpdateCreditCount()
        {
            CreditCount = 0;
            foreach (ProgramSubjectModel subjectModel in _choicedProSubjectModels)
                CreditCount += subjectModel.StudyUnit;
        }


        /// <summary>
        /// Kiểm tra xem Folder được truyền vào sẽ hoàn thành trong tương lai hay không
        /// nếu người dùng chọn học những môn thuộc folder đó.
        /// </summary>
        /// <param name="programFolder"></param>
        /// <returns></returns>
        private bool IsFolderWillCompleteInFuture(ProgramFolder programFolder)
        {
            if (programFolder.StudyMode == StudyMode.AllowSelection)
            {
                int needLearn = programFolder.NeedLearnToComplete();
                int hasChoiced = 0;
                foreach (ProgramSubjectModel subjectModel in _choicedProSubjectModels)
                {
                    if (subjectModel.ChildOfNode == programFolder.Id)
                        hasChoiced++;
                }
                return (needLearn - hasChoiced) == 0;
            }
            return false;
        }
    }
}
