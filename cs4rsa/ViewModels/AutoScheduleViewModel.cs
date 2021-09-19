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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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

        public ObservableCollection<ProgramFolderModel> ProgramFolderModels { get; set; }
        public ObservableCollection<ProgramSubjectModel> ChoicedProSubjectModels { get; set; }

        // Đây là nơi chứa các combination model, người dùng sẽ nhìn thấy cái này.
        public ObservableCollection<CombinationModel> CombinationModels { get; set; }
        // Đây là nơi chứa các cấu hình class group model trước khi được gen lên thành combination model
        // để hiển thị cho người dùng.
        private List<List<ClassGroupModel>> _hideActualClassGroupModels { get; set; }
        public ObservableCollection<SubjectModel> SubjectModels { get; set; }

        // Đây là nơi chứa tất cả các class group model dùng để render.
        public List<ClassGroupModel> ClassGroupModelsContainer { get; set; }


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
            get => _selectedProSubjectInChoiced;
            set
            {
                _selectedProSubjectInChoiced = value;
                OnPropertyChanged();
            }
        }

        private CombinationModel _selectedCombinationModel;
        public CombinationModel SelectedCombinationModel
        {
            get => _selectedCombinationModel;
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
            get => _isRemoveClassGroupInvalid;
            set { _isRemoveClassGroupInvalid = value; OnPropertyChanged(); }
        }

        private bool _isAllowConflict;
        public bool IsAllowConflict
        {
            get => _isAllowConflict;
            set { _isAllowConflict = value; OnPropertyChanged(); }
        }

        private int _combinationCount = 0;
        public int CombinationCount
        {
            get => _combinationCount;
            set
            {
                _combinationCount = value;
                OnPropertyChanged();
            }
        }

        private int _choicedCount = 0;
        public int ChoicedCount
        {
            get => _choicedCount;
            set
            {
                _choicedCount = value;
                OnPropertyChanged();
            }
        }

        private int _creditCount = 0;
        public int CreditCount
        {
            get => _creditCount;
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
            ClassGroupModelsContainer = new List<ClassGroupModel>();
            _hideActualClassGroupModels = new List<List<ClassGroupModel>>();

            ChoiceAccountCommand = new RelayCommand(OnChoiceAccountCommand);
            AddCommand = new RelayCommand(OnAddSubject, CanAdd);
            SubjectDownloadCommand = new RelayCommand(OnDownload, CanDownload);
            DeleteCommand = new RelayCommand(OnDelete);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, CanDeleteAll);
            GotoCourseCommand = new RelayCommand(OnGoToCourse);
            WatchDetailCommand = new RelayCommand(OnWatchDetail);
            GenCommand = new RelayCommand(OnStartGen);
            ShowOnSimuCommand = new RelayCommand(OnShowOnSimu, CanShowOnSimu);
            OpenInNewWindowCommand = new RelayCommand(OnOpenInNewWindow);
            _isRemoveClassGroupInvalid = true;
            _isAllowConflict = false;
        }

        private void OnStartGen(object obj)
        {
            while (true)
            {
                List<ClassGroupModel> generatedCombination = new List<ClassGroupModel>();
                OnGen().ForEach(item => generatedCombination.Add(item));
                if (generatedCombination != null)
                {
                    // Sao chép lại list gen
                    List<ClassGroupModel> CopyList = new List<ClassGroupModel>();
                    foreach (ClassGroupModel item in generatedCombination)
                    {
                        ClassGroupModel copy = (ClassGroupModel) item.Clone();
                        CopyList.Add(copy);
                    }

                    _hideActualClassGroupModels.Add(CopyList);
                    CombinationModel combinationModel = new CombinationModel(SubjectModels.ToList(), generatedCombination);
                    if (combinationModel.IsValid())
                    {
                        CombinationModels.Add(combinationModel);
                        break;
                    }
                    continue;
                }
                else
                {
                    MessageBus.Default.Publish(new Cs4rsaSnackbarMessage("Đã đến cấu hình cuối"));
                    break;
                }
            }
        }



        private void UpdateClassGroupModelContainer()
        {
            foreach (SubjectModel subjectModel in SubjectModels)
            {
                ClassGroupModelsContainer.AddRange(subjectModel.ClassGroupModels);
            }
        }

        private List<ClassGroupModel> OnGen()
        {
            if (IsHaveCombination())
            {
                if (Gen<ClassGroupModel>.IsLastCombination(
                    _hideActualClassGroupModels.Last(),
                    SubjectModels.Count,
                    ClassGroupModelsContainer))
                {
                    return null;
                }
                else
                {
                    return Gen<ClassGroupModel>.GenNext(_hideActualClassGroupModels.Last(), SubjectModels.Count, ClassGroupModelsContainer);
                }
            }
            else
            {
                return Gen<ClassGroupModel>.GenFirst(SubjectModels.Count, ClassGroupModelsContainer);
            }
        }

        /// <summary>
        /// Kiểm tra xem đã có ít nhất một cấu hình
        /// bên trong danh sách tạm hay chưa.
        /// </summary>
        /// <returns></returns>
        private bool IsHaveCombination()
        {
            return _hideActualClassGroupModels.Count() > 0;
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
            //return _selectedProSubject != null
            //    && _selectedProSubject.IsAvaiable
            //    && _selectedProSubject.IsDone == false
            //    && !ChoicedProSubjectModels.Contains(_selectedProSubject);
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
            vm.CloseDialogCallback = (Application.Current.MainWindow.DataContext as MainViewModel).CloseDialog;
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
            CombinationModels.Clear();
            List<string> choiceSubjectCodes = ChoicedProSubjectModels.Select(item => item.ProgramSubject.SubjectCode).ToList();
            List<string> wereDownloadedSubjectCodes = SubjectModels.Select(item => item.SubjectCode).ToList();

            // khi cần tải == 0
            if (choiceSubjectCodes.Count == 0)
                wereDownloadedSubjectCodes.Clear();

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
                (Application.Current.MainWindow.DataContext as MainViewModel).OpenDialog(autoSortSubjectLoadUC);
            }
        }

        /// <summary>
        /// Loại bỏ các class group còn 0 chỗ ngồi nhằm tối ưu hoá hiệu suất sắp xếp.
        /// </summary>
        private void CleanClassGroup()
        {
            foreach (SubjectModel subjectModel in SubjectModels)
            {
                subjectModel.ClassGroupModels = subjectModel.ClassGroupModels.Where(item => item.EmptySeat > 0).ToList();
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
            (Application.Current.MainWindow.DataContext as MainViewModel).CloseDialog();
            foreach (SubjectModel subjectModel in subjectModels)
            {
                SubjectModels.Add(subjectModel);
            }
            UpdateClassGroupModelContainer();
            // filter again
            CleanClassGroup();
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
            {
                CreditCount += subjectModel.StudyUnit;
            }
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
