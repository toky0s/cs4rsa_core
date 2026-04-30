using Cs4rsa.Common;
using Cs4rsa.Common.Interfaces;
using Cs4rsa.Database;
using Cs4rsa.Database.Implements;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;
using Cs4rsa.Infrastructure.Common;
using Cs4rsa.Messages.Publishers.UIs;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Views;
using Cs4rsa.Module.ManuallySchedule.Events;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.Service.Conflict.DataTypes.Enums;
using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.CourseCrawler.Crawlers;
using Cs4rsa.Service.Notification;
using Cs4rsa.Service.Notification.Models;
using Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.UI.ScheduleTable.Interfaces;
using Cs4rsa.UI.ScheduleTable.Models;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Data;

using static Cs4rsa.Module.ManuallySchedule.Events.ChoosedVmMsgs;


namespace Cs4rsa.Module.ManuallySchedule.ViewModels
{
    public class MainSchedulingViewModel : BindableBase
    {
        #region Fields
        private List<Discipline> _searchDisciplines;
        private List<Keyword> _searchKeywords;
        #endregion

        #region Commands

        #region Context menu commands when user right-click on Subject in search box
        private DelegateCommand<UserSchedule> _deleteUserScheduleCommand;
        public DelegateCommand<UserSchedule> DeleteUserScheduleCommand =>
            _deleteUserScheduleCommand ?? (_deleteUserScheduleCommand = new DelegateCommand<UserSchedule>(ExecuteDeleteUserScheduleCommand, CanExecuteDeleteUserScheduleCommand));

