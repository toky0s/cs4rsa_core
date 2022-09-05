using CourseSearchService.Crawlers.Interfaces;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.DialogViews;
using cs4rsa_core.Dialogs.Implements;
using cs4rsa_core.Dialogs.MessageBoxService;
using cs4rsa_core.Interfaces;
using cs4rsa_core.Messages.Publishers;
using cs4rsa_core.Messages.Publishers.Dialogs;
using cs4rsa_core.ModelExtensions;
using cs4rsa_core.Models;
using cs4rsa_core.ViewModelFunctions;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using HelperService;
using MaterialDesignThemes.Wpf;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SubjectCrawlService1.Crawlers.Interfaces;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace cs4rsa_core.ViewModels
{
    public sealed class SearchSessionViewModel : ViewModelBase
    {
        #region Commands
        public IAsyncRelayCommand AddCommand { get; set; }
        private bool canRunAddCommand = true;
        public bool CanRunAddCommand
        {
            get => canRunAddCommand;
            set
            {
                canRunAddCommand = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
        public AsyncRelayCommand ImportDialogCommand { get; set; }
        public RelayCommand GotoCourseCommand { get; set; }
        public RelayCommand DetailCommand { get; set; }
        #endregion

        #region Properties
        private Discipline selectedDiscipline;
        public Discipline SelectedDiscipline
        {
            get => selectedDiscipline;

            set
            {
                selectedDiscipline = value;
                OnPropertyChanged();
            }
        }

        private Keyword selectedKeyword;
        public Keyword SelectedKeyword
        {
            get => selectedKeyword;
            set
            {
                selectedKeyword = value;
                CanAddSubjectChange();
                OnPropertyChanged();
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged(); }
        }

        private FullMatchSearchingKeyword _selectedFullMatchSearchingKeyword;
        public FullMatchSearchingKeyword SelectedFullMatchSearchingKeyword
        {
            get { return _selectedFullMatchSearchingKeyword; }
            set { _selectedFullMatchSearchingKeyword = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Keyword> DisciplineKeywordModels { get; set; }
        public ObservableCollection<SubjectModel> SubjectModels { get; set; }
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public ObservableCollection<FullMatchSearchingKeyword> FullMatchSearchingKeywords { get; set; }

        /// <summary>
        /// Danh sách các bộ lịch đã lưu
        /// </summary>
        public ObservableCollection<Session> SavedSchedules { get; set; }

        private int _totalSubject;
        public int TotalSubject
        {
            get => _totalSubject;
            set
            {
                _totalSubject = value;
                OnPropertyChanged();
            }
        }

        private SubjectModel _selectedSubjectModel;
        public SubjectModel SelectedSubjectModel
        {
            get => _selectedSubjectModel;
            set
            {
                _selectedSubjectModel = value;
                DeleteCommand.NotifyCanExecuteChanged();
                Messenger.Send(new SearchVmMsgs.SelectedSubjectChangedMsg(value));
            }
        }

        private int totalCredits;
        public int TotalCredits
        {
            get => totalCredits;
            set
            {
                totalCredits = value;
                OnPropertyChanged();
            }
        }

        private int _currentView;

        public int CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        private bool _isUseCache;

        public bool IsUseCache
        {
            get { return _isUseCache; }
            set { _isUseCache = value; OnPropertyChanged(); }
        }

        #endregion

        #region Services
        private readonly ICourseCrawler _courseCrawler;
        private readonly ISubjectCrawler _subjectCrawler;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ColorGenerator _colorGenerator;
        private readonly IOpenInBrowser _openInBrowser;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion

        public SearchSessionViewModel(
            ICourseCrawler courseCrawler, 
            IUnitOfWork unitOfWork,
            ISubjectCrawler subjectCrawler, 
            ColorGenerator colorGenerator, 
            IOpenInBrowser openInBrowser,
            ISnackbarMessageQueue snackbarMessageQueue
        )
        {
            _courseCrawler = courseCrawler;
            _subjectCrawler = subjectCrawler;
            _unitOfWork = unitOfWork;
            _colorGenerator = colorGenerator;
            _openInBrowser = openInBrowser;
            _snackbarMessageQueue = snackbarMessageQueue;

            WeakReferenceMessenger.Default.Register<ImportSessionVmMsgs.ExitImportSubjectMsg>(this, async (r, m) =>
            {
                await CloseDialogAndHandleSessionManagerResult(m.Value);
            });

            WeakReferenceMessenger.Default.Register<AutoScheduleVmMsgs.ShowOnSimuMsg>(this, (r, m) =>
            {
                ShowOnSimuMsgHandler(m.Value);
            });

            WeakReferenceMessenger.Default.Register<UpdateVmMsgs.UpdateSuccessMsg>(this, async (r, m) =>
            {
                await UpdateSuccessMsgHandler();
            });

            WeakReferenceMessenger.Default.Register<SubjectImporterVmMsgs.ExitImportSubjectMsg>(this, (r, m) =>
            {
                ExitImportSubjectMsgHandler(m.Value.Item1, m.Value.Item2);
            });

            DisciplineKeywordModels = new();
            SubjectModels = new();
            Disciplines = new();
            FullMatchSearchingKeywords = new();
            SavedSchedules = new();
            SearchText = "";
            CurrentView = 0;
            IsUseCache = true;

            AddCommand = new AsyncRelayCommand(OnAddSubjectAsync);
            DeleteCommand = new RelayCommand(OnDeleteSubject, CanDeleteSubject);
            ImportDialogCommand = new(OnOpenImportDialog);
            GotoCourseCommand = new RelayCommand(OnGotoCourse, () => true);
            DeleteAllCommand = new RelayCommand(OnDeleteAll);
            DetailCommand = new RelayCommand(OnDetail);
        }

        /// <summary>
        /// Load danh sách các bộ lịch đã lưu
        /// </summary>
        /// <returns></returns>
        public async Task LoadSavedSchedules()
        {
            SavedSchedules.Clear();
            IEnumerable<Session> sessions = await _unitOfWork.Sessions.GetAllAsync();
            foreach (Session session in sessions)
            {
                SavedSchedules.Add(session);
            }
        }

        private readonly ShowDetailsSubjectUC showDetailsSubjectUC = new();
        private void OnDetail()
        {
            (showDetailsSubjectUC.DataContext as ShowDetailsSubjectViewModel).SubjectModel = _selectedSubjectModel;
            OpenDialog(showDetailsSubjectUC);
        }

        public async Task LoadDiscipline()
        {
            List<Discipline> disciplines = await _unitOfWork.Disciplines.GetAllIncludeKeywordAsync();
            foreach (Discipline discipline in disciplines)
            {
                Disciplines.Add(discipline);
            }
            SelectedDiscipline = Disciplines[0];
        }

        internal async Task LoadSelectedDisciplineAndKeyword(Discipline discipline, Keyword keyword)
        {
            Discipline actualDiscipline = await _unitOfWork.Disciplines.GetByIdAsync(discipline.DisciplineId);
            Keyword actualKeyword = await _unitOfWork.Keywords.GetByIdAsync(keyword.KeywordId);
            SelectedDiscipline = actualDiscipline;
            SelectedKeyword = actualKeyword;
        }

        public async Task LoadSearchItemSource(string text)
        {
            text = text.Trim().ToLower();
            if (text.Equals("")) return;

            FullMatchSearchingKeywords.Clear();

            Task<List<Keyword>> result1 = _unitOfWork.Keywords.GetByDisciplineStartWith(text);
            Task<List<Keyword>> result2 = _unitOfWork.Keywords.GetBySubjectNameContains(text);
            List<Keyword>[] whenAllResult = await Task.WhenAll(result1, result2);
            foreach (List<Keyword> keywords in whenAllResult)
            {
                foreach (Keyword keyword in keywords)
                {
                    FullMatchSearchingKeyword fullMacth = new()
                    {
                        Keyword = keyword,
                        Discipline = keyword.Discipline
                    };
                    FullMatchSearchingKeywords.Add(fullMacth);
                }
            }

            if (text.Contains(' '))
            {
                string[] textSplit = text.Split(new char[] { ' ' }, StringSplitOptions.None);
                string discipline = textSplit[0];
                string keyword1 = textSplit[1];
                List<Keyword> keywordsBySubjectCode = await _unitOfWork.Keywords.GetByDisciplineAndKeyword1(discipline, keyword1);
                foreach (Keyword kw in keywordsBySubjectCode)
                {
                    FullMatchSearchingKeyword fullMatch = new()
                    {
                        Keyword = kw,
                        Discipline = kw.Discipline
                    };
                    FullMatchSearchingKeywords.Add(fullMatch);
                }
            }

            if (FullMatchSearchingKeywords.Count == 0)
            {
                Keyword keyword = new()
                {
                    CourseId = 000000,
                    SubjectName = "Không tìm thấy tên môn này",
                    Color = "#ffffff"
                };
                Discipline discipline = new()
                {
                    Name = "Không tìm thấy mã môn này"
                };
                FullMatchSearchingKeyword fullMatchSearchingKeyword = new()
                {
                    Keyword = keyword,
                    Discipline = discipline
                };
                FullMatchSearchingKeywords.Add(fullMatchSearchingKeyword);
            }
        }

        private void OnDeleteAll()
        {
            List<SubjectModel> actionData = new();
            foreach(SubjectModel subjectModel in SubjectModels)
            {
                Messenger.Send(new SearchVmMsgs.DelSubjectMsg(subjectModel));
                SubjectModel restoreSubject = subjectModel.DeepClone();
                actionData.Add(restoreSubject);
            }
            SubjectModels.Clear();
            Messenger.Send(new SearchVmMsgs.DelAllSubjectMsg(null));
            CanAddSubjectChange();
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.NotifyCanExecuteChanged();
            _snackbarMessageQueue.Enqueue("Đã xoá hết", "HOÀN TÁC", OnRestore, actionData);
        }

        private void OnRestore(IEnumerable<SubjectModel> obj)
        {
            foreach (SubjectModel subjectModel in obj)
            {
                AddSubjectAndReload(subjectModel);
            }
        }

        private void OnGotoCourse()
        {
            int courseId = _selectedSubjectModel.CourseId;
            string semesterValue = _courseCrawler.GetCurrentSemesterValue();
            string url = $@"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid={courseId}&timespan={semesterValue}&t=s";
            _openInBrowser.Open(url);
        }

        /// <summary>
        /// Load lại data môn học từ cơ sở dữ liệu lên
        /// </summary>
        private async Task ReloadDisciplineAndKeyWord()
        {
            Disciplines.Clear();
            List<Discipline> disciplines = await _unitOfWork.Disciplines.GetAllIncludeKeywordAsync();
            disciplines.ForEach(discipline => Disciplines.Add(discipline));
            SelectedDiscipline = Disciplines[0];
            LoadDisciplineKeyword(SelectedDiscipline);
        }

        private readonly ImportSessionUC _importSessionUC = new();
        private async Task OnOpenImportDialog()
        {
            ImportSessionViewModel vm = _importSessionUC.DataContext as ImportSessionViewModel;
            OpenDialog(_importSessionUC);
            await vm.LoadScheduleSession();
        }

        private async Task CloseDialogAndHandleSessionManagerResult(SessionManagerResult result)
        {
            CloseDialog();
            if (result != null)
            {
                SubjectImporterUC subjectImporterUC = new();
                SubjectImporterViewModel vm = subjectImporterUC.DataContext as SubjectImporterViewModel;
                vm.SessionManagerResult = result;
                OpenDialog(subjectImporterUC);
                await vm.Run();
            }
        }

        private void ExitImportSubjectMsgHandler(ImportResult importResult, SessionManagerResult sessionManagerResult)
        {
            ImportSubjects(importResult.SubjectModels);
            var choicer = new ClassGroupChoicer();
            choicer.Start(importResult.SubjectModels, sessionManagerResult.SubjectInfoDatas);
            SelectedSubjectModel = SubjectModels[0];
        }

        private bool CanDeleteSubject()
        {
            return _selectedSubjectModel != null;
        }

        private void OnDeleteSubject()
        {
            Messenger.Send(new SearchVmMsgs.DelSubjectMsg(_selectedSubjectModel));

            SubjectModel actionData = _selectedSubjectModel.DeepClone();

            string message = $"Vừa xoá môn {_selectedSubjectModel.SubjectName}";
            SubjectModels.Remove(_selectedSubjectModel);
            _snackbarMessageQueue.Enqueue(message, "HOÀN TÁC", OnRestoreSubjectModel, actionData);
            CanAddSubjectChange();
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.NotifyCanExecuteChanged();
        }

        private void OnRestoreSubjectModel(SubjectModel obj)
        {
            AddSubjectAndReload(obj);
        }

        /// <summary>
        /// Load Keyword sau khi chọn discipline.
        /// </summary>
        /// <param name="discipline">Discipline.</param>
        public void LoadDisciplineKeyword(Discipline discipline)
        {
            DisciplineKeywordModels.Clear();
            Discipline currentDiscipline = Disciplines.Where(d => d.DisciplineId == discipline.DisciplineId).FirstOrDefault();
            List<Keyword> keywords = currentDiscipline.Keywords;
            keywords.ForEach(keyword => DisciplineKeywordModels.Add(keyword));
            SelectedKeyword = DisciplineKeywordModels[0];
        }

        private async Task OnAddSubjectAsync()
        {
            CanAddSubjectChange(false);

            SubjectDownloadingUC subjectDownloadingUC = new();
            SubjectDownloadingViewModel vm = subjectDownloadingUC.DataContext as SubjectDownloadingViewModel;
            string subjectName = selectedKeyword.SubjectName;
            string subjectCode = selectedDiscipline.Name + " " + selectedKeyword.Keyword1;
            vm.SubjectName = subjectName;
            vm.SubjectCode = subjectCode;
            OpenDialog(subjectDownloadingUC);

            Subject subject = await _subjectCrawler.Crawl(selectedDiscipline.Name, selectedKeyword.Keyword1, IsUseCache);
            if (subject != null)
            {
                SubjectModel subjectModel = await SubjectModel.CreateAsync(subject, _colorGenerator);
                if (subjectModel.ClassGroupModels.Count == 1)
                {
                    Messenger.Send(new ClassGroupSessionVmMsgs.ClassGroupAddedMsg(subjectModel.ClassGroupModels[0]));
                }
                AddSubjectAndReload(subjectModel);
                CloseDialog();
                SelectedSubjectModel = SubjectModels.Last();
            }
            else
            {
                CloseDialog();
                _snackbarMessageQueue.Enqueue("Môn học không tồn tại trong học kỳ này");
                CanAddSubjectChange();
            }
        }

        public void OnAddSubjectFromUriAsync(Uri uri)
        {
            var queries = HttpUtility.ParseQueryString(uri.Query);
            string courseId = queries.Get("courseid");
            string p = queries.Get("p");
            string timespan = queries.Get("timespan");
            string t = queries.Get("t");

            bool isDtuCourseHost = uri.Host == "courses.duytan.edu.vn";
            bool isRightAbsPath = uri.AbsolutePath == "/Sites/Home_ChuongTrinhDaoTao.aspx";

            if (courseId != null && p != null && timespan != null && t != null && isDtuCourseHost && isRightAbsPath)
            {
                _snackbarMessageQueue.Enqueue("Đúng đường dẫn");
            }
            else
            {
                _snackbarMessageQueue.Enqueue("Sai đường dẫn");
            }
        }

        /// <summary>
        /// Cập nhật tổng số môn học.
        /// </summary>
        private void UpdateSubjectAmount()
        {
            TotalSubject = SubjectModels.Count;
            Messenger.Send(new SearchVmMsgs.SubjectItemChangedMsg(new Tuple<int, int>(TotalCredits, TotalSubject)));
        }

        /// <summary>
        /// Cập nhật tổng số tín chỉ
        /// </summary>
        private void UpdateCreditTotal()
        {
            TotalCredits = 0;
            foreach (SubjectModel subject in SubjectModels)
            {
                TotalCredits += subject.StudyUnit;
            }
            Messenger.Send(new SearchVmMsgs.SubjectItemChangedMsg(new Tuple<int, int>(TotalCredits, TotalSubject)));
        }

        private void CanAddSubjectChange(bool? value = null)
        {
            if (value == null)
            {
                IEnumerable<int> courseIds = SubjectModels.Select(item => item.CourseId);
                if (selectedKeyword == null)
                {
                    CanRunAddCommand = true;
                    return;
                }
                CanRunAddCommand = !courseIds.Contains(SelectedKeyword.CourseId);
            }
            else
            {
                CanRunAddCommand = value.Value;
            }
        }

        private void ImportSubjects(IEnumerable<SubjectModel> importSubjects)
        {
            foreach (SubjectModel subject in SubjectModels)
            {
                Messenger.Send(new SearchVmMsgs.DelSubjectMsg(subject));
            }

            SubjectModels.Clear();
            foreach (SubjectModel subject in importSubjects)
            {
                SubjectModels.Add(subject);
            }
            TotalSubject = SubjectModels.Count;
            CanAddSubjectChange();
            UpdateCreditTotal();
            UpdateSubjectAmount();
        }

        /// <summary>
        /// Thêm một SubjectModel vào danh sách.
        /// Load lại tổng số tín chỉ và tổng số môn học
        /// </summary>
        private void AddSubjectAndReload(SubjectModel subjectModel)
        {
            SubjectModels.Add(subjectModel);
            TotalSubject = SubjectModels.Count;
            CanAddSubjectChange();
            UpdateCreditTotal();
            UpdateSubjectAmount();
        }

        private async Task UpdateSuccessMsgHandler()
        {
            DisciplineKeywordModels.Clear();
            Disciplines.Clear();
            await ReloadDisciplineAndKeyWord();
        }

        private void ShowOnSimuMsgHandler(CombinationModel combination)
        {
            ImportSubjects(combination.SubjecModels);
            ClassGroupChoicer choicer = new();
            choicer.Start(combination.ClassGroupModels);
        }
    }
}
