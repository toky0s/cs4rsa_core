using CourseSearchService.Crawlers.Interfaces;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.DialogViews;
using cs4rsa_core.Dialogs.Implements;
using cs4rsa_core.Interfaces;
using cs4rsa_core.Messages;
using cs4rsa_core.Models;
using cs4rsa_core.Utils;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using CurriculumCrawlerService.Crawlers.Interfaces;
using HelperService;
using LightMessageBus;
using LightMessageBus.Interfaces;
using Microsoft.Toolkit.Mvvm.Input;
using ProgramSubjectCrawlerService.Crawlers;
using ProgramSubjectCrawlerService.DataTypes;
using ProgramSubjectCrawlerService.DataTypes.Enums;
using SubjectCrawlService1.Crawlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace cs4rsa_core.ViewModels
{
    public class AutoScheduleViewModel : ViewModelBase, IMessageHandler<ExitLoginMessage>, IMessageHandler<AddCombinationMessage>
    {
        #region Properties
        private bool _isRemoveClassGroupInvalid;
        public bool IsRemoveClassGroupInvalid { 
            get => _isRemoveClassGroupInvalid;
            set
            {
                _isRemoveClassGroupInvalid = value;
                OnPropertyChanged();
            }
        }
        private string _visibility = "Hidden";
        public string Visibility
        {
            get { return _visibility; }
            set { _visibility = value; OnPropertyChanged(); }
        }

        private Student _student;
        public Student Student
        {
            get => _student;
            set
            {
                _student = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ProgramFolderModel> ProgramFolderModels { get; set; }
        public ObservableCollection<ProgramSubjectModel> ChoicedProSubjectModels { get; set; }
        public List<List<ClassGroupModel>> _filteredClassGroupModels;
        public List<List<ClassGroupModel>> _classGroupModelsOfClass;

        // Đây là nơi chứa các combination model, người dùng sẽ nhìn thấy cái này.
        public ObservableCollection<CombinationModel> CombinationModels { get; set; }
        public ObservableCollection<SubjectModel> SubjectModels { get; set; }


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
                AddCommand.NotifyCanExecuteChanged();
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
                ShowOnSimuCommand.NotifyCanExecuteChanged();
                OnPropertyChanged();
            }
        }


        public RelayCommand HideConflictCommand { get; set; }

        private int _combinationCount;
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

        private int _creditCount;
        public int CreditCount
        {
            get => _creditCount;
            set
            {
                _creditCount = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Command
        private ProgramDiagram _programDiagram;
        public AsyncRelayCommand ChoiceAccountCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public AsyncRelayCommand CannotAddReasonCommand { get; set; }
        public AsyncRelayCommand SubjectDownloadCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
        public RelayCommand GotoCourseCommand { get; set; }
        public AsyncRelayCommand WatchDetailCommand { get; set; }
        public RelayCommand GenCommand { get; set; }
        public RelayCommand ShowOnSimuCommand { get; set; }
        public RelayCommand OpenInNewWindowCommand { get; set; }
        public RelayCommand FilterChangedCommand { get; set; }
        #endregion

        #region Dependencies
        private readonly ColorGenerator _colorGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        private readonly ICurriculumCrawler _curriculumCrawler;
        private readonly IPreParSubjectCrawler _preParSubjectCrawler;
        private readonly IOpenInBrowser _openInBrowser;
        #endregion

        private Cs4rsaGen _cs4rsaGen;

        public AutoScheduleViewModel(ICourseCrawler courseCrawler,
            ColorGenerator colorGenerator, IUnitOfWork unitOfWork, ICurriculumCrawler curriculumCrawler,
            IPreParSubjectCrawler preParSubjectCrawler, IOpenInBrowser openInBrowser)
        {
            _openInBrowser = openInBrowser;
            _curriculumCrawler = curriculumCrawler;
            _preParSubjectCrawler = preParSubjectCrawler;
            _colorGenerator = colorGenerator;
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;

            MessageBus.Default.FromAny().Where<ExitLoginMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<AddCombinationMessage>().Notify(this);

            ProgramFolderModels = new ObservableCollection<ProgramFolderModel>();
            ChoicedProSubjectModels = new ObservableCollection<ProgramSubjectModel>();
            CombinationModels = new ObservableCollection<CombinationModel>();
            SubjectModels = new ObservableCollection<SubjectModel>();
            _filteredClassGroupModels = new List<List<ClassGroupModel>>();

            ChoiceAccountCommand = new AsyncRelayCommand(OnChoiceAccountCommand);
            AddCommand = new RelayCommand(OnAddSubject, CanAdd);
            SubjectDownloadCommand = new AsyncRelayCommand(OnDownload, CanDownload);
            DeleteCommand = new RelayCommand(OnDelete);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, CanDeleteAll);
            GotoCourseCommand = new RelayCommand(OnGoToCourse);
            WatchDetailCommand = new AsyncRelayCommand(OnWatchDetail);
            GenCommand = new RelayCommand(OnStartGen, CanGen);
            ShowOnSimuCommand = new RelayCommand(OnShowOnSimu, CanShowOnSimu);
            OpenInNewWindowCommand = new RelayCommand(OnOpenInNewWindow);
            HideConflictCommand = new RelayCommand(OnHideConflictCommand);
            FilterChangedCommand = new RelayCommand(OnFiltering);
            CvsCombination = new();
            CvsCombination.Source = CombinationModels;
            CvsCombination.Filter += CombinationFilter;
        }

        private void OnFiltering()
        {
            _filteredClassGroupModels.Clear();
            foreach (List<ClassGroupModel> classGroupModels in _classGroupModelsOfClass)
            {
                List<ClassGroupModel> r = classGroupModels.Where(item => Filter(item))
                    .ToList();
                _filteredClassGroupModels.Add(r);
            }
            _cs4rsaGen = new Cs4rsaGen(_filteredClassGroupModels);
            CombinationModels.Clear();
            GenCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Nơi triển khai bộ lọc trước khi sắp xếp
        /// </summary>
        /// <param name="classGroupModel"></param>
        /// <returns></returns>
        private bool Filter(ClassGroupModel classGroupModel)
        {
            bool flag = true;
            if (_isRemoveClassGroupInvalid)
            {
                flag = classGroupModel.IsHaveSchedule() && classGroupModel.EmptySeat > 0;
            }
            return flag;
        }

        private void CombinationFilter(object sender, FilterEventArgs e)
        {
            //CombinationModel combinationModel = (CombinationModel)e.Item;
            //e.Accepted = !combinationModel.IsConflict;
            e.Accepted = true;
        }

        internal CollectionViewSource CvsCombination { get; set; }
        public ICollectionView AllCombination => CvsCombination.View;

        private void OnHideConflictCommand()
        {
            CvsCombination.View.Refresh();
        }

        private bool CanGen()
        {
            return CombinationModels.Count == 0 && SubjectModels.Count > 0;
        }

        private void OnStartGen()
        {
            _cs4rsaGen.Backtracking(0);
            GenCommand.NotifyCanExecuteChanged();
        }

        private void OnOpenInNewWindow()
        {
            //CombinationContainerWindow combinationContainerWindow = new CombinationContainerWindow(CombinationModels.ToList(), this);
            //combinationContainerWindow.Topmost = true;
            //combinationContainerWindow.Show();
        }

        private async Task OnChoiceAccountCommand()
        {
            StudentInputUC studentInputUC = new();
            StudentInputViewModel vm = studentInputUC.DataContext as StudentInputViewModel;
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(studentInputUC);
            await vm.LoadStudentInfos();
        }

        private async Task LoadStudent(LoginResult loginResult)
        {
            if (loginResult != null)
            {
                ChoicedProSubjectModels.Clear();
                CombinationModels.Clear();
                SubjectModels.Clear();
                Student = loginResult.Student;
                await LoadProgramSubject();
            }
        }

        private bool CanDeleteAll()
        {
            return ChoicedProSubjectModels.Count > 0;
        }

        private void OnDeleteAll()
        {
            ChoicedProSubjectModels.Clear();
            SubjectModels.Clear();
            CombinationModels.Clear();
            DeleteAllCommand.NotifyCanExecuteChanged();
            AddCommand.NotifyCanExecuteChanged();
            GenCommand.NotifyCanExecuteChanged();
        }

        public async Task LoadProgramSubject()
        {
            Visibility = "Visible";
            ProgramFolderModels.Clear();
            ProgramDiagramCrawler programDiagramCrawler = new("", _student.SpecialString, _curriculumCrawler, _unitOfWork, _preParSubjectCrawler);
            programDiagramCrawler.AddProgramFolder = AddProgramFolder;
            await programDiagramCrawler.ToProgramDiagram();
            Visibility = "Hidden";
        }

        private async Task AddProgramFolder(ProgramFolder programFolder)
        {
            ProgramFolderModel programFolderModel = await ProgramFolderModel.CreateAsync(programFolder, _colorGenerator, _unitOfWork);
            ProgramFolderModels.Add(programFolderModel);
        }

        private bool CanAdd()
        {
            return _selectedProSubject != null
                && _selectedProSubject.IsAvaiable
                && _selectedProSubject.IsDone == false
                && !ChoicedProSubjectModels.Contains(_selectedProSubject);
        }

        private bool CanShowOnSimu()
        {
            return _selectedCombinationModel != null
                    && !_selectedCombinationModel.HaveAClassGroupHaveNotSchedule
                    && !_selectedCombinationModel.HaveAClassGroupHaveZeroEmptySeat;
        }

        private void OnShowOnSimu()
        {
            ShowOnSimuMessage showOnSimuMessage = new(_selectedCombinationModel);
            MessageBus.Default.Publish(showOnSimuMessage);
        }

        private bool CanDownload()
        {
            return true;
        }

        private async Task OnWatchDetail()
        {
            ProSubjectDetailUC proSubjectDetailUC = new();
            ProSubjectDetailViewModel vm = proSubjectDetailUC.DataContext as ProSubjectDetailViewModel;
            vm.ProgramDiagram = _programDiagram;
            vm.ProgramSubjectModel = _selectedProSubject;
            vm.AddCallback = OnAddSubject;
            vm.CloseDialogCallback = (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog;
            await vm.LoadPreProSubjectModels();
            await vm.LoadParProSubjectModels();
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(proSubjectDetailUC);
        }

        private void OnGoToCourse()
        {
            if (_selectedProSubjectInChoiced != null)
            {
                string courseId = _selectedProSubjectInChoiced.CourseId;
                string semesterValue = _courseCrawler.GetCurrentSemesterValue();
                string url = $@"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid={courseId}&timespan={semesterValue}&t=s";
                _openInBrowser.Open(url);
            }
        }

        private void OnDelete()
        {
            if (SubjectModels.Any(subjectModel => subjectModel.SubjectName == _selectedProSubjectInChoiced.SubjectName))
            {
                SubjectModel needRemove = SubjectModels
                    .FirstOrDefault(subjectModel => subjectModel.SubjectName == _selectedProSubjectInChoiced.SubjectName);
                SubjectModels.Remove(needRemove);
            }
            ChoicedProSubjectModels.Remove(_selectedProSubjectInChoiced);
            SubjectDownloadCommand.NotifyCanExecuteChanged();
            AddCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            GenCommand.NotifyCanExecuteChanged();
            UpdateChoicedCount();
            UpdateCreditCount();
        }

        /// <summary>
        /// Phương thức này không chỉ thực hiện download mà nó còn thực hiện đồng bộ
        /// giữa đã chọn và đã tải nhằm tối ưu tốc độ, thay vì việc thực hiện tải lại
        /// rất mất thời gian.
        /// </summary>
        /// <param name="obj"></param>
        private async Task OnDownload()
        {
            CombinationModels.Clear();
            List<string> choiceSubjectCodes = ChoicedProSubjectModels.Select(item => item.ProgramSubject.SubjectCode).ToList();
            List<string> wereDownloadedSubjectCodes = SubjectModels.Select(item => item.SubjectCode).ToList();

            // khi cần tải == 0
            if (choiceSubjectCodes.Count == 0)
            {
                wereDownloadedSubjectCodes.Clear();
            }

            // to do: Xác định subject nào cần tải dựa vào phép trừ tập hợp
            List<string> needDownloadNames = choiceSubjectCodes.Except(wereDownloadedSubjectCodes).ToList();
            List<ProgramSubjectModel> needDownload = ChoicedProSubjectModels.Where(item => needDownloadNames.Contains(item.ProgramSubject.SubjectCode)).ToList();

            if (needDownload.Count > 0)
            {
                AutoSortSubjectLoadUC autoSortSubjectLoadUC = new();
                AutoSortSubjectLoadViewModel vm = autoSortSubjectLoadUC.DataContext as AutoSortSubjectLoadViewModel;
                vm.ProgramSubjectModels = needDownload;
                (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(autoSortSubjectLoadUC);
                List<SubjectModel> subjectModels = await vm.Download();
                (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
                foreach (SubjectModel subjectModel in subjectModels)
                {
                    SubjectModels.Add(subjectModel);
                }
            }
            _classGroupModelsOfClass = SubjectModels.Select(item => item.ClassGroupModels).ToList();
            OnFiltering();
            GenCommand.NotifyCanExecuteChanged();
        }

        private void OnAddSubject()
        {
            if (_selectedProSubject != null)
            {
                ChoicedProSubjectModels.Add(SelectedProSubject);
                SubjectDownloadCommand.NotifyCanExecuteChanged();
                AddCommand.NotifyCanExecuteChanged();
                DeleteAllCommand.NotifyCanExecuteChanged();
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
                    {
                        hasChoiced++;
                    }
                }
                return (needLearn - hasChoiced) == 0;
            }
            return false;
        }

        public async void Handle(ExitLoginMessage message)
        {
            await LoadStudent(message.Source);
        }

        public void Handle(AddCombinationMessage message)
        {
            List<int> indexes = message.Source;
            List<ClassGroupModel> result = new();
            for (int i = 0; i < indexes.Count; i++)
            {
                result.Add(_filteredClassGroupModels[i][indexes[i]]);
            }
            CombinationModel combinationModel = new(SubjectModels.ToList(), result);
            CombinationModels.Add(combinationModel);
        }
    }
}
