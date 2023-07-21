using CommunityToolkit.Mvvm.ComponentModel;
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
using Cs4rsa.Messages.Publishers.UIs;
using Cs4rsa.ModelExtensions;
using Cs4rsa.Models;
using Cs4rsa.Services.CourseSearchSvc.Crawlers;
using Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;

using MaterialDesignThemes.Wpf;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace Cs4rsa.ViewModels.ManualScheduling
{
    public sealed partial class SearchViewModel : ViewModelBase
    {
        #region Fields
        private readonly ShowDetailsSubjectUC _showDetailsSubjectUC;
        private readonly ImportSessionUC _importSessionUC;
        private List<Discipline> _searchDisciplines;
        private List<Keyword> _searchKeywords;
        #endregion

        #region Commands
        public AsyncRelayCommand AddCommand { get; set; }
        public RelayCommand ImportDialogCommand { get; set; }
        public AsyncRelayCommand<SubjectModel> ReloadCommand { get; set; }
        public RelayCommand<SubjectModel> DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }

        /// <summary>
        /// Command đi tới trang Course.
        /// </summary>
        public RelayCommand<SubjectModel> GotoCourseCommand { get; set; }
        public RelayCommand<SubjectModel> DetailCommand { get; set; }
        public RelayCommand<int> GotoViewCommand { get; set; }
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

        [ObservableProperty]
        private CombinationModel _sltCombi;

        [ObservableProperty]
        private Discipline _selectedDiscipline;

        [ObservableProperty]
        private Keyword _selectedKeyword;

        [ObservableProperty]
        private FullMatchSearchingKeyword _searchingKeyword;

        [ObservableProperty]
        private SubjectModel _selectedSubjectModel;

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private int _totalSubject;

        [ObservableProperty]
        private int _totalCredits;

        [ObservableProperty]
        private bool _isUseCache;

        /// <summary>
        /// Index hiện tại của View Search
        /// 0: Search
        /// 1: Store
        /// </summary>
        [ObservableProperty]
        private int _crrScrIdx;
        #endregion

        #region Services
        private readonly ColorGenerator _colorGenerator;
        private readonly CourseCrawler _courseCrawler;
        private readonly ISubjectCrawler _subjectCrawler;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOpenInBrowser _openInBrowser;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion

        public SearchViewModel(
            ColorGenerator colorGenerator,
            CourseCrawler courseCrawler,
            IUnitOfWork unitOfWork,
            ISubjectCrawler subjectCrawler,
            IOpenInBrowser openInBrowser,
            ISnackbarMessageQueue snackbarMessageQueue
        )
        {
            #region Fields
            _showDetailsSubjectUC = new();
            _importSessionUC = new();
            #endregion

            #region Services
            _courseCrawler = courseCrawler;
            _subjectCrawler = subjectCrawler;
            _unitOfWork = unitOfWork;
            _colorGenerator = colorGenerator;
            _openInBrowser = openInBrowser;
            _snackbarMessageQueue = snackbarMessageQueue;
            #endregion

            #region Messengers
            Messenger.Register<ImportSessionVmMsgs.ExitImportSubjectMsg>(this, (r, m) =>
            {
                Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await HandleImportSubjects(m.Value);
                });
            });

            Messenger.Register<AutoVmMsgs.ShowOnSimuMsg>(this, (r, m) =>
            {
                ImportSubjects(m.Value);
            });

            Messenger.Register<UpdateVmMsgs.UpdateSuccessMsg>(this, (r, m) =>
            {
                DisciplineKeywordModels.Clear();
                Disciplines.Clear();
                SubjectModels.Clear();
                ReloadDisciplineAndKeyWord();
            });

            Messenger.Register<ScheduleBlockMsgs.SelectedMsg>(this, (r, m) =>
            {
                if (m.Value is SchoolClassBlock @schoolClassBlock)
                {
                    SelectedSubjectModel = SubjectModels
                        .Where(sm => sm.SubjectCode.Equals(schoolClassBlock.Id))
                        .FirstOrDefault();
                }
            });

            Messenger.Register<AutoVmMsgs.SaveStoreMsg>(this, (r, m) =>
            {
                SubjectModels.Clear();
                ComModels.Clear();

                Messenger.Send(new SearchVmMsgs.DelAllSubjectMsg());

                AddCommand.NotifyCanExecuteChanged();
                DeleteAllCommand.NotifyCanExecuteChanged();

                UpdateCreditTotal();
                UpdateSubjectAmount();

                foreach (CombinationModel item in m.Value)
                {
                    ComModels.Add(item);
                }
            });

            #endregion

            #region Pros
            DisciplineKeywordModels = new();
            SubjectModels = new();
            Disciplines = new();
            FullMatchSearchingKeywords = new();
            SavedSchedules = new();
            ComModels = new();
            SearchText = string.Empty;
            IsUseCache = true;
            #endregion

            #region Commands
            AddCommand = new AsyncRelayCommand(OnAdd, () => !IsAlreadyDownloaded(SelectedKeyword));
            DeleteCommand = new(OnDelete);
            ImportDialogCommand = new(OnOpenImportDialog);
            GotoCourseCommand = new(OnGotoCourse);
            GotoViewCommand = new((idx) => CrrScrIdx = idx);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, () => SubjectModels.Any());
            DetailCommand = new((SubjectModel subjectModel) =>
            {
                ((ShowDetailsSubjectViewModel)_showDetailsSubjectUC.DataContext).SubjectModel = subjectModel;
                OpenDialog(_showDetailsSubjectUC);
            });
            ReloadCommand = new(OnReload);
            #endregion

            LoadDiscipline();
            Application.Current.Dispatcher.Invoke(LoadSavedSchedules);
        }

        partial void OnSltCombiChanged(CombinationModel value)
        {
            // TODO: Handle Special Subject sau khi đi nghĩa vụ về
            if (value != null)
            {
                IEnumerable<SubjectModel> subjectModels = value.SubjecModels;
                SubjectModels.Clear();
                foreach (SubjectModel sjm in subjectModels)
                {
                    SubjectModels.Add(sjm);
                }

                // Đánh giá Phase Store xác định tuần ngăn cách
                foreach (ClassGroupModel cgm in value.ClassGroupModels)
                {
                    Messenger.Send(new ClassGroupSessionVmMsgs.ClassGroupAddedMsg(cgm));
                }

                SelectedSubjectModel = SubjectModels.FirstOrDefault();

                AddCommand.NotifyCanExecuteChanged();
                DeleteAllCommand.NotifyCanExecuteChanged();
            }
        }

        partial void OnSelectedSubjectModelChanged(SubjectModel value)
        {
            DeleteCommand.NotifyCanExecuteChanged();
            Messenger.Send(new SearchVmMsgs.SelectedSubjectChangedMsg(value));
        }

        partial void OnSelectedDisciplineChanged(Discipline value)
        {
            if (value != null)
            {
                LoadKeywordByDiscipline(value);
            }
        }

        partial void OnSelectedKeywordChanged(Keyword value)
        {
            AddCommand.NotifyCanExecuteChanged();
        }

        partial void OnSearchingKeywordChanged(FullMatchSearchingKeyword value)
        {
            if (value != null && value.Keyword != null && value.Discipline.DisciplineId != 0)
            {
                Discipline dcl = Disciplines.First(dcl => dcl.DisciplineId == value.Discipline.DisciplineId);
                SelectedDiscipline = dcl;
                SelectedKeyword = value.Keyword;
                SearchText = string.Empty;
                AddCommand.NotifyCanExecuteChanged();
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
        }

        partial void OnSearchTextChanged(string value)
        {
            if (value.Trim().Length == 0) return;
            LoadSearchItemSource(value);
        }

        /// <summary>
        /// Load danh sách các bộ lịch đã lưu
        /// </summary>
        private void LoadSavedSchedules()
        {
            SavedSchedules.Clear();
            List<UserSchedule> sessions = _unitOfWork.UserSchedules.GetAll();
            foreach (UserSchedule session in sessions)
            {
                SavedSchedules.Add(session);
            }
        }

        private void LoadDiscipline()
        {
            _searchDisciplines = _unitOfWork.Disciplines.GetAllIncludeKeyword();
            _searchKeywords = _searchDisciplines.SelectMany(d => d.Keywords).ToList();

            foreach (Discipline discipline in _searchDisciplines)
            {
                Disciplines.Add(discipline);
            }
            SelectedDiscipline = Disciplines[0];
        }

        /// <summary>
        /// Tải lại môn học bị lỗi.
        /// </summary>
        /// <param name="subjectModel">SubjectModel</param>
        private async Task OnReload(SubjectModel subjectModel)
        {
            subjectModel.IsDownloading = true;
            subjectModel.IsError = false;
            subjectModel.ErrorMessage = null;

            Keyword kw = _unitOfWork.Keywords.GetKeyword(subjectModel.CourseId);
            Discipline ds = _unitOfWork.Disciplines.GetDisciplineByID(kw.DisciplineId);
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
            IEnumerable<Keyword> keywords = _searchKeywords
                .Where(k => StringHelper.ReplaceVietNamese(k.SubjectName)
                                        .ToLower()
                                        .Contains(StringHelper.ReplaceVietNamese(text).ToLower())
                         || StringHelper.ReplaceVietNamese(k.Discipline.Name + k.Keyword1)
                                        .ToLower()
                                        .Contains(
                                            StringHelper.ReplaceVietNamese(text
                                            .Replace(VmConstants.StrSpace, string.Empty)
                                        ).ToLower())
                )
                .Take(10);
            foreach (Keyword kw in keywords)
            {
                FullMatchSearchingKeyword fullMatch = new()
                {
                    Keyword = kw,
                    Discipline = kw.Discipline
                };
                FullMatchSearchingKeywords.Add(fullMatch);
            }

            if (FullMatchSearchingKeywords.Count > 0) return;
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

        private void OnDeleteAll()
        {
            #region Clone Subject Models
            List<SubjectModel> subjects = new();
            foreach (SubjectModel subjectModel in SubjectModels)
            {
                SubjectModel restoreSubject = subjectModel.DeepClone();
                subjects.Add(restoreSubject);
            }
            #endregion

            #region Clone ClassGroup Models
            List<ClassGroupModel> classGroupModels = new();
            ChoosedViewModel choosedVm = GetViewModel<ChoosedViewModel>();
            foreach (ClassGroupModel classGroupModel in choosedVm.ClassGroupModels)
            {
                classGroupModels.Add(classGroupModel.DeepClone());
            }
            #endregion

            SubjectModels.Clear();
            Messenger.Send(new SearchVmMsgs.DelAllSubjectMsg());
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.NotifyCanExecuteChanged();
            Tuple<IEnumerable<SubjectModel>, IEnumerable<ClassGroupModel>> actionData = new(subjects, classGroupModels);
            _snackbarMessageQueue.Enqueue(VmConstants.SnbDeleteAll, VmConstants.SnbRestore, AddSubjectWithCgm, actionData);
        }

        private void OnGotoCourse(SubjectModel subjectModel)
        {
            int courseId = subjectModel.CourseId;
            string semesterValue = _courseCrawler.CurrentSemesterValue;
            string url = $@"http://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid={courseId}&timespan={semesterValue}&t=s";
            _openInBrowser.Open(url);
        }

        /// <summary>
        /// Load lại data môn học từ cơ sở dữ liệu lên
        /// </summary>
        private void ReloadDisciplineAndKeyWord()
        {
            Disciplines.Clear();
            IEnumerable<Discipline> disciplines = _unitOfWork.Disciplines.GetAllIncludeKeyword();
            foreach (Discipline discipline in disciplines)
            {
                Disciplines.Add(discipline);
            }
            SelectedDiscipline = Disciplines[0];
            LoadKeywordByDiscipline(SelectedDiscipline);
        }

        private void OnOpenImportDialog()
        {
            ImportSessionViewModel vm = _importSessionUC.DataContext as ImportSessionViewModel;
            OpenDialog(_importSessionUC);
            vm.LoadScheduleSession();
        }

        private async Task HandleImportSubjects(IEnumerable<UserSubject> userSubjects)
        {
            if (userSubjects == null) return;
            SubjectModels.Clear();
            Messenger.Send(new SearchVmMsgs.DelAllSubjectMsg());

            List<Keyword> kws = new();
            foreach (UserSubject userSubject in userSubjects)
            {
                Keyword kw = _unitOfWork.Keywords.GetKeyword(userSubject.SubjectCode);
                kws.Add(kw);
            }

            InsertPseudoSubjects(kws, userSubjects);

            List<Task> downloadTasks = new();
            List<UserSubject> listOfUserSubjects = userSubjects.ToList();
            for (int i = 0; i < kws.Count; i++)
            {
                downloadTasks.Add(OnAddSubjectAsync(kws[i], listOfUserSubjects[i]));
            }
            await Task.WhenAll(downloadTasks);
            SelectedSubjectModel = SubjectModels[0];
        }

        private void InsertPseudoSubject(Keyword keyword)
        {
            SubjectModel pseudoSubjectModel = SubjectModel.CreatePseudo(
                keyword.SubjectName,
                keyword.Discipline.Name + VmConstants.StrSpace + keyword.Keyword1,
                keyword.Color,
                keyword.CourseId
            );
            SubjectModels.Insert(0, pseudoSubjectModel);
            AddCommand.NotifyCanExecuteChanged();
        }

        private void InsertPseudoSubjects(List<Keyword> keywords, IEnumerable<UserSubject> userSubjects)
        {
            UserSubject[] userSubjectArr = userSubjects.ToArray();
            for (int i = 0; i < keywords.Count; i++)
            {
                Keyword kw = keywords[i];
                kw.Discipline = _unitOfWork.Disciplines.GetDisciplineByID(kw.DisciplineId);
                SubjectModel pseudoSubjectModel = SubjectModel.CreatePseudo(
                    kw.SubjectName,
                    kw.Discipline.Name + VmConstants.StrSpace + kw.Keyword1,
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

            ClassGroupModel classGroupModel = GetViewModel<ChoosedViewModel>()
                .ClassGroupModels
                .FirstOrDefault(cgm => cgm.SubjectCode.Equals(sm.SubjectCode));

            if (classGroupModel != null)
            {
                ClassGroupModel classGroupModelClone = classGroupModel.DeepClone();
                classGroupModels = new List<ClassGroupModel>() { classGroupModelClone };
            }

            Messenger.Send(new SearchVmMsgs.DelSubjectMsg(sm));
            SubjectModel subjectModel = sm.DeepClone();

            List<SubjectModel> subjectModels = new()
            {
                subjectModel
            };

            Tuple<IEnumerable<SubjectModel>, IEnumerable<ClassGroupModel>> actionData = new(subjectModels, classGroupModels);

            string message = CredizText.ManualMsg002(sm.SubjectName);
            SubjectModels.Remove(sm);
            _snackbarMessageQueue.Enqueue(message, VmConstants.SnbRestore, AddSubjectWithCgm, actionData);
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Load Keyword sau khi chọn discipline.
        /// </summary>
        /// <param name="discipline">Discipline.</param>
        public void LoadKeywordByDiscipline(Discipline discipline)
        {
            DisciplineKeywordModels.Clear();
            Discipline currentDiscipline = Disciplines.Where(d => d.DisciplineId == discipline.DisciplineId).FirstOrDefault();
            List<Keyword> keywords = currentDiscipline.Keywords;
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
        /// Thực hiện tại Subject. Thông báo nếu Subject không tồn tại, ngược lại
        /// thay thế pseudo Subject bằng Subject đã tải được. 
        /// <br></br>
        /// Nếu không có Subject nào đang được tải, thực hiện select Subject 
        /// đầu tiên trong danh sách. Thực hiện tính lại tổng Subject, tổng 
        /// tín chỉ, số lượng môn học. Và trả về Subject Model đã tải được. 
        /// <br></br>
        /// Bất kỳ lỗi nào xuất hiện trong quá trình này, thêm message lỗi vào pseudo subject và trả về null.
        /// </remarks>
        /// <param name="keyword">Keyword</param>
        /// <returns>Task of SubjectModel</returns>
        private async Task<SubjectModel> OnAddSubjectAsync(Keyword keyword)
        {
            try
            {
                SubjectModel subjectModel = await DownloadSubject(
                    keyword.Discipline.Name,
                    keyword.Keyword1,
                    VmConstants.IntInvalidCourseId,
                    IsUseCache
                );

                if (subjectModel == null)
                {
                    _snackbarMessageQueue.Enqueue(VmConstants.SnbNotFoundSubjectInThisSemester);
                    return null;
                }

                ReplacePseudoSubject(subjectModel);

                if (!SubjectModels.Where(sm => sm.IsDownloading).Any())
                {
                    SelectedSubjectModel = subjectModel;
                }

                TotalSubject = SubjectModels.Count;
                UpdateCreditTotal();
                UpdateSubjectAmount();

                return subjectModel;
            }
            catch (Exception e)
            {
                AddErrorToPseudoSubject(e.Message, keyword.CourseId);
                return null;
            }
        }

        /// <summary>
        /// Xử lý Add Subject từ bộ lịch đã lưu.
        /// </summary>
        /// <param name="keyword">Keyword</param>
        /// <param name="userSubject">UserSubject</param>
        private async Task OnAddSubjectAsync(Keyword keyword, UserSubject userSubject)
        {
            SubjectModel subjectModel = await OnAddSubjectAsync(keyword);
            if (subjectModel == null) return;

            ClassGroupModel classGroupModel;
            classGroupModel = subjectModel.ClassGroupModels
                    .Where(cgm => cgm.Name.Equals(userSubject.ClassGroup))
                    .First();
            Debug.Assert(classGroupModel != null);
            if (classGroupModel == null) return;
            if (subjectModel.IsSpecialSubject)
            {
                classGroupModel.PickSchoolClass(userSubject.SchoolClass);
            }

            List<ClassGroupModel> cgms = new()
            {
                classGroupModel
            };
            Messenger.Send(new SearchVmMsgs.SelectCgmsMsg(cgms));
        }

        private async Task<SubjectModel> DownloadSubject(
            string discipline,
            string keyword1,
            int courseId,
            bool isUseCache)
        {
            Subject subject;
            if (courseId == VmConstants.IntInvalidCourseId)
            {
                subject = await _subjectCrawler.Crawl(discipline, keyword1, isUseCache, true);
            }
            else
            {
                subject = await _subjectCrawler.Crawl(courseId, isUseCache, true);
            }
            if (subject == null) return null;
            AddCommand.NotifyCanExecuteChanged();
            return SubjectModel.Create(subject, _colorGenerator);
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

            if (
                    courseId != null 
                 && p != null 
                 && timespan != null 
                 && t != null 
                 && isDtuCourseHost 
                 && isRightAbsPath
            )
            {
                int intCourseId = int.Parse(courseId);
                if (IsAlreadyDownloaded(intCourseId))
                {
                    _snackbarMessageQueue.Enqueue(VmConstants.SnbAlreadyDownloaded);
                    return;
                }

                Keyword keyword = _unitOfWork.Keywords.GetByCourseId(intCourseId);
                if (keyword == null)
                {
                    _snackbarMessageQueue.Enqueue(CredizText.ManualMsg003(courseId));
                    return;
                }

                InsertPseudoSubject(keyword);
                await OnAddSubjectAsync(keyword);
            }
            else
            {
                _snackbarMessageQueue.Enqueue(CredizText.ManualMsg004());
            }
        }

        /// <summary>
        /// Cập nhật tổng số môn học.
        /// </summary>
        private void UpdateSubjectAmount()
        {
            DeleteAllCommand.NotifyCanExecuteChanged();
            TotalSubject = SubjectModels.Count;
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
        }

        /// <summary>
        /// Kiếm tra xem rằng một Subject đã có 
        /// sẵn trong danh sách đã tải xuống hay chưa.
        /// </summary>
        /// <param name="courseId">Course ID</param>
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

        private void ImportSubjects(CombinationModel combinationModel)
        {
            foreach (SubjectModel subject in SubjectModels)
            {
                Messenger.Send(new SearchVmMsgs.DelSubjectMsg(subject));
            }
            SubjectModels.Clear();

            foreach (SubjectModel subject in combinationModel.SubjecModels)
            {
                SubjectModels.Add(subject);
            }
            TotalSubject = SubjectModels.Count;
            AddCommand.NotifyCanExecuteChanged();
            UpdateCreditTotal();
            UpdateSubjectAmount();
            foreach (ClassGroupModel classGroupModel in combinationModel.ClassGroupModels)
            {
                Messenger.Send(new ClassGroupSessionVmMsgs.ClassGroupAddedMsg(classGroupModel));
            }
        }

        /// <summary>
        /// Được sử dụng cùng thao tác sau khi Xoá (tất cả).
        /// </summary>
        private void AddSubjectWithCgm(Tuple<IEnumerable<SubjectModel>, IEnumerable<ClassGroupModel>> actionData)
        {
            IEnumerable<SubjectModel> subjectModels = actionData.Item1;
            IEnumerable<ClassGroupModel> classes = actionData.Item2;

            foreach (SubjectModel subjectModel in subjectModels)
            {
                SubjectModels.Add(subjectModel);
            }

            SelectedSubjectModel = actionData.Item1.First();

            TotalSubject = SubjectModels.Count;
            AddCommand.NotifyCanExecuteChanged();
            UpdateCreditTotal();
            UpdateSubjectAmount();

            if (actionData.Item2 != null)
            {
                Messenger.Send(new SearchVmMsgs.SelectCgmsMsg(classes));
            }
        }

        /// <summary>
        /// Thay thế Pseudo Subject bằng Real Subject.
        /// </summary>
        /// <param name="subjectModel">SubjectModel</param>
        private void ReplacePseudoSubject(SubjectModel subjectModel)
        {
            for (int i = 0; i < SubjectModels.Count; i++)
            {
                if (SubjectModels[i].CourseId.Equals(subjectModel.CourseId))
                {
                    SubjectModels[i].AssignData(subjectModel);
                    return;
                }
            }
        }

        /// <summary>
        /// Thêm Msg lỗi khi quá trình tải Subject bị lỗi.
        /// </summary>
        /// <param name="errMsg">Error Message</param>
        /// <param name="courseId">Course ID</param>
        private void AddErrorToPseudoSubject(string errMsg, int courseId)
        {
            for (int i = 0; i < SubjectModels.Count; i++)
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
