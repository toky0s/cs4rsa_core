using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Dialogs.Implements;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.ModelExtensions;
using Cs4rsa.Models;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;
using Cs4rsa.ViewModelFunctions;

using MaterialDesignThemes.Wpf;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Threading;

namespace Cs4rsa.ViewModels
{
    public sealed class SearchSessionViewModel : ViewModelBase
    {
        #region Fields
        private readonly SubjectDownloadingUC _subjectDownloadingUC;
        private readonly ShowDetailsSubjectUC _showDetailsSubjectUC;
        #endregion

        #region Commands
        public AsyncRelayCommand AddCommand { get; set; }
        public AsyncRelayCommand ImportDialogCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
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
                if (value != null)
                {
                    LoadDisciplineKeyword(value);
                }
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
                AddCommand.NotifyCanExecuteChanged();
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
            set
            {
                _selectedFullMatchSearchingKeyword = value;
                OnPropertyChanged();
                if (value != null
                    && value.Discipline.DisciplineId != 0
                    && value.Keyword != null)
                {
                    SelectedDiscipline = value.Discipline;
                    SelectedKeyword = value.Keyword;
                    SearchText = string.Empty;
                    AddCommand.NotifyCanExecuteChanged();
                    if (!IsAlreadyDownloaded(value.Keyword.CourseId))
                    {
                        DispatcherOperation dispatcherOperation = Application.Current.Dispatcher.InvokeAsync(
                            async () => await OpenDownloadSubjectDialog(
                                value.Discipline.Name,
                                value.Keyword.Keyword1,
                                courseId: VMConstants.INT_INVALID_COURSEID,
                                IsUseCache)
                        );
                    }
                }
            }
        }

        public ObservableCollection<Keyword> DisciplineKeywordModels { get; set; }
        public ObservableCollection<SubjectModel> SubjectModels { get; set; }
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public ObservableCollection<FullMatchSearchingKeyword> FullMatchSearchingKeywords { get; set; }

