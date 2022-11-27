using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Dialogs.Implements;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Models;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.Crawlers;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;

using MaterialDesignThemes.Wpf;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.ViewModels
{
    public class AutoScheduleViewModel : ViewModelBase
    {
        #region Properties

        private bool _isFinding;
        public bool IsFinding
        {
            get { return _isFinding; }
            set { _isFinding = value; OnPropertyChanged(); }
        }

        private bool _isCalculated;

        public bool IsCalculated
        {
            get { return _isCalculated; }
            set { _isCalculated = value; OnPropertyChanged(); }
        }

        private Student _selectedStudent;

        public Student SelectedStudent
        {
            get { return _selectedStudent; }
            set
            {
                if (value.StudentId == "0")
                {
                    _selectedStudent = null;
                }
                else
                {
                    _selectedStudent = value;
                }
                LoadProgramCommand.NotifyCanExecuteChanged();
            }
        }


        public ObservableCollection<ProgramFolderModel> ProgramFolderModels { get; set; }
        public ObservableCollection<ProgramSubjectModel> ChoicedProSubjectModels { get; set; }
        public ObservableCollection<Student> Students { get; set; }
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

        private int _choicedCount;
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

#pragma warning disable CS0649 // Field 'AutoScheduleViewModel._programDiagram' is never assigned to, and will always have its default value null
        private ProgramDiagram _programDiagram;
#pragma warning restore CS0649 // Field 'AutoScheduleViewModel._programDiagram' is never assigned to, and will always have its default value null

        private readonly List<List<ClassGroupModel>> _filteredClassGroupModels;

        private List<List<ClassGroupModel>> _classGroupModelsOfClass;
        #endregion

        #region Commands
        public AsyncRelayCommand CannotAddReasonCommand { get; set; }
        public AsyncRelayCommand SubjectDownloadCommand { get; set; }
        public AsyncRelayCommand WatchDetailCommand { get; set; }
        public AsyncRelayCommand LoadProgramCommand { get; set; }

        public RelayCommand AddCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
        public RelayCommand GotoCourseCommand { get; set; }
        public RelayCommand ShowOnSimuCommand { get; set; }
        public RelayCommand OpenInNewWindowCommand { get; set; }
        public RelayCommand FilterChangedCommand { get; set; }
        public RelayCommand ResetFilterCommand { get; set; }
        public RelayCommand CalculateCommand { get; set; }
        public RelayCommand ValidGenCommand { get; set; }
        #endregion

        #region Dependencies
        private readonly ColorGenerator _colorGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        private readonly IOpenInBrowser _openInBrowser;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly ProgramDiagramCrawler _programDiagramCrawler;
        #endregion

        #region Filter Properties
        private bool isRemoveClassGroupInvalid;
        public bool IsRemoveClassGroupInvalid { get => isRemoveClassGroupInvalid; set { isRemoveClassGroupInvalid = value; OnPropertyChanged(); } }

        private bool phanThanh;
        private bool quangTrung;
        private bool nguyenVanLinh254;
        private bool nguyenVanLinh137;
        private bool hoaKhanhNam;
        private bool vietTin;
        public bool PhanThanh { get => phanThanh; set { phanThanh = value; OnPropertyChanged(); } }
        public bool QuangTrung { get => quangTrung; set { quangTrung = value; OnPropertyChanged(); } }
        public bool NguyenVanLinh254 { get => nguyenVanLinh254; set { nguyenVanLinh254 = value; OnPropertyChanged(); } }
        public bool NguyenVanLinh137 { get => nguyenVanLinh137; set { nguyenVanLinh137 = value; OnPropertyChanged(); } }
        public bool HoaKhanhNam { get => hoaKhanhNam; set { hoaKhanhNam = value; OnPropertyChanged(); } }
        public bool VietTin { get => vietTin; set { vietTin = value; OnPropertyChanged(); } }

        private bool mon_Aft;
        private bool mon_Mor;
        private bool mon_Nig;
        private bool tue_Aft;
        private bool tue_Mor;
        private bool tue_Nig;
        private bool wed_Aft;
        private bool wed_Mor;
        private bool wed_Nig;
        private bool thur_Aft;
        private bool thur_Mor;
        private bool thur_Nig;
        private bool fri_Aft;
        private bool fri_Mor;
        private bool fri_Nig;
        private bool sat_Aft;
        private bool sat_Mor;
        private bool sat_Nig;
        private bool sun_Aft;
        private bool sun_Mor;
        private bool sun_Nig;

        public bool Mon_Aft { get => mon_Aft; set { mon_Aft = value; OnPropertyChanged(); } }
        public bool Mon_Mor { get => mon_Mor; set { mon_Mor = value; OnPropertyChanged(); } }
        public bool Mon_Nig { get => mon_Nig; set { mon_Nig = value; OnPropertyChanged(); } }
        public bool Tue_Aft { get => tue_Aft; set { tue_Aft = value; OnPropertyChanged(); } }
        public bool Tue_Mor { get => tue_Mor; set { tue_Mor = value; OnPropertyChanged(); } }
        public bool Tue_Nig { get => tue_Nig; set { tue_Nig = value; OnPropertyChanged(); } }
        public bool Wed_Aft { get => wed_Aft; set { wed_Aft = value; OnPropertyChanged(); } }
        public bool Wed_Mor { get => wed_Mor; set { wed_Mor = value; OnPropertyChanged(); } }
        public bool Wed_Nig { get => wed_Nig; set { wed_Nig = value; OnPropertyChanged(); } }
        public bool Thur_Aft { get => thur_Aft; set { thur_Aft = value; OnPropertyChanged(); } }
        public bool Thur_Mor { get => thur_Mor; set { thur_Mor = value; OnPropertyChanged(); } }
        public bool Thur_Nig { get => thur_Nig; set { thur_Nig = value; OnPropertyChanged(); } }
        public bool Fri_Aft { get => fri_Aft; set { fri_Aft = value; OnPropertyChanged(); } }
        public bool Fri_Mor { get => fri_Mor; set { fri_Mor = value; OnPropertyChanged(); } }
        public bool Fri_Nig { get => fri_Nig; set { fri_Nig = value; OnPropertyChanged(); } }
        public bool Sat_Aft { get => sat_Aft; set { sat_Aft = value; OnPropertyChanged(); } }
        public bool Sat_Mor { get => sat_Mor; set { sat_Mor = value; OnPropertyChanged(); } }
        public bool Sat_Nig { get => sat_Nig; set { sat_Nig = value; OnPropertyChanged(); } }
        public bool Sun_Aft { get => sun_Aft; set { sun_Aft = value; OnPropertyChanged(); } }
        public bool Sun_Mor { get => sun_Mor; set { sun_Mor = value; OnPropertyChanged(); } }
        public bool Sun_Nig { get => sun_Nig; set { sun_Nig = value; OnPropertyChanged(); } }
        #endregion

        #region State
        // Nơi chưa danh sách tất cả các index của kết quả Gen.
        private List<List<int>> _tempResult;

        // Đánh dấu index của các lần gen
        private int _genIndex;

        #endregion

        public AutoScheduleViewModel(
            ICourseCrawler courseCrawler,
            IUnitOfWork unitOfWork,
            IOpenInBrowser openInBrowser,
            ISnackbarMessageQueue snackbarMessageQueue,
            ProgramDiagramCrawler programDiagramCrawler,
            ColorGenerator colorGenerator
        )
        {
            _openInBrowser = openInBrowser;
            _colorGenerator = colorGenerator;
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;
            _programDiagramCrawler = programDiagramCrawler;

            ProgramFolderModels = new();
            ChoicedProSubjectModels = new();
            CombinationModels = new();
            SubjectModels = new();
            Students = new();

            _filteredClassGroupModels = new();
            _classGroupModelsOfClass = new();

            SubjectDownloadCommand = new AsyncRelayCommand(OnDownload, CanDownload);
            WatchDetailCommand = new AsyncRelayCommand(OnWatchDetail);
            LoadProgramCommand = new AsyncRelayCommand(LoadProgramSubject, CanLoadProgram);

            AddCommand = new RelayCommand(OnAddSubject, CanAdd);
            DeleteCommand = new RelayCommand(OnDelete);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, CanDeleteAll);
            GotoCourseCommand = new RelayCommand(OnGoToCourse);
            ShowOnSimuCommand = new RelayCommand(OnShowOnSimu, CanShowOnSimu);
            FilterChangedCommand = new RelayCommand(OnFiltering);
            ResetFilterCommand = new RelayCommand(OnResetFilter);
            CalculateCommand = new RelayCommand(OnCalculate);
            ValidGenCommand = new RelayCommand(OnValidGen);

            PhanThanh = true;
            QuangTrung = true;
            NguyenVanLinh254 = true;
            NguyenVanLinh137 = true;
            VietTin = true;
            HoaKhanhNam = true;

            IsRemoveClassGroupInvalid = true;
            IsCalculated = false;
            _tempResult = new();
            _genIndex = 0;
        }

        private bool CanLoadProgram()
        {
            return _selectedStudent != null;
        }

        public async Task LoadStudents()
        {
            IEnumerable<Student> students = await _unitOfWork.Students.GetAllAsync();
            Student defaultStudent = new()
            {
                StudentId = "0",
                Name = "Chọn tài khoản..."
            };
            Students.Add(defaultStudent);
            foreach (Student student in students)
            {
                Students.Add(student);
            }
        }

        private void OnValidGen()
        {
            for (int i = _genIndex; i < _tempResult.Count; i++)
            {
                List<ClassGroupModel> classGroupModels = new();
                for (int j = 0; j < _tempResult[i].Count; j++)
                {
                    int where = _tempResult[_genIndex][j];
                    classGroupModels.Add(_classGroupModelsOfClass[j][where]);
                }
                CombinationModel combinationModel = new(SubjectModels, classGroupModels);
                CombinationModels.Add(combinationModel);
                _genIndex++;
                if (!combinationModel.IsHaveTimeConflicts() && !combinationModel.IsHavePlaceConflicts() && combinationModel.IsCanShow)
                {
                    break;
                }
            }
            if (_genIndex == _tempResult.Count)
            {
                _snackbarMessageQueue.Enqueue(VMConstants.SNB_AT_LAST_SCHEDULE);
                IsCalculated = false;
            }
        }


        /// <summary>
        /// Chạy đa luồng thực hiện list danh sách tất cả các index.
        /// </summary>
        private void OnCalculate()
        {
            OnFiltering();
            BackgroundWorker backgroundWorker = new();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            if (_filteredClassGroupModels.Count > 0)
            {
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _tempResult = e.Result as List<List<int>>;
            IsCalculated = true;
            string message = $"Đã tính toán xong với {_tempResult.Count} kết quả";
            _snackbarMessageQueue.Enqueue(message);
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Cs4rsaGen cs4rsaGen = new(_filteredClassGroupModels);
            cs4rsaGen.TempResult.Clear();
            cs4rsaGen.Backtracking(0);
            e.Result = cs4rsaGen.TempResult;
        }

        private void OnResetFilter()
        {
            IsRemoveClassGroupInvalid = true;

            PhanThanh = true;
            QuangTrung = true;
            NguyenVanLinh254 = true;
            NguyenVanLinh137 = true;
            VietTin = true;
            HoaKhanhNam = true;

            Mon_Aft = false;
            Mon_Mor = false;
            Mon_Nig = false;
            Tue_Aft = false;
            Tue_Mor = false;
            Tue_Nig = false;
            Wed_Aft = false;
            Wed_Mor = false;
            Wed_Nig = false;
            Thur_Aft = false;
            Thur_Mor = false;
            Thur_Nig = false;
            Fri_Aft = false;
            Fri_Mor = false;
            Fri_Nig = false;
            Sat_Aft = false;
            Sat_Mor = false;
            Sat_Nig = false;
            Sun_Aft = false;
            Sun_Mor = false;
            Sun_Nig = false;
            OnFiltering();
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
            CombinationModels.Clear();
            IsCalculated = false;
            _genIndex = 0;
            _tempResult.Clear();
        }

        /// <summary>
        /// Nơi triển khai bộ lọc trước khi sắp xếp
        /// </summary>
        private bool Filter(ClassGroupModel classGroupModel)
        {
            bool flagIsRemoveClassGroupInValid = IsRemoveClassGroupInValid(classGroupModel);
            bool flagIsPlaceFiltering = IsPlaceFilter(classGroupModel);
            bool flagIsFreeDayFilter = IsFreeDayFilter(classGroupModel);

            return flagIsRemoveClassGroupInValid && flagIsPlaceFiltering && flagIsFreeDayFilter;
        }

        #region FilteringMethod / Chứa danh sách các phương thức lọc
        private bool IsPlaceFilter(ClassGroupModel classGroupModel)
        {
            Dictionary<Place, bool> placeFilters = new()
            {
                { Place.QUANGTRUNG, QuangTrung },
                { Place.NVL_254, NguyenVanLinh254 },
                { Place.NVL_137, NguyenVanLinh137 },
                { Place.PHANTHANH, PhanThanh },
                { Place.VIETTIN, VietTin },
                { Place.HOAKHANH, HoaKhanhNam },
            };

            foreach (KeyValuePair<Place, bool> placeKeyValue in placeFilters)
            {
                if (placeKeyValue.Value)
                {
                    if (classGroupModel.Places.Contains(placeKeyValue.Key))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool IsRemoveClassGroupInValid(ClassGroupModel classGroupModel)
        {
            return !IsRemoveClassGroupInvalid || classGroupModel.IsHaveSchedule()
                && classGroupModel.EmptySeat > 0;
        }

        /// <summary>
        /// Bộ lọc những ngày rảnh
        /// </summary>
        private bool IsFreeDayFilter(ClassGroupModel classGroupModel)
        {
            Dictionary<Services.SubjectCrawlerSvc.DataTypes.Enums.Session, bool> Mon = new()
            {
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Morning, Mon_Mor },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Afternoon, Mon_Aft },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Night, Mon_Nig },
            };
            Dictionary<Services.SubjectCrawlerSvc.DataTypes.Enums.Session, bool> Tue = new()
            {
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Morning, Tue_Mor },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Afternoon, Tue_Aft },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Night, Tue_Nig },
            };
            Dictionary<Services.SubjectCrawlerSvc.DataTypes.Enums.Session, bool> Wed = new()
            {
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Morning, Wed_Mor },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Afternoon, Wed_Aft },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Night, Wed_Nig },
            };
            Dictionary<Services.SubjectCrawlerSvc.DataTypes.Enums.Session, bool> Thur = new()
            {
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Morning, Thur_Mor },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Afternoon, Thur_Aft },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Night, Thur_Nig },
            };
            Dictionary<Services.SubjectCrawlerSvc.DataTypes.Enums.Session, bool> Fri = new()
            {
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Morning, Fri_Mor },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Afternoon, Fri_Aft },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Night, Fri_Nig },
            };
            Dictionary<Services.SubjectCrawlerSvc.DataTypes.Enums.Session, bool> Sat = new()
            {
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Morning, Sat_Mor },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Afternoon, Sat_Aft },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Night, Sat_Nig },
            };
            Dictionary<Services.SubjectCrawlerSvc.DataTypes.Enums.Session, bool> Sun = new()
            {
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Morning, Sun_Mor },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Afternoon, Sun_Aft },
                { Services.SubjectCrawlerSvc.DataTypes.Enums.Session.Night, Sun_Nig },
            };

            Dictionary<DayOfWeek, Dictionary<Services.SubjectCrawlerSvc.DataTypes.Enums.Session, bool>> DayOfWeekAndSessionFilter = new()
            {
                { DayOfWeek.Monday, Mon },
                { DayOfWeek.Tuesday, Tue },
                { DayOfWeek.Wednesday, Wed },
                { DayOfWeek.Thursday, Thur },
                { DayOfWeek.Friday, Fri },
                { DayOfWeek.Saturday, Sat },
                { DayOfWeek.Sunday, Sun },
            };

            foreach (KeyValuePair<DayOfWeek, Dictionary<Services.SubjectCrawlerSvc.DataTypes.Enums.Session, bool>> dayOfWeekFilter in DayOfWeekAndSessionFilter)
            {
                foreach (KeyValuePair<Services.SubjectCrawlerSvc.DataTypes.Enums.Session, bool> sessionKeyValuePair in dayOfWeekFilter.Value)
                {
                    if (sessionKeyValuePair.Value)
                    {
                        bool isHasDayOfWeek = classGroupModel.Schedule.ScheduleTime.ContainsKey(dayOfWeekFilter.Key);
                        if (isHasDayOfWeek)
                        {
                            List<StudyTime> studyTimes = classGroupModel.Schedule.ScheduleTime[dayOfWeekFilter.Key];
                            IEnumerable<Services.SubjectCrawlerSvc.DataTypes.Enums.Session> sessions = studyTimes.Select(item => item.GetSession());
                            if (sessions.Contains(sessionKeyValuePair.Key))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        #endregion

        private bool CanDeleteAll()
        {
            return ChoicedProSubjectModels.Count > 0;
        }

        private void OnDeleteAll()
        {
            ChoicedProSubjectModels.Clear();
            SubjectModels.Clear();
            CombinationModels.Clear();
            _classGroupModelsOfClass.Clear();
            _filteredClassGroupModels.Clear();
            _tempResult.Clear();

            _genIndex = 0;
            IsCalculated = false;

            DeleteAllCommand.NotifyCanExecuteChanged();
            AddCommand.NotifyCanExecuteChanged();
            ValidGenCommand.NotifyCanExecuteChanged();
        }

        public async Task LoadProgramSubject()
        {
            IsFinding = true;
            ProgramFolderModels.Clear();

            ProgramFolder[] folders = await _programDiagramCrawler.ToProgramDiagram(
                string.Empty,
                _selectedStudent.SpecialString,
                _selectedStudent.StudentId
            );

            foreach (ProgramFolder folder in folders)
            {
                await AddProgramFolder(folder);
            }

            IsFinding = false;
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
            Messenger.Send(new AutoScheduleVmMsgs.ShowOnSimuMsg(_selectedCombinationModel));
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
            await vm.LoadPreProSubjectModels();
            await vm.LoadParProSubjectModels();
            OpenDialog(proSubjectDetailUC);
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
            _classGroupModelsOfClass = SubjectModels.Select(item => item.ClassGroupModels).ToList();
            SubjectDownloadCommand.NotifyCanExecuteChanged();
            AddCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            IsCalculated = false;
            _genIndex = 0;
            _tempResult.Clear();
            UpdateChoicedCount();
            UpdateCreditCount();
            OnFiltering();
        }

        /// <summary>
        /// Phương thức này không chỉ thực hiện download mà nó còn thực hiện đồng bộ
        /// giữa đã chọn và đã tải nhằm tối ưu tốc độ, thay vì việc thực hiện tải lại
        /// rất mất thời gian.
        /// </summary>
        private async Task OnDownload()
        {
            CombinationModels.Clear();
            IEnumerable<string> choiceSubjectCodes = ChoicedProSubjectModels.Select(item => item.ProgramSubject.SubjectCode);
            IEnumerable<string> wereDownloadedSubjectCodes = SubjectModels.Select(item => item.SubjectCode);

            if (!choiceSubjectCodes.Any())
            {
                wereDownloadedSubjectCodes = new List<string>();
            }

            IEnumerable<string> needDownloadNames = choiceSubjectCodes.Except(wereDownloadedSubjectCodes);
            IEnumerable<ProgramSubjectModel> needDownload = ChoicedProSubjectModels
                .Where(item => needDownloadNames.Contains(item.ProgramSubject.SubjectCode));

            if (needDownload.Any())
            {
                AutoSortSubjectLoadUC autoSortSubjectLoadUC = new();
                AutoSortSubjectLoadViewModel vm = autoSortSubjectLoadUC.DataContext as AutoSortSubjectLoadViewModel;
                OpenDialog(autoSortSubjectLoadUC);
                IAsyncEnumerable<SubjectModel> subjectModels = vm.Download(needDownload);
                CloseDialog();
                await foreach (SubjectModel subjectModel in subjectModels)
                {
                    SubjectModels.Add(subjectModel);
                }
            }
            _classGroupModelsOfClass = SubjectModels.Select(item => item.ClassGroupModels).ToList();
            IsCalculated = false;
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
    }
}
