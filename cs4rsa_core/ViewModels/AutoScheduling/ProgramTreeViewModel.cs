using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Models;
using Cs4rsa.Models.AutoScheduling;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;

using MaterialDesignThemes.Wpf;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.ViewModels.AutoScheduling
{
    internal sealed partial class ProgramTreeViewModel : ViewModelBase
    {
        private readonly MyProgramUC _myProgramUC;
        private readonly AccountUC _accountUC;

        /// <summary>
        /// Danh sách bộ Cgm sau khi được filter
        /// 
        /// Danh sách này sẽ được load lại mỗi khi người dùng
        /// 1. Thêm một PSM (đã tải) mới.
        /// 2. Xoá một (hoặc tất cả) PSM.
        /// 3. Thực hiện một filter trong danh sách các filter.
        /// 4. Bật tắt sử dụng filter.
        /// Thông qua phương thức ReGen().
        /// </summary>
        private readonly List<List<ClassGroupModel>> _rltAftFilterCgms;

        public ObservableCollection<Student> Students { get; set; }
        public ObservableCollection<PlanTableModel> PlanTableModels { get; set; }
        public ObservableCollection<ProgramFolderModel> ProgramFolderModels { get; set; }
        public ObservableCollection<SubjectModel> SubjectModels { get; set; }

        /// <summary>
        /// Danh sách các môn đã chọn để sắp xếp
        /// </summary>
        public ObservableCollection<ProgramSubjectModel> ChoosedProSubjectModels { get; set; }

        [ObservableProperty]
        private bool _isFinding;

        /// <summary>
        /// Sau khi xoá một hoặc nhiều PSM. Các CGM đã được tải
        /// sẽ được giữ lại hoặc không và cờ IsDownloaded của PSM 
        /// dựa vào giá trị của trường này. Nếu True, việc xoá không
        /// làm mất những CGM đã tải, ngược lại mọi CGM sẽ bị xoá
        /// hết và đưa cờ IsDownloaded về False.
        /// </summary>
        [ObservableProperty]
        private bool _isSave;

        /// <summary>
        /// Cờ xác định có sử dụng bộ lọc hay không
        /// Nếu có, tất cả các CGM đều phải đi qua bộ
        /// lọc trước khi tính toán.
        /// Nếu không, mọi CGM đều có thể đi qua bộ lọc.
        /// </summary>
        [ObservableProperty]
        private bool _isUseFilter;
        [ObservableProperty]
        private Student _selectedStudent;

        [ObservableProperty]
        private bool _phanThanh;
        [ObservableProperty]
        private bool _quangTrung;
        [ObservableProperty]
        private bool _nguyenVanLinh254;
        [ObservableProperty]
        private bool _nguyenVanLinh137;
        [ObservableProperty]
        private bool _hoaKhanhNam;
        [ObservableProperty]
        private bool _vietTin;
        [ObservableProperty]
        private bool _online;

        [ObservableProperty]
        private bool _mon_Aft;
        [ObservableProperty]
        private bool _mon_Mor;
        [ObservableProperty]
        private bool _mon_Nig;
        [ObservableProperty]
        private bool _tue_Aft;
        [ObservableProperty]
        private bool _tue_Mor;
        [ObservableProperty]
        private bool _tue_Nig;
        [ObservableProperty]
        private bool _wed_Aft;
        [ObservableProperty]
        private bool _wed_Mor;
        [ObservableProperty]
        private bool _wed_Nig;
        [ObservableProperty]
        private bool _thur_Aft;
        [ObservableProperty]
        private bool _thur_Mor;
        [ObservableProperty]
        private bool _thur_Nig;
        [ObservableProperty]
        private bool _fri_Aft;
        [ObservableProperty]
        private bool _fri_Mor;
        [ObservableProperty]
        private bool _fri_Nig;
        [ObservableProperty]
        private bool _sat_Aft;
        [ObservableProperty]
        private bool _sat_Mor;
        [ObservableProperty]
        private bool _sat_Nig;
        [ObservableProperty]
        private bool _sun_Aft;
        [ObservableProperty]
        private bool _sun_Mor;
        [ObservableProperty]
        private bool _sun_Nig;

        /// <summary>
        /// Nơi chưa danh sách tất cả các index của kết quả Gen.
        /// </summary>
        private List<List<int>> _tempResult;

        // Đánh dấu index của các lần gen
        private int _genIndex;

        [ObservableProperty]
        private bool _isCalculated;

        [ObservableProperty]
        private bool _isValidGen;

        /// <summary>
        /// Cờ xác định có PASS các CGM không có lịch hoặc hết chỗ hay không.
        /// Nếu có, các CGM không có lịch hoặc hết chỗ sẽ bị remove.
        /// Nếu không, CGM không có lịch hoặc hết chỗ có thể chấp nhận.
        /// </summary>
        [ObservableProperty]
        private bool _isRemoveClassGroupInvalid;

        [ObservableProperty]
        private CombinationModel _selectedCombinationModel;

        [ObservableProperty]
        private int _combinationCount;

        [ObservableProperty]
        private bool _isDownloading;

        public AsyncRelayCommand CannotAddReasonCommand { get; set; }
        public AsyncRelayCommand LoadProgramCommand { get; set; }
        public AsyncRelayCommand DownloadCommand { get; set; }
        public AsyncRelayCommand FilterChangedCommand { get; set; }
        public AsyncRelayCommand ValidGenCommand { get; set; }
        public AsyncRelayCommand<ProgramSubjectModel> AddCommand { get; set; }
        public AsyncRelayCommand CalculateCommand { get; set; }
        public AsyncRelayCommand ResetFilterCommand { get; set; }

        public RelayCommand CollapseCommand { get; set; }

        /// <summary>
        /// Hiển thị Dialog chứa chương trình học dự kiến
        /// của người dùng hiện tại
        /// </summary>
        public RelayCommand MyProgramCommand { get; set; }
        public RelayCommand<ProgramSubjectModel> DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
        public RelayCommand GotoCourseCommand { get; set; }
        public RelayCommand ShowOnSimuCommand { get; set; }
        public RelayCommand OpenInNewWindowCommand { get; set; }
        public RelayCommand AccountCommand { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ColorGenerator _colorGenerator;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly ISubjectCrawler _subjectCrawler;
        public ProgramTreeViewModel(
            IUnitOfWork unitOfWork,
            ColorGenerator colorGenerator,
            ISnackbarMessageQueue snackbarMessageQueue,
            ISubjectCrawler subjectCrawler
        )
        {
            _unitOfWork = unitOfWork;
            _colorGenerator = colorGenerator;
            _snackbarMessageQueue = snackbarMessageQueue;
            _subjectCrawler = subjectCrawler;
            _tempResult = new();
            _myProgramUC = new() { DataContext = this };
            _accountUC = new();
            _rltAftFilterCgms = new();

            IsDownloading = false;
            IsSave = true;
            IsUseFilter = true;
            ChoosedProSubjectModels = new();
            PlanTableModels = new();
            ProgramFolderModels = new();
            Students = new();
            SubjectModels = new();

            PhanThanh = true;
            QuangTrung = true;
            NguyenVanLinh254 = true;
            NguyenVanLinh137 = true;
            VietTin = true;
            HoaKhanhNam = true;
            Online = true;
            IsRemoveClassGroupInvalid = true;

            LoadProgramCommand = new(LoadProgramSubject, () => _selectedStudent != null);
            AccountCommand = new(() => OpenDialog(_accountUC));

            AddCommand = new((ProgramSubjectModel psm) => OnAddCommand(psm));

            CalculateCommand = new(
                OnCalculate,
                () => ChoosedProSubjectModels.Where(psm => psm.IsDownloaded)
                                             .Count() == ChoosedProSubjectModels.Count
                    && ChoosedProSubjectModels.Any()
                    && !_tempResult.Any()
                    && !_isCalculated
            );

            DeleteCommand = new((psm) => OnDelete(psm));

            DeleteAllCommand = new(
                OnDeleteAll,
                () => ChoosedProSubjectModels.Any()
            );

            DownloadCommand = new(
                OnDownload,
                () => ChoosedProSubjectModels
                        .Where(psm => psm.IsDownloaded == false)
                        .Any()
            );

            FilterChangedCommand = new(OnFiltering);

            MyProgramCommand = new(
                () => OpenDialog(_myProgramUC),
                () => _selectedStudent != null && !_selectedStudent.StudentId.Equals("0")
            );

            ResetFilterCommand = new(OnResetFilter);

            ValidGenCommand = new(
                OnValidGen,
                () => _tempResult.Any()
                    && ChoosedProSubjectModels.Any()
                    && !IsValidGen
            );

            Messenger.Register<SessionInputVmMsgs.ExitFindStudentMsg>(this, (r, m) =>
            {
                Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadStudents();
                });
            });

            Messenger.Register<AccountVmMsgs.DelStudentMsg>(this, (r, m) =>
            {
                Student student = Students.Where(student => student.StudentId.Equals(m.Value)).First();
                Students.Remove(student);
                ProgramFolderModels.Clear();
            });

            Messenger.Register<AccountVmMsgs.UndoDelStudentMsg>(this, (r, m) =>
            {
                Students.Add(m.Value);
                if (Students.Count == 1)
                {
                    SelectedStudent = Students[0];
                }
            });

            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await LoadStudents();
            });

            CalculateCommand.NotifyCanExecuteChanged();
            DownloadCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedStudentChanged(Student value)
        {
            Application.Current.Dispatcher.InvokeAsync(async () => await LoadStudentPlan());
            LoadProgramCommand.NotifyCanExecuteChanged();
            MyProgramCommand.NotifyCanExecuteChanged();
        }

        private async Task OnResetFilter()
        {
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

            PhanThanh = true;
            QuangTrung = true;
            NguyenVanLinh254 = true;
            NguyenVanLinh137 = true;
            VietTin = true;
            HoaKhanhNam = true;
            Online = true;

            IsRemoveClassGroupInvalid = true;

            await OnFiltering();
        }

        private void OnDelete(ProgramSubjectModel psm)
        {
            psm.IsChoosed = false;
            psm.IsDownloaded = IsSave;
            psm.Status = string.Empty;
            if (!IsSave)
            {
                psm.Cgms.Clear();
                psm.ReviewFtCgms.Clear();
            }

            _genIndex = 0;
            _tempResult.Clear();
            IsCalculated = false;

            ChoosedProSubjectModels.Remove(psm);
            CalculateCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            DownloadCommand.NotifyCanExecuteChanged();
            ValidGenCommand.NotifyCanExecuteChanged();
            AddCommand.NotifyCanExecuteChanged();

            ReGen();
        }

        /// <summary>
        /// OnDeleteAll
        /// 
        /// 1. Chuyển cờ IsChoosed và IsDownloaded của từng PSM
        ///    và clear Cgms tương ứng với PSM.
        /// 2. Clear ChoosedProSubjectModels. 
        /// </summary>
        private void OnDeleteAll()
        {
            foreach (ProgramSubjectModel psm in ChoosedProSubjectModels)
            {
                psm.IsChoosed = false;
                psm.IsDownloaded = IsSave;
                psm.Status = string.Empty;

                if (!IsSave)
                {
                    psm.Cgms.Clear();
                    psm.ReviewFtCgms.Clear();
                }
            }
            IsCalculated = false;
            _genIndex = 0;
            _tempResult.Clear();

            ChoosedProSubjectModels.Clear();
            CalculateCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            ValidGenCommand.NotifyCanExecuteChanged();
            DownloadCommand.NotifyCanExecuteChanged();

            ReGen();
        }

        private async Task OnAddCommand(ProgramSubjectModel psm)
        {
            psm.FilterFuncs = new List<Func<ClassGroupModel, bool>>()
            {
                IsFreeDayFilter,
                IsPlaceFilter,
                IsRemoveClassGroupInValid
            };
            psm.IsChoosed = true;

            if (psm.IsDownloaded)
            {
                if (_isUseFilter)
                {
                    await psm.ApplyFilter();
                }
                else
                {
                    psm.ResetFilter();
                }

                ReGen();
            }

            IsCalculated = false;

            ChoosedProSubjectModels.Add(psm);
            AddCommand.NotifyCanExecuteChanged();
            CalculateCommand.NotifyCanExecuteChanged();
            DownloadCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
        }

        private async Task LoadStudents()
        {
            IAsyncEnumerable<Student> students = _unitOfWork.Students.GetAll();
            await foreach (Student student in students)
            {
                Students.Add(student);
            }
            if (Students.Any())
            {
                SelectedStudent = Students.First();
            }
        }

        /// <summary>
        /// Load chương trình học dự kiến của sinh viên.
        /// Đánh giá các môn học có sẵn trong học kỳ hiện tại
        /// cùng với tình trạng học của môn học đó so với
        /// chương trình học (Đã qua, Đang học/chưa có điểm, Chưa học)
        /// </summary>
        private async Task LoadStudentPlan()
        {
            if (_selectedStudent == null) return;
            PlanTableModels.Clear();

            string planPath = CredizText.PathPlanJsonFile(_selectedStudent.CurriculumId);
            if (File.Exists(planPath))
            {
                string json = await File.ReadAllTextAsync(planPath);
                PlanTable[] planTables = JsonConvert.DeserializeObject<PlanTable[]>(json);

                foreach (PlanTable planTable in planTables)
                {
                    PlanTableModels.Add(await PlanTableModel.Build(planTable, _unitOfWork));
                }
            }
            else
            {
                MessageBox.Show(
                    CredizText.AutoMsg001(planPath, _selectedStudent.Name),
                    "Thông báo",
                    MessageBoxButton.OK);
            }
        }

        private async Task LoadProgramSubject()
        {
            ProgramFolderModels.Clear();
            string programPath = CredizText.PathProgramJsonFile(_selectedStudent.StudentId);
            if (File.Exists(programPath))
            {
                string json = await File.ReadAllTextAsync(programPath);
                ProgramFolder[] programFolders = JsonConvert.DeserializeObject<ProgramFolder[]>(json);

                List<Task<ProgramFolderModel>> tasks = new();
                foreach (ProgramFolder programFolder in programFolders)
                {
                    Task<ProgramFolderModel> pfmTask = ProgramFolderModel.CreateAsync(
                        programFolder,
                        _colorGenerator,
                        _unitOfWork
                    );
                    tasks.Add(pfmTask);
                }
                ProgramFolderModel[] pfms = await Task.WhenAll(tasks);

                foreach (ProgramFolderModel pfm in pfms)
                {
                    ProgramFolderModels.Add(pfm);
                }
            }
            else
            {
                MessageBox.Show(CredizText.AutoMsg001(programPath, _selectedStudent.Name), "Thông báo", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Phương thức này không chỉ thực hiện download mà nó còn thực hiện đồng bộ
        /// giữa đã chọn và đã tải nhằm tối ưu tốc độ, thay vì việc thực hiện tải lại
        /// rất mất thời gian.
        /// </summary>
        private async Task OnDownload()
        {
            IEnumerable<ProgramSubjectModel> needToDownloadPsms = ChoosedProSubjectModels.Where(psm => psm.IsDownloaded == false);
            foreach (ProgramSubjectModel psm in needToDownloadPsms)
            {
                psm.IsDownloading = true;
            }

            if (needToDownloadPsms.Any())
            {
                IsDownloading = true;
                List<Task> tasks = new();
                foreach (ProgramSubjectModel psm in needToDownloadPsms)
                {
                    tasks.Add(Download(psm));
                }
                await Task.WhenAll(tasks);
                IsDownloading = false;
                IsCalculated = false;
            }

            CalculateCommand.NotifyCanExecuteChanged();
        }

        public async Task Download(ProgramSubjectModel psm)
        {
            Subject subject;
            try
            {
                subject = await _subjectCrawler.Crawl(int.Parse(psm.CourseId), true, true);

                if (subject == null)
                {
                    psm.Status = "Môn học không tồn tại";
                    psm.Exists = false;
                    psm.IsDownloading = false;
                    psm.IsDownloaded = false;
                }
                else
                {
                    SubjectModel sm = await SubjectModel.CreateAsync(subject, _colorGenerator);
                    psm.AddCgms(sm.ClassGroupModels);
                    if (_isUseFilter)
                    {
                        await psm.ApplyFilter();
                    }
                    else
                    {
                        psm.ResetFilter();
                    }

                    psm.IsStarted = true;
                    psm.IsDownloaded = true;

                    ReGen();
                    psm.IsDownloading = false;
                    psm.Exists = true;
                }
            }
            catch (Exception e)
            {
                psm.Status = e.Message;
                psm.Exists = false;
                psm.IsDownloading = false;
                psm.IsDownloaded = false;
                psm.IsStarted = false;
            }
        }

        partial void OnSelectedCombinationModelChanged(CombinationModel value)
        {
            ShowOnSimuCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Bắt đầu việc các tạo bộ lịch dựa trên kết quả tạm đã tính được.
        /// Đưa danh sách bộ lịch sang bộ tạo kết quả hiển thị.
        /// </summary>
        private async Task OnValidGen()
        {
            List<CombinationModel> cbms = await Task.Run(ValidGenDoWork);
            if (cbms.Count > 0)
            {
                Messenger.Send(new AutoVmMsgs.AddCombinationsMsg(cbms));
                IsValidGen = true;
                ValidGenCommand.NotifyCanExecuteChanged();
            }
            else if (cbms.Count == 0)
            {

            }
        }

        private List<CombinationModel> ValidGenDoWork()
        {
            List<CombinationModel> cbms = new();
            for (int i = _genIndex; i < _tempResult.Count; i++)
            {
                List<ClassGroupModel> classGroupModels = new();
                for (int j = 0; j < _tempResult[i].Count; j++)
                {
                    int where = _tempResult[_genIndex][j];
                    classGroupModels.Add(_rltAftFilterCgms[j][where]);
                }
                CombinationModel combinationModel = new(SubjectModels, classGroupModels);
                cbms.Add(combinationModel);
                _genIndex++;
            }
            _snackbarMessageQueue.Enqueue(VMConstants.SNB_CAL_DONE);
            return cbms;
        }

        /// <summary>
        /// Chạy đa luồng thực hiện list danh sách tất cả các index.
        /// </summary>
        private async Task OnCalculate()
        {
            _tempResult = await Task.Run(OnCalculateDoWork);
            IsCalculated = true;
            string message;
            if (_tempResult.Any())
            {
                message = $"Đã tính toán xong với {_tempResult.Count} kết quả";
                IsValidGen = false;
                ValidGenCommand.NotifyCanExecuteChanged();
            }
            else
            {
                message = "Không có kết quả nào hết";
            }
            _snackbarMessageQueue.Enqueue(message);
        }

        private List<List<int>> OnCalculateDoWork()
        {
            List<IEnumerable<ClassGroupModel>> fltCgms = ChoosedProSubjectModels
                .Select(psm => psm.GetRltAftFilter())
                .ToList();
            Cs4rsaGen cs4rsaGen = new(fltCgms);
            cs4rsaGen.TempResult.Clear();
            cs4rsaGen.Backtracking(0);
            return cs4rsaGen.TempResult;
        }

        /// <summary>
        /// Thực hiện lọc danh sách các CGM từ PSM
        /// </summary>
        private async Task OnFiltering()
        {
            if (_isUseFilter)
            {
                foreach (ProgramSubjectModel psm in ChoosedProSubjectModels)
                {
                    await psm.ApplyFilter();
                }
            }
            else
            {
                foreach (ProgramSubjectModel psm in ChoosedProSubjectModels)
                {
                    psm.ResetFilter();
                }
            }

            IsCalculated = false;
            IsValidGen = false;
            _genIndex = 0;
            _tempResult.Clear();

            CalculateCommand.NotifyCanExecuteChanged();
            ReGen();
        }

        /// <summary>
        /// Bộ lọc những ngày rảnh
        /// </summary>
        private bool IsFreeDayFilter(ClassGroupModel classGroupModel)
        {
            Dictionary<Session, bool> Mon = new()
            {
                { Session.Morning,   Mon_Mor },
                { Session.Afternoon, Mon_Aft },
                { Session.Night,     Mon_Nig },
            };
            Dictionary<Session, bool> Tue = new()
            {
                { Session.Morning,   Tue_Mor },
                { Session.Afternoon, Tue_Aft },
                { Session.Night,     Tue_Nig },
            };
            Dictionary<Session, bool> Wed = new()
            {
                { Session.Morning,   Wed_Mor },
                { Session.Afternoon, Wed_Aft },
                { Session.Night,     Wed_Nig },
            };
            Dictionary<Session, bool> Thur = new()
            {
                { Session.Morning,   Thur_Mor },
                { Session.Afternoon, Thur_Aft },
                { Session.Night,     Thur_Nig },
            };
            Dictionary<Session, bool> Fri = new()
            {
                { Session.Morning,   Fri_Mor },
                { Session.Afternoon, Fri_Aft },
                { Session.Night,     Fri_Nig },
            };
            Dictionary<Session, bool> Sat = new()
            {
                { Session.Morning,   Sat_Mor },
                { Session.Afternoon, Sat_Aft },
                { Session.Night,     Sat_Nig },
            };
            Dictionary<Session, bool> Sun = new()
            {
                { Session.Morning,   Sun_Mor },
                { Session.Afternoon, Sun_Aft },
                { Session.Night,     Sun_Nig },
            };

            Dictionary<DayOfWeek, Dictionary<Session, bool>> DayOfWeekAndSessionFilter = new()
            {
                { DayOfWeek.Monday,     Mon  },
                { DayOfWeek.Tuesday,    Tue  },
                { DayOfWeek.Wednesday,  Wed  },
                { DayOfWeek.Thursday,   Thur },
                { DayOfWeek.Friday,     Fri  },
                { DayOfWeek.Saturday,   Sat  },
                { DayOfWeek.Sunday,     Sun  },
            };

            foreach (KeyValuePair<DayOfWeek, Dictionary<Session, bool>> dayOfWeekFilter in DayOfWeekAndSessionFilter)
            {
                foreach (KeyValuePair<Session, bool> sessionKeyValuePair in dayOfWeekFilter.Value)
                {
                    if (!sessionKeyValuePair.Value) continue;

                    bool isHasDayOfWeek = classGroupModel.Schedule.ScheduleTime.ContainsKey(dayOfWeekFilter.Key);
                    if (!isHasDayOfWeek) continue;

                    List<StudyTime> studyTimes = classGroupModel.Schedule.ScheduleTime[dayOfWeekFilter.Key];
                    IEnumerable<Session> sessions = studyTimes.Select(item => item.GetSession());
                    if (sessions.Contains(sessionKeyValuePair.Key))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Bộ lọc nơi học.
        /// PASS: Khi CGM có nơi học thoả mãn hoặc không chứa nơi học.
        /// FAIL: Khi CGM chứa nơi học, nhưng không có nơi nào thoả mãn.
        /// </summary>
        /// <param name="classGroupModel">ClassGroupModel</param>
        /// <returns>Boolean</returns>
        private bool IsPlaceFilter(ClassGroupModel classGroupModel)
        {
            Dictionary<Place, bool> placeFilters = new()
            {
                { Place.QUANGTRUNG ,QuangTrung          },
                { Place.NVL_254    ,NguyenVanLinh254    },
                { Place.NVL_137    ,NguyenVanLinh137    },
                { Place.PHANTHANH  ,PhanThanh           },
                { Place.VIETTIN    ,VietTin             },
                { Place.HOAKHANH   ,HoaKhanhNam         },
                { Place.ONLINE     ,Online              },
            };

            if (!classGroupModel.Places.Any()) return true;

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
            return !IsRemoveClassGroupInvalid
                || (classGroupModel.IsHaveSchedule() && classGroupModel.EmptySeat > 0);
        }

        private void ReGen()
        {
            _rltAftFilterCgms.Clear();
            foreach (ProgramSubjectModel psm in ChoosedProSubjectModels)
            {
                _rltAftFilterCgms.Add(psm.GetRltAftFilter().ToList());
            }
        }
    }
}
