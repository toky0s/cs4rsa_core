using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Dialogs.Implements;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Models;
using Cs4rsa.Models.AutoScheduling;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;

using MaterialDesignThemes.Wpf;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        [ObservableProperty]
        public bool _isRemoveClassGroupInvalid;

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
        [ObservableProperty]
        private Student _selectedStudent;
        [ObservableProperty]
        private ProgramSubjectModel _selectedProSubject;
        [ObservableProperty]
        public bool _phanThanh;
        [ObservableProperty]
        public bool _quangTrung;
        [ObservableProperty]
        public bool _nguyenVanLinh254;
        [ObservableProperty]
        public bool _nguyenVanLinh137;
        [ObservableProperty]
        public bool _hoaKhanhNam;
        [ObservableProperty]
        public bool _vietTin;

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
        /// Môn học được chọn trên cây chương trình
        /// </summary>
        [ObservableProperty]
        private ProgramSubjectModel _sltSubjectTreeItem;

        public AsyncRelayCommand LoadProgramCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand CalculateCommand { get; set; }
        public RelayCommand AccountCommand { get; set; }

        /// <summary>
        /// Hiển thị Dialog chứa chương trình học dự kiến
        /// của người dùng hiện tại
        /// </summary>
        public RelayCommand MyProgramCommand { get; set; }

        /// <summary>
        /// Nơi chưa danh sách tất cả các index của kết quả Gen.
        /// </summary>
        private List<List<int>> _tempResult;

        // Đánh dấu index của các lần gen
        private int _genIndex;

        [ObservableProperty]
        private bool _isCalculated;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ColorGenerator _colorGenerator;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;

        private readonly List<IEnumerable<ClassGroupModel>> _filteredClassGroupModels;
        private List<List<ClassGroupModel>> _classGroupModelsOfClass;


        [ObservableProperty]
        private CombinationModel _selectedCombinationModel;

        [ObservableProperty]
        private int _combinationCount;

        public AsyncRelayCommand CannotAddReasonCommand { get; set; }
        public AsyncRelayCommand DownloadCommand { get; set; }

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
        public RelayCommand GotoCourseCommand { get; set; }
        public RelayCommand ShowOnSimuCommand { get; set; }
        public RelayCommand OpenInNewWindowCommand { get; set; }
        public RelayCommand FilterChangedCommand { get; set; }
        public RelayCommand ResetFilterCommand { get; set; }
        public RelayCommand ValidGenCommand { get; set; }



        public ProgramTreeViewModel(
            IUnitOfWork unitOfWork,
            ColorGenerator colorGenerator,
            ISnackbarMessageQueue snackbarMessageQueue
            )
        {
            _unitOfWork = unitOfWork;
            _colorGenerator = colorGenerator;
            _snackbarMessageQueue = snackbarMessageQueue;
            _filteredClassGroupModels = new();
            _tempResult = new();
            _myProgramUC = new() { DataContext = this };
            _accountUC = new();

            ChoosedProSubjectModels = new();
            PlanTableModels = new();
            ProgramFolderModels = new();
            Students = new();
            SubjectModels = new();

            LoadProgramCommand = new(LoadProgramSubject, () => _selectedStudent != null);
            AccountCommand = new(() => OpenDialog(_accountUC));

            AddCommand = new(OnAddCommand,
                () => _sltSubjectTreeItem != null
                    && !_sltSubjectTreeItem.IsDone
                    && _sltSubjectTreeItem.IsAvaiable
                    && !ChoosedProSubjectModels.Contains(_sltSubjectTreeItem)
            );

            CalculateCommand = new(
                OnCalculate,
                () => SubjectModels.Any()
            );

            MyProgramCommand = new(
                () => OpenDialog(_myProgramUC),
                () => _selectedStudent != null && !_selectedStudent.StudentId.Equals("0")
            );


            DownloadCommand = new(OnDownload, () => ChoosedProSubjectModels.Any());

            ValidGenCommand = new(
                OnValidGen,
                () => _tempResult.Any()
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
        }

        partial void OnSelectedStudentChanged(Student value)
        {
            Application.Current.Dispatcher.InvokeAsync(async () => await LoadStudentPlan());
            LoadProgramCommand.NotifyCanExecuteChanged();
            MyProgramCommand.NotifyCanExecuteChanged();
        }

        partial void OnSltSubjectTreeItemChanged(ProgramSubjectModel value)
        {
            AddCommand.NotifyCanExecuteChanged();
        }

        private void OnAddCommand()
        {
            ChoosedProSubjectModels.Add(_sltSubjectTreeItem);
            AddCommand.NotifyCanExecuteChanged();
            DownloadCommand.NotifyCanExecuteChanged();
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
            IEnumerable<string> choiceSubjectCodes = ChoosedProSubjectModels.Select(item => item.ProgramSubject.SubjectCode);
            IEnumerable<string> wereDownloadedSubjectCodes = SubjectModels.Select(item => item.SubjectCode);

            if (!choiceSubjectCodes.Any())
            {
                wereDownloadedSubjectCodes = new List<string>();
            }

            IEnumerable<string> needDownloadNames = choiceSubjectCodes.Except(wereDownloadedSubjectCodes);
            IEnumerable<ProgramSubjectModel> needDownload = ChoosedProSubjectModels
                .Where(item => needDownloadNames.Contains(item.ProgramSubject.SubjectCode));

            if (needDownload.Any())
            {
                AutoSortSubjectLoadUC autoSortSubjectLoadUC = new();
                AutoSortSubjectLoadViewModel vm = autoSortSubjectLoadUC.DataContext as AutoSortSubjectLoadViewModel;
                OpenDialog(autoSortSubjectLoadUC);
                IAsyncEnumerable<SubjectModel> subjectModels = vm.Download(needDownload);
                await foreach (SubjectModel subjectModel in subjectModels)
                {
                    SubjectModels.Add(subjectModel);
                }
                CloseDialog();
            }

            _classGroupModelsOfClass = SubjectModels.Select(item => item.ClassGroupModels).ToList();
            IsCalculated = false;
            CalculateCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedCombinationModelChanged(CombinationModel value)
        {
            ShowOnSimuCommand.NotifyCanExecuteChanged();
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

                if (!combinationModel.IsHaveTimeConflicts()
                    && !combinationModel.IsHavePlaceConflicts()
                    && combinationModel.IsCanShow)
                {
                    // Gửi kết quả sang danh sách kết quả.
                    Messenger.Send(new AutoVmMsgs.AddCombinationMsg(combinationModel));
                }
                _genIndex++;
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

        /// <summary>
        /// Thực hiện lọc từ danh sách môn học đã tải.
        /// </summary>
        private void OnFiltering()
        {
            _filteredClassGroupModels.Clear();
            foreach (List<ClassGroupModel> classGroupModels in _classGroupModelsOfClass)
            {
                // IEnumerable<ClassGroupModel> r = classGroupModels.Where(item => Filter(item));
                IEnumerable<ClassGroupModel> r = classGroupModels.Where(item => true);
                _filteredClassGroupModels.Add(r);
            }
            //CombinationModels.Clear();
            IsCalculated = false;
            _genIndex = 0;
            _tempResult.Clear();
        }

        /// <summary>
        /// Lọc các ClassGroupModel thoả mãn.
        /// </summary>
        private bool Filter(ClassGroupModel classGroupModel)
        {
            return IsRemoveClassGroupInValid(classGroupModel)
                && IsPlaceFilter(classGroupModel)
                && IsFreeDayFilter(classGroupModel);
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

        private bool IsPlaceFilter(ClassGroupModel classGroupModel)
        {
            Dictionary<Place, bool> placeFilters = new()
            {
                { Place.QUANGTRUNG, QuangTrung          },
                { Place.NVL_254,    NguyenVanLinh254    },
                { Place.NVL_137,    NguyenVanLinh137    },
                { Place.PHANTHANH,  PhanThanh           },
                { Place.VIETTIN,    VietTin             },
                { Place.HOAKHANH,   HoaKhanhNam         },
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

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _tempResult = e.Result as List<List<int>>;
            IsCalculated = true;
            string message;
            if (_tempResult.Any())
            {
                message = $"Đã tính toán xong với {_tempResult.Count} kết quả";
                ValidGenCommand.NotifyCanExecuteChanged();
            }
            else
            {
                message = "Không có kết quả nào hết";
            }

            _snackbarMessageQueue.Enqueue(message);
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Cs4rsaGen cs4rsaGen = new(_filteredClassGroupModels);
            cs4rsaGen.TempResult.Clear();
            cs4rsaGen.Backtracking(0);
            e.Result = cs4rsaGen.TempResult;
        }
    }
}
