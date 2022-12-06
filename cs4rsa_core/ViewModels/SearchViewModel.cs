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
using Cs4rsa.Interfaces;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Messages.Publishers.UIs;
using Cs4rsa.ModelExtensions;
using Cs4rsa.Models;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;
using Cs4rsa.Utils.Models;
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
    internal sealed partial class SearchViewModel : ViewModelBase
    {
        #region Fields
        private readonly ShowDetailsSubjectUC _showDetailsSubjectUC;
        private readonly ImportSessionUC _importSessionUC;
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
        public ObservableCollection<Keyword> DisciplineKeywordModels { get; set; }
        public ObservableCollection<SubjectModel> SubjectModels { get; set; }
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public ObservableCollection<FullMatchSearchingKeyword> FullMatchSearchingKeywords { get; set; }
        public ObservableCollection<UserSchedule> SavedSchedules { get; set; }

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
        #endregion

        #region Services
        private readonly ColorGenerator _colorGenerator;
        private readonly PhaseStore _phaseStore;
        private readonly ICourseCrawler _courseCrawler;
        private readonly ISubjectCrawler _subjectCrawler;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOpenInBrowser _openInBrowser;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion

        public SearchViewModel(
            ColorGenerator colorGenerator,
            PhaseStore phaseStore,
            ICourseCrawler courseCrawler,
            IUnitOfWork unitOfWork,
            ISubjectCrawler subjectCrawler,
            IOpenInBrowser openInBrowser,
            ISnackbarMessageQueue snackbarMessageQueue
        )
        {
            _showDetailsSubjectUC = new();
            _importSessionUC = new();

            _phaseStore = phaseStore;
            _courseCrawler = courseCrawler;
            _subjectCrawler = subjectCrawler;
            _unitOfWork = unitOfWork;
            _colorGenerator = colorGenerator;
            _openInBrowser = openInBrowser;
            _snackbarMessageQueue = snackbarMessageQueue;

            Messenger.Register<ImportSessionVmMsgs.ExitImportSubjectMsg>(this, (r, m) =>
            {
                Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await HandleImportSubjects(m.Value);
                });
            });

            Messenger.Register<AutoScheduleVmMsgs.ShowOnSimuMsg>(this, (r, m) =>
            {
                ImportSubjects(m.Value.SubjecModels);
                ClassGroupChoicer choicer = new();
                choicer.Start(m.Value.ClassGroupModels);
            });

            Messenger.Register<UpdateVmMsgs.UpdateSuccessMsg>(this, async (r, m) =>
            {
                DisciplineKeywordModels.Clear();
                Disciplines.Clear();
                await ReloadDisciplineAndKeyWord();
            });

            Messenger.Register<ScheduleBlockMsgs.SelectedMsg>(this, (r, m) =>
            {
                TimeBlock timeBlock = m.Value;
                if (timeBlock.ScheduleTableItemType == ScheduleTableItemType.SchoolClass)
                {
                    SelectedSubjectModel = SubjectModels.Where(sm => sm.SubjectCode.Equals(m.Value.Id.Space)).FirstOrDefault();
                }
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
            SearchText = string.Empty;
            IsUseCache = true;

            AddCommand = new AsyncRelayCommand(
                async () =>
                {
                    InsertPseudoSubject(SelectedKeyword);
                    await OnAddSubjectAsync(_selectedKeyword);
                },
                () => !IsAlreadyDownloaded(SelectedKeyword)
            );

            DeleteCommand = new RelayCommand(OnDelete, () => _selectedSubjectModel != null);
            ImportDialogCommand = new(OnOpenImportDialog);
            GotoCourseCommand = new RelayCommand(OnGotoCourse, () => true);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, () => SubjectModels.Any());
            DetailCommand = new RelayCommand(() =>
            {
                (_showDetailsSubjectUC.DataContext as ShowDetailsSubjectViewModel).SubjectModel = _selectedSubjectModel;
                OpenDialog(_showDetailsSubjectUC);
            });
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
                        async () =>
                        {
                            InsertPseudoSubject(value.Keyword);
                            await OnAddSubjectAsync(_selectedKeyword);
                        }
                    );
                }
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            Application.Current.Dispatcher.InvokeAsync(async () => await LoadSearchItemSource(value));
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

        public async Task LoadDiscipline()
        {
            List<Discipline> disciplines = await _unitOfWork.Disciplines.GetAllIncludeKeywordAsync();
            foreach (Discipline discipline in disciplines)
            {
                Disciplines.Add(discipline);
            }
            SelectedDiscipline = Disciplines[0];
        }

        private async Task LoadSearchItemSource(string text)
        {
            text = text.Trim().ToLower();
            FullMatchSearchingKeywords.Clear();

            IAsyncEnumerable<Keyword> result1 = _unitOfWork.Keywords.GetByDisciplineStartWith(text);
            IAsyncEnumerable<Keyword> result2 = _unitOfWork.Keywords.GetBySubjectNameContains(text);
            //List<Keyword>[] whenAllResult = await Task.WhenAll(result1, result2);
            await foreach (Keyword keyword in result1)
            {
                FullMatchSearchingKeyword fullMatch = new()
                {
                    Keyword = keyword,
                    Discipline = keyword.Discipline
                };
                FullMatchSearchingKeywords.Add(fullMatch);
            }

            await foreach (Keyword keyword in result2)
            {
                FullMatchSearchingKeyword fullMatch = new()
                {
                    Keyword = keyword,
                    Discipline = keyword.Discipline
                };
                FullMatchSearchingKeywords.Add(fullMatch);
            }


            if (text.Contains(VMConstants.CHAR_SPACE))
            {
                string[] textSplit = text.Split(new char[] { VMConstants.CHAR_SPACE }, StringSplitOptions.None);
                string discipline = textSplit[0];
                string keyword1 = textSplit[1];
                IAsyncEnumerable<Keyword> keywordsBySubjectCode = _unitOfWork.Keywords.GetByDisciplineAndKeyword1(discipline, keyword1);
                await foreach (Keyword kw in keywordsBySubjectCode)
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
            ChoosedSessionViewModel choosedVm = GetViewModel<ChoosedSessionViewModel>();
            foreach (ClassGroupModel classGroupModel in choosedVm.ClassGroupModels)
            {
                classGroupModels.Add(classGroupModel.DeepClone());
            }
            #endregion

            SubjectModels.Clear();
            _phaseStore.RemoveAll();
            Messenger.Send(new SearchVmMsgs.DelAllSubjectMsg());
            UpdateCreditTotal();
            UpdateSubjectAmount();
            AddCommand.NotifyCanExecuteChanged();
            Tuple<IEnumerable<SubjectModel>, IEnumerable<ClassGroupModel>> actionData = new(subjects, classGroupModels);
            _snackbarMessageQueue.Enqueue(VMConstants.SNB_DELETE_ALL, VMConstants.SNBAC_RESTORE, AddSubjectWithCgm, actionData);
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
            LoadKeywordByDiscipline(SelectedDiscipline);
        }

        private async Task OnOpenImportDialog()
        {
            ImportSessionViewModel vm = _importSessionUC.DataContext as ImportSessionViewModel;
            OpenDialog(_importSessionUC);
            await vm.LoadScheduleSession();
        }

        private async Task HandleImportSubjects(IEnumerable<UserSubject> userSubjects)
        {
            if (userSubjects != null)
            {
                SubjectModels.Clear();
                Messenger.Send(new SearchVmMsgs.DelAllSubjectMsg());

                List<Task<Keyword>> kwTasks = new();
                foreach (UserSubject userSubject in userSubjects)
                {
                    Task<Keyword> kwTask = _unitOfWork.Keywords.GetKeyword(userSubject.SubjectCode);
                    kwTasks.Add(kwTask);
                }

                Keyword[] keywords = await Task.WhenAll(kwTasks);
                InsertPseudoSubjects(keywords);

                List<Task> downloadTasks = new();
                List<UserSubject> listOfUserSubjects = userSubjects.ToList();
                for (int i = 0; i < keywords.Length; i++)
                {
                    downloadTasks.Add(OnAddSubjectAsync(keywords[i], listOfUserSubjects[i]));
                }
                await Task.WhenAll(downloadTasks);
                SelectedSubjectModel = SubjectModels[0];
            }
        }

        private void InsertPseudoSubject(Keyword keyword)
        {
            SubjectModel pseudoSubjectModel = SubjectModel.CreatePseudo(
                keyword.SubjectName,
                keyword.Discipline.Name + VMConstants.STR_SPACE + keyword.Keyword1,
                keyword.Color,
                keyword.CourseId
            );
            SubjectModels.Insert(0, pseudoSubjectModel);
            AddCommand.NotifyCanExecuteChanged();
        }

        private void InsertPseudoSubjects(IEnumerable<Keyword> keywords)
        {
            foreach (Keyword keyword in keywords)
            {
                SubjectModel pseudoSubjectModel = SubjectModel.CreatePseudo(
                    keyword.SubjectName,
                    keyword.Discipline.Name + VMConstants.STR_SPACE + keyword.Keyword1,
                    keyword.Color,
                    keyword.CourseId
                );
                SubjectModels.Insert(0, pseudoSubjectModel);
            }
        }

        private void OnDelete()
        {
            IEnumerable<ClassGroupModel> classGroupModels = new List<ClassGroupModel>();

            ClassGroupModel classGroupModel = GetViewModel<ChoosedSessionViewModel>()
                .ClassGroupModels
                    .Where(cgm => cgm.SubjectCode.Equals(_selectedSubjectModel.SubjectCode))
                    .FirstOrDefault();

            if (classGroupModel != null)
            {
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
            _phaseStore.RemoveClassGroup(_selectedSubjectModel);
            SubjectModels.Remove(_selectedSubjectModel);
            _snackbarMessageQueue.Enqueue(message, VMConstants.SNBAC_RESTORE, AddSubjectWithCgm, actionData);
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

        private async Task<SubjectModel> OnAddSubjectAsync(Keyword keyword)
        {
            SubjectModel subjectModel = await DownloadSubject(
                keyword.Discipline.Name,
                keyword.Keyword1,
                VMConstants.INT_INVALID_COURSEID,
                IsUseCache
            );

            if (subjectModel == null)
            {
                _snackbarMessageQueue.Enqueue(VMConstants.SNB_NOT_FOUND_SUBJECT_IN_THIS_SEMESTER);
                return null;
            }

            ReplacePseudoSubject(subjectModel);

            int downloadingSubjectCount = SubjectModels.Where(sm => sm.IsDownloading).Count();
            if (downloadingSubjectCount == 0)
            {
                SelectedSubjectModel = subjectModel;
            }

            TotalSubject = SubjectModels.Count;
            AddCommand.NotifyCanExecuteChanged();
            UpdateCreditTotal();
            UpdateSubjectAmount();

            return subjectModel;
        }

        private async Task OnAddSubjectAsync(Keyword keyword, UserSubject userSubject)
        {
            SubjectModel subjectModel = await OnAddSubjectAsync(keyword);
            ClassGroupModel classGroupModel;
            classGroupModel = subjectModel.ClassGroupModels
                   .Where(cgm => cgm.Name.Equals(userSubject.ClassGroup))
                   .First();
            if (subjectModel.IsSpecialSubject)
            {
                classGroupModel.PickSchoolClass(userSubject.SchoolClass);
            }

            if (classGroupModel != null)
            {
                List<ClassGroupModel> cgms = new()
                {
                    classGroupModel
                };
                Messenger.Send(new SearchVmMsgs.SelectCgmsMsg(cgms));
            }
        }

        private async Task<SubjectModel> DownloadSubject(
            string discipline,
            string keyword1,
            int courseId,
            bool isUseCache)
        {
            Subject subject;
            if (courseId != VMConstants.INT_INVALID_COURSEID)
            {
                subject = await _subjectCrawler.Crawl(courseId, isUseCache);
            }
            else
            {
                subject = await _subjectCrawler.Crawl(discipline, keyword1, isUseCache);
            }

            if (subject != null)
            {
                AddCommand.NotifyCanExecuteChanged();
                return await SubjectModel.CreateAsync(subject, _colorGenerator);
            }
            return null;
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

                InsertPseudoSubject(keywords.First());
                await OnAddSubjectAsync(keywords.First());
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
            DeleteAllCommand.NotifyCanExecuteChanged();
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
                _phaseStore.AddClassGroupModels(classes);
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
    }
}
