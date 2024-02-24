using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

using Cs4rsa.Common;
using Cs4rsa.Common.Interfaces;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;
using Cs4rsa.Messages.Publishers.UIs;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Views;
using Cs4rsa.Module.ManuallySchedule.Events;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Dialog.Interfaces;
using Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.UI.ScheduleTable.Models;

using MaterialDesignThemes.Wpf;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Cs4rsa.Module.ManuallySchedule.ViewModels
{
    public class SearchViewModel : BindableBase
    {
        #region Fields
        private List<Discipline> _searchDisciplines;
        private List<Keyword> _searchKeywords;
        #endregion

        #region Commands
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand ImportDialogCommand { get; set; }
        public DelegateCommand<SubjectModel> ReloadCommand { get; set; }
        public DelegateCommand<SubjectModel> DeleteCommand { get; set; }
        public DelegateCommand DeleteAllCommand { get; set; }
        public DelegateCommand<SubjectModel> DetailCommand { get; set; }
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
                if (value != null && !value.IsDownloading && !value.IsError)
                {
                    SetProperty(ref _selectedSubjectModel, value);
                    OnSelectedSubjectModelChanged(value);
                }
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetProperty(ref _searchText, value);
                if (value.Trim().Length == 0) return;
                LoadSearchItemSource(value);
            }
        }

        private bool _isUseCache;
        public bool IsUseCache
        {
            get { return _isUseCache; }
            set { SetProperty(ref _isUseCache, value); }
        }
        #endregion

        #region Services
        private readonly ISubjectCrawler _subjectCrawler;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOpenInBrowser _openInBrowser;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        #endregion

        public SearchViewModel(
            IEventAggregator eventAggregator,
            IUnitOfWork unitOfWork,
            ISubjectCrawler subjectCrawler,
            IOpenInBrowser openInBrowser,
            IDialogService dialogService,
            ISnackbarMessageQueue snackbarMessageQueue
        )
        {
            #region Services
            _subjectCrawler = subjectCrawler;
            _unitOfWork = unitOfWork;
            _openInBrowser = openInBrowser;
            _snackbarMessageQueue = snackbarMessageQueue;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            #endregion

            #region Messengers
            _eventAggregator.GetEvent<ImportSessionVmMsgs.ExitImportSubjectMsg>().Subscribe(HandlerExitImportSubjectMsg);

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
            SearchText = string.Empty;
            IsUseCache = true;
            #endregion

            #region Commands
            AddCommand = new DelegateCommand(async () => await OnAdd(), () => !IsAlreadyDownloaded(SelectedKeyword));
            DeleteCommand = new DelegateCommand<SubjectModel>(OnDelete);
            //ImportDialogCommand = new DelegateCommand(OnOpenImportDialog);
            ImportDialogCommand = new DelegateCommand(OpenScheduleBagDialog);
            DeleteAllCommand = new DelegateCommand(OnDeleteAll, () => SubjectModels.Any());
            DetailCommand = new DelegateCommand<SubjectModel>((SubjectModel subjectModel) =>
            {
                var showDetailsSubjectUc = new ShowDetailsSubjectUC();
                ((ShowDetailsSubjectUCViewModel)showDetailsSubjectUc.DataContext).SubjectModel = subjectModel;
                _dialogService.OpenDialog(showDetailsSubjectUc);
            });
            ReloadCommand = new DelegateCommand<SubjectModel>(OnReload);
            #endregion

            LoadDiscipline();
            LoadSavedSchedules();
        }

        private void HandlerExitImportSubjectMsg(IEnumerable<UserSubject> payload)
        {
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await HandleImportSubjects(payload);
            });
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
            FullMatchSearchingKeywords.Clear();
            var keywords = _searchKeywords
                .Where(k => StringHelper.ReplaceVietnamese(k.SubjectName).ToLower().Contains(StringHelper.ReplaceVietnamese(text).ToLower())
                         || StringHelper.ReplaceVietnamese(k.Discipline.Name + k.Keyword1).ToLower().Contains(StringHelper.ReplaceVietnamese(text.Replace(" ", string.Empty)).ToLower())
                ).Take(10);
            foreach (var kw in keywords)
            {
                var fullMatch = new FullMatchSearchingKeyword()
                {
                    Keyword = kw,
                    Discipline = kw.Discipline
                };
                FullMatchSearchingKeywords.Add(fullMatch);
            }

            if (FullMatchSearchingKeywords.Count > 0) return;
            var keyword = new Keyword()
            {
                CourseId = "000000",
                SubjectName = "Không tìm thấy tên môn này",
                Color = "#ffffff"
            };
            var discipline = new Discipline()
            {
                Name = "Không tìm thấy mã môn này"
            };
            var fullMatchSearchingKeyword = new FullMatchSearchingKeyword()
            {
                Keyword = keyword,
                Discipline = discipline
            };
            FullMatchSearchingKeywords.Add(fullMatchSearchingKeyword);
        }

        private void OnDeleteAll()
        {
            SubjectModels.Clear();
            AddCommand.RaiseCanExecuteChanged();
            _eventAggregator.GetEvent<SearchVmMsgs.DelAllSubjectMsg>().Publish();
            Debug.WriteLine("Search " + _eventAggregator.GetHashCode());

            var subjects = new List<SubjectModel>();
            foreach (var subjectModel in SubjectModels)
            {
                var restoreSubject = subjectModel.DeepClone();
                subjects.Add(restoreSubject);
            }

            var classGroupModels = new List<ClassGroupModel>();
            //ChoseViewModel choseVm = GetViewModel<ChoseViewModel>();
            //foreach (ClassGroupModel classGroupModel in choseVm.ClassGroupModels)
            //{
            //    classGroupModels.Add(classGroupModel.DeepClone());
            //}
            var actionData = new Tuple<IEnumerable<SubjectModel>, IEnumerable<ClassGroupModel>>(subjects, classGroupModels);
            _snackbarMessageQueue.Enqueue("Đã xoá hết", "HOÀN TÁC", AddSubjectWithCgm, actionData);
        }

        private void OnGotoCourse(SubjectModel subjectModel)
        {
            var courseId = subjectModel.CourseId;
            var semesterValue = _unitOfWork.Settings.GetByKey(Setting.SemesterValue);
            var url = $@"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid={courseId}&timespan={semesterValue}&t=s";
            _openInBrowser.Open(url);
        }

        /// <summary>
        /// Load lại data môn học từ cơ sở dữ liệu lên
        /// </summary>
        private void ReloadDisciplineAndKeyWord()
        {
            Disciplines.Clear();
            IEnumerable<Discipline> disciplines = _unitOfWork.Disciplines.GetAllIncludeKeyword();
            foreach (var discipline in disciplines)
            {
                Disciplines.Add(discipline);
            }
            SelectedDiscipline = Disciplines[0];
            LoadKeywordByDiscipline(SelectedDiscipline);
        }

        private void OnOpenImportDialog()
        {
            var importSessionUc = new ImportSessionUC();
            var vm = (ImportSessionUCViewModel)importSessionUc.DataContext;
            _dialogService.OpenDialog(importSessionUc);
            vm.LoadScheduleSession();
        }

        private async void OpenScheduleBagDialog()
        {
            var scheduleBag = new ScheduleBag();
            var vm = (ScheduleBagViewModel)scheduleBag.DataContext;
            _dialogService.OpenDialog(scheduleBag);
            await vm.LoadScheduleSession();
        }

        private async Task HandleImportSubjects(IEnumerable<UserSubject> userSubjects)
        {
            if (userSubjects == null) return;
            SubjectModels.Clear();
            _eventAggregator.GetEvent<SearchVmMsgs.DelAllSubjectMsg>().Publish();

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
            IEnumerable<ClassGroupModel> classGroupModels = new List<ClassGroupModel>();

            // Get ClassGroupModel hiện được chọn trong ChoseViewModel
            // sau đó clone một bản để có thể Undo.
            ClassGroupModel classGroupModel = null;

            //classGroupModel = GetViewModel<ChoseViewModel>()
            //    .ClassGroupModels
            //    .FirstOrDefault(cgm => cgm.SubjectCode.Equals(sm.SubjectCode));

            if (classGroupModel != null)
            {
                var classGroupModelClone = classGroupModel.DeepClone();
                classGroupModels = new List<ClassGroupModel>() { classGroupModelClone };
            }

            _eventAggregator.GetEvent<SearchVmMsgs.DelSubjectMsg>().Publish(sm);
            var subjectModel = sm.DeepClone();

            var subjectModels = new List<SubjectModel>() { subjectModel };
            var actionData = new Tuple<IEnumerable<SubjectModel>, IEnumerable<ClassGroupModel>>(subjectModels, classGroupModels);
            SubjectModels.Remove(sm);
            _snackbarMessageQueue.Enqueue($"Đã xoá môn {sm.SubjectName}", "HOÀN TÁC", AddSubjectWithCgm, actionData);
            AddCommand.RaiseCanExecuteChanged();
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
                if (isUseCache && keyword.Cache != null)
                {
                    subject = await _subjectCrawler.Crawl(keyword.Cache);
                }
                else
                {
                    var semester = _unitOfWork.Settings.GetByKey("CurrentSemesterValue");
                    subject = await _subjectCrawler.Crawl(keyword.CourseId, semester);
                }
                if (subject == null)
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
        /// Được sử dụng cùng thao tác sau khi Xoá (tất cả).
        /// </summary>
        private void AddSubjectWithCgm(Tuple<IEnumerable<SubjectModel>, IEnumerable<ClassGroupModel>> actionData)
        {
            var subjectModels = actionData.Item1;
            var classes = actionData.Item2;

            foreach (var subjectModel in subjectModels)
            {
                SubjectModels.Add(subjectModel);
            }

            SelectedSubjectModel = actionData.Item1.First();

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
        /// Thêm Msg lỗi khi quá trình tải Subject bị lỗi.
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
    }
}
