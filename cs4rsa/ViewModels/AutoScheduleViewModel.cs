using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.DialogViews;
using cs4rsa.Dialogs.Implements;
using cs4rsa.Helpers;
using cs4rsa.Messages;
using cs4rsa.Models;
using cs4rsa.Views;
using LightMessageBus;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

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

        private ObservableCollection<SubjectModel> _subjectModels = new ObservableCollection<SubjectModel>();
        public ObservableCollection<SubjectModel> SubjectModels
        {
            get { return _subjectModels; }
            set { _subjectModels = value; }
        }

        private List<ClassGroupModel> _classGroupModelContainer = new List<ClassGroupModel>();

        public List<ClassGroupModel> ClassGroupModelContainer
        {
            get { return _classGroupModelContainer; }
            set { _classGroupModelContainer = value; }
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

        public RelayCommand ChoiceAccountCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand CannotAddReasonCommand { get; set; }
        public RelayCommand SubjectDownloadCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
        public RelayCommand GotoCourseCommand { get; set; }
        public RelayCommand WatchDetailCommand { get; set; }
        public RelayCommand GenCommand { get; set; }
        public RelayCommand ShowOnSimuCommand { get; set; }
        public RelayCommand OpenInNewWindowCommand { get; set; }


        public AutoScheduleViewModel()
        {
            ChoiceAccountCommand = new RelayCommand(OnChoiceAccountCommand);
            AddCommand = new RelayCommand(OnAddSubject, CanAdd);
            SubjectDownloadCommand = new RelayCommand(OnDownload, CanDownload);
            DeleteCommand = new RelayCommand(OnDelete);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, CanDeleteAll);
            GotoCourseCommand = new RelayCommand(OnGoToCourse);
            WatchDetailCommand = new RelayCommand(OnWatchDetail);
            GenCommand = new RelayCommand(OnGen);
            ShowOnSimuCommand = new RelayCommand(OnShowOnSimu, CanShowOnSimu);
            OpenInNewWindowCommand = new RelayCommand(OnOpenInNewWindow);
            _isRemoveClassGroupInvalid = true;
            _isAllowConflict = false;
        }

        private void UpdateClassGroupModelContainer()
        {
            foreach (SubjectModel subjectModel in _subjectModels)
            {
                _classGroupModelContainer.AddRange(subjectModel.ClassGroupModels);
            }
        }

        private void OnGen(object obj)
        {

            // Gen cấu hình đầu tiên là tiền đề là các cấu hình tiếp theo.
            int subjectModelsCount = _subjectModels.Count();
            int countGen = 0;
            int countValidGen = 0;
            List<ClassGroupModel> currentCombi = new List<ClassGroupModel>();
            CombinationModel combinationModel;
            List<ClassGroupModel> nextCombi;

            if (Gen<ClassGroupModel>.IsLastCombination(currentCombi, subjectModelsCount, _classGroupModelContainer)
                && subjectModelsCount > 0)
            {
                MessageBus.Default.Publish<Cs4rsaSnackbarMessage>(new Cs4rsaSnackbarMessage("Đã đến cấu hình cuối"));
                return;
            }

            do
            {
                if (IsHaveFirstCombiOrMore() || currentCombi.Count() > 0)
                {
                    if (countGen<=countValidGen)
                        currentCombi = _combinationModels.Last().ClassGroupModels;
                    nextCombi = Gen<ClassGroupModel>.GenNext(currentCombi, subjectModelsCount, _classGroupModelContainer);
                    combinationModel = new CombinationModel(_subjectModels.ToList(), nextCombi);
                }
                else
                {
                    nextCombi = Gen<ClassGroupModel>.GenFirst(subjectModelsCount, _classGroupModelContainer);
                    combinationModel = new CombinationModel(_subjectModels.ToList(), nextCombi);
                }

                countGen++;
                currentCombi = nextCombi;
            }
            while (!combinationModel.IsValid());

            _combinationModels.Add(combinationModel);
            countValidGen++;
        }

        /// <summary>
        /// Kiểm tra xem đã có ít nhất một Combi hay chưa.
        /// </summary>
        /// <returns></returns>
        private bool IsHaveFirstCombiOrMore()
        {
            return _combinationModels.Count > 0;
        }

        private void OnOpenInNewWindow(object obj)
        {
            CombinationContainerWindow combinationContainerWindow = new CombinationContainerWindow(_combinationModels.ToList(), this);
            combinationContainerWindow.Topmost = true;
            combinationContainerWindow.Show();
        }

        private void OnChoiceAccountCommand(object obj)
        {
            LoginUC loginUC = new LoginUC();
            LoginDialogViewModel vm = loginUC.DataContext as LoginDialogViewModel;
            vm.CloseDialogCallback = CloseDialogAndHandleLoginResult;
            (App.Current.MainWindow.DataContext as MainViewModel).OpenDialog(loginUC);
        }

        private void CloseDialogAndHandleLoginResult(LoginResult loginResult)
        {
            (App.Current.MainWindow.DataContext as MainViewModel).CloseDialog();
            if (loginResult != null)
            {
                _programSubjectModels.Clear();
                _choicedProSubjectModels.Clear();
                _combinationModels.Clear();
                StudentModel = loginResult.StudentModel;
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
            DeleteAllCommand.RaiseCanExecuteChanged();
        }

        public void LoadProgramSubject()
        {
            ProgramSubjectLoadUC programSubjectLoadUC = new ProgramSubjectLoadUC();
            ProSubjectLoadViewModel vm = programSubjectLoadUC.DataContext as ProSubjectLoadViewModel;
            vm.SpecialString = _studentModel.StudentInfo.SpecialString;
            vm.CloseDialogCallback = CloseDialogAndHandleProSubjectLoadResult;
            vm.Load();
            (App.Current.MainWindow.DataContext as MainViewModel).OpenDialog(programSubjectLoadUC);
        }

        private void CloseDialogAndHandleProSubjectLoadResult(ProSubjectLoadResult result)
        {
            (App.Current.MainWindow.DataContext as MainViewModel).CloseDialog();
            if (result != null)
            {
                _programDiagram = result.ProgramDiagram;
                List<ProgramSubject> programSubjects = _programDiagram.GetAllProSubject();
                _programSubjectModels.Clear();
                foreach (ProgramSubject subject in programSubjects)
                {
                    ProgramSubjectModel proSubjectModel = new ProgramSubjectModel(subject, _programDiagram);
                    _programSubjectModels.Add(proSubjectModel);
                }
            }
        }

        private bool CanAdd()
        {
            return true;
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

        private bool CanDownload()
        {
            return _choicedProSubjectModels.Count > 0;
        }

        private void OnWatchDetail(object obj)
        {

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
            SubjectDownloadCommand.RaiseCanExecuteChanged();
            AddCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
            UpdateChoicedCount();
            UpdateCreditCount();
        }

        private void OnDownload(object obj)
        {
            AutoSortSubjectLoadUC autoSortSubjectLoadUC = new AutoSortSubjectLoadUC();
            AutoSortSubjectLoadVM vm = autoSortSubjectLoadUC.DataContext as AutoSortSubjectLoadVM;
            vm.CloseDialogCallback += CloseDialogAndHandleDownloadResult;
            vm.ProgramSubjectModels = ChoicedProSubjectModels.ToList();
            vm.Download();
            (App.Current.MainWindow.DataContext as MainViewModel).OpenDialog(autoSortSubjectLoadUC);
        }

        private void CloseDialogAndHandleDownloadResult(IEnumerable<SubjectModel> subjectModels)
        {
            (App.Current.MainWindow.DataContext as MainViewModel).CloseDialog();
            foreach (SubjectModel subjectModel in subjectModels)
            {
                _subjectModels.Add(subjectModel);
            }
            UpdateClassGroupModelContainer();
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
            SubjectDownloadCommand.RaiseCanExecuteChanged();
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
