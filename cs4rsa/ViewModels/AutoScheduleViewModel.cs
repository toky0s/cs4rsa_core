using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Dialogs.DialogResults;
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

        public ObservableCollection<ProgramFolderModel> ProgramFolderModels { get; set; }
        public ObservableCollection<ProgramSubjectModel> ChoicedProSubjectModels { get; set; }
        public ObservableCollection<CombinationModel> CombinationModels { get; set; }
        public ObservableCollection<SubjectModel> SubjectModels { get; set; }
        public List<ClassGroupModel> ClassGroupModelContainer { get; set; }

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
                OnPropertyChanged();
                AddCommand.RaiseCanExecuteChanged();
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
            ProgramFolderModels = new ObservableCollection<ProgramFolderModel>();
            ChoicedProSubjectModels = new ObservableCollection<ProgramSubjectModel>();
            CombinationModels = new ObservableCollection<CombinationModel>();
            SubjectModels = new ObservableCollection<SubjectModel>();
            ClassGroupModelContainer = new List<ClassGroupModel>();

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
            foreach (SubjectModel subjectModel in SubjectModels)
            {
                ClassGroupModelContainer.AddRange(subjectModel.ClassGroupModels);
            }
        }

        private void OnGen(object obj)
        {
            // Gen cấu hình đầu tiên là tiền đề là các cấu hình tiếp theo.
            int subjectModelsCount = SubjectModels.Count();
            int countGen = 0;
            int countValidGen = 0;
            List<ClassGroupModel> currentCombi = new List<ClassGroupModel>();
            CombinationModel combinationModel;
            List<ClassGroupModel> nextCombi;

            do
            {
                if (IsHaveFirstCombiOrMore() || currentCombi.Count() > 0)
                {
                    if (countGen <= countValidGen)
                        currentCombi = CombinationModels.Last().ClassGroupModels;
                    if (Gen<ClassGroupModel>.IsLastCombination(currentCombi, subjectModelsCount, ClassGroupModelContainer))
                    {
                        MessageBus.Default.Publish<Cs4rsaSnackbarMessage>(new Cs4rsaSnackbarMessage("Đã đến cấu hình cuối"));
                        return;
                    }
                    nextCombi = Gen<ClassGroupModel>.GenNext(currentCombi, subjectModelsCount, ClassGroupModelContainer);
                    combinationModel = new CombinationModel(SubjectModels.ToList(), nextCombi);
                }
                else
                {
                    nextCombi = Gen<ClassGroupModel>.GenFirst(subjectModelsCount, ClassGroupModelContainer);
                    combinationModel = new CombinationModel(SubjectModels.ToList(), nextCombi);
                }

                countGen++;
                currentCombi = new List<ClassGroupModel>(nextCombi);
            }
            while (!combinationModel.IsValid());

            CombinationModels.Add(combinationModel);
            countValidGen++;
        }

        /// <summary>
        /// Kiểm tra xem đã có ít nhất một Combi hay chưa.
        /// </summary>
        /// <returns></returns>
        private bool IsHaveFirstCombiOrMore()
        {
            return CombinationModels.Count > 0;
        }

        private void OnOpenInNewWindow(object obj)
        {
            CombinationContainerWindow combinationContainerWindow = new CombinationContainerWindow(CombinationModels.ToList(), this);
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
                ChoicedProSubjectModels.Clear();
                CombinationModels.Clear();
                StudentModel = loginResult.StudentModel;
                LoadProgramSubject();
            }
        }

        private bool CanDeleteAll()
        {
            return ChoicedProSubjectModels.Count > 0;
        }

        private void OnDeleteAll(object obj)
        {
            ChoicedProSubjectModels.Clear();
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
                ProgramFolderModels.Clear();
                foreach (ProgramFolder programFolder in _programDiagram.ProgramFolders)
                {
                    ProgramFolderModel programFolderModel = new ProgramFolderModel(programFolder);
                    ProgramFolderModels.Add(programFolderModel);
                }
            }
        }

        private bool CanAdd()
        {
            return true;
            return _selectedProSubject != null
                && _selectedProSubject.IsAvaiable
                && _selectedProSubject.IsDone == false
                && !ChoicedProSubjectModels.Contains(_selectedProSubject);
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
            return ChoicedProSubjectModels.Count > 0;
        }

        private void OnWatchDetail(object obj)
        {
            ProSubjectDetailUC proSubjectDetailUC = new ProSubjectDetailUC();
            ProSubjectDetailVM vm = proSubjectDetailUC.DataContext as ProSubjectDetailVM;
            vm.ProgramDiagram = _programDiagram;
            vm.ProgramSubjectModel = _selectedProSubject;
            vm.AddCallback = OnAddSubject;
            vm.CloseDialogCallback = (App.Current.MainWindow.DataContext as MainViewModel).CloseDialog;
            vm.LoadPreProSubjectModels();
            vm.LoadParProSubjectModels();
            (App.Current.MainWindow.DataContext as MainViewModel).OpenDialog(proSubjectDetailUC);
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
            ChoicedProSubjectModels.Remove(_selectedProSubjectInChoiced);
            SubjectDownloadCommand.RaiseCanExecuteChanged();
            AddCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
            UpdateChoicedCount();
            UpdateCreditCount();
        }

        /// <summary>
        /// Phương thức này không chỉ thực hiện download mà nó còn thực hiện đồng bộ
        /// giữa đã chọn và đã tải nhằm tối ưu tốc độ, thay vì việc thực hiện tải lại
        /// rất mất thời gian.
        /// </summary>
        /// <param name="obj"></param>
        private void OnDownload(object obj)
        {
            List<string> choiceSubjectCodes = ChoicedProSubjectModels.Select(item => item.ProgramSubject.SubjectCode).ToList();
            List<string> wereDownloadedSubjectCodes = SubjectModels.Select(item => item.SubjectCode).ToList();
            
            // to do: Xác định subject cần xoá để đồng bộ tập hợp.
            List<string> needDeleteNames = wereDownloadedSubjectCodes.Except(choiceSubjectCodes).ToList();
            needDeleteNames.ForEach(item => RemoveSubjectModelWithNameInDownloaded(item));

            // to do: Xác định subject nào cần tải dựa vào phép trừ tập hợp
            List<string> needDownloadNames = choiceSubjectCodes.Except(wereDownloadedSubjectCodes).ToList();
            List<ProgramSubjectModel> needDownload = ChoicedProSubjectModels.Where(item => needDownloadNames.Contains(item.ProgramSubject.SubjectCode)).ToList();

            if (needDownload.Count > 0)
            {
                AutoSortSubjectLoadUC autoSortSubjectLoadUC = new AutoSortSubjectLoadUC();
                AutoSortSubjectLoadVM vm = autoSortSubjectLoadUC.DataContext as AutoSortSubjectLoadVM;
                vm.CloseDialogCallback += CloseDialogAndHandleDownloadResult;
                vm.ProgramSubjectModels = needDownload;
                vm.Download();
                (App.Current.MainWindow.DataContext as MainViewModel).OpenDialog(autoSortSubjectLoadUC);
            }
        }

        private void RemoveSubjectModelWithNameInDownloaded(string name)
        {
            foreach (SubjectModel item in SubjectModels)
            {
                if (item.SubjectCode == name)
                {
                    SubjectModels.Remove(item);
                    break;
                }
            }
        }

        private void CloseDialogAndHandleDownloadResult(IEnumerable<SubjectModel> subjectModels)
        {
            (App.Current.MainWindow.DataContext as MainViewModel).CloseDialog();
            foreach (SubjectModel subjectModel in subjectModels)
            {
                SubjectModels.Add(subjectModel);
            }
            UpdateClassGroupModelContainer();
        }

        private void UpdateCombinationCount()
        {
            CombinationCount = CombinationModels.Count;
        }

        private void OnAddSubject(object obj)
        {
            if (_selectedProSubject != null)
            {
                ChoicedProSubjectModels.Add(SelectedProSubject);
                SubjectDownloadCommand.RaiseCanExecuteChanged();
                AddCommand.RaiseCanExecuteChanged();
                DeleteAllCommand.RaiseCanExecuteChanged();
                UpdateCreditCount();
                UpdateChoicedCount();
            }
        }

        private void UpdateChoicedCount()
        {
            ChoicedCount = ChoicedProSubjectModels.Count;
        }

        private void UpdateCreditCount()
        {
            CreditCount = 0;
            foreach (ProgramSubjectModel subjectModel in ChoicedProSubjectModels)
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
                foreach (ProgramSubjectModel subjectModel in ChoicedProSubjectModels)
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