        /// <summary>
        /// Danh sách các bộ lịch đã lưu
        /// </summary>
        public ObservableCollection<UserSchedule> SavedSchedules { get; set; }

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
            _subjectDownloadingUC = new();
            _showDetailsSubjectUC = new();

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

            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await LoadDiscipline();
                await LoadSavedSchedules();
            });

            DisciplineKeywordModels = new();
            SubjectModels = new();
            Disciplines = new();
            FullMatchSearchingKeywords = new();
            SavedSchedules = new();
            SearchText = "";
            CurrentView = 0;
            IsUseCache = true;

            AddCommand = new AsyncRelayCommand(OnAddSubjectAsync, () => !IsAlreadyDownloaded(SelectedKeyword));
            DeleteCommand = new RelayCommand(OnDeleteSubject, CanDeleteSubject);
            ImportDialogCommand = new(OnOpenImportDialog);
            GotoCourseCommand = new RelayCommand(OnGotoCourse, () => true);
            DeleteAllCommand = new RelayCommand(OnDeleteAll);
            DetailCommand = new RelayCommand(OnDetail);
        }

        /// <summary>
        /// Load danh sách các bộ lịch đã lưu
        /// </summary>
        public async Task LoadSavedSchedules()
        {
            SavedSchedules.Clear();
            IEnumerable<UserSchedule> sessions = await _unitOfWork.UserSchedule.GetAllAsync();
            foreach (UserSchedule session in sessions)
            {
                SavedSchedules.Add(session);
            }
        }

        private void OnDetail()
        {
            (_showDetailsSubjectUC.DataContext as ShowDetailsSubjectViewModel).SubjectModel = _selectedSubjectModel;
            OpenDialog(_showDetailsSubjectUC);
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
            #region Subjects
            List<SubjectModel> subjects = new();
            foreach (SubjectModel subjectModel in SubjectModels)
            {
                SubjectModel restoreSubject = subjectModel.DeepClone();
                subjects.Add(restoreSubject);
            }
            #endregion

            #region ClassGroupModels
            List<ClassGroupModel> classGroupModels = new();
            ChoosedSessionViewModel choicedSessionViewModel = GetViewModel<ChoosedSessionViewModel>();
            ObservableCollection<ClassGroupModel> classGroupColl = choicedSessionViewModel.ClassGroupModels;
            foreach (ClassGroupModel classGroupModel in classGroupColl)
            {
                classGroupModels.Add(classGroupModel.DeepClone());
            }
            #endregion

            Tuple<IEnumerable<SubjectModel>, IEnumerable<ClassGroupModel>> actionData = new(subjects, classGroupModels);

            SubjectModels.Clear();
            Messenger.Send(new SearchVmMsgs.DelAllSubjectMsg(null));
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.NotifyCanExecuteChanged();
            _snackbarMessageQueue.Enqueue(VMConstants.SNB_DELETE_ALL, VMConstants.SNBAC_RESTORE, AddSubjectAndReload, actionData);
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

        private async Task CloseDialogAndHandleSessionManagerResult(IEnumerable<UserSubject> result)
        {
            CloseDialog();
            if (result != null)
            {
                SubjectImporterUC subjectImporterUC = new();
                SubjectImporterViewModel vm = subjectImporterUC.DataContext as SubjectImporterViewModel;
                OpenDialog(subjectImporterUC);
                await vm.Run(result);
                CloseDialog();
            }
        }

        private void ExitImportSubjectMsgHandler(IEnumerable<SubjectModel> subjectModels, IEnumerable<UserSubject> userSubjects)
        {
            ImportSubjects(subjectModels);
            var choicer = new ClassGroupChoicer();
            choicer.Start(subjectModels, userSubjects);
            SelectedSubjectModel = SubjectModels[0];
        }

        private bool CanDeleteSubject()
        {
            return _selectedSubjectModel != null;
        }

        private void OnDeleteSubject()
        {
            IEnumerable<ClassGroupModel> classGroupModels = new List<ClassGroupModel>();

            if (GetViewModel<ChoosedSessionViewModel>().ClassGroupModels
                    .Where(cgm => cgm.SubjectCode.Equals(_selectedSubjectModel.SubjectCode))
                    .Any())
            {
                ClassGroupModel classGroupModel = GetViewModel<ChoosedSessionViewModel>().ClassGroupModels
                    .Where(cgm => cgm.SubjectCode.Equals(_selectedSubjectModel.SubjectCode))
                    .First();
                ClassGroupModel classGroupModelClone = classGroupModel.DeepClone();
                classGroupModels = new List<ClassGroupModel>() { classGroupModelClone };
            }

            Messenger.Send(new SearchVmMsgs.DelSubjectMsg(_selectedSubjectModel));
            SubjectModel subjectModel = _selectedSubjectModel.DeepClone();

            List<SubjectModel> subjectModels = new()
            {
                subjectModel
            };

            Tuple<IEnumerable<SubjectModel>, IEnumerable<ClassGroupModel>> actionData = new(subjectModels, classGroupModels);

            string message = $"Vừa xoá môn {_selectedSubjectModel.SubjectName}";
            SubjectModels.Remove(_selectedSubjectModel);
            _snackbarMessageQueue.Enqueue(message, VMConstants.SNBAC_RESTORE, AddSubjectAndReload, actionData);
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.NotifyCanExecuteChanged();
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

            await OpenDownloadSubjectDialog(
                selectedDiscipline.Name,
                selectedKeyword.Keyword1,
                VMConstants.INT_INVALID_COURSEID,
                IsUseCache);
        }

        private async Task OpenDownloadSubjectDialog(
            string discipline,
            string keyword1,
            int courseId,
            bool isUseCache)
        {
            SubjectDownloadingViewModel vm = _subjectDownloadingUC.DataContext as SubjectDownloadingViewModel;
            OpenDialog(_subjectDownloadingUC);

            Subject subject;
            if (courseId != VMConstants.INT_INVALID_COURSEID)
            {
                await vm.ReEvaluated(courseId);
                subject = await _subjectCrawler.Crawl(courseId, isUseCache);
            }
            else
            {
                string subjectName = selectedKeyword.SubjectName;
                string subjectCode = selectedDiscipline.Name + " " + selectedKeyword.Keyword1;
                vm.SubjectName = subjectName;
                vm.SubjectCode = subjectCode;
                subject = await _subjectCrawler.Crawl(discipline, keyword1, isUseCache);
            }

            if (subject != null)
            {
                List<SubjectModel> subjectModels = new();
                SubjectModel subjectModel = await SubjectModel.CreateAsync(subject, _colorGenerator);
                subjectModels.Add(subjectModel);

                Tuple<IEnumerable<SubjectModel>, IEnumerable<ClassGroupModel>> data = new(subjectModels, null);
                AddSubjectAndReload(data);
                CloseDialog();
                SelectedSubjectModel = SubjectModels.Last();
            }
            else
            {
                CloseDialog();
                _snackbarMessageQueue.Enqueue(VMConstants.SNB_NOT_FOUND_SUBJECT_IN_THIS_SEMESTER);
                AddCommand.NotifyCanExecuteChanged();
            }
        }

        public async void OnAddSubjectFromUriAsync(Uri uri)
        {
            NameValueCollection queries = HttpUtility.ParseQueryString(uri.Query);
            string courseId = queries.Get("courseid");
            string p = queries.Get("p");
            string timespan = queries.Get("timespan");
            string t = queries.Get("t");

            bool isDtuCourseHost = uri.Host == "courses.duytan.edu.vn";
            bool isRightAbsPath = uri.AbsolutePath == "/Sites/Home_ChuongTrinhDaoTao.aspx";

            if (courseId != null && p != null && timespan != null && t != null && isDtuCourseHost && isRightAbsPath)
            {
                int intCourseId = int.Parse(courseId);
                if (IsAlreadyDownloaded(intCourseId))
                {
                    _snackbarMessageQueue.Enqueue(VMConstants.SNB_ALREADY_DOWNLOADED);
                    return;
                }

                IEnumerable<Keyword> keywords = _unitOfWork.Keywords.Find(kw => kw.CourseId == intCourseId);
                if (!keywords.Any())
                {
                    _snackbarMessageQueue.Enqueue("Không tồn tại " + courseId);
                    return;
                }

                await OpenDownloadSubjectDialog(null, null, intCourseId, IsUseCache);
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

        /// <summary>
        /// Kiếm tra xem rằng một Subject đã có 
        /// sẵn trong danh sách đã tải xuống hay chưa.
        /// </summary>
        /// <param name="courseId">Course ID</param>
        /// <returns></returns>
        private bool IsAlreadyDownloaded(int courseId)
        {
            IEnumerable<int> courseIds = SubjectModels.Select(item => item.CourseId);
            return courseIds.Contains(courseId);
        }

        private bool IsAlreadyDownloaded(Keyword keyword)
        {
            if (keyword != null)
            {
                IEnumerable<int> courseIds = SubjectModels.Select(item => item.CourseId);
                return courseIds.Contains(keyword.CourseId);
            }
            return true;
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
            AddCommand.NotifyCanExecuteChanged();
            UpdateCreditTotal();
            UpdateSubjectAmount();
        }

        /// <summary>
        /// Thêm một SubjectModel vào danh sách.
        /// Load lại tổng số tín chỉ và tổng số môn học.
        /// Đồng thời thêm select thêm các ClassGroup nếu có.
        /// </summary>
        private void AddSubjectAndReload(Tuple<IEnumerable<SubjectModel>, IEnumerable<ClassGroupModel>> actionData)
        {
            foreach (SubjectModel subjectModel in actionData.Item1)
            {
                SubjectModels.Add(subjectModel);
            }
            if (actionData.Item2 != null)
            {
                Messenger.Send(new SearchVmMsgs.SelectClassGroupModelsMsg(actionData.Item2));
            }
            SelectedSubjectModel = SubjectModels[0];
            TotalSubject = SubjectModels.Count;
            AddCommand.NotifyCanExecuteChanged();
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
