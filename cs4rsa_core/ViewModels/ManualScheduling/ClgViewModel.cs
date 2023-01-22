using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Dialogs.Implements;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Services.TeacherCrawlerSvc.Models;
using Cs4rsa.Utils.Interfaces;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Cs4rsa.ViewModels.ManualScheduling
{
    /// <summary>
    /// Class group View Model
    /// </summary>
    internal sealed partial class ClgViewModel : ViewModelBase
    {
        #region Properties
        public ObservableCollection<ClassGroupModel> ClassGroupModels { get; set; }
        public ObservableCollection<TeacherModel> Teachers { get; set; }

        private readonly ICollectionView _classGroupModelsView;

        [ObservableProperty]
        private ClassGroupModel _selectedClassGroup;

        [ObservableProperty]
        private int _teacherCount;

        [ObservableProperty]
        private TeacherModel _selectedTeacher;

        [ObservableProperty]
        private SubjectModel _selectedSubject;
        #endregion

        #region Day Filters
        private bool _monday;
        public bool Monday
        {
            get => _monday;
            set
            {
                _monday = value;
                OnPropertyChanged();
                OnFilter();
            }
        }

        private bool _tuesday;
        public bool Tuesday
        {
            get => _tuesday;
            set
            {
                _tuesday = value;
                OnPropertyChanged();
                OnFilter();
            }
        }

        private bool _wednessday;
        public bool Wednesday
        {
            get => _wednessday;
            set
            {
                _wednessday = value;
                OnPropertyChanged();
                OnFilter();
            }
        }

        private bool _thursday;
        public bool Thursday
        {
            get => _thursday;
            set
            {
                _thursday = value;
                OnPropertyChanged();
                OnFilter();
            }
        }

        private bool _friday;
        public bool Friday
        {
            get => _friday;
            set
            {
                _friday = value;
                OnPropertyChanged();
                OnFilter();
            }
        }

        private bool _saturday;
        public bool Saturday
        {
            get => _saturday;
            set
            {
                _saturday = value;
                OnPropertyChanged();
                OnFilter();
            }
        }

        private bool _sunday;
        public bool Sunday
        {
            get => _sunday;
            set
            {
                _sunday = value;
                OnPropertyChanged();
                OnFilter();
            }
        }
        #endregion

        #region Seat Filters
        private bool _hasSeat;

        public bool HasSeat
        {
            get { return _hasSeat; }
            set
            {
                _hasSeat = value; OnPropertyChanged();
                OnFilter();
            }
        }

        private bool _hasSchedule;
        public bool HasSchedule
        {
            get { return _hasSchedule; }
            set
            {
                _hasSchedule = value; OnPropertyChanged();
                OnFilter();
            }
        }
        #endregion

        #region Session Filters
        private bool _morning;

        public bool Morning
        {
            get { return _morning; }
            set
            {
                _morning = value; OnPropertyChanged();
                OnFilter();
            }
        }
        private bool _afternoon;

        public bool Afternoon
        {
            get { return _afternoon; }
            set
            {
                _afternoon = value; OnPropertyChanged();
                OnFilter();
            }
        }
        private bool _night;

        public bool Night
        {
            get { return _night; }
            set
            {
                _night = value; OnPropertyChanged();
                OnFilter();
            }
        }

        #endregion

        #region Phase Filters
        private bool _onlyPhaseFirst;

        public bool PhaseFirst
        {
            get { return _onlyPhaseFirst; }
            set
            {
                _onlyPhaseFirst = value; OnPropertyChanged();
                OnFilter();
            }
        }

        private bool _onlyPhaseSecond;

        public bool PhaseSecond
        {
            get { return _onlyPhaseSecond; }
            set
            {
                _onlyPhaseSecond = value; OnPropertyChanged();
                OnFilter();
            }
        }
        private bool _bothPhase;

        public bool PhaseBoth
        {
            get { return _bothPhase; }
            set
            {
                _bothPhase = value; OnPropertyChanged();
                OnFilter();
            }
        }


        #endregion

        #region Place Filters
        private bool _placeQuangTrung;
        public bool PlaceQuangTrung
        {
            get { return _placeQuangTrung; }
            set
            {
                _placeQuangTrung = value; OnPropertyChanged();
                OnFilter();
            }
        }

        private bool _placeHoaKhanh;
        public bool PlaceHoaKhanh
        {
            get { return _placeHoaKhanh; }
            set { _placeHoaKhanh = value; OnPropertyChanged(); OnFilter(); }
        }

        private bool _placePhanThanh;
        public bool PlacePhanThanh
        {
            get { return _placePhanThanh; }
            set { _placePhanThanh = value; OnPropertyChanged(); OnFilter(); }
        }

        private bool _placeVietTin;
        public bool PlaceVietTin
        {
            get { return _placeVietTin; }
            set { _placeVietTin = value; OnPropertyChanged(); OnFilter(); }
        }

        private bool _place137NVL;
        public bool Place137NVL
        {
            get { return _place137NVL; }
            set { _place137NVL = value; OnPropertyChanged(); OnFilter(); }
        }

        private bool _place254NVL;
        public bool Place254NVL
        {
            get { return _place254NVL; }
            set
            {
                _place254NVL = value;
                OnPropertyChanged();
                OnFilter();
            }
        }

        private bool _placeOnline;

        public bool PlaceOnline
        {
            get { return _placeOnline; }
            set { _placeOnline = value; OnPropertyChanged(); OnFilter(); }
        }

        #endregion

        #region Commands
        public RelayCommand GotoCourseCommand { get; set; }
        public RelayCommand ShowDetailsSchoolClassesCommand { get; set; }
        public RelayCommand FilterCommand { get; set; }
        public RelayCommand ResetFilterCommand { get; set; }
        #endregion

        #region Services
        private readonly PhaseStore _phaseStore;
        private readonly IOpenInBrowser _openInBrowser;
        #endregion

        public ClgViewModel(
            PhaseStore phaseStore,
            IOpenInBrowser openInBrowser
        )
        {
            _phaseStore = phaseStore;
            _openInBrowser = openInBrowser;

            Messenger.Register<SearchVmMsgs.DelSubjectMsg>(this, (r, m) =>
            {
                ClassGroupModels.Clear();
                SelectedSubject = null;
            });

            Messenger.Register<SearchVmMsgs.DelAllSubjectMsg>(this, (r, m) =>
            {
                ClassGroupModels.Clear();
                TeacherCount = 0;
                SelectedSubject = null;
            });

            Messenger.Register<SearchVmMsgs.SelectedSubjectChangedMsg>(this, (r, m) =>
            {
                SelectedSubjectChangedHandler(m.Value);
            });

            Messenger.Register<ChoosedVmMsgs.DelClassGroupChoiceMsg>(this, (r, m) =>
            {
                SelectedClassGroup = null;
            });

            // Xử lý sự kiện chọn SchoolClass trong một ClassGroup thuộc Special Subject
            Messenger.Register<ShowDetailsSchoolClassesVmMsgs.ExitChooseMsg>(this, (r, m) =>
            {
                ClassGroupResult classGroupResult = m.Value;
                ClassGroupModel classGroupModel = classGroupResult.ClassGroupModel;
                string schoolClassName = classGroupResult.SelectedSchoolClassModel.SchoolClassName;
                classGroupModel.ReRenderScheduleRequest(schoolClassName);
                _phaseStore.AddClassGroupModel(classGroupModel);
                Messenger.Send(new ClassGroupSessionVmMsgs.ClassGroupAddedMsg(classGroupModel));
            });

            ClassGroupModels = new();
            _classGroupModelsView = CollectionViewSource.GetDefaultView(ClassGroupModels);
            _classGroupModelsView.Filter = ClassGroupFilter;

            TeacherCount = 0;
            Teachers = new();
            GotoCourseCommand = new RelayCommand(OnGotoCourse);
            ShowDetailsSchoolClassesCommand = new RelayCommand(OnShowDetailsSchoolClasses);
            FilterCommand = new RelayCommand(OnFilter);
            ResetFilterCommand = new RelayCommand(OnResetFilter, CanResetFilter);

            InitFilter();
        }

        partial void OnSelectedTeacherChanged(TeacherModel value)
        {
            OnFilter();
        }

        partial void OnSelectedClassGroupChanged(ClassGroupModel value)
        {
            if (value != null)
            {
                if (value.IsBelongSpecialSubject)
                {
                    OnShowDetailsSchoolClasses();
                }
                else
                {
                    _phaseStore.AddClassGroupModel(value);
                    Messenger.Send(new ClassGroupSessionVmMsgs.ClassGroupAddedMsg(value));
                }
            }
        }

        private bool CanResetFilter()
        {
            return
            Monday != false ||
            Tuesday != false ||
            Wednesday != false ||
            Thursday != false ||
            Friday != false ||
            Saturday != false ||
            Sunday != false ||

            Place137NVL != false ||
            Place254NVL != false ||
            PlaceHoaKhanh != false ||
            PlacePhanThanh != false ||
            PlaceQuangTrung != false ||
            PlaceVietTin != false ||
            PlaceOnline != false ||

            PhaseFirst != false ||
            PhaseSecond != false ||
            PhaseBoth != false ||

            HasSeat != false ||
            HasSchedule != false ||

            Morning != false ||
            Afternoon != false ||
            Night != false;
        }

        private bool ClassGroupFilter(object obj)
        {
            ClassGroupModel classGroupModel = obj as ClassGroupModel;
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
            Monday = false;
            Tuesday = false;
            Wednesday = false;
            Thursday = false;
            Friday = false;
            Saturday = false;
            Sunday = false;

            Place137NVL = false;
            Place254NVL = false;
            PlaceHoaKhanh = false;
            PlacePhanThanh = false;
            PlaceQuangTrung = false;
            PlaceVietTin = false;
            PlaceOnline = false;

            PhaseFirst = false;
            PhaseSecond = false;
            PhaseBoth = false;

            HasSeat = true;
            HasSchedule = true;

            Morning = false;
            Afternoon = false;
            Night = false;
        }

        /// <summary>
        /// Đặt lại bộ lọc.
        /// </summary>
        private void OnResetFilter()
        {
            Monday = false;
            Tuesday = false;
            Wednesday = false;
            Thursday = false;
            Friday = false;
            Saturday = false;
            Sunday = false;

            Place137NVL = false;
            Place254NVL = false;
            PlaceHoaKhanh = false;
            PlacePhanThanh = false;
            PlaceQuangTrung = false;
            PlaceVietTin = false;
            PlaceOnline = false;

            PhaseFirst = false;
            PhaseSecond = false;
            PhaseBoth = false;

            HasSeat = false;
            HasSchedule = false;

            Morning = false;
            Afternoon = false;
            Night = false;
        }

        /// <summary>
        /// Thực hiện lọc.
        /// </summary>
        private void OnFilter()
        {
            _classGroupModelsView.Refresh();
            ResetFilterCommand.NotifyCanExecuteChanged();
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
            if (SelectedTeacher == null || SelectedTeacher.TeacherId == 0)
                return true;
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

            Dictionary<DayOfWeek, bool> checkedDates = new()
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

            foreach (DayOfWeek day in checkedDates.Keys)
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

            Dictionary<Session, bool> checkSessions = new()
            {
                { Session.Morning, Morning },
                { Session.Afternoon, Afternoon },
                { Session.Night, Night }
            };

            checkSessions = checkSessions.Where(pair => pair.Value == true)
                .ToDictionary(p => p.Key, p => p.Value);

            foreach (Session session in checkSessions.Keys)
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
            Dictionary<Place, bool> checkboxAndPlace = new()
            {
                { Place.QUANGTRUNG, _placeQuangTrung },
                { Place.VIETTIN, _placeVietTin },
                { Place.PHANTHANH, _placePhanThanh },
                { Place.HOAKHANH, _placeHoaKhanh },
                { Place.NVL_137, _place137NVL },
                { Place.NVL_254, _place254NVL },
                { Place.ONLINE, _placeOnline }
            };
            checkboxAndPlace = checkboxAndPlace
                .Where(pair => pair.Value)
                .ToDictionary(p => p.Key, p => p.Value);
            foreach (Place place in checkboxAndPlace.Keys)
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
            ShowDetailsSchoolClassesUC showDetailsSchoolClassesUC = new();
            ShowDetailsSchoolClassesViewModel vm = showDetailsSchoolClassesUC.DataContext as ShowDetailsSchoolClassesViewModel;
            vm.ClassGroupModel = _selectedClassGroup;

            foreach (SchoolClassModel scm in _selectedClassGroup.GetSchoolClassModels())
            {
                if (scm.Type != _selectedClassGroup.CompulsoryClass.Type)
                {
                    vm.SchoolClassModels.Add(scm);
                }
            }
            OpenDialog(showDetailsSchoolClassesUC);
        }

        private void OnGotoCourse()
        {
            string url = _selectedClassGroup.ClassGroup.GetUrl();
            _openInBrowser.Open(url);
        }

        private void SelectedSubjectChangedHandler(SubjectModel subjectModel)
        {
            SelectedSubject = subjectModel;
            ClassGroupModels.Clear();
            Teachers.Clear();
            if (SelectedSubject != null)
            {
                #region Add ClassGroupModel
                foreach (ClassGroupModel classGroupModel in SelectedSubject.ClassGroupModels)
                {
                    ClassGroupModels.Add(classGroupModel);
                }
                #endregion

                #region Add Teacher
                TeacherModel allTeacher = new(0, "TẤT CẢ");
                Teachers.Add(allTeacher);

                List<string> tempTeachers = SelectedSubject.TempTeachers;
                // Chống trùng lặp giảng viên
                foreach (TeacherModel teacher in SelectedSubject.Teachers)
                {
                    if (teacher != null && !Teachers.Where(t => t.TeacherId == teacher.TeacherId).Any())
                    {
                        Teachers.Add(teacher);
                        tempTeachers.Remove(teacher.Name);
                    }
                }

                // Giảng viên còn sót lại trong danh sách temp
                // mà không có detail page được xem là giảng viên thỉnh giảng
                if (tempTeachers.Count > 0)
                {
                    for (int i = 0; i < tempTeachers.Count; i++)
                    {
                        TeacherModel guestLecturer = new(i + 1, tempTeachers[i]);
                        Teachers.Add(guestLecturer);
                    }
                }
                SelectedTeacher = Teachers[0];
                #endregion

                // Count teacher exclude ALL option
                TeacherCount = Teachers.Count - 1;
                _classGroupModelsView.Refresh();
            }
        }
    }
}
