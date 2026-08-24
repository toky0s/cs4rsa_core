using Cs4rsa.Common.Interfaces;
using Cs4rsa.Database;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Database.Models;
using Cs4rsa.Infrastructure.Common;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Views;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Module.ManuallySchedule.Services;
using Cs4rsa.Module.ManuallySchedule.UC;
using Cs4rsa.Module.ManuallySchedule.Utils;
using Cs4rsa.Module.Shared;
using Cs4rsa.Module.Shared.Events;
using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.SubjectCrawler.Crawlers.Interfaces;
using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;
using Cs4rsa.UI.Helper;
using Cs4rsa.UI.ScheduleTable;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Cs4rsa.Module.ManuallySchedule.ViewModels
{
    public class MainSchedulingViewModel : BindableBase
    {
        #region Fields
        private List<Discipline> _searchDisciplines;
        private List<Keyword> _searchKeywords;
        #endregion

        #region Filter class groups
        private readonly Debouncer _debouncer;

        private ObservableCollection<FilterSubjectViewModel> _filterSubjectViewModels = new ObservableCollection<FilterSubjectViewModel>();
        public ObservableCollection<FilterSubjectViewModel> FilterSubjectViewModels
        {
            get { return _filterSubjectViewModels; }
            set { SetProperty(ref _filterSubjectViewModels, value); }
        }

        /// <summary>
        ///Just show filters which it's IsDisplay = True
        /// </summary>
        public ICollectionView VisibleFilters { get; set; }

        #region Time Range
        private int _minRange = 0;
        public int LowValue
        {
            get { return _minRange; }
            set { SetProperty(ref _minRange, value); _debouncer.Debounce(); }
        }

        private int _maxRange = 28;
        public int HighValue
        {
            get { return _maxRange; }
            set { SetProperty(ref _maxRange, value); _debouncer.Debounce(); }
        }
        #endregion

        private bool _seatAvailable;
        public bool SeatAvailable
        {
            get { return _seatAvailable; }
            set { SetProperty(ref _seatAvailable, value); _debouncer.Debounce(); }
        }

        #endregion

        #region Sort class groups
        /// <summary>
        /// Danh sách các class group theo dựa theo Subject hiện tại được chọn.
        /// </summary>
        public ICollectionView CurrentClassGroupModelsView { get; set; }

        public enum ClassGroupSortField { Name, EmptySeat }
        private ClassGroupSortField _currentSortField = ClassGroupSortField.Name;
        private ListSortDirection _currentSortDirection = ListSortDirection.Ascending;

        private DelegateCommand<string> _changeSortDirectionCommand;
        public DelegateCommand<string> ChangeSortDirectionCommand =>
            _changeSortDirectionCommand ?? (_changeSortDirectionCommand = new DelegateCommand<string>(ExecuteChangeSortDirectionCommand, CanExecuteChangeSortDirectionCommand));

        /// <summary>
        /// Change sort direction
        /// </summary>
        /// <param name="direction">ASC/DESC</param>
        void ExecuteChangeSortDirectionCommand(string direction)
        {
            if (direction != "ASC" && direction != "DESC") throw new ArgumentException("Invalid sort direction");
            if (direction == "ASC")
            {
                _currentSortDirection = ListSortDirection.Ascending;
            }
            else
            {
                _currentSortDirection = ListSortDirection.Descending;
            }
            ApplySort();
        }

        bool CanExecuteChangeSortDirectionCommand(string direction)
        {
            return true;
        }

        private DelegateCommand<string> _sortCommand;
        public DelegateCommand<string> SortCommand =>
            _sortCommand ?? (_sortCommand = new DelegateCommand<string>(ExecuteSortCommand));

        private void ExecuteSortCommand(string fieldName)
        {
            // Toggle direction nếu click cùng column
            Enum.TryParse(fieldName, out ClassGroupSortField field);
            _currentSortField = field;
            ApplySort();
        }

        private void ApplySort()
        {
            CurrentClassGroupModelsView.SortDescriptions.Clear();
            CurrentClassGroupModelsView.SortDescriptions.Add(
                new SortDescription(_currentSortField.ToString(), _currentSortDirection)
            );
        }
        private DelegateCommand<string> _changeDirectionCommand;
        public DelegateCommand<string> ChangeDirectionCommand =>
            _changeDirectionCommand ?? (_changeDirectionCommand = new DelegateCommand<string>(ExecuteChangeDirectionCommand));

        void ExecuteChangeDirectionCommand(string parameter)
        {

        }

        #endregion

        #region Commands in Quick import tab in search box

        private string _shareString;
        public string ShareString
        {
            get { return _shareString; }
            set
            {
                SetProperty(ref _shareString, value);
                LoadShareStringCommand.RaiseCanExecuteChanged();
            }
        }
        private DelegateCommand _loadShareStringCommand;
        public DelegateCommand LoadShareStringCommand =>
            _loadShareStringCommand ?? (_loadShareStringCommand = new DelegateCommand(ExecuteLoadShareStringCommand, CanExecuteLoadShareStringCommand));

        void ExecuteLoadShareStringCommand()
        {
            var output = _shareStringService.GetSubjectFromShareString(ShareString);
            var serialize = JsonConvert.SerializeObject(output);
            _logger.LogTrace("Load share string: {output}", serialize);

            _dialogService.ShowDialog(nameof(ScheduleDetailUC), new DialogParameters()
            {
                { "UserSchedule",  null },
                { "UserSubjects", output }
            }, async r => await LoadUserScheduleOnClose(r));
        }

        private async Task LoadUserScheduleOnClose(IDialogResult r)
        {
            if (r.Result == ButtonResult.OK)
            {
                _logger.LogInformation("ScheduleDetailUC closed with OK");

                var parameters = r.Parameters;
                var isMergeAction = parameters.GetValue<string>("Action") == "Merge";
                var userSubjects = parameters.GetValue<ObservableCollection<UserSubject>>("UserSubjects");

                if (userSubjects == null) return;

                // Go to Search tab
                SearchBoxSelectedIndex = 0;

                // NGƯỜI DÙNG CHỌN MERGE OPTION
                if (isMergeAction)
                {
                    // Chỉ tải những môn chưa có sẵn.
                    var currSubjectCodes = SubjectModels.Select(sm => sm.SubjectCode).ToHashSet();
                    var downloadTasks = new List<Task>();
                    userSubjects
                        // Trong lịch đã lưu, lấy ra các môn học có trạng thái OK và chưa tồn tại trong SubjectModels để tải,
                        // còn những môn đã tồn tại thì chỉ cần set selected class group là được, ko cần tải lại.
                        .Where(us => us.Status == "OK" && !currSubjectCodes.Contains(us.SubjectCode))
                        .Select(us =>
                        {
                            var kw = _unitOfWork.Keywords.GetKeywordBySubjectCode(us.SubjectCode);
                            kw.Discipline = _unitOfWork.Disciplines.GetDisciplineByID(kw.DisciplineId);
                            // Add task to download
                            downloadTasks.Add(OnAddSubjectAsync(kw, us));
                            // Add pseudo subject
                            return new SubjectModel(
                                kw.SubjectName,
                                kw.Discipline.Name + " " + kw.Keyword1,
                                kw.Color,
                                kw.CourseId,
                                us
                            );
                        })
                        .ToList()
                        .ForEach(sm => SubjectModels.Insert(0, sm));

                    await Task.WhenAll(downloadTasks);

                    // Nếu subject đã có sẵn, không cần download lại, chỉ cần đổi selected.
                    var dicClassGroups = userSubjects
                        .Where(us => currSubjectCodes.Contains(us.SubjectCode))
                        .ToDictionary(us => us.SubjectCode, us => us.ClassGroup);

                    // Lấy ra ClassGroupModel có tên bằng với tên đã lưu.
                    SubjectModels
                        .Where(sm => dicClassGroups.ContainsKey(sm.SubjectCode))
                        .ToList()
                        .ForEach(sm =>
                        {
                            // Set selected class group name to find the ClassGroupModel which has the same name as the saved one.
                            sm.SelectedClassGroupName = dicClassGroups[sm.SubjectCode];

                            // Lấy ra Class Group Model ứng với class group đã lưu của môn học đó.
                            var tempCgm = sm
                                .ClassGroupModels
                                .First(cgm => dicClassGroups.ContainsKey(sm.SubjectCode) && dicClassGroups[sm.SubjectCode] == cgm.Name);

                            // Nếu là môn đặc biệt, sẽ không có class group, mà sẽ lưu trực tiếp
                            // tên lớp học đã chọn vào UserSubject.SchoolClass, nên sẽ lấy tên
                            // lớp học đó để điền vào ClassGroupModel.PickSchoolClass.
                            if (sm.IsSpecialSubject)
                            {
                                var selectedSchoolClass = userSubjects.First(us => us.ClassGroup == tempCgm.Name).SchoolClass;
                                tempCgm.PickSchoolClass(selectedSchoolClass);
                            }

                            // Tìm class group cũ đã chọn của Subject và remove
                            var oldSelectedClassGroup = SelectedClassGroupModels.FirstOrDefault(cgm => cgm.SubjectCode == sm.SubjectCode);
                            if (oldSelectedClassGroup != null)
                            {
                                SelectedClassGroupModels.Remove(oldSelectedClassGroup);
                            }

                            SelectedClassGroupModels.Add(tempCgm);
                        });
                }
                // NGƯỜI DÙNG CHỌN OVERWRITE
                else
                {
                    SubjectModels.Clear();

                    // Get keywords from subjects then add pseudo subjects to SubjectModels before downloading real subjects,
                    // to make sure the order of subjects is the same as userSubjects order.
                    var keywords = userSubjects.Select(userSubject => _unitOfWork.Keywords.GetKeywordBySubjectCode(userSubject.SubjectCode)).ToList();

                    //InsertPseudoSubjects(keywords, userSubjects);
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

                    // Download real subjects in parallel, after all subjects are downloaded,
                    // set SelectedSubjectModel to the first subject to show details of that subject.
                    var downloadTasks = new List<Task>();
                    for (var i = 0; i < keywords.Count; i++)
                    {
                        downloadTasks.Add(OnAddSubjectAsync(keywords[i], userSubjects[i]));
                    }
                    await Task.WhenAll(downloadTasks);
                }

                // Sau khi tải xong thì chọn subject đầu tiên.
                SelectedSubjectModel = SubjectModels[0];
                SyncSelectedClassGroup();

                ShareString = string.Empty;
                RunScheduleValidator();
                VisibleFilters.Refresh();
            }
            else
            {
                _logger.LogInformation("ScheduleDetailUC closed");
            }
        }

        bool CanExecuteLoadShareStringCommand()
        {
            return !string.IsNullOrEmpty(ShareString);
        }
        #endregion

        #region Commands in Store tab in search box
        private DelegateCommand<UserSchedule> _shareCommand;
        public DelegateCommand<UserSchedule> ShareCommand =>
            _shareCommand ?? (_shareCommand = new DelegateCommand<UserSchedule>(ExecuteShareCommand, CanExecuteShareCommand));

        void ExecuteShareCommand(UserSchedule userSchedule)
        {
            var userSubjects = _unitOfWork.UserSchedules
                    .GetSessionDetails(userSchedule.UserScheduleId)
                    .Select(
                        sd => new UserSubject()
                        {
                            SubjectCode = sd.SubjectCode,
                            SubjectName = sd.SubjectName,
                            ClassGroup = sd.ClassGroup,
                            SchoolClass = sd.SelectedSchoolClass,
                            RegisterCode = sd.RegisterCode
                        }
                    ).ToArray();

            _dialogService.ShowDialog(nameof(ShareStringUC), new DialogParameters()
            {
                {"UserSubjects", userSubjects }
            }, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    _logger.LogInformation("ShareStringUC closed with OK");
                }
                else
                {
                    _logger.LogInformation("ShareStringUC closed");
                }
            });
        }

        bool CanExecuteShareCommand(UserSchedule userSchedule)
        {
            return true;
        }

        private DelegateCommand _addCommand;
        public DelegateCommand AddCommand => _addCommand
            ?? (_addCommand = new DelegateCommand(async () => await ExecuteAddCommand(), () => !IsAlreadyDownloaded(SelectedKeyword)));
        private async Task ExecuteAddCommand()
        {
            var url = $"https://courses.duytan.edu.vn/Sites/Home_ChuongTrinhDaoTao.aspx?p=home_listcoursedetail&courseid={SelectedKeyword.CourseId}&timespan={SelectedKeyword.SemesterId}&t=s";
            _logger.LogInformation("Add command executed - Load subject={url}", url);
            InsertPseudoSubject(SelectedKeyword);
            var subjectModel = await OnAddSubjectAsync(SelectedKeyword);


            SelectedSubjectModel = subjectModel;
            RunScheduleValidator();

            DeleteAllCommand.RaiseCanExecuteChanged();
        }
        #endregion

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
                        ToastService.Instance.Info("Delete schedule", $"Schedule {userSchedule.Name} has been deleted");

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

        private DelegateCommand<SubjectModel> _deleteCommand;
        public DelegateCommand<SubjectModel> DeleteCommand => _deleteCommand ?? (_deleteCommand = new DelegateCommand<SubjectModel>(ExecuteDeleteCommand));

        /// <summary>
        /// Xoá môn học đã tải.
        /// </summary>
        /// <param name="sm">SubjectModel.</param>
        private void ExecuteDeleteCommand(SubjectModel sm)
        {
            var subjectCode = sm.SubjectCode;
            SubjectModels.Remove(sm);
            HideFilterBySubjectCode(subjectCode);
            SelectedSubjectModel = null;
            var isBelongToDeletedSubject = CurrentClassGroupModels.All(cgm => cgm.SubjectCode == subjectCode);

            if (isBelongToDeletedSubject)
            {
                CurrentClassGroupModels.Clear();
            }
            ToastService.Instance.Info("Notification", "Đã xoá môn " + sm.SubjectName);
        }

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

        private DelegateCommand<SubjectModel> _copyErrorCommand;
        public DelegateCommand<SubjectModel> CopyErrorCommand =>
            _copyErrorCommand ?? (_copyErrorCommand = new DelegateCommand<SubjectModel>(ExecuteCopyErrorCommand, CanExecuteCopyErrorCommand));
        private bool CanExecuteCopyErrorCommand(SubjectModel subjectModel)
        {
            return subjectModel != null && subjectModel.IsError;
        }

        private void ExecuteCopyErrorCommand(SubjectModel subjectModel)
        {
            Clipboard.SetText(subjectModel.ErrorMessage);
            _logger.LogInformation("User copy error message of subject model: {name}", subjectModel.SubjectName);
        }

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
                }, async r => await LoadUserScheduleOnClose(r));
            }
        }

        bool CanExecuteLoadUserScheduleCommand(UserSchedule userSchedule)
        {
            return true;
        }

        #endregion

        #region Command for buttons are under selected classes table
        private DelegateCommand<object> _removeSelectedCommand;
        public DelegateCommand<object> RemoveSelectedCommand =>
            _removeSelectedCommand ?? (_removeSelectedCommand = new DelegateCommand<object>(ExecuteRemoveSelectedCommand, CanExecuteRemoveSelectedCommand));

        void ExecuteRemoveSelectedCommand(object parameter)
        {
            if (!(parameter is System.Collections.IList selectedItems)) return;

            for (int i = 0; i < selectedItems.Count; i++)
            {
                if (selectedItems[i] is ClassGroupModel cg)
                {
                    SelectedClassGroupModels.Remove(cg);
                }
            }

            DeleteAllChooseCommand.RaiseCanExecuteChanged();
            _logger.LogInformation("User remove selected class groups successfully.");
        }

        bool CanExecuteRemoveSelectedCommand(object parameter)
        {
            if (!(parameter is System.Collections.IList selectedItems)) return false;

            return SelectedClassGroupModels.Count > 0 && selectedItems.Count > 0;
        }


        /// <summary>
        /// Nút xoá tất cả các lớp đã chọn, không xoá môn đã chọn
        /// </summary>
        private DelegateCommand _deleteAllChooseCommand;
        public DelegateCommand DeleteAllChooseCommand =>
            _deleteAllChooseCommand ?? (_deleteAllChooseCommand = new DelegateCommand(ExecuteDeleteAllChooseCommand, () => SelectedClassGroupModels.Count > 0));

        /// <summary>
        /// Xoá tất cả
        /// </summary>
        private void ExecuteDeleteAllChooseCommand()
        {
            SelectedClassGroupModels.Clear();

            UpdateConflicts();
            CleanDays();
            RunScheduleValidator();

            DeleteAllChooseCommand.RaiseCanExecuteChanged();
        }
        #endregion

        private DelegateCommand _openSearchCommand;
        public DelegateCommand OpenSearchCommand =>
            _openSearchCommand ?? (_openSearchCommand = new DelegateCommand(ExecuteOpenSearchCommand, CanExecuteOpenSearchCommand));

        void ExecuteOpenSearchCommand()
        {
            // Gửi cho Dialog thông tin các Subject đã được tải rồi,
            // dialog sẽ có thể dễ dàng disable chúng trong tập kết quả.
            var param = new DialogParameters
            {
                { "DownloadSubjectCodes", SubjectModels.Select(subject => subject.SubjectCode).ToHashSet() }
            };
            _dialogService.ShowDialog(nameof(SearchSubjectUC), param, async r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    var searchResult = r.Parameters.GetValue<SearchResult>("SelectedSearchResult");
                    var selectedDiscipline = Disciplines.FirstOrDefault(item => item.Name == searchResult.Discipline)
                        ?? throw new ArgumentException($"Cannot find discipline with name {searchResult.Discipline}");
                    var selectedKeyword = selectedDiscipline.Keywords.FirstOrDefault(item => item.Keyword1 == searchResult.Keyword)
                        ?? throw new ArgumentException($"Cannot find keyword with name {searchResult.Keyword} in discipline {searchResult.Discipline}");
                    InsertPseudoSubject(selectedKeyword);
                    await OnAddSubjectAsync(selectedKeyword);
                }
            });
        }

        bool CanExecuteOpenSearchCommand()
        {
            return true;
        }


        private DelegateCommand<SubjectModel> _reloadCommand;
        public DelegateCommand<SubjectModel> ReloadCommand => _reloadCommand ?? (_reloadCommand = new DelegateCommand<SubjectModel>(ExecuteReloadCommand, CanExecuteReloadCommand));

        /// <summary>
        /// Chỉ có thể thực hiện reload nếu Subject Model bị lỗi.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool CanExecuteReloadCommand(SubjectModel model)
        {
            return true;
        }

        /// <summary>
        /// Tải lại môn học bị lỗi.
        /// </summary>
        /// <param name="subjectModel">SubjectModel</param>
        private async void ExecuteReloadCommand(SubjectModel subjectModel)
        {
            subjectModel.IsDownloading = true;
            subjectModel.IsError = false;
            subjectModel.ErrorMessage = string.Empty;

            var kw = _unitOfWork.Keywords.GetKeywordBySubjectCode(subjectModel.SubjectCode);
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
            CreateOrShowFilterForSubject(subjectModel);
            VisibleFilters.Refresh();
        }

        /// <summary>
        /// Nút xoá tất cả các môn đã chọn bao gồm cả các lớp đã chọn
        /// </summary>

        private DelegateCommand _deleteAllCommand;
        public DelegateCommand DeleteAllCommand => _deleteAllCommand ?? (_deleteAllCommand = new DelegateCommand(ExecuteDeleteAllCommand, CanExecuteDeleteAllCommand));

        private bool CanExecuteDeleteAllCommand()
        {
            return SubjectModels.Any();
        }

        private void ExecuteDeleteAllCommand()
        {
            _logger.LogInformation("User click on Delete All button");
            SubjectModels.Clear();

            ToastService.Instance.Info(
                "Notification",
                "Đã xoá tất cả môn học"
            );
        }

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

        private DelegateCommand<string> _unSelectClassGroupCommand;
        public DelegateCommand<string> UnSelectClassGroupCommand =>
            _unSelectClassGroupCommand ?? (_unSelectClassGroupCommand = new DelegateCommand<string>(ExecuteUnSelectClassGroupCommand, CanExecuteUnSelectClassGroupCommand));

        void ExecuteUnSelectClassGroupCommand(string classGroupName)
        {
            _logger.LogInformation("User unselect a class group with name {ClassGroupName}", classGroupName);
            var removedClassGroupModel = SelectedClassGroupModels.FirstOrDefault(cg => cg.Name == classGroupName);
            if (removedClassGroupModel != null)
            {
                removedClassGroupModel.SpecialSchoolClassModels.Clear();
                SelectedClassGroupModels.Remove(removedClassGroupModel);
                RunScheduleValidator();
            }
        }

        bool CanExecuteUnSelectClassGroupCommand(string classGroupName)
        {
            return true;
        }

        #region Properties
        public ObservableCollection<Keyword> DisciplineKeywordModels { get; set; }
        public ObservableCollection<SubjectModel> SubjectModels { get; set; }
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public ObservableCollection<FullMatchSearchingKeyword> FullMatchSearchingKeywords { get; set; }
        public ObservableCollection<UserSchedule> SavedSchedules { get; set; }

        private ObservableCollection<WarningModel> _warningModels;
        public ObservableCollection<WarningModel> WarningModels => _warningModels ?? (_warningModels = new ObservableCollection<WarningModel>());

        /// <summary>
        /// Khi người dùng chọn một Subject từ những Subject đã tải xuống. 
        /// Danh sách các Class Group có trong Subject đó sẽ được hiển thị 
        /// trong một list box ngay bên dưới. List đó sẽ được binding tới biến này.
        /// </summary>
        private ObservableCollection<ClassGroupModel> _currentClassGroupModels;
        public ObservableCollection<ClassGroupModel> CurrentClassGroupModels => _currentClassGroupModels ?? (_currentClassGroupModels = new ObservableCollection<ClassGroupModel>());

        /// <summary>
        /// Combination Models which was saved in the Store.
        /// </summary>
        public ObservableCollection<CombinationModel> ComModels { get; set; }
        public ObservableCollection<UserSchedule> UserSchedules { get; set; }

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

                // Nếu một Subject khác NULL được chọn, thì phải đảm bảo Subject đó đã tải xong và không bị lỗi.
                if (value != null && !value.IsDownloading && !value.IsError)
                {
                    SetProperty(ref _selectedSubjectModel, value);

                    // Tìm và tạo filter dựa trên Subject đã chọn
                    CreateOrShowFilterForSubject(value);
                    VisibleFilters.Refresh();

                    // Thêm danh sách class group theo subject đã chọn
                    CurrentClassGroupModels.Clear();
                    CurrentClassGroupModels.AddRange(value.ClassGroupModels);
                    CurrentClassGroupModelsView.Refresh();

                    SyncSelectedClassGroup();

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ExpandSubjectFilter(value.SubjectCode, "Set SelectedSubjectModel");
                    }), DispatcherPriority.Loaded);
                }
            }
        }

        /// <summary>
        /// Mở filter của một subject dựa vào Subject Code và đóng filter của subject khác. Nếu không tìm thấy filter của subject thì không làm gì cả.
        /// </summary>
        /// <param name="subjectCode"></param>
        private void ExpandSubjectFilter(string subjectCode, string fromMethod)
        {
            _logger.LogDebug("Call ExpandSubjectFilter - From method {fromMethod}", fromMethod);
            FilterSubjectViewModels.Where(f => f.SubjectCode != subjectCode).ToList().ForEach(f => f.IsExpanded = false);
            var openFilter = FilterSubjectViewModels.FirstOrDefault(f => f.SubjectCode.Equals(subjectCode));
            if (openFilter != null)
            {
                openFilter.IsExpanded = true;
            }
        }

        /// <summary>
        /// Trong trường hợp Bulk Load từ Share String, hoặc Load từ Store,
        /// Nếu trước đó trên Subject đã chọn, có class group đã chọn sẵn. Thì chọn Class Group đó và hiển thị trên lịch.
        /// </summary>
        private void SyncSelectedClassGroup()
        {
            if (SelectedSubjectModel == null) return;

            SelectedClassGroup = CurrentClassGroupModels
                .FirstOrDefault(x => x.Name == SelectedSubjectModel.SelectedClassGroupName);
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

        private readonly List<ObservableCollection<TimeBlock>> _schedules;

        public ObservableCollection<TimeBlock> Week1 { get; set; }
        public ObservableCollection<TimeBlock> Week2 { get; set; }

        public ObservableCollection<string> Timelines { get; set; }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set { SetProperty(ref _isConnected, value); }
        }
        #endregion

        #region Services
        private readonly ISubjectCrawler _subjectCrawler;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOpenInBrowser _openInBrowser;
        private readonly ILogger<MainSchedulingViewModel> _logger;
        private readonly IScheduleValidator _scheduleValidator;
        private readonly IDialogService _dialogService;
        private readonly ITimeBlockGenerator _timeBlockGenerator;
        private readonly IShareStringService _shareStringService;
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
            ILogger<MainSchedulingViewModel> logger,
            IDialogService dialogService,
            IScheduleValidator scheduleValidator,
            ITimeBlockGenerator timeBlockGenerator,
            NetworkMonitor networkMonitor,
            IShareStringService shareStringService
        )
        {
            #region Services
            _subjectCrawler = subjectCrawler;
            _unitOfWork = unitOfWork;
            _openInBrowser = openInBrowser;
            _dialogService = dialogService;
            _logger = logger;
            _scheduleValidator = scheduleValidator;
            _timeBlockGenerator = timeBlockGenerator;
            _shareStringService = shareStringService;
            #endregion

            #region Subscribe Events
            eventAggregator.GetEvent<Event_MainWindow_HotKey_Ctrl_E>().Subscribe(() => OpenSearchCommand.Execute());
            #endregion

            #region Pros
            DisciplineKeywordModels = new ObservableCollection<Keyword>();
            SubjectModels = new ObservableCollection<SubjectModel>();
            SubjectModels.CollectionChanged += SubjectModels_CollectionChanged;

            Disciplines = new ObservableCollection<Discipline>();
            FullMatchSearchingKeywords = new ObservableCollection<FullMatchSearchingKeyword>();
            SavedSchedules = new ObservableCollection<UserSchedule>();
            ComModels = new ObservableCollection<CombinationModel>();
            UserSchedules = new ObservableCollection<UserSchedule>();
            
            WarningModels.CollectionChanged += ConflictInfos_CollectionChanged;
            #endregion

            #region Commands

            GotoCourseCommand = new DelegateCommand<SubjectModel>(ExecuteGotoCourseCommand);
            #endregion

            TeacherCount = 0;
            AnyTeacherName = true;
            TeacherNames = new ObservableCollection<string>();
            GotoCourseClassCommand = new DelegateCommand(OnGotoCourse);
            ShowDetailsSchoolClassesCommand = new DelegateCommand(OnShowDetailsSchoolClasses);
            FilterCommand = new DelegateCommand(OnFilter);
            ResetFilterCommand = new DelegateCommand(OnResetFilter, CanResetFilter);

            PlaceConflictFinderModels = new ObservableCollection<PlaceConflict>();
            ConflictCollection = new ObservableCollection<Conflict>();
            ConflictCollection.CollectionChanged += Conflicts_CollectionChanged;

            SelectedClassGroupModels = new ObservableCollection<ClassGroupModel>();
            SelectedClassGroupModels.CollectionChanged += SelectedClassGroupModels_CollectionChanged;
            CurrentClassGroupModelsView = CollectionViewSource.GetDefaultView(CurrentClassGroupModels);
            CurrentClassGroupModelsView.Filter += SelectedClassGroupModelsView_DoFilter;

            #region Filter class group Config
            _debouncer = new Debouncer(500, () => CurrentClassGroupModelsView.Refresh());
            #endregion

            VisibleFilters = CollectionViewSource.GetDefaultView(FilterSubjectViewModels);
            VisibleFilters.Filter = VisibleFilter_DoFilter;

            #region Weeks and Timelines
            Week1 = new ObservableCollection<TimeBlock>();

            Week2 = new ObservableCollection<TimeBlock>();

            _schedules = new List<ObservableCollection<TimeBlock>>() { Week1, Week2 };

            Timelines = new ObservableCollection<string>();
            foreach (var timeline in UI.ScheduleTable.Utils.TimeLines)
            {
                Timelines.Add(timeline);
            }
            #endregion


            InitFilter();
            LoadDiscipline();
            networkMonitor.ConnectivityChanged += NetworkMonitor_ConnectivityChanged;
        }

        private bool SelectedClassGroupModelsView_DoFilter(object obj)
        {
            var subjectCode = SelectedSubjectModel.SubjectCode;

            // Lấy filter info dựa trên subject code hiện tại đang chọn.
            var filter = FilterSubjectViewModels.FirstOrDefault(f => f.SubjectCode.Equals(subjectCode));

            if (filter != null)
            {
                var classGroupModel = obj as ClassGroupModel;
                var filterInfo = filter.AskRequestFilter();

                // Do filter
                var isMatchDayOfWeek = classGroupModel.Schedule.GetSchoolDays().Intersect(filterInfo.SelectedDayOfWeeks).Any();
                var isMatchLectures = classGroupModel.TeacherNames.Intersect(filterInfo.LectureNames).Any();

#pragma warning disable IDE0075
                // Nếu checkbox seat được check, thì phải kiểm tra xem class group có empty seat hay không, nếu không check thì bỏ qua.
                var hasEmptySeat = SeatAvailable ? classGroupModel.EmptySeat > 0 : true;
#pragma warning restore IDE0075

                var lowValue = ConvertUnitToDateTime(LowValue);
                var highValue = ConvertUnitToDateTime(HighValue);
                var isMatchTime = classGroupModel.Schedule.ScheduleTime
                    .SelectMany(item => item.Value)
                    .Any(item => item.End >= lowValue && item.Start <= highValue);
                return isMatchDayOfWeek
                    && isMatchLectures
                    && hasEmptySeat
                    && isMatchTime;
            }

            return false;
        }

        private DateTime ConvertUnitToDateTime(int unit)
        {
            // Mỗi đơn vị = 30 phút
            int totalMinutes = unit * 30;

            // Bắt đầu từ 7:00
            int startHour = 7;
            int hour = startHour + (totalMinutes / 60);
            int minute = totalMinutes % 60;
            var now = DateTime.Now;
            DateTime dateTime = new DateTime(
                now.Year,
                now.Month,
                now.Day,
                hour, minute, 0
            );
            return dateTime;
        }

        /// <summary>
        /// Filter này quyết định subject filter nào sẽ hiển thị
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool VisibleFilter_DoFilter(object obj)
        {
            if (obj is FilterSubjectViewModel filter)
            {
                return filter.IsDisplayed;
            }
            return false;
        }

        private void NetworkMonitor_ConnectivityChanged(bool obj)
        {
            IsConnected = obj;
        }

        private void Conflicts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var timeConflictsToRemove = Week1
                .Concat(Week2)
                .Where(block => block.ScheduleTableItemType == TimeBlockType.TimeConflict)
                .ToList();
            foreach (var item in timeConflictsToRemove)
            {
                Week1.Remove(item);
                Week2.Remove(item);
            }
            var timeBlocks = ConflictCollection.SelectMany(conflict => _timeBlockGenerator.Generate(conflict));
            AddTimeBlockToSchedule(timeBlocks);
        }

        private void ConflictInfos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _logger.LogInformation("Conflict collection changed");
        }

        private void HideFilterBySubjectCode(string subjectCode)
        {
            // Don't remove filters, just hide.
            var filter = FilterSubjectViewModels.FirstOrDefault(f => f.SubjectCode.Equals(subjectCode));
            if (filter != null)
            {
                // Hide filter from UI
                filter.IsDisplayed = false;
            }
        }

        private void SubjectModels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // ADD
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var addedSubject = e.NewItems.Cast<SubjectModel>().First();
                _logger.LogInformation("Added subject with name {subjectName}", addedSubject.SubjectName);
            }
            // REMOVE
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                e.OldItems.Cast<SubjectModel>().ToList().ForEach(sm =>
                {
                    // Remove class group model which is belong to subject model that is removed
                    var classGroup = SelectedClassGroupModels.FirstOrDefault(classGroupModel => classGroupModel.SubjectCode.Equals(sm.SubjectCode));
                    if (classGroup != null)
                    {
                        _logger.LogInformation("Remove the selected class group before remove the subject");
                        SelectedClassGroupModels.Remove(classGroup);
                    }
                    _logger.LogInformation("Removed subject with name {subjectName}", sm.SubjectName);

                    HideFilterBySubjectCode(sm.SubjectCode);
                });
            }
            // REPLACE
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                // Hide old
                e.OldItems.Cast<SubjectModel>().ToList().ForEach(sm =>
                {
                    HideFilterBySubjectCode(sm.SubjectCode);
                });
            }
            // RESET (Clear)
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                // 1. Đưa teacher count về 0
                TeacherCount = 0;

                // 2. Xoá hết class group model đã chọn
                SelectedClassGroupModels.Clear();
                UpdateConflicts();

                // 3. Xoá hết danh sách ClassGroup theo Subject đã chọn
                CurrentClassGroupModels.Clear();

                // 5. Ẩn các filter hiện có trên màn hình, tuy vậy trong bộ nhớ vẫn còn lưu trữ.
                foreach (var filter in FilterSubjectViewModels)
                {
                    filter.IsDisplayed = false;
                }

                CurrentClassGroupModelsView.Refresh();
                VisibleFilters.Refresh();
            }

            DeleteAllCommand.RaiseCanExecuteChanged();
            AddCommand.RaiseCanExecuteChanged();
            DeleteAllChooseCommand.RaiseCanExecuteChanged();
        }

        private void CreateOrShowFilterForSubject(SubjectModel subjectModel)
        {
            var filter = FilterSubjectViewModels.FirstOrDefault(f => f.SubjectCode == subjectModel.SubjectCode);
            if (filter == null)
            {
                // Create new filter
                var filterVm = new FilterSubjectViewModel
                {
                    SubjectCode = subjectModel.SubjectCode,
                    SubjectName = subjectModel.SubjectName,
                    Color = subjectModel.Color,
                    IsDisplayed = true,
                    Lectures = new ObservableCollection<MultiSelectionItem>()
                };

                var teacherNames = subjectModel.Subject.ClassGroups.SelectMany(
                    classGroup => classGroup.SchoolClasses.SelectMany(
                        schoolClass => schoolClass.TeacherNames))
                    .Distinct() // Lấy danh sách tên giảng viên duy nhất
                    .OrderBy(value => value) // Sắp xếp tên giảng viên tăng dần
                    .Select(teacherName => new MultiSelectionItem { Label = teacherName, ID = teacherName })
                    .ToList();

                filterVm.Lectures.AddRange(teacherNames);
                filterVm.SelectedLectures.AddRange(teacherNames);
                filterVm.Filter += _debouncer.Debounce;
                FilterSubjectViewModels.Add(filterVm);
            }
            else
            {
                // Show existing filter
                filter.IsDisplayed = true;
            }
        }

        private void SelectedClassGroupModels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Để tránh việc lặp lại code khi thêm hoặc thay thế nhiều class group model,
            // tạo một hàm riêng để điền tên lớp học đã chọn vào SubjectModel tương ứng.
            void FillClassGroupNameToSubjectModel(ClassGroupModel[] newItems)
            {
                // Fill selected class group name to subjects.
                foreach (var classGroupModel in newItems)
                {
                    var subjectModel = SubjectModels.First(sjm => sjm.SubjectName == classGroupModel.ClassGroup.SubjectName);
                    subjectModel.SelectedClassGroupName = classGroupModel.Name;
                }
            }

            // Loại bỏ selected class group name được chọn tương ứng trên mỗi subject.
            void CleanSelectedClassGroupNameFromSubjectModel(ClassGroupModel[] oldItems)
            {
                foreach (var classGroupModel in oldItems)
                {
                    var subjectModel = SubjectModels.FirstOrDefault(sjm => sjm.SubjectName == classGroupModel.ClassGroup.SubjectName);
                    if (subjectModel != null)
                    {
                        subjectModel.SelectedClassGroupName = null;
                    }
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var classGroups = e.NewItems.Cast<ClassGroupModel>().ToArray();
                for (int i = 0; i < classGroups.Length; i++)
                {
                    AddScheduleItems(classGroups[i]);
                }
                FillClassGroupNameToSubjectModel(classGroups);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var classGroups = e.OldItems.Cast<ClassGroupModel>().ToArray();
                CleanSelectedClassGroupNameFromSubjectModel(classGroups);
                var id = TimeBlockGroupID.GenerateId(classGroups[0].SubjectCode);
                RemoveScheduleItem(id);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                var oldClassGroups = e.OldItems.Cast<ClassGroupModel>().ToArray();
                var newClassGroups = e.NewItems.Cast<ClassGroupModel>().ToArray();
                CleanSelectedClassGroupNameFromSubjectModel(oldClassGroups);
                for (int i = 0; i < oldClassGroups.Length; i++)
                {
                    var id = TimeBlockGroupID.GenerateId(oldClassGroups[i].SubjectCode);
                    RemoveScheduleItem(TimeBlockGroupID.GenerateId(oldClassGroups[i].SubjectCode));
                }
                for (int i = 0; i < newClassGroups.Length; i++)
                {
                    AddScheduleItems(newClassGroups[i]);
                }
                FillClassGroupNameToSubjectModel(newClassGroups);
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in SubjectModels)
                {
                    item.SelectedClassGroupName = null;
                    item.ClassGroupModels.ForEach(c => c.ClearSelectedSchoolClass());
                }
                CleanDays();
                SelectedClassGroup = null;
            }

            UpdateConflicts();

            SaveCommand.RaiseCanExecuteChanged();
            DeleteAllChooseCommand.RaiseCanExecuteChanged();
            RemoveSelectedCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Chạy lại validator để cảnh báo cho user.
        /// 
        /// Chạy method này sau mỗi User action.
        /// </summary>
        private void RunScheduleValidator()
        {
            WarningModels.Clear();
            var schoolClasses = SelectedClassGroupModels
                .SelectMany(classGroupModel => classGroupModel.CurrentSchoolClassModels)
                .ToList();
            var warningModels = _scheduleValidator.ValidateSchedule(schoolClasses);
            WarningModels.AddRange(warningModels);
        }

        private void ExecuteGotoCourseCommand(SubjectModel model)
        {
            string url = model.Subject.GetLink();
            _openInBrowser.Open(url);
        }

        private void OnSearchingKeywordChanged(FullMatchSearchingKeyword value)
        {
            if (value == null || value.Keyword == null || value.Discipline.DisciplineId == 0) return;
            var dcl = Disciplines.First(d => d.DisciplineId == value.Discipline.DisciplineId);
            SelectedDiscipline = dcl;
            SelectedKeyword = value.Keyword;
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
            _logger.LogInformation("User add subject {subjectName}", keyword.SubjectName);
            try
            {
                //throw new Exception("Dummy Exception");

                // 1. Thực hiện tải Subject. 
                const bool UseCache = true;
                var (subjectModel, cache) = await DownloadSubject(keyword, UseCache);

                // 2.3. Cập nhật lên local
                var index = DisciplineKeywordModels.IndexOf(keyword);
                if (index >= 0)
                {
                    var colKeyword = DisciplineKeywordModels[index];
                    colKeyword.Cache = cache;
                    DisciplineKeywordModels.RemoveAt(index);
                    DisciplineKeywordModels.Insert(index, colKeyword);
                    SelectedKeyword = colKeyword;
                }

                // 2. Thông báo nếu Subject không tồn tại, ngược lại thay thế Pseudo Subject bằng Subject đã tải được. 
                if (subjectModel == null)
                {
                    _logger.LogError("Không tìm thấy môn {SubjectName} trong học kỳ này", keyword.SubjectName);
                    return null;
                }

                var pseudoSubject = SubjectModels.First(sm => sm.CourseId.Equals(subjectModel.CourseId));
                pseudoSubject.AssignData(subjectModel);
                pseudoSubject.IsDownloading = false;

                // 3. Nếu không có Subject nào đang được tải, thực hiện select Subject đầu tiên trong danh sách. 
                //if (!SubjectModels.Any(sm => sm.IsDownloading))
                //{
                //    SelectedSubjectModel = subjectModel;
                //}

                // 4. Trả về Subject Model đã tải được. 
                return subjectModel;
            }
            catch (Exception e)
            {
                _logger.LogError("There is an error when downloading subject {SubjectName}, course id {CourseId}, semester id {SemesterID}. Error message: {ErrorMessage}",
                    keyword.SubjectName,
                    keyword.CourseId,
                    keyword.SemesterId,
                    e.Message
                );

                // 5. Bất kỳ lỗi nào xuất hiện trong quá trình này, thêm message lỗi vào pseudo subject và trả về null.
                for (var i = 0; i < SubjectModels.Count; i++)
                {
                    if (SubjectModels[i].CourseId.Equals(keyword.CourseId))
                    {
                        var subjectMd = SubjectModels[i];
                        subjectMd.IsError = true;
                        subjectMd.IsDownloading = false;
                        subjectMd.ErrorMessage = e.Message;
                    }
                }

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
            SelectedClassGroupModels.Add(classGroupModel);
            DeleteAllCommand.RaiseCanExecuteChanged();

            // Add filter tương ứng.
            CreateOrShowFilterForSubject(subjectModel);
        }

        private async Task<(SubjectModel, string)> DownloadSubject(Keyword keyword, bool isUseCache)
        {
            return await Task.Run(async () =>
            {
                Subject subject;
                string cache;
                // 1. Sử dụng cache và có sẵn cache trong DB.
                if (isUseCache && !string.IsNullOrWhiteSpace(keyword.Cache))
                {
                    cache = keyword.Cache;
                    subject = _subjectCrawler.CrawlFromCache(keyword.Cache, keyword.CourseId, keyword.SemesterId);
                }
                // 2. Không sử dụng cache
                else
                {
                    var semesterId = _unitOfWork.Settings.GetByKey(DbConsts.StCurrentSemesterValue);
                    (subject, cache) = await _subjectCrawler.Crawl(keyword.CourseId, semesterId);
                    // 2.2. Cập nhật lại cache
                    _unitOfWork.Keywords.UpdateCacheByKeywordId(keyword.KeywordId, semesterId, cache);
                }
                if (subject is null)
                {
                    throw new ArgumentException("Subject is null");
                }
                else
                {
                    return (new SubjectModel(subject, keyword.Color), cache);
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
                    return;
                }

                var keyword = _unitOfWork.Keywords.GetByCourseId(courseId);
                if (keyword == null)
                {
                    return;
                }

                InsertPseudoSubject(keyword);
                await OnAddSubjectAsync(keyword);
            }
            else
            {
                //_snackbarMessageQueue.Enqueue("Sai đường dẫn");
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

        #region Class group view model
        #region Properties
        public ObservableCollection<string> TeacherNames { get; set; }

        private ClassGroupModel _selectedClassGroup;
        public ClassGroupModel SelectedClassGroup
        {
            get { return _selectedClassGroup; }
            set
            {
                if (!SelectedClassGroupModels.Contains(value))
                {
                    SetProperty(ref _selectedClassGroup, value);
                    Application.Current.Dispatcher.BeginInvoke(
                        System.Windows.Threading.DispatcherPriority.Background,
                        new Action(() => OnSelectedClassGroupChanged(value))
                    );
                }
                else
                {
                    // Chỉ highlight, không trigger OnSelectedClassGroupChanged
                    SetProperty(ref _selectedClassGroup, value);
                }
            }
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
                    if (value.SpecialSchoolClassModels.Count == 0)
                    {
                        _logger.LogInformation("User select class group {classGroupName} which belong to special subject, open details school class window", value.Name);
                        OnShowDetailsSchoolClasses();
                    }
                    else
                    {
                        _logger.LogInformation("User has selected school class for this class group already");
                    }
                }
                else
                {
                    AddOrReplaceClassGroupModel(value);
                    _logger.LogInformation("Add block to schedule with class group {classGroupName}", value.Name);
                }
                RunScheduleValidator();
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
            ResetFilterCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Hiển thị chi tiết của một SchoolClass
        /// </summary>
        public void OnShowDetailsSchoolClasses()
        {
            var schoolClassModels = SelectedClassGroup.NormalSchoolClassModels
                .Where(item => item.Type != SelectedClassGroup.CompulsoryClass.Type)
                .ToImmutableArray();

            _dialogService.ShowDialog(nameof(ShowDetailsSchoolClassesUC), new DialogParameters()
            {
                {"SelectedClassGroup", SelectedClassGroup},
                {"SchoolClassModels", schoolClassModels},
            }, OnSelectSpecialClassPopupClosed);
        }

        private void OnSelectSpecialClassPopupClosed(IDialogResult result)
        {
            if (result.Result == ButtonResult.None) return;

            var classGroupModel = result.Parameters.GetValue<ClassGroupModel>("ClassGroupModel");
            var selectedSchoolClassModel = result.Parameters.GetValue<SchoolClassModel>("SelectedSchoolClassModel");
            var schoolClassName = selectedSchoolClassModel.SchoolClassName;
            classGroupModel.ReRenderSchedule(schoolClassName);
            AddOrReplaceClassGroupModel(classGroupModel);
        }

        private void OnGotoCourse()
        {
            var url = SelectedClassGroup.ClassGroup.GetUrl();
            _openInBrowser.Open(url);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Danh sách các ClassGroupModel đã chọn để hiển thị ở phần Lịch đã chọn.
        /// </summary>
        public ObservableCollection<ClassGroupModel> SelectedClassGroupModels { get; set; }

        public ObservableCollection<Conflict> ConflictCollection { get; set; }

        private Conflict _selectedConflict;
        public Conflict SelectedConflict
        {
            get { return _selectedConflict; }
            set { SetProperty(ref _selectedConflict, value); }
        }

        public ObservableCollection<PlaceConflict> PlaceConflictFinderModels { get; set; }
        #endregion

        #region Commands

        private DelegateCommand<WarningModel> _solveConflictCommand;
        public DelegateCommand<WarningModel> SolveConflictCommand =>
            _solveConflictCommand ?? (_solveConflictCommand = new DelegateCommand<WarningModel>(ExecuteSolveConflictCommand, CanExecuteSolveConflictCommand));

        void ExecuteSolveConflictCommand(WarningModel warningModel)
        {
            _logger.LogDebug("User click on solve conflict button with warning {warningTitle} and type {warningType}", warningModel.WarningTitle, warningModel.WarningType);

            switch (warningModel.WarningType)
            {
                case WarningType.TimeConflict:

                    break;
                case WarningType.PlaceConflict:
                    _logger.LogDebug("This is a place conflict warning");
                    break;
                default:
                    _logger.LogWarning("This is an unknown type of warning");
                    break;
            }

            if (warningModel.WarningType.Equals(WarningType.TimeConflict))
            {
                if (warningModel.TryGetContext(out TimeConflictContext context))
                {
                    string classGroupName_A = null;
                    string classGroupName_B = null;

                    _logger.LogDebug("This is a time conflict warning");
                    var ctx = (TimeConflictContext)warningModel.Context;
                    classGroupName_A = ctx.ClassGroupModel_A.ClassGroup.Name;
                    classGroupName_B = ctx.ClassGroupModel_B.ClassGroup.Name;
                    _logger.LogDebug("Conflict between class group {classGroupA} and class group {classGroupB}", classGroupName_A, classGroupName_B);

                    Conflict conflict = ConflictCollection
                        .Where(c =>
                            c.LessonA.ClassGroupName.Equals(classGroupName_A)
                            && c.LessonB.ClassGroupName.Equals(classGroupName_B))
                        .FirstOrDefault();

                    if (conflict == null)
                    {
                        _logger.LogError("Cannot find conflict model with class group {classGroupA} and class group {classGroupB}", classGroupName_A, classGroupName_B);
                        throw new Exception($"Cannot find conflict model with class group {classGroupName_A} and class group {classGroupName_B}");
                    }

                    ClassGroupModel classGroupModel_A = context.ClassGroupModel_A;
                    ClassGroupModel classGroupModel_B = context.ClassGroupModel_B;
                    var parameter = new DialogParameters()
                    {
                        {"ConflictModel", conflict},
                        {"ClassGroupModelA", classGroupModel_A},
                        {"ClassGroupModelB", classGroupModel_B},
                    };
                    _dialogService.ShowDialog(nameof(SolveConflictUC), parameter, callback =>
                    {
                        if (callback.Result == ButtonResult.Cancel)
                        {
                            _logger.LogInformation("User cancelled solve conflict dialog");
                        }
                        else if (callback.Result == ButtonResult.OK)
                        {
                            bool valid = callback.Parameters.TryGetValue("RemovedClassGroupModel", out ClassGroupModel removedClassGroupModel);
                            if (valid)
                            {
                                _logger.LogInformation("User solved conflict by removing {0}", removedClassGroupModel.Name);
                                SelectedClassGroupModels.Remove(removedClassGroupModel);
                                RunScheduleValidator();
                            }
                        }
                    });
                }
            }
        }

        bool CanExecuteSolveConflictCommand(WarningModel warningModel)
        {
            return true;
        }
        #endregion

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
        private void UpdateConflictCollection(IList<SchoolClassModel> schoolClassModels)
        {
            ConflictCollection.Clear();
            var conflicts = new List<Conflict>();
            for (var i = 0; i < schoolClassModels.Count; ++i)
            {
                var schoolClassModel_i = schoolClassModels[i];
                for (var k = i + 1; k < schoolClassModels.Count; ++k)
                {
                    var schoolClassModel_k = schoolClassModels[k];

                    if (schoolClassModels[i].SchoolClass.ClassGroupName.Equals(schoolClassModel_k.SchoolClass.ClassGroupName))
                    {
                        continue;
                    }

                    var lessonA = schoolClassModel_i.SchoolClass.ConvertToLesson();
                    var lessonB = schoolClassModel_k.SchoolClass.ConvertToLesson();

                    var conflict = new Conflict(lessonA, lessonB);
                    var conflictTime = conflict.ConflictTime;
                    if (conflictTime != null)
                    {
                        conflicts.Add(conflict);
                    }
                }
            }
            ConflictCollection.AddRange(conflicts);
        }

        /// <summary>
        /// Thực hiện bắt cặp tất cả các ClassGroupModel có 
        /// trong Collection để phát hiện các Conflict Place.
        /// </summary>
        private void UpdatePlaceConflictCollection(IList<SchoolClassModel> schoolClassModels)
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
                        schoolClassModels[i].SchoolClass.Metadata,
                        schoolClassModels[i].Phase,
                        schoolClassModels[i].SchoolClassName,
                        schoolClassModels[i].SchoolClass.ClassGroupName,
                        schoolClassModels[i].SubjectCode,
                        schoolClassModels[i].SchoolClass.SubjectName
                    );

                    var lessonB = new Lesson(
                        schoolClassModels[k].StudyWeek,
                        schoolClassModels[k].Schedule,
                        schoolClassModels[k].DayPlaceMetaData,
                        schoolClassModels[k].SchoolClass.Metadata,
                        schoolClassModels[k].Phase,
                        schoolClassModels[k].SchoolClassName,
                        schoolClassModels[k].SchoolClass.ClassGroupName,
                        schoolClassModels[k].SubjectCode,
                        schoolClassModels[i].SchoolClass.SubjectName
                    );

                    var placeConflict = new PlaceConflict(lessonA, lessonB);
                    var conflictPlace = placeConflict.ConflictPlace;
                    if (conflictPlace != null)
                    {
                        PlaceConflictFinderModels.Add(placeConflict);
                    }
                }
            }
        }

        private void AddOrReplaceClassGroupModel(ClassGroupModel classGroupModel)
        {
            if (classGroupModel != null)
            {
                var classGroupModelIndex = IsReallyHaveAnotherVersionInChoicedList(classGroupModel);
                if (classGroupModelIndex != -1)
                    SelectedClassGroupModels[classGroupModelIndex] = classGroupModel;
                else
                    SelectedClassGroupModels.Add(classGroupModel);
            }
        }

        private void UpdateConflicts()
        {
            var schoolClasses = SelectedClassGroupModels
                .SelectMany(cgm => cgm.CurrentSchoolClassModels)
                .ToArray();
            UpdateConflictCollection(schoolClasses);
            UpdatePlaceConflictCollection(schoolClasses);
        }

        /// <summary>
        /// Loại bỏ ScheduleItem khỏi mô phỏng.
        /// 
        /// </summary>
        /// <param name="id">
        /// ID với SchoolClassModel sẽ là Subject Code của nó.
        /// ID của các Conflict sẽ là sự kết hợp giữa hai tên SchoolClassModel.
        /// </param>
        private void RemoveScheduleItem(TimeBlockGroupID id)
        {
            foreach (var week in _schedules)
            {
                var toRemove = week.Where(block => block.Id.Equals(id)).ToList();
                foreach (TimeBlock block in toRemove)
                {
                    week.Remove(block);
                }
            }
        }

        /// <summary>
        /// Thay thế ClassGroupModel cũ trong bộ mô phỏng (nếu có)
        /// bằng ClassGroupModel mới được thêm.
        /// </summary>
        /// <param name="classGroupModel">ClassGroupModel</param>
        private void AddScheduleItems(ClassGroupModel classGroupModel)
        {
            IEnumerable<SchoolClassModel> schoolClassModels = classGroupModel.CurrentSchoolClassModels;
            var timeBlocks = schoolClassModels.SelectMany(scm => _timeBlockGenerator.Generate(scm));

            AddTimeBlockToSchedule(timeBlocks);
        }

        /// <summary>
        /// IMPORTANT!!!
        /// 
        /// Vẽ một <see cref="IScheduleTableItem"/> lên mô phỏng.
        /// </summary>
        /// <param name="scheduleItem">IScheduleTableItem</param>
        private void AddTimeBlockToSchedule(IEnumerable<TimeBlock> timeBlocks)
        {
            foreach (var timeBlock in timeBlocks)
            {
                if (timeBlock.Phase == Phase.First || timeBlock.Phase == Phase.Second)
                {
                    var week = timeBlock.Phase == Phase.First ? Week1 : Week2;
                    week.Add(timeBlock);
                }
                else if (timeBlock.Phase == Phase.All)
                {
                    Week1.Add(timeBlock);
                    Week2.Add(timeBlock);
                }
                else
                {
                    Week2.Add(timeBlock);
                }
            }
        }

        private void CleanDays()
        {
            Week1.Clear();
            Week2.Clear();
        }
    }
}
