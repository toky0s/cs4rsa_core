using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Cs4rsa.Common.Interfaces;
using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Views;
using Cs4rsa.Module.ManuallySchedule.Events;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Service.Dialog.Interfaces;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.Service.TeacherCrawler.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Cs4rsa.Module.ManuallySchedule.ViewModels
{
    /// <summary>
    /// Class group View Model
    /// </summary>
    public sealed partial class ClgViewModel : BindableBase
    {
        #region Properties
        public ObservableCollection<ClassGroupModel> ClassGroupModels { get; set; }
        public ObservableCollection<TeacherModel> Teachers { get; set; }

        private readonly ICollectionView _classGroupModelsView;

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

        private TeacherModel _selectedTeacher;
        public TeacherModel SelectedTeacher
        {
            get { return _selectedTeacher; }
            set { SetProperty(ref _selectedTeacher, value); OnFilter(); }
        }

        private SubjectModel _selectedSubject;
        public SubjectModel SelectedSubject
        {
            get { return _selectedSubject; }
            set { SetProperty(ref _selectedSubject, value); }
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
        public DelegateCommand GotoCourseCommand { get; set; }
        public DelegateCommand ShowDetailsSchoolClassesCommand { get; set; }
        public DelegateCommand FilterCommand { get; set; }
        public DelegateCommand ResetFilterCommand { get; set; }
        #endregion

        #region Services
        private readonly IOpenInBrowser _openInBrowser;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        #endregion

        public ClgViewModel(
            IOpenInBrowser openInBrowser, 
            IDialogService dialogService,
            IEventAggregator eventAggregator)
        {
            var hc = eventAggregator.GetHashCode();

            _openInBrowser = openInBrowser;
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;

            _eventAggregator.GetEvent<SearchVmMsgs.DelSubjectMsg>().Subscribe(sujectModel =>
            {
                ClassGroupModels.Clear();
                SelectedSubject = null;
            });

            _eventAggregator.GetEvent<SearchVmMsgs.DelAllSubjectMsg>().Subscribe(() =>
            {
                ClassGroupModels.Clear();
                TeacherCount = 0;
                SelectedSubject = null;
            });

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
                eventAggregator.GetEvent<ClassGroupSessionVmMsgs.ClassGroupAddedMsg>().Publish(classGroupModel);
            });

            ClassGroupModels = new ObservableCollection<ClassGroupModel>();
            _classGroupModelsView = CollectionViewSource.GetDefaultView(ClassGroupModels);
            _classGroupModelsView.Filter = ClassGroupFilter;

            TeacherCount = 0;
            Teachers = new ObservableCollection<TeacherModel>();
            GotoCourseCommand = new DelegateCommand(OnGotoCourse);
            ShowDetailsSchoolClassesCommand = new DelegateCommand(OnShowDetailsSchoolClasses);
            FilterCommand = new DelegateCommand(OnFilter);
            ResetFilterCommand = new DelegateCommand(OnResetFilter, CanResetFilter);

            InitFilter();
        }

        private void OnSelectedClassGroupChanged(ClassGroupModel value)
        {
            if (value != null)
            {
                if (value.IsBelongSpecialSubject)
                {
                    OnShowDetailsSchoolClasses();
                }
                else
                {
                    _eventAggregator.GetEvent<ClassGroupSessionVmMsgs.ClassGroupAddedMsg>().Publish(value);
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
            if (SelectedTeacher == null || SelectedTeacher.TeacherId == 0) return true;
            return classGroupModel.GetTeacherModels().Contains(SelectedTeacher)
                || classGroupModel.TempTeacher.Contains(SelectedTeacher.Name);
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
            _dialogService.OpenDialog(showDetailsSchoolClassesUC);
        }

        private void OnGotoCourse()
        {
            var url = SelectedClassGroup.ClassGroup.GetUrl();
            _openInBrowser.Open(url);
        }

        private void SelectedSubjectChangedHandler(SubjectModel subjectModel)
        {
            SelectedSubject = subjectModel;
            ClassGroupModels.Clear();
            Teachers.Clear();
            if (SelectedSubject != null && SelectedSubject.ClassGroupModels != null)
            {
                foreach (var classGroupModel in SelectedSubject.ClassGroupModels)
                {
                    ClassGroupModels.Add(classGroupModel);
                }

                var allTeacher = new TeacherModel(0, "TẤT CẢ");
                Teachers.Add(allTeacher);

                var tempTeachers = SelectedSubject.TempTeachers;
                // Chống trùng lặp giảng viên
                foreach (var teacher in SelectedSubject.Teachers)
                {
                    if (teacher != null && !Teachers.Any(t => t.TeacherId == teacher.TeacherId))
                    {
                        Teachers.Add(teacher);
                        tempTeachers.Remove(teacher.Name);
                    }
                }

                // Giảng viên còn sót lại trong danh sách temp
                // mà không có detail page được xem là giảng viên thỉnh giảng
                if (tempTeachers.Count > 0)
                {
                    for (var i = 0; i < tempTeachers.Count; i++)
                    {
                        var guestLecturer = new TeacherModel(i + 1, tempTeachers[i]);
                        Teachers.Add(guestLecturer);
                    }
                }
                SelectedTeacher = Teachers[0];

                // Count teacher exclude ALL option
                TeacherCount = Teachers.Count - 1;
                _classGroupModelsView.Refresh();
            }
        }
    }
}