        void ExecuteDeleteUserScheduleCommand(UserSchedule userSchedule)
        {
            var result = MessageBox.Show(
                  $"Are you sure to delete the schedule {userSchedule.Name}?"
                , "Notification"
                , MessageBoxButton.YesNo
                , MessageBoxImage.Question
            );
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var removeResult = _unitOfWork.UserSchedules.Remove(userSchedule.UserScheduleId);
                    if (removeResult == 0)
                    {
                        _ = MessageBox.Show(
                              $"Cannot find the schedule {userSchedule.Name} to delete"
                            , "Notification"
                            , MessageBoxButton.OK
                            , MessageBoxImage.Error
                        );
                    }
                    else
                    {
                        UserSchedules.Remove(userSchedule);
                        _notificationService.SendNotification("Delete schedule", $"Schedule {userSchedule.Name} has been deleted", fromAction: "Delete user schedule");

                    }
                } 
                catch (Exception ex)
                {
                    _ = MessageBox.Show(
                          $"There is an issue while deleting the schedule: {ex.Message}"
                        , "Notification"
                        , MessageBoxButton.OK
                        , MessageBoxImage.Error
                    );
                }
            }
        }

        bool CanExecuteDeleteUserScheduleCommand(UserSchedule userSchedule)
        {
            return userSchedule.UserScheduleId != 0;
        }

        public DelegateCommand<SubjectModel> DeleteCommand { get; set; }
        public DelegateCommand<SubjectModel> GotoCourseCommand { get; set; }
        private DelegateCommand<SubjectModel> _detailCommand;
        public DelegateCommand<SubjectModel> DetailCommand =>
             _detailCommand ?? (_detailCommand = new DelegateCommand<SubjectModel>(ExecuteDetailCommand, CanExecuteDetailCommand));

        private bool CanExecuteDetailCommand(SubjectModel model)
        {
            return true;
        }

        private void ExecuteDetailCommand(SubjectModel model)
        {
            string semesterValue = _unitOfWork.Settings.GetByKey(DbConsts.StCurrentSemesterValue);
            string url = $@"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid={model.CourseId}&timespan={semesterValue}&t=s";
            var dialogParameter = new DialogParameters
            {
                { "Url", url },
                { "SubjectModel", model }
            };
            _dialogService.ShowDialog(nameof(ShowDetailsSubjectUC), dialogParameter, r => { _logger.LogInformation("ShowDetailsSubjectUC closed"); });
        }

        public DelegateCommand<SubjectModel> CopyErrorCommand { get; set; }

        private DelegateCommand<UserSchedule> _loadUserScheduleCommand;
        public DelegateCommand<UserSchedule> LoadUserScheduleCommand =>
            _loadUserScheduleCommand ?? (_loadUserScheduleCommand = new DelegateCommand<UserSchedule>(ExecuteLoadUserScheduleCommand, CanExecuteLoadUserScheduleCommand));

        void ExecuteLoadUserScheduleCommand(UserSchedule userSchedule)
        {
            if (userSchedule != null)
            {
                _dialogService.ShowDialog(nameof(ScheduleDetailUC), new DialogParameters()
                {
                    {"UserSchedule", userSchedule }
                }, async r => {
                    if (r.Result == ButtonResult.OK)
                    {
                        _logger.LogInformation("ScheduleDetailUC closed with OK");
                        SearchBoxSelectedIndex = 0;
                        var parameters = r.Parameters;
                        var userSubjects = parameters.GetValue<ObservableCollection<UserSubject>>("UserSubjects");
                        await HandleImportSubjects(userSubjects);
                        // Go to Search tab
                    }
                    else
                    {
                        _logger.LogInformation("ScheduleDetailUC closed");
                    }
                });
            }
        }

        bool CanExecuteLoadUserScheduleCommand(UserSchedule userSchedule)
        {
            return true;
        }

        #endregion

        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand ImportDialogCommand { get; set; }
        public DelegateCommand<SubjectModel> ReloadCommand { get; set; }
        /// <summary>
        /// Nút xoá tất cả các môn đã chọn bao gồm cả các lớp đã chọn
        /// </summary>
        public DelegateCommand DeleteAllCommand { get; set; }
        /// <summary>
        /// Nút xoá tất cả các lớp đã chọn, không xoá môn đã chọn
        /// </summary>
        public DelegateCommand DeleteAllChooseCommand { get; set; }
        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand, CanExecuteSaveCommand));

        void ExecuteSaveCommand()
        {
            var saveSessionUc = new SaveSessionUC();
            var vm = (SaveSessionUCViewModel)saveSessionUc.DataContext;

            IDialogParameters parameters = new DialogParameters
            {
                { "SelectedClassGroupModels", SelectedClassGroupModels }
            };
            _dialogService.ShowDialog(nameof(SaveSessionUC), parameters, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    _logger.LogInformation("Save schedule closed");
                }
            });
        }

        bool CanExecuteSaveCommand()
        {
            return SelectedClassGroupModels.Count > 0;
        }
        #endregion

        #region Properties
        public ObservableCollection<Keyword> DisciplineKeywordModels { get; set; }
        public ObservableCollection<SubjectModel> SubjectModels { get; set; }
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public ObservableCollection<FullMatchSearchingKeyword> FullMatchSearchingKeywords { get; set; }
        public ObservableCollection<UserSchedule> SavedSchedules { get; set; }

        /// <summary>
        /// Combination Models which was saved in the Store.
        /// </summary>
        public ObservableCollection<CombinationModel> ComModels { get; set; }
        public ObservableCollection<UserSchedule> UserSchedules { get; set; }

        private CombinationModel _sltCombi;
        public CombinationModel SltCombi
        {
            get { return _sltCombi; }
            set { SetProperty(ref _sltCombi, value); OnSltCombiChanged(value); }
        }

        private Discipline _selectedDiscipline;
        public Discipline SelectedDiscipline
        {
            get { return _selectedDiscipline; }
            set
            {
                SetProperty(ref _selectedDiscipline, value);
                if (value != null)
                {
                    LoadKeywordByDiscipline(value);
                }
            }
        }

        private Keyword _selectedKeyword;
        public Keyword SelectedKeyword
        {
            get { return _selectedKeyword; }
            set
            {
                SetProperty(ref _selectedKeyword, value);
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        private FullMatchSearchingKeyword _searchingKeyword;
        public FullMatchSearchingKeyword SearchingKeyword
        {
            get { return _searchingKeyword; }
            set
            {
                SetProperty(ref _searchingKeyword, value);
                OnSearchingKeywordChanged(value);
            }
        }

        private SubjectModel _selectedSubjectModel;
        public SubjectModel SelectedSubjectModel
        {
            get { return _selectedSubjectModel; }
            set
            {
                if (value == null)
                {
                    SetProperty(ref _selectedSubjectModel, null);
                    return;
                }

                if (value != null && !value.IsDownloading && !value.IsError)
                {
                    SetProperty(ref _selectedSubjectModel, value);
                    OnSelectedSubjectModelChanged(value);
                }
            }
        }

        private CancellationTokenSource _timeout;

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetProperty(ref _searchText, value);
                if (_timeout != null && !_timeout.IsCancellationRequested)
                {
                    _timeout.Cancel();
                    _timeout = null;
                }
                _timeout = JSFunctionality.SetTimeout(() =>
                {
                    Application.Current.Dispatcher.Invoke(() => LoadSearchItemSource(value));
                }, 500);
            }
        }

        private bool _isUseCache;
        public bool IsUseCache
        {
            get { return _isUseCache; }
            set { SetProperty(ref _isUseCache, value); }
        }

        private int _searchBoxSelectedIndex;
        public int SearchBoxSelectedIndex
        {
            get { return _searchBoxSelectedIndex; }
            set
            {
                SetProperty(ref _searchBoxSelectedIndex, value);
                LoadScheduleSession();
            }
        }
        #endregion

        #region Services
        private readonly ISubjectCrawler _subjectCrawler;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOpenInBrowser _openInBrowser;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<MainSchedulingViewModel> _logger;
        private readonly INotificationService _notificationService;
        private readonly IDialogService _dialogService;
        #endregion

        public void LoadScheduleSession()
        {
            if (SearchBoxSelectedIndex == 1)
            {
                UserSchedules.Clear();
                var sessions = _unitOfWork.UserSchedules.GetAll();
                UserSchedules.AddRange(sessions);
            }
        }

        public MainSchedulingViewModel(
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork,
            ISubjectCrawler subjectCrawler,
            IOpenInBrowser openInBrowser,
            ISnackbarMessageQueue snackbarMessageQueue,
            ILogger<MainSchedulingViewModel> logger,
            INotificationService notificationsService,
            IDialogService dialogService
        )
        {
            #region Services
            _subjectCrawler = subjectCrawler;
            _unitOfWork = unitOfWork;
            _openInBrowser = openInBrowser;
            _snackbarMessageQueue = snackbarMessageQueue;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _logger = logger;
            _notificationService = notificationsService;
            #endregion

            #region Subscribe Events

            //eventAggregator.GetEvent<AutoVmMsgs.ShowOnSimuMsg>().Subscribe().Register<AutoVmMsgs.ShowOnSimuMsg>(this, (r, m) =>
            //{
            //    ImportSubjects(m.Value);
            //});

            //eventAggregator.GetEvent<UpdateVmMsgs.UpdateSuccessMsg>().Subscribe().Register<UpdateVmMsgs.UpdateSuccessMsg>(this, (r, m) =>
            //{
            //    DisciplineKeywordModels.Clear();
            //    Disciplines.Clear();
            //    SubjectModels.Clear();
            //    ReloadDisciplineAndKeyWord();
            //});

            _eventAggregator.GetEvent<ScheduleBlockMsgs.SelectedMsg>().Subscribe(value =>
            {
                if (value is SchoolClassBlock schoolClassBlock)
                {
                    SelectedSubjectModel = SubjectModels.FirstOrDefault(sm => sm.SubjectCode.Equals(schoolClassBlock.Id));
                }
            });

            //eventAggregator.GetEvent<AutoVmMsgs.SaveStoreMsg>().Subscribe().Register<AutoVmMsgs.SaveStoreMsg>(this, (r, m) =>
            //{
            //    SubjectModels.Clear();
            //    ComModels.Clear();

            //    Messenger.Send(new SearchVmMsgs.DelAllSubjectMsg());

            //    AddCommand.NotifyCanExecuteChanged();
            //    DeleteAllCommand.NotifyCanExecuteChanged();

            //    UpdateCreditTotal();
            //    UpdateSubjectAmount();

            //    foreach (CombinationModel item in m.Value)
            //    {
            //        ComModels.Add(item);
            //    }
            //});

            #endregion

            #region Pros
            DisciplineKeywordModels = new ObservableCollection<Keyword>();
            SubjectModels = new ObservableCollection<SubjectModel>();
            Disciplines = new ObservableCollection<Discipline>();
            FullMatchSearchingKeywords = new ObservableCollection<FullMatchSearchingKeyword>();
            SavedSchedules = new ObservableCollection<UserSchedule>();
            ComModels = new ObservableCollection<CombinationModel>();
            UserSchedules = new ObservableCollection<UserSchedule>();

            SearchText = string.Empty;
            IsUseCache = true;
            #endregion

            #region Commands
            AddCommand = new DelegateCommand(async () => await OnAdd(), () => !IsAlreadyDownloaded(SelectedKeyword));
            DeleteCommand = new DelegateCommand<SubjectModel>(OnDelete);
            ImportDialogCommand = new DelegateCommand(OpenScheduleBagDialog);
            DeleteAllCommand = new DelegateCommand(OnDeleteAll, () => SubjectModels.Any());
            GotoCourseCommand = new DelegateCommand<SubjectModel>(ExecuteGotoCourseCommand);
            
            CopyErrorCommand = new DelegateCommand<SubjectModel>(subjectModel =>
            {
                if (subjectModel.IsError)
                {
                    Clipboard.SetText(subjectModel.ErrorMessage);
                    _snackbarMessageQueue.Enqueue("Đã sao chép lỗi vào clipboard");
                }
            });
            ReloadCommand = new DelegateCommand<SubjectModel>(OnReload);
            
            #endregion

            LoadDiscipline();
            LoadSavedSchedules();

            InitClgViewModel();
            #region Messengers
            _eventAggregator.GetEvent<SearchVmMsgs.DelSubjectMsg>().Subscribe(payload =>
            {
                DelSubjectMsgHandler(payload);
            });

            _eventAggregator.GetEvent<SearchVmMsgs.DelAllSubjectMsg>().Subscribe(DelAllSubjectMsgHandler);
            _eventAggregator.GetEvent<SearchVmMsgs.SelectCgmsMsg>().Subscribe(payload =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    AddClassGroupModelsAndReload(payload);
                });
            });

            _eventAggregator.GetEvent<SolveConflictVmMsgs.RemoveChoicedClassMsg>().Subscribe(payload =>
            {
                //_dialogService.CloseDialog();
                RemoveChoosedClassMsgHandler(payload);
            });

            //eventAggregator.GetEvent<UpdateVmMsgs.UpdateSuccessMsg>().Subscribe(payload =>
            //{
            //    ClassGroupModels.Clear();
            //    ConflictModels.Clear();
            //    PlaceConflictFinderModels.Clear();
            //});

            // Click vào block thì đồng thời select class group model tương ứng.
            _eventAggregator.GetEvent<ScheduleBlockMsgs.SelectedMsg>().Subscribe(payload =>
            {
                if (payload.GetType() == typeof(SchoolClassBlock))
                {
                    var schoolClassBlock = (SchoolClassBlock)payload;
                    var result = SelectedClassGroupModels.FirstOrDefault(cgm => cgm.ClassGroup.Name.Equals(schoolClassBlock.SchoolClassUnit.SchoolClass.ClassGroupName));
                    if (result != null)
                    {
                        SelectedClassGroupModel = result;
                    }
                }
                else
                {
                    return;
                }
            });
            #endregion

            DeleteChooseCommand = new DelegateCommand(OnDelete, () => _selectedClassGroupModel != null);
            DeleteAllChooseCommand = new DelegateCommand(OnDeleteAllChoose, () => SelectedClassGroupModels.Count > 0);
            CopyCodeCommand = new DelegateCommand(OnCopyCode);
            SolveConflictCommand = new DelegateCommand(OnSolve);
            OpenShareStringWindowCommand = new DelegateCommand(OnOpenShareStringWindow);

            PlaceConflictFinderModels = new ObservableCollection<PlaceConflictFinderModel>();
            ConflictModels = new ObservableCollection<ConflictModel>();
            SelectedClassGroupModels = new ObservableCollection<ClassGroupModel>();
            SelectedClassGroupModels.CollectionChanged += SelectedClassGroupModels_CollectionChanged;

            InitSchedulerViewModel();
        }

        private void SelectedClassGroupModels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void ExecuteGotoCourseCommand(SubjectModel model)
        {
            string semesterValue = _unitOfWork.Settings.GetByKey(DbConsts.StCurrentSemesterValue);
            string url = $@"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid={model.CourseId}&timespan={semesterValue}&t=s";
            _openInBrowser.Open(url);
            _notificationService.SendNotification("Notification", $"Đang mở {model.SubjectName} trên trình duyệt", fromAction: "Goto course from search box");
        }

        private void OnSltCombiChanged(CombinationModel value)
        {
            if (value == null) return;
            var subjectModels = value.SubjectModels;
            SubjectModels.Clear();
            foreach (var sjm in subjectModels)
            {
                SubjectModels.Add(sjm);
            }

            // Đánh giá Phase Store xác định tuần ngăn cách
            foreach (var cgm in value.ClassGroupModels)
            {
                _eventAggregator.GetEvent<ClassGroupSessionVmMsgs.ClassGroupAddedMsg>().Publish(cgm);
            }

            SelectedSubjectModel = SubjectModels.FirstOrDefault();

            AddCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
        }

        private void OnSelectedSubjectModelChanged(SubjectModel value)
        {
            DeleteCommand.RaiseCanExecuteChanged();
            _eventAggregator.GetEvent<SearchVmMsgs.SelectedSubjectChangedMsg>().Publish(value);
        }

        private void OnSearchingKeywordChanged(FullMatchSearchingKeyword value)
        {
            if (value == null || value.Keyword == null || value.Discipline.DisciplineId == 0) return;
            var dcl = Disciplines.First(d => d.DisciplineId == value.Discipline.DisciplineId);
            SelectedDiscipline = dcl;
            SelectedKeyword = value.Keyword;
            SearchText = string.Empty;
            AddCommand.RaiseCanExecuteChanged();
            if (!IsAlreadyDownloaded(value.Keyword.CourseId))
            {
                Application.Current.Dispatcher.InvokeAsync(
                    async () =>
                    {
                        InsertPseudoSubject(value.Keyword);
                        await OnAddSubjectAsync(SelectedKeyword);
                    }
                );
            }
        }

        /// <summary>
        /// Load danh sách các bộ lịch đã lưu
        /// </summary>
        private void LoadSavedSchedules()
        {
            SavedSchedules.Clear();
            var sessions = _unitOfWork.UserSchedules.GetAll();
            foreach (var session in sessions)
            {
                SavedSchedules.Add(session);
            }
        }

        private void LoadDiscipline()
        {
            _searchDisciplines = _unitOfWork.Disciplines.GetAllIncludeKeyword();
            _searchKeywords = _searchDisciplines.SelectMany(d => d.Keywords).ToList();

            foreach (var discipline in _searchDisciplines)
            {
                Disciplines.Add(discipline);
            }
            SelectedDiscipline = Disciplines[0];
        }

        /// <summary>
        /// Tải lại môn học bị lỗi.
        /// </summary>
        /// <param name="subjectModel">SubjectModel</param>
        private async void OnReload(SubjectModel subjectModel)
        {
            subjectModel.IsDownloading = true;
            subjectModel.IsError = false;
            subjectModel.ErrorMessage = string.Empty;

            var kw = _unitOfWork.Keywords.GetKeywordBySubjectCode(subjectModel.CourseId);
            var ds = _unitOfWork.Disciplines.GetDisciplineByID(kw.DisciplineId);
            kw.Discipline = ds;
            if (subjectModel.UserSubject == null)
            {
                await OnAddSubjectAsync(kw);
            }
            else
            {
                await OnAddSubjectAsync(kw, subjectModel.UserSubject);
            }
        }

        private void LoadSearchItemSource(string text)
        {
            const int Maximum = 5;
            text = text.Trim();

            FullMatchSearchingKeywords.Clear();
            var keywords = _searchKeywords
                .Where(k =>
                       StringHelper.ReplaceVietnamese(k.SubjectName).ToLower()
                        .Contains(StringHelper.ReplaceVietnamese(text).ToLower())
                    || StringHelper.ReplaceVietnamese(k.Discipline.Name + k.Keyword1).ToLower()
                        .Contains(StringHelper.ReplaceVietnamese(text.Replace(" ", string.Empty)).ToLower())
                )
                .Take(Maximum)
                .AsParallel();
            foreach (var kw in keywords)
            {
                var fullMatch = new FullMatchSearchingKeyword()
                {
                    Keyword = kw,
                    Discipline = kw.Discipline
                };
                FullMatchSearchingKeywords.Add(fullMatch);
            }
        }

        private void OnDeleteAll()
        {
            _logger.LogInformation("User click on Delete All button");
            HandlerDelAllSubjectMsg();
            DelAllSubjectMsgHandler();
            CleanDays();
            SubjectModels.Clear();

            SelectedClassGroup = null;

            DeleteAllCommand.RaiseCanExecuteChanged();
            AddCommand.RaiseCanExecuteChanged();
            _notificationService.SendNotification(
                "Notification",
                "Đã xoá tất cả môn học",
                fromAction: "Delete all subject from search box"
            );
        }

        private async void OpenScheduleBagDialog()
        {
            var scheduleBag = new ScheduleBag();
            var vm = (ScheduleBagViewModel)scheduleBag.DataContext;
            //_dialogService.OpenDialog(scheduleBag);
            //await vm.LoadScheduleSession();
        }

        private async Task HandleImportSubjects(IEnumerable<UserSubject> userSubjects)
        {
            if (userSubjects == null) return;
            SubjectModels.Clear();
            HandlerDelAllSubjectMsg();
            DelAllSubjectMsgHandler();
            CleanDays();

            var keywords = userSubjects.Select(userSubject => _unitOfWork.Keywords.GetKeywordBySubjectCode(userSubject.SubjectCode)).ToList();

            InsertPseudoSubjects(keywords, userSubjects);

            var downloadTasks = new List<Task>();
            var listOfUserSubjects = userSubjects.ToList();
            for (var i = 0; i < keywords.Count; i++)
            {
                downloadTasks.Add(OnAddSubjectAsync(keywords[i], listOfUserSubjects[i]));
            }
            await Task.WhenAll(downloadTasks);
            SelectedSubjectModel = SubjectModels[0];
        }

        private void InsertPseudoSubject(Keyword keyword)
        {
            var pseudoSubjectModel = new SubjectModel(
                keyword.SubjectName,
                keyword.Discipline.Name + " " + keyword.Keyword1,
                keyword.Color,
                keyword.CourseId
            );
            SubjectModels.Insert(0, pseudoSubjectModel);
            AddCommand.RaiseCanExecuteChanged();
        }

        private void InsertPseudoSubjects(IReadOnlyList<Keyword> keywords, IEnumerable<UserSubject> userSubjects)
        {
            var userSubjectArr = userSubjects.ToArray();
            for (var i = 0; i < keywords.Count; i++)
            {
                var kw = keywords[i];
                kw.Discipline = _unitOfWork.Disciplines.GetDisciplineByID(kw.DisciplineId);
                var pseudoSubjectModel = new SubjectModel(
                    kw.SubjectName,
                    kw.Discipline.Name + " " + kw.Keyword1,
                    kw.Color,
                    kw.CourseId,
                    userSubjectArr[i]
                );
                SubjectModels.Insert(0, pseudoSubjectModel);
            }
        }

        /// <summary>
        /// Xoá môn học đã tải.
        /// </summary>
        /// <param name="sm">SubjectModel.</param>
        private void OnDelete(SubjectModel sm)
        {
            _eventAggregator.GetEvent<SearchVmMsgs.DelSubjectMsg>().Publish(sm);
            SubjectModels.Remove(sm);
            SelectedSubjectModel = null;
            AddCommand.RaiseCanExecuteChanged();
            _notificationService.SendNotification("Notification", "Đã xoá môn " + sm.SubjectName, fromAction: "Delete subject from search box");
        }

        /// <summary>
        /// Load Keyword sau khi chọn discipline.
        /// </summary>
        /// <param name="discipline">Discipline.</param>
        private void LoadKeywordByDiscipline(Discipline discipline)
        {
            DisciplineKeywordModels.Clear();
            var currentDiscipline = Disciplines.First(d => d.DisciplineId == discipline.DisciplineId);
            var keywords = currentDiscipline.Keywords;
            keywords.ForEach(keyword => DisciplineKeywordModels.Add(keyword));
            SelectedKeyword = DisciplineKeywordModels[0];
        }

        private async Task OnAdd()
        {
            InsertPseudoSubject(SelectedKeyword);
            await OnAddSubjectAsync(SelectedKeyword);
        }

        /// <summary>
        /// Thêm một task tải Subject.
        /// </summary>
        /// <remarks>
        /// 1. Thực hiện tải Subject.
        /// <br></br>
        /// 2. Thông báo nếu Subject không tồn tại, ngược lại thay thế Pseudo Subject bằng Subject đã tải được. 
        /// <br></br>
        /// 3. Nếu không có Subject nào đang được tải, thực hiện select Subject đầu tiên trong danh sách. 
        /// <br></br>
        /// 4. Thực hiện tính lại tổng Subject, tổng tín chỉ, số lượng môn học. Và trả về Subject Model đã tải được. 
        /// <br></br>
        /// 5. Bất kỳ lỗi nào xuất hiện trong quá trình này, thêm message lỗi vào pseudo subject và trả về null.
        /// </remarks>
        /// <param name="keyword">Keyword</param>
        /// <returns>Task</returns>
        private async Task<SubjectModel> OnAddSubjectAsync(Keyword keyword)
        {
            try
            {
                // 1. Thực hiện tải Subject. 
                var subjectModel = await DownloadSubject(keyword, IsUseCache);

                // 2. Thông báo nếu Subject không tồn tại, ngược lại thay thế Pseudo Subject bằng Subject đã tải được. 
                if (subjectModel == null)
                {
                    _snackbarMessageQueue.Enqueue($"Không tìm thấy môn {keyword.SubjectName} trong học kỳ này");
                    return null;
                }

                ReplacePseudoSubject(subjectModel);

                // 3. Nếu không có Subject nào đang được tải, thực hiện select Subject đầu tiên trong danh sách. 
                if (!SubjectModels.Any(sm => sm.IsDownloading))
                {
                    SelectedSubjectModel = subjectModel;
                }

                // 4. Trả về Subject Model đã tải được. 
                return subjectModel;
            }
            catch (Exception e)
            {
                // 5. Bất kỳ lỗi nào xuất hiện trong quá trình này, thêm message lỗi vào pseudo subject và trả về null.
                AddErrorToPseudoSubject(e.Message, keyword.CourseId);
                return null;
            }
            finally
            {
                AddCommand.RaiseCanExecuteChanged();
                DeleteAllCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Xử lý Add Subject từ bộ lịch đã lưu.
        /// </summary>
        /// <param name="keyword">Keyword</param>
        /// <param name="userSubject">UserSubject</param>
        private async Task OnAddSubjectAsync(Keyword keyword, UserSubject userSubject)
        {
            var subjectModel = await OnAddSubjectAsync(keyword);
            if (subjectModel == null) return;
            // Lấy ra ClassGroupModel có tên bằng với tên đã lưu.
            var classGroupModel = subjectModel
                .ClassGroupModels
                .First(cgm => cgm.Name.Equals(userSubject.ClassGroup));
            if (subjectModel.IsSpecialSubject)
            {
                classGroupModel.PickSchoolClass(userSubject.SchoolClass);
            }

            var cgms = new List<ClassGroupModel>()
            {
                classGroupModel
            };
            _eventAggregator.GetEvent<SearchVmMsgs.SelectCgmsMsg>().Publish(cgms);
        }

        private async Task<SubjectModel> DownloadSubject(Keyword keyword, bool isUseCache)
        {
            return await Task.Run(async () =>
            {
                Subject subject;
                // 1. Sử dụng cache và có sẵn cache trong DB.
                if (isUseCache && !string.IsNullOrWhiteSpace(keyword.Cache))
                {
                    subject = _subjectCrawler.CrawlFromCache(keyword.Cache, keyword.CourseId);
                }
                // 2. Không sử dụng cache
                else
                {
                    string cache;
                    var semester = _unitOfWork.Settings.GetByKey(DbConsts.StCurrentSemesterValue);
                    (subject, cache) = await _subjectCrawler.Crawl(keyword.CourseId, semester);
                    // 2.2. Cập nhật lại cache
                    _unitOfWork.Keywords.UpdateCacheByKeywordId(keyword.KeywordId, cache);
                    // 2.3. Cập nhật lên local
                    keyword.Cache = cache;
                }
                if (subject is null)
                {
                    return null;
                }
                else
                {
                    return new SubjectModel(subject, keyword.Color);
                }
            });
        }

        public async void OnAddSubjectFromUriAsync(Uri uri)
        {
            var queries = HttpUtility.ParseQueryString(uri.Query);
            var courseId = queries.Get("courseid");
            var p = queries.Get("p");
            var timespan = queries.Get("timespan");
            var t = queries.Get("t");

            var isDtuCourseHost = uri.Host == "courses.duytan.edu.vn";
            var isRightAbsPath = uri.AbsolutePath == "/Sites/Home_ChuongTrinhDaoTao.aspx";

            if (
                    courseId != null
                 && p != null
                 && timespan != null
                 && t != null
                 && isDtuCourseHost
                 && isRightAbsPath
            )
            {
                if (IsAlreadyDownloaded(courseId))
                {
                    _snackbarMessageQueue.Enqueue("Môn này đã được tải");
                    return;
                }

                var keyword = _unitOfWork.Keywords.GetByCourseId(courseId);
                if (keyword == null)
                {
                    _snackbarMessageQueue.Enqueue($"Không tồn tại {courseId}");
                    return;
                }

                InsertPseudoSubject(keyword);
                await OnAddSubjectAsync(keyword);
            }
            else
            {
                _snackbarMessageQueue.Enqueue("Sai đường dẫn");
            }
        }

        /// <summary>
        /// Kiếm tra xem rằng một Subject đã có 
        /// sẵn trong danh sách đã tải xuống hay chưa.
        /// </summary>
        /// <param name="courseId">Course ID</param>
        private bool IsAlreadyDownloaded(string courseId)
        {
            var courseIds = SubjectModels.Select(item => item.CourseId);
            return courseIds.Contains(courseId);
        }

        private bool IsAlreadyDownloaded(Keyword keyword)
        {
            if (keyword != null)
            {
                var courseIds = SubjectModels.Select(item => item.CourseId);
                return courseIds.Contains(keyword.CourseId);
            }
            return true;
        }

        private void ImportSubjects(CombinationModel combinationModel)
        {
            foreach (var subject in SubjectModels)
            {
                _eventAggregator.GetEvent<SearchVmMsgs.DelSubjectMsg>().Publish(subject);
            }
            SubjectModels.Clear();

            foreach (var subject in combinationModel.SubjectModels)
            {
                SubjectModels.Add(subject);
            }
            AddCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
            foreach (var classGroupModel in combinationModel.ClassGroupModels)
            {
                _eventAggregator.GetEvent<ClassGroupSessionVmMsgs.ClassGroupAddedMsg>().Publish(classGroupModel);
            }
        }

        /// <summary>
        /// Hoàn tác sau khi xoá
        /// </summary>
        private void AddSubjectWithCgm(Tuple<List<SubjectModel>, List<ClassGroupModel>> actionData)
        {
            var (subjectModels, classes) = actionData;

            /* 1. Loại bỏ subject trùng lặp 
             * Trong trường hợp người dùng xoá môn học, sau đó thêm lại môn học đó
             * sau đó nhấn vào nút Hoàn tác.
             */
            SubjectComparer subjectComparer = new SubjectComparer();
            for (int i = 0; i < SubjectModels.Count; i++)
            {
                foreach (var subject in subjectModels)
                {
                    if (subjectComparer.Equals(SubjectModels[i], subject))
                    {
                        SubjectModels.RemoveAt(i);
                    }
                }
            }

            SubjectModels.AddRange(subjectModels);
            SelectedSubjectModel = subjectModels.First();

            AddCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();

            if (actionData.Item2 != null)
            {
                _eventAggregator.GetEvent<SearchVmMsgs.SelectCgmsMsg>().Publish(classes);
            }
        }

        /// <summary>
        /// Thay thế Pseudo Subject bằng Subject được tải xuống.
        /// </summary>
        /// <param name="subjectModel">SubjectModel</param>
        private void ReplacePseudoSubject(SubjectModel subjectModel)
        {
            var pseudoSubject = SubjectModels.First(sm => sm.CourseId.Equals(subjectModel.CourseId));
            pseudoSubject.AssignData(subjectModel);
        }

        /// <summary>
        /// Thêm message lỗi khi quá trình tải Subject bị lỗi.
        /// </summary>
        /// <param name="errMsg">Error Message</param>
        /// <param name="courseId">Course ID</param>
        private void AddErrorToPseudoSubject(string errMsg, string courseId)
        {
            for (var i = 0; i < SubjectModels.Count; i++)
            {
                if (SubjectModels[i].CourseId.Equals(courseId))
                {
                    var subjectMd = SubjectModels[i];
                    subjectMd.IsError = true;
                    subjectMd.IsDownloading = false;
                    subjectMd.ErrorMessage = errMsg;
                    return;
                }
            }
        }


        #region Class group view model
        #region Properties
        public ObservableCollection<ClassGroupModel> ClassGroupModels { get; set; }
        public ObservableCollection<string> TeacherNames { get; set; }

        private ICollectionView _classGroupModelsView;

        private ClassGroupModel _selectedClassGroup;
        public ClassGroupModel SelectedClassGroup
        {
            get { return _selectedClassGroup; }
            set { SetProperty(ref _selectedClassGroup, value); OnSelectedClassGroupChanged(value); }
        }

        private int _teacherCount;
        public int TeacherCount
        {
            get { return _teacherCount; }
            set { SetProperty(ref _teacherCount, value); }
        }

        private string _selectedTeacherName;
        public string SelectedTeacherName
        {
            get { return _selectedTeacherName; }
            set { SetProperty(ref _selectedTeacherName, value); OnFilter(); }
        }

        private SubjectModel _selectedSubject;
        public SubjectModel SelectedSubject
        {
            get { return _selectedSubject; }
            set { SetProperty(ref _selectedSubject, value); }
        }

        private bool _anyTeacherName;
        public bool AnyTeacherName
        {
            get { return _anyTeacherName; }
            set { SetProperty(ref _anyTeacherName, value); }
        }
        #endregion

        #region Day Filters
        private bool _monday;
        private bool _tuesday;
        private bool _wednesday;
        private bool _thursday;
        private bool _friday;
        private bool _saturday;
        private bool _sunday;
        public bool Monday { get => _monday; set { SetProperty(ref _monday, value); OnFilter(); } }
        public bool Tuesday { get => _tuesday; set { SetProperty(ref _tuesday, value); OnFilter(); } }
        public bool Wednesday { get => _wednesday; set { SetProperty(ref _wednesday, value); OnFilter(); } }
        public bool Thursday { get => _thursday; set { SetProperty(ref _thursday, value); OnFilter(); } }
        public bool Friday { get => _friday; set { SetProperty(ref _friday, value); OnFilter(); } }
        public bool Saturday { get => _saturday; set { SetProperty(ref _saturday, value); OnFilter(); } }
        public bool Sunday { get => _sunday; set { SetProperty(ref _sunday, value); OnFilter(); } }
        #endregion

        #region Seat Filters
        private bool _hasSeat;
        public bool HasSeat { get => _hasSeat; set { SetProperty(ref _hasSeat, value); OnFilter(); } }

        private bool _hasSchedule;
        public bool HasSchedule { get => _hasSchedule; set { SetProperty(ref _hasSchedule, value); OnFilter(); } }
        #endregion

        #region Session Filters
        private bool _morning;
        public bool Morning { get => _morning; set { SetProperty(ref _morning, value); OnFilter(); } }

        private bool _afternoon;
        public bool Afternoon { get => _afternoon; set { SetProperty(ref _afternoon, value); OnFilter(); } }

        private bool _night;
        public bool Night { get => _night; set { SetProperty(ref _night, value); OnFilter(); } }
        #endregion

        #region Phase Filters
        private bool _onlyPhaseFirst;
        public bool PhaseFirst { get => _onlyPhaseFirst; set { SetProperty(ref _onlyPhaseFirst, value); OnFilter(); } }

        private bool _onlyPhaseSecond;
        public bool PhaseSecond { get => _onlyPhaseSecond; set { SetProperty(ref _onlyPhaseSecond, value); OnFilter(); } }
        public bool PhaseBoth { get => _bothPhase; set { SetProperty(ref _bothPhase, value); OnFilter(); } }

        private bool _bothPhase;
        #endregion

        #region Place Filters
        private bool _placeHoaKhanh;
        private bool _placePhanThanh;
        private bool _placeVietTin;
        private bool _place137NVL;
        private bool _place254NVL;
        private bool _placeOnline;
        private bool _placeQuangTrung;
        public bool PlaceHoaKhanh { get => _placeHoaKhanh; set { SetProperty(ref _placeHoaKhanh, value); OnFilter(); } }
        public bool PlacePhanThanh { get => _placePhanThanh; set { SetProperty(ref _placePhanThanh, value); OnFilter(); } }
        public bool PlaceVietTin { get => _placeVietTin; set { SetProperty(ref _placeVietTin, value); OnFilter(); } }
        public bool Place137NVL { get => _place137NVL; set { SetProperty(ref _place137NVL, value); OnFilter(); } }
        public bool Place254NVL { get => _place254NVL; set { SetProperty(ref _place254NVL, value); OnFilter(); } }
        public bool PlaceOnline { get => _placeOnline; set { SetProperty(ref _placeOnline, value); OnFilter(); } }
        public bool PlaceQuangTrung { get => _placeQuangTrung; set { SetProperty(ref _placeQuangTrung, value); OnFilter(); } }
        #endregion

        #region Commands
        public DelegateCommand GotoCourseClassCommand { get; set; }
        public DelegateCommand ShowDetailsSchoolClassesCommand { get; set; }
        public DelegateCommand FilterCommand { get; set; }
        public DelegateCommand ResetFilterCommand { get; set; }
        #endregion

        void InitClgViewModel()
        {
            _eventAggregator.GetEvent<SearchVmMsgs.DelSubjectMsg>().Subscribe(sujectModel =>
            {
                ClassGroupModels.Clear();
                SelectedSubject = null;
            });

            //_eventAggregator.GetEvent<SearchVmMsgs.DelAllSubjectMsg>().Subscribe(n =>
            //{
            //    ClassGroupModels.Clear();
            //    TeacherCount = 0;
            //    SelectedSubject = null;
            //});

            _eventAggregator.GetEvent<SearchVmMsgs.DelAllSubjectMsg>().Subscribe(HandlerDelAllSubjectMsg);

            _eventAggregator.GetEvent<SearchVmMsgs.SelectedSubjectChangedMsg>().Subscribe(SelectedSubjectChangedHandler);

            _eventAggregator.GetEvent<ChoosedVmMsgs.DelClassGroupChoiceMsg>().Subscribe(cgm =>
            {
                SelectedClassGroup = null;
            });

            //_eventAggregator.GetEvent<UpdateVmMsgs.UpdateSuccessMsg>().Subscribe(this, (r, m) =>
            //{
            //    ClassGroupModels.Clear();
            //    Teachers.Clear();
            //});

            // Xử lý sự kiện chọn SchoolClass trong một ClassGroup thuộc Special Subject
            _eventAggregator.GetEvent<ShowDetailsSchoolClassesVmMsgs.ExitChooseMsg>().Subscribe(payload =>
            {
                var classGroupModel = payload.ClassGroupModel;
                var schoolClassName = payload.SelectedSchoolClassModel.SchoolClassName;
                classGroupModel.ReRenderSchedule(schoolClassName);
                _eventAggregator.GetEvent<ClassGroupSessionVmMsgs.ClassGroupAddedMsg>().Publish(classGroupModel);
            });

            ClassGroupModels = new ObservableCollection<ClassGroupModel>();
            _classGroupModelsView = CollectionViewSource.GetDefaultView(ClassGroupModels);
            _classGroupModelsView.Filter = ClassGroupFilter;

            TeacherCount = 0;
            AnyTeacherName = true;
            TeacherNames = new ObservableCollection<string>();
            GotoCourseClassCommand = new DelegateCommand(OnGotoCourse);
            ShowDetailsSchoolClassesCommand = new DelegateCommand(OnShowDetailsSchoolClasses);
            FilterCommand = new DelegateCommand(OnFilter);
            ResetFilterCommand = new DelegateCommand(OnResetFilter, CanResetFilter);

            InitFilter();
        }

        private void HandlerDelAllSubjectMsg()
        {
            ClassGroupModels.Clear();
            TeacherCount = 0;
            SelectedSubject = null;
        }

        /// <summary>
        /// Xử lý sự kiện chọn một ClassGroupModel
        /// </summary>
        /// <param name="value"></param>
        private void OnSelectedClassGroupChanged(ClassGroupModel value)
        {
            if (value != null)
            {
                if (value.IsBelongSpecialSubject)
                {
                    // TODO: Haven't test yet, need to test before publish.
                    _logger.LogInformation("User select class group {classGroupName} which belong to special subject, open details school class window", value.Name);
                    OnShowDetailsSchoolClasses();
                }
                else
                {
                    AddClassGroupModel(value);
                    RemoveScheduleItem(value.SubjectCode);
                    AddClassGroup(value);
                    _logger.LogInformation("Add block to schedule with class group {classGroupName}", value.Name);
                }
            }
        }

        private bool CanResetFilter()
        {
            return
            Monday
            || Tuesday
            || Wednesday
            || Thursday
            || Friday
            || Saturday
            || Sunday

            || Place137NVL
            || Place254NVL
            || PlaceHoaKhanh
            || PlacePhanThanh
            || PlaceQuangTrung
            || PlaceVietTin
            || PlaceOnline

            || PhaseFirst
            || PhaseSecond
            || PhaseBoth

            || HasSeat
            || HasSchedule
            || Morning
            || Afternoon
            || Night;
        }

        private bool ClassGroupFilter(object obj)
        {
            var classGroupModel = obj as ClassGroupModel;
            return CheckDayOfWeek(classGroupModel)
                && CheckSession(classGroupModel)
                && CheckSeat(classGroupModel)
                && CheckSchedule(classGroupModel)
                && CheckPhase(classGroupModel)
                && CheckTeacher(classGroupModel)
                && CheckPlace(classGroupModel);
        }

        /// <summary>
        /// Khởi tạo bộ lọc mặc định.
        /// </summary>
        private void InitFilter()
        {
            Monday = Tuesday = Wednesday = Thursday = Friday = Saturday = Sunday =
            Place137NVL = Place254NVL = PlaceHoaKhanh = PlacePhanThanh = PlaceQuangTrung = PlaceVietTin = PlaceOnline =
            PhaseFirst = PhaseSecond = PhaseBoth =
            Morning = Afternoon = Night = false;
            HasSeat = HasSchedule = true;
        }

        /// <summary>
        /// Đặt lại bộ lọc.
        /// </summary>
        private void OnResetFilter()
        {
            Monday = Tuesday = Wednesday = Thursday = Friday = Saturday = Sunday =
            Place137NVL = Place254NVL = PlaceHoaKhanh = PlacePhanThanh = PlaceQuangTrung = PlaceVietTin = PlaceOnline =
            PhaseFirst = PhaseSecond = PhaseBoth =
            Morning = Afternoon = Night =
            HasSeat = HasSchedule = false;
        }

        /// <summary>
        /// Thực hiện lọc.
        /// </summary>
        private void OnFilter()
        {
            _classGroupModelsView.Refresh();
            ResetFilterCommand.RaiseCanExecuteChanged();
        }

        private bool CheckSeat(ClassGroupModel classGroupModel)
        {
            if (HasSeat)
            {
                return classGroupModel.EmptySeat > 0;
            }
            return true;
        }

        private bool CheckSchedule(ClassGroupModel classGroupModel)
        {
            if (HasSchedule)
            {
                return classGroupModel.HaveSchedule;
            }
            return true;
        }

        private bool CheckTeacher(ClassGroupModel classGroupModel)
        {
            try
            {
                return SelectedTeacherName == "TẤT CẢ"
                || classGroupModel.TeacherNames.Contains(SelectedTeacherName);
            }
            catch
            {
                return true;
            }
        }

        private bool CheckDayOfWeek(ClassGroupModel classGroupModel)
        {
            if (Monday == false &&
                Tuesday == false &&
                Wednesday == false &&
                Thursday == false &&
                Friday == false &&
                Saturday == false &&
                Sunday == false)
            {
                return true;
            }

            var checkedDates = new Dictionary<DayOfWeek, bool>()
            {
                { DayOfWeek.Monday, Monday },
                { DayOfWeek.Tuesday, Tuesday },
                { DayOfWeek.Wednesday, Wednesday },
                { DayOfWeek.Thursday, Thursday },
                { DayOfWeek.Friday, Friday },
                { DayOfWeek.Saturday, Saturday },
                { DayOfWeek.Sunday, Sunday }
            };

            checkedDates = checkedDates
                .Where(pair => pair.Value)
                .ToDictionary(p => p.Key, p => p.Value);

            foreach (var day in checkedDates.Keys)
            {
                if (!classGroupModel.Schedule.GetSchoolDays().Contains(day))
                    return false;
            }
            return true;
        }

        private bool CheckSession(ClassGroupModel classGroupModel)
        {
            if (Afternoon == false &&
                Morning == false &&
                Night == false)
                return true;

            var checkSessions = new Dictionary<Session, bool>()
            {
                { Session.Morning, Morning },
                { Session.Afternoon, Afternoon },
                { Session.Night, Night }
            };

            checkSessions = checkSessions
                .Where(pair => pair.Value == true)
                .ToDictionary(p => p.Key, p => p.Value);

            foreach (var session in checkSessions.Keys)
            {
                if (!classGroupModel.Schedule.GetSessions().Contains(session))
                    return false;
            }
            return true;
        }

        private bool CheckPhase(ClassGroupModel classGroupModel)
        {
            if (PhaseBoth == false && PhaseFirst == false && PhaseSecond == false)
            {
                return true;
            }
            if (_onlyPhaseFirst)
                return classGroupModel.Phase == Phase.First;
            else if (_onlyPhaseSecond)
                return classGroupModel.Phase == Phase.Second;
            else
                return classGroupModel.Phase == Phase.All;
        }

        private bool CheckPlace(ClassGroupModel classGroupModel)
        {
            if (_placeQuangTrung == false
                && _placeVietTin == false
                && _placePhanThanh == false
                && _placeHoaKhanh == false
                && _place137NVL == false
                && _place254NVL == false
                && _placeOnline == false)
                return true;
            var checkboxAndPlace = new Dictionary<Place, bool>()
            {
                { Place.QuangTrung, _placeQuangTrung },
                { Place.VietTin, _placeVietTin },
                { Place.PhanThanh, _placePhanThanh },
                { Place.HoaKhanh, _placeHoaKhanh },
                { Place.Nvl137, _place137NVL },
                { Place.Nvl254, _place254NVL },
                { Place.Online, _placeOnline }
            };
            checkboxAndPlace = checkboxAndPlace
                .Where(pair => pair.Value)
                .ToDictionary(p => p.Key, p => p.Value);
            foreach (var place in checkboxAndPlace.Keys)
            {
                if (!classGroupModel.Places.Contains(place))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Hiển thị chi tiết của một SchoolClass
        /// </summary>
        public void OnShowDetailsSchoolClasses()
        {
            var showDetailsSchoolClassesUC = new ShowDetailsSchoolClassesUC();
            var vm = (ShowDetailsSchoolClassesViewModel)showDetailsSchoolClassesUC.DataContext;
            vm.ClassGroupModel = SelectedClassGroup;

            foreach (var scm in SelectedClassGroup.NormalSchoolClassModels)
            {
                if (scm.Type != SelectedClassGroup.CompulsoryClass.Type)
                {
                    vm.SchoolClassModels.Add(scm);
                }
            }
            //_dialogService.OpenDialog(showDetailsSchoolClassesUC);
        }

        private void OnGotoCourse()
        {
            var url = SelectedClassGroup.ClassGroup.GetUrl();
            _openInBrowser.Open(url);
        }

        private void SelectedSubjectChangedHandler(SubjectModel subjectModel)
        {
            AnyTeacherName = subjectModel.TeacherNames.Count > 0;
            SelectedSubject = subjectModel;
            ClassGroupModels.Clear();
            TeacherNames.Clear();

            if (SelectedSubject != null
                && SelectedSubject.ClassGroupModels != null)
            {
                foreach (var classGroupModel in SelectedSubject.ClassGroupModels)
                {
                    // Thêm danh sách các lớp học của môn đã chọn
                    ClassGroupModels.Add(classGroupModel);
                }

                TeacherNames.Add("TẤT CẢ");
                foreach (var teacherName in SelectedSubject.TeacherNames)
                {
                    if (teacherName != null && !TeacherNames.Contains(teacherName))
                    {
                        // Thêm danh sách giảng viên hiện tại
                        TeacherNames.Add(teacherName);
                    }
                }

                SelectedTeacherName = TeacherNames[0];

                // Count teacher exclude ALL option
                TeacherCount = TeacherNames.Count - 1;
                _classGroupModelsView.Refresh();
            }
        }
        #endregion


        #region Choose view model
        private readonly List<ClassGroupModel> _undoClassGroupModels = new List<ClassGroupModel>();

        #region Properties
        /// <summary>
        /// Danh sách các ClassGroupModel đã chọn để hiển thị ở phần Lịch đã chọn.
        /// </summary>
        public ObservableCollection<ClassGroupModel> SelectedClassGroupModels { get; set; }

        private ClassGroupModel _selectedClassGroupModel;
        public ClassGroupModel SelectedClassGroupModel
        {
            get => _selectedClassGroupModel;
            set
            {
                SetProperty(ref _selectedClassGroupModel, value);
                DeleteChooseCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<ConflictModel> ConflictModels { get; set; }

        private ConflictModel _selectedConflictModel;
        public ConflictModel SelectedConflictModel
        {
            get { return _selectedConflictModel; }
            set { SetProperty(ref _selectedConflictModel, value); }
        }

        public ObservableCollection<PlaceConflictFinderModel> PlaceConflictFinderModels { get; set; }
        #endregion

        #region Commands
        public DelegateCommand OpenShareStringWindowCommand { get; set; }
        public DelegateCommand DeleteChooseCommand { get; set; }

        public DelegateCommand CopyCodeCommand { get; set; }
        public DelegateCommand SolveConflictCommand { get; set; }
        #endregion

        private void InitChooseViewModel()
        {
            
        }

        /// <summary>
        /// Giải quyết xung đột
        /// </summary>
        private void OnSolve()
        {
            var solveConflictUc = new SolveConflictUC();
            var vm = new SolveConflictViewModel(SelectedConflictModel, _unitOfWork, _eventAggregator);
            solveConflictUc.DataContext = vm;
            //_dialogService.OpenDialog(solveConflictUc);
        }

        /// <summary>
        /// Sao chép mã môn
        /// </summary>
        private void OnCopyCode()
        {
            if (SelectedClassGroupModel.RegisterCodes.Count > 0)
            {
                var registerCode = SelectedClassGroupModel.RegisterCodes.First();
                Clipboard.SetData(DataFormats.Text, registerCode);
                _notificationService.SendNotification("Sao chép mã đăng ký", $"Đã sao chép mã đăng ký {registerCode} vào clipboard", "CopyRegisterCode");
            }
            else
            {
                _notificationService.SendNotification("Sao chép mã đăng ký", $"Lớp {SelectedClassGroupModel.Name} không có mã đăng ký để sao chép", "CopyRegisterCode");
            }
        }

        /// <summary>
        /// Xoá tất cả
        /// </summary>
        private void OnDeleteAllChoose()
        {
            // 1. Reset undo data
            _undoClassGroupModels.Clear();
            foreach (var classGroupModel in SelectedClassGroupModels)
            {
                _undoClassGroupModels.Add(classGroupModel.DeepClone());
            }

            SelectedClassGroupModels.Clear();
            UpdateConflicts();
            DeleteAllChooseCommand.RaiseCanExecuteChanged();
            CleanDays();
        }

        private void OnDelete()
        {
            var actionData = _selectedClassGroupModel.DeepClone();
            _eventAggregator.GetEvent<DelClassGroupChoiceMsg>().Publish(_selectedClassGroupModel);

            SelectedClassGroupModels.Remove(_selectedClassGroupModel);
            _snackbarMessageQueue.Enqueue(
                $"Đã xoá lớp {_selectedClassGroupModel.Name}",
                "HOÀN TÁC",
                (obj) => AddClassGroupModel(actionData),
                actionData
            );

            DeleteAllChooseCommand.RaiseCanExecuteChanged();
            DeleteChooseCommand.RaiseCanExecuteChanged();
            UpdateConflicts();
        }

        /// <summary>
        /// Mở Dialog lưu session
        /// </summary>
        /// <returns>Task</returns>
        private void OpenSaveDialog()
        {
            var saveSessionUc = new SaveSessionUC();
            var vm = (SaveSessionUCViewModel)saveSessionUc.DataContext;
            vm.ClassGroupModels = SelectedClassGroupModels;
            //_dialogService.OpenDialog(saveSessionUc);
        }

        private void OnOpenShareStringWindow()
        {
            //var shareStringUc = new ShareStringUC(); // TODO: Chưa set view model
            //var vm = shareStringUc.DataContext as ShareStringUCViewModel;
            //vm.ShareString = _shareStringGenerator.GetShareString(SelectedClassGroupModels); ;
            //_dialogService.OpenDialog(shareStringUc);
        }

        /// <summary>
        /// Kiểm tra xem một Class Group Model nào đó có tồn tại một
        /// phiên bản cùng Subject ClassGroupName nhưng khác tên khác không.
        /// </summary>
        /// <param name="classGroupModel">Một Class Group Model.</param>
        /// <returns>Trả về index của ClassGroupModel nếu nó có SubjectCode
        /// bằng với ClassGroupModel được truyền vào nếu không trả về -1.</returns>
        private int IsReallyHaveAnotherVersionInChoicedList(ClassGroupModel classGroupModel)
        {
            for (var i = 0; i < SelectedClassGroupModels.Count; ++i)
            {
                if (SelectedClassGroupModels[i].SubjectCode.Equals(classGroupModel.SubjectCode))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Thực hiện bắt cặp tất cả các ClassGroupModel có 
        /// trong Collection để phát hiện các Conflict Time.
        /// </summary>
        private void UpdateConflictModelCollection(List<SchoolClassModel> schoolClassModels)
        {
            ConflictModels.Clear();
            for (var i = 0; i < schoolClassModels.Count; ++i)
            {
                for (var k = i + 1; k < schoolClassModels.Count; ++k)
                {
                    if (schoolClassModels[i].SchoolClass.ClassGroupName
                        .Equals(schoolClassModels[k].SchoolClass.ClassGroupName))
                    {
                        continue;
                    }

                    var lessonA = new Lesson(
                            schoolClassModels[i].StudyWeek,
                            schoolClassModels[i].Schedule,
                            schoolClassModels[i].DayPlaceMetaData,
                            schoolClassModels[i].SchoolClass.GetMetaDataMap(),
                            schoolClassModels[i].Phase,
                            schoolClassModels[i].SchoolClassName,
                            schoolClassModels[i].SchoolClass.ClassGroupName,
                            schoolClassModels[i].SubjectCode
                    );

                    var lessonB = new Lesson(
                            schoolClassModels[k].StudyWeek,
                            schoolClassModels[k].Schedule,
                            schoolClassModels[k].DayPlaceMetaData,
                            schoolClassModels[k].SchoolClass.GetMetaDataMap(),
                            schoolClassModels[k].Phase,
                            schoolClassModels[k].SchoolClassName,
                            schoolClassModels[k].SchoolClass.ClassGroupName,
                            schoolClassModels[k].SubjectCode
                    );

                    var conflict = new Conflict(lessonA, lessonB);
                    var conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                    {
                        var conflictModel = new ConflictModel(conflict);
                        ConflictModels.Add(conflictModel);
                    }
                }
            }
            _eventAggregator.GetEvent<ConflictCollChangedMsg>().Publish(ConflictModels);
        }

        /// <summary>
        /// Thực hiện bắt cặp tất cả các ClassGroupModel có 
        /// trong Collection để phát hiện các Conflict Place.
        /// </summary>
        private void UpdatePlaceConflictCollection(List<SchoolClassModel> schoolClassModels)
        {
            PlaceConflictFinderModels.Clear();
            for (var i = 0; i < schoolClassModels.Count; ++i)
            {
                for (var k = i + 1; k < schoolClassModels.Count; ++k)
                {
                    var lessonA = new Lesson(
                        schoolClassModels[i].StudyWeek,
                        schoolClassModels[i].Schedule,
                        schoolClassModels[i].DayPlaceMetaData,
                        schoolClassModels[i].SchoolClass.GetMetaDataMap(),
                        schoolClassModels[i].Phase,
                        schoolClassModels[i].SchoolClassName,
                        schoolClassModels[i].SchoolClass.ClassGroupName,
                        schoolClassModels[i].SubjectCode
                    );

                    var lessonB = new Lesson(
                        schoolClassModels[k].StudyWeek,
                        schoolClassModels[k].Schedule,
                        schoolClassModels[k].DayPlaceMetaData,
                        schoolClassModels[k].SchoolClass.GetMetaDataMap(),
                        schoolClassModels[k].Phase,
                        schoolClassModels[k].SchoolClassName,
                        schoolClassModels[k].SchoolClass.ClassGroupName,
                        schoolClassModels[k].SubjectCode
                    );

                    var placeConflict = new PlaceConflictFinder(lessonA, lessonB);
                    var conflictPlace = placeConflict.GetPlaceConflict();
                    if (conflictPlace != null)
                    {
                        var placeConflictModel = new PlaceConflictFinderModel(placeConflict);
                        PlaceConflictFinderModels.Add(placeConflictModel);
                    }
                }
            }
            _eventAggregator.GetEvent<PlaceConflictCollChangedMsg>().Publish(PlaceConflictFinderModels);
        }

        private void AddClassGroupModel(ClassGroupModel classGroupModel)
        {
            if (classGroupModel != null)
            {
                var classGroupModelIndex = IsReallyHaveAnotherVersionInChoicedList(classGroupModel);
                if (classGroupModelIndex != -1)
                    SelectedClassGroupModels[classGroupModelIndex] = classGroupModel;
                else
                    SelectedClassGroupModels.Add(classGroupModel);
            }
            UpdateConflicts();
            DeleteAllChooseCommand.RaiseCanExecuteChanged();
        }

        private void AddClassGroupModelsAndReload(IEnumerable<ClassGroupModel> classGroupModels)
        {
            foreach (var classGroupModel in classGroupModels)
            {
                SelectedClassGroupModels.Add(classGroupModel);
            }
            UpdateConflicts();
            DeleteAllChooseCommand.RaiseCanExecuteChanged();
        }

        private void UpdateShareString()
        {

        }

        private void UpdateConflicts()
        {
            var schoolClasses = new List<SchoolClassModel>();
            foreach (var classGroupModel in SelectedClassGroupModels)
            {
                if (classGroupModel.IsSpecialClassGroup)
                {
                    schoolClasses.AddRange(classGroupModel.CurrentSchoolClassModels);
                }
                else
                {
                    schoolClasses.AddRange(classGroupModel.CurrentSchoolClassModels);
                    //schoolClassModels.AddRange(classGroupModel.ClassGroup.SchoolClasses);
                }
            }
            UpdateConflictModelCollection(schoolClasses);
            UpdatePlaceConflictCollection(schoolClasses);
        }

        #region Handlers | Xử lý các message được gửi đến

        /// <summary>
        /// Xử lý Remove Choiced Class Message
        /// </summary>
        private void RemoveChoosedClassMsgHandler(string className)
        {
            if (className == string.Empty || className == null)
            {
                _snackbarMessageQueue.Enqueue("Tên lớp cần bỏ chọn không hợp lệ");
                return;
            }
            ClassGroupModel actionData;
            for (var i = 0; i < SelectedClassGroupModels.Count; i++)
            {
                if (SelectedClassGroupModels[i].Name == className)
                {
                    actionData = SelectedClassGroupModels[i].DeepClone();
                    _eventAggregator.GetEvent<DelClassGroupChoiceMsg>().Publish(SelectedClassGroupModels[i]);
                    SelectedClassGroupModels.RemoveAt(i);
                    _snackbarMessageQueue.Enqueue(
                        $"Đã xoá {className}",
                        "HOÀN TÁC",
                        obj => AddClassGroupModel(actionData),
                        actionData
                    );

                    DeleteAllChooseCommand.RaiseCanExecuteChanged();
                    DeleteChooseCommand.RaiseCanExecuteChanged();
                    UpdateConflicts();
                    break;
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện xoá môn học
        /// </summary>
        /// <param name="message">Thông tin sự kiện môn học đã xoá</param>
        private void DelSubjectMsgHandler(SubjectModel subjectModel)
        {
            foreach (var classGroupModel in SelectedClassGroupModels)
            {
                if (classGroupModel.SubjectCode.Equals(subjectModel.SubjectCode))
                {
                    SelectedClassGroupModels.Remove(classGroupModel);
                    break;
                }
            }
            UpdateConflicts();
            DeleteAllChooseCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Xử lý sự kiện xoá toàn bộ môn học
        /// </summary>
        private void DelAllSubjectMsgHandler()
        {

            // 2. Xoá hết class group model đã chọn
            SelectedClassGroupModels.Clear();
            UpdateConflicts();
            DeleteAllChooseCommand.RaiseCanExecuteChanged();

            // 3. Xoá bộ lịch
            CleanDays();
        }
        #endregion

        #endregion

        #region Scheduler View Model
        private bool isSummerSemester;
        public bool IsSummerSemester
        {
            get { return isSummerSemester; }
            set { SetProperty(ref isSummerSemester, value); }
        }

        private List<ObservableCollection<ObservableCollection<TimeBlock>>> _schedules;

        #region Properties
        public ObservableCollection<TimeBlock> Phase1_Monday { get; set; }
        public ObservableCollection<TimeBlock> Phase1_Tuesday { get; set; }
        public ObservableCollection<TimeBlock> Phase1_Wednesday { get; set; }
        public ObservableCollection<TimeBlock> Phase1_Thursday { get; set; }
        public ObservableCollection<TimeBlock> Phase1_Friday { get; set; }
        public ObservableCollection<TimeBlock> Phase1_Saturday { get; set; }
        public ObservableCollection<TimeBlock> Phase1_Sunday { get; set; }
        public ObservableCollection<TimeBlock> Phase2_Monday { get; set; }
        public ObservableCollection<TimeBlock> Phase2_Tuesday { get; set; }
        public ObservableCollection<TimeBlock> Phase2_Wednesday { get; set; }
        public ObservableCollection<TimeBlock> Phase2_Thursday { get; set; }
        public ObservableCollection<TimeBlock> Phase2_Friday { get; set; }
        public ObservableCollection<TimeBlock> Phase2_Saturday { get; set; }
        public ObservableCollection<TimeBlock> Phase2_Sunday { get; set; }

        public ObservableCollection<ObservableCollection<TimeBlock>> Week1 { get; set; }
        public ObservableCollection<ObservableCollection<TimeBlock>> Week2 { get; set; }

        public ObservableCollection<string> Timelines { get; set; }

        #endregion

        public void InitSchedulerViewModel()
        {
            _eventAggregator.GetEvent<SearchVmMsgs.SelectCgmsMsg>().Subscribe(payload =>
            {
                foreach (var c in payload)
                {
                    RemoveScheduleItem(c.SubjectCode);
                    AddClassGroup(c);
                }
            });

            _eventAggregator.GetEvent<SearchVmMsgs.DelAllSubjectMsg>().Subscribe(CleanDays);

            _eventAggregator.GetEvent<SearchVmMsgs.DelSubjectMsg>().Subscribe(payload => RemoveScheduleItem(payload.SubjectCode));

            _eventAggregator.GetEvent<SearchVmMsgs.DelSubjectMsg>().Subscribe(payload => RemoveScheduleItem(payload.SubjectCode));

            _eventAggregator.GetEvent<ChoosedVmMsgs.UndoDelAllMsg>().Subscribe(payload =>
            {
                var classGroupModels = payload.Item1;
                var conflictModels = payload.Item2;
                var placeConflicts = payload.Item3;

                foreach (var cgm in classGroupModels)
                {
                    AddClassGroup(cgm);
                }
                foreach (var conflictModel in conflictModels)
                {
                    AddScheduleItem(conflictModel);
                }
                foreach (var placeConflict in placeConflicts)
                {
                    AddScheduleItem(placeConflict);
                }
            });

            _eventAggregator.GetEvent<ChoosedVmMsgs.ConflictCollChangedMsg>().Subscribe(payload =>
            {
                var conflictIds = payload.Select(cm => cm.GetId());
                RemoveConflictNotInContains(conflictIds, ConflictType.Time);
                AddNewConflicts(payload);
            });

            _eventAggregator.GetEvent<ChoosedVmMsgs.PlaceConflictCollChangedMsg>().Subscribe(payload =>
            {
                var conflictIds = payload.Select(cm => cm.GetId());
                RemoveConflictNotInContains(conflictIds, ConflictType.Place);
                AddNewConflicts(payload);
            });

            _eventAggregator.GetEvent<ChoosedVmMsgs.DelClassGroupChoiceMsg>().Subscribe(payload =>
            {
                foreach (var scm in payload.CurrentSchoolClassModels)
                {
                    RemoveScheduleItem(scm.SubjectCode);
                }
            });

            _eventAggregator.GetEvent<ChoosedVmMsgs.DelAllClassGroupChoiceMsg>().Subscribe(CleanDays);

            _eventAggregator.GetEvent<ChoosedVmMsgs.UndoDelMsg>().Subscribe(AddClassGroup);

            IsSummerSemester = "Học Kỳ Hè".Equals(_unitOfWork.Settings.GetByKey(DbConsts.StCurrentSemesterInfo));

            #region Weeks and Timelines
            Phase1_Monday = new ObservableCollection<TimeBlock>();
            Phase1_Tuesday = new ObservableCollection<TimeBlock>();
            Phase1_Wednesday = new ObservableCollection<TimeBlock>();
            Phase1_Thursday = new ObservableCollection<TimeBlock>();
            Phase1_Friday = new ObservableCollection<TimeBlock>();
            Phase1_Saturday = new ObservableCollection<TimeBlock>();
            Phase1_Sunday = new ObservableCollection<TimeBlock>();

            Phase2_Monday = new ObservableCollection<TimeBlock>();
            Phase2_Tuesday = new ObservableCollection<TimeBlock>();
            Phase2_Wednesday = new ObservableCollection<TimeBlock>();
            Phase2_Thursday = new ObservableCollection<TimeBlock>();
            Phase2_Friday = new ObservableCollection<TimeBlock>();
            Phase2_Saturday = new ObservableCollection<TimeBlock>();
            Phase2_Sunday = new ObservableCollection<TimeBlock>();

            Week1 = new ObservableCollection<ObservableCollection<TimeBlock>>()
            {
                Phase1_Monday,
                Phase1_Tuesday,
                Phase1_Wednesday,
                Phase1_Thursday,
                Phase1_Friday,
                Phase1_Saturday,
                Phase1_Sunday
            };

            Week2 = new ObservableCollection<ObservableCollection<TimeBlock>>()
            {
                Phase2_Monday,
                Phase2_Tuesday,
                Phase2_Wednesday,
                Phase2_Thursday,
                Phase2_Friday,
                Phase2_Saturday,
                Phase2_Sunday
            };

            _schedules = new List<ObservableCollection<ObservableCollection<TimeBlock>>>() { Week1, Week2 };

            Timelines = new ObservableCollection<string>();
            foreach (var timeline in Cs4rsa.UI.ScheduleTable.Utils.Utils.TimeLines)
            {
                Timelines.Add(timeline);
            }
            #endregion
        }

        private void AddNewConflicts(IEnumerable<IScheduleTableItem> scheduleTableItems)
        {
            scheduleTableItems = scheduleTableItems.Where(sti => !Exists(sti));
            foreach (var sti in scheduleTableItems)
            {
                AddScheduleItem(sti);
            }
        }

        private bool Exists(IScheduleTableItem item)
        {
            foreach (var week in _schedules)
            {
                foreach (var day in week)
                {
                    foreach (var timeBlock in day)
                    {
                        if (timeBlock.Id == item.GetId()) return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Loại bỏ ScheduleItem khỏi mô phỏng.
        /// 
        /// </summary>
        /// <param name="id">
        /// ID với SchoolClassModel sẽ là Subject Code của nó.
        /// ID của các Conflict sẽ là sự kết hợp giữa hai tên SchoolClassModel.
        /// </param>
        private void RemoveScheduleItem(string id)
        {
            foreach (var week in _schedules)
            {
                foreach (var day in week)
                {
                    var currentIndex = 0;
                    while (currentIndex < day.Count)
                    {
                        if (id == day[currentIndex].Id)
                        {
                            day.RemoveAt(currentIndex);
                            continue;
                        }
                        currentIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Thay thế ClassGroupModel cũ trong bộ mô phỏng (nếu có)
        /// bằng ClassGroupModel mới được thêm.
        /// </summary>
        /// <param name="classGroupModel">ClassGroupModel</param>
        private void AddClassGroup(ClassGroupModel classGroupModel)
        {
            IEnumerable<SchoolClassModel> schoolClassModels;
            if (classGroupModel.IsSpecialClassGroup)
            {
                schoolClassModels = classGroupModel.CurrentSchoolClassModels;
            }
            else
            {
                schoolClassModels = classGroupModel.NormalSchoolClassModels;
            }

            foreach (var schoolClassModel in schoolClassModels)
            {
                AddScheduleItem(schoolClassModel);
            }
        }

        /// <summary>
        /// IMPORTANT!!!
        /// 
        /// Vẽ một <see cref="IScheduleTableItem"/> lên mô phỏng.
        /// </summary>
        /// <param name="scheduleItem">IScheduleTableItem</param>
        private void AddScheduleItem(IScheduleTableItem scheduleItem)
        {
            var timeBlocks = scheduleItem.GetBlocks();
            var phase = scheduleItem.GetPhase();
            foreach (var timeBlock in timeBlocks)
            {
                var dayIndex = timeBlock.DayOfWeek.ToIndex();
                if (phase == Phase.First || phase == Phase.Second)
                {
                    var week = phase == Phase.First ? Week1 : Week2;
                    week[dayIndex].Add(timeBlock);
                }
                else if (phase == Phase.All)
                {
                    Week1[dayIndex].Add(timeBlock);
                    Week2[dayIndex].Add(timeBlock);
                }
                // Phase.Summer
                else
                {
                    Week2[dayIndex].Add(timeBlock);
                }
            }
        }

        private void CleanDays()
        {
            Phase1_Monday.Clear();
            Phase1_Tuesday.Clear();
            Phase1_Wednesday.Clear();
            Phase1_Thursday.Clear();
            Phase1_Friday.Clear();
            Phase1_Saturday.Clear();
            Phase1_Sunday.Clear();

            Phase2_Monday.Clear();
            Phase2_Tuesday.Clear();
            Phase2_Wednesday.Clear();
            Phase2_Thursday.Clear();
            Phase2_Friday.Clear();
            Phase2_Saturday.Clear();
            Phase2_Sunday.Clear();
        }

        private void RemoveConflictNotInContains(IEnumerable<string> conflictIds, ConflictType conflictType)
        {
            foreach (var week in _schedules)
            {
                foreach (var day in week)
                {
                    var currentIndex = 0;
                    while (currentIndex < day.Count)
                    {
                        if (!conflictIds.Contains(day[currentIndex].Id)
                            && (day[currentIndex].ScheduleTableItemType == ScheduleTableItemType.TimeConflict
                            || day[currentIndex].ScheduleTableItemType == ScheduleTableItemType.PlaceConflict)
                            && day[currentIndex].Id.StartsWith(conflictType == ConflictType.Time ? "tc" : "pc"))
                        {
                            day.RemoveAt(currentIndex);
                            continue;
                        }
                        currentIndex++;
                    }
                }
            }
        }
        #endregion
    }
}
