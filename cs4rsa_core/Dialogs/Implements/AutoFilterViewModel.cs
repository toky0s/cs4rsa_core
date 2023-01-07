using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Cs4rsa.BaseClasses;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Models;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils.Interfaces;

using MaterialDesignThemes.Wpf;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Dialogs.Implements
{
    internal partial class AutoFilterViewModel : DialogVmBase
    {
        #region Filter Properties
        private readonly List<IEnumerable<ClassGroupModel>> _filteredClassGroupModels;
        private List<List<ClassGroupModel>> _classGroupModelsOfClass;

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

        #region Properties
        public ObservableCollection<ProgramFolderModel> ProgramFolderModels { get; set; }
        public ObservableCollection<ProgramSubjectModel> ChoicedProSubjectModels { get; set; }
        public ObservableCollection<SubjectModel> SubjectModels { get; set; }

        [ObservableProperty]
        private bool _isFinding;


        [ObservableProperty]
        private ProgramSubjectModel _selectedProSubjectInChoiced;

        [ObservableProperty]
        private CombinationModel _selectedCombinationModel;

        [ObservableProperty]
        private int _combinationCount;

        [ObservableProperty]
        private int _choicedCount;

        [ObservableProperty]
        private int _creditCount;
        #endregion

        public ObservableCollection<CombinationModel> CombinationModels { get; set; }

        [ObservableProperty]
        private bool _isCalculated;


        #region Commands
        public AsyncRelayCommand CannotAddReasonCommand { get; set; }
        public AsyncRelayCommand SubjectDownloadCommand { get; set; }

        public RelayCommand CalculateCommand { get; set; }
        public RelayCommand FilterChangedCommand { get; set; }
        public RelayCommand ResetFilterCommand { get; set; }
        public RelayCommand ValidGenCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
        public RelayCommand GotoCourseCommand { get; set; }
        public RelayCommand ShowOnSimuCommand { get; set; }
        public RelayCommand OpenInNewWindowCommand { get; set; }
        public RelayCommand AddCommand { get; set; }

        #endregion

        #region Dependencies
        private readonly ICourseCrawler _courseCrawler;
        private readonly IOpenInBrowser _openInBrowser;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion
        public AutoFilterViewModel(
             ICourseCrawler courseCrawler,
            IOpenInBrowser openInBrowser,
            ISnackbarMessageQueue snackbarMessageQueue
        )
        {
            _openInBrowser = openInBrowser;
            _courseCrawler = courseCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;

            SubjectDownloadCommand = new AsyncRelayCommand(OnDownload, CanDownload);

            DeleteCommand = new RelayCommand(OnDelete);

            GotoCourseCommand = new RelayCommand(OnGoToCourse);


            CombinationModels = new();

            IsCalculated = false;

            FilterChangedCommand = new RelayCommand(OnFiltering);
            ResetFilterCommand = new RelayCommand(OnResetFilter);


            ProgramFolderModels = new();
            ChoicedProSubjectModels = new();

            SubjectModels = new();

            PhanThanh = true;
            QuangTrung = true;
            NguyenVanLinh254 = true;
            NguyenVanLinh137 = true;
            VietTin = true;
            HoaKhanhNam = true;
            IsRemoveClassGroupInvalid = true;

            _filteredClassGroupModels = new();
            _classGroupModelsOfClass = new();
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
            Dictionary<Session, bool> Mon = new()
            {
                { Session.Morning, Mon_Mor },
                { Session.Afternoon, Mon_Aft },
                { Session.Night, Mon_Nig },
            };
            Dictionary<Session, bool> Tue = new()
            {
                { Session.Morning, Tue_Mor },
                { Session.Afternoon, Tue_Aft },
                { Session.Night, Tue_Nig },
            };
            Dictionary<Session, bool> Wed = new()
            {
                { Session.Morning, Wed_Mor },
                { Session.Afternoon, Wed_Aft },
                { Session.Night, Wed_Nig },
            };
            Dictionary<Session, bool> Thur = new()
            {
                { Session.Morning, Thur_Mor },
                { Session.Afternoon, Thur_Aft },
                { Session.Night, Thur_Nig },
            };
            Dictionary<Session, bool> Fri = new()
            {
                { Session.Morning, Fri_Mor },
                { Session.Afternoon, Fri_Aft },
                { Session.Night, Fri_Nig },
            };
            Dictionary<Session, bool> Sat = new()
            {
                { Session.Morning, Sat_Mor },
                { Session.Afternoon, Sat_Aft },
                { Session.Night, Sat_Nig },
            };
            Dictionary<Session, bool> Sun = new()
            {
                { Session.Morning, Sun_Mor },
                { Session.Afternoon, Sun_Aft },
                { Session.Night, Sun_Nig },
            };

            Dictionary<DayOfWeek, Dictionary<Session, bool>> DayOfWeekAndSessionFilter = new()
            {
                { DayOfWeek.Monday, Mon },
                { DayOfWeek.Tuesday, Tue },
                { DayOfWeek.Wednesday, Wed },
                { DayOfWeek.Thursday, Thur },
                { DayOfWeek.Friday, Fri },
                { DayOfWeek.Saturday, Sat },
                { DayOfWeek.Sunday, Sun },
            };

            foreach (KeyValuePair<DayOfWeek, Dictionary<Session, bool>> dayOfWeekFilter in DayOfWeekAndSessionFilter)
            {
                foreach (KeyValuePair<Session, bool> sessionKeyValuePair in dayOfWeekFilter.Value)
                {
                    if (sessionKeyValuePair.Value)
                    {
                        bool isHasDayOfWeek = classGroupModel.Schedule.ScheduleTime.ContainsKey(dayOfWeekFilter.Key);
                        if (isHasDayOfWeek)
                        {
                            List<StudyTime> studyTimes = classGroupModel.Schedule.ScheduleTime[dayOfWeekFilter.Key];
                            IEnumerable<Session> sessions = studyTimes.Select(item => item.GetSession());
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
        }
        #endregion 

        private bool CanDownload()
        {
            return true;
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
    }
}
