using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

using Cs4rsa.Common;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Messages.Publishers.UIs;
using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Views;
using Cs4rsa.Module.ManuallySchedule.Events;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Module.ManuallySchedule.Utils;
using Cs4rsa.Service.Conflict.DataTypes;
using Cs4rsa.Service.Conflict.Models;
using Cs4rsa.Service.Dialog.Interfaces;
using Cs4rsa.UI.ScheduleTable.Models;

using MaterialDesignThemes.Wpf;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

using static Cs4rsa.Module.ManuallySchedule.Events.ChoosedVmMsgs;

namespace Cs4rsa.Module.ManuallySchedule.ViewModels
{
    public class ChoseViewModel : BindableBase
    {
        #region Properties
        private string _shareString;

        public ObservableCollection<ClassGroupModel> ClassGroupModels { get; set; }

        private ClassGroupModel _selectedClassGroupModel;
        public ClassGroupModel SelectedClassGroupModel
        {
            get => _selectedClassGroupModel;
            set
            {
                SetProperty(ref _selectedClassGroupModel, value);
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<ConflictModel> ConflictModels { get; set; }

        private ConflictModel _selectedConflictModel;
        public ConflictModel SelectedConflictModel
        {
            get { return _selectedConflictModel; }
            set { SetProperty(ref _selectedConflictModel, value); }
        }

        public ObservableCollection<PlaceConflictFinderModel> PlaceConflictFinderModels { get; set; }
        #endregion

        #region Commands
        public DelegateCommand OpenShareStringWindowCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand DeleteAllCommand { get; set; }
        public DelegateCommand CopyCodeCommand { get; set; }
        public DelegateCommand SolveConflictCommand { get; set; }
        #endregion

        #region DI
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly ShareString _shareStringGenerator;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDialogService _dialogService;
        #endregion

        public ChoseViewModel(
            ISnackbarMessageQueue snackbarMessageQueue,
            IUnitOfWork unitOfWork,
            IEventAggregator eventAggregator,
            IDialogService dialogService,
            ShareString shareString
        )
        {
            _snackbarMessageQueue = snackbarMessageQueue;
            _shareStringGenerator = shareString;
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            _unitOfWork = unitOfWork;

            Debug.WriteLine("ChoseViewModel " + _eventAggregator.GetHashCode());

            #region Messengers
            _eventAggregator.GetEvent<SearchVmMsgs.DelSubjectMsg>().Subscribe(payload =>
            {
                DelSubjectMsgHandler(payload);
            });

            _eventAggregator.GetEvent<SearchVmMsgs.DelAllSubjectMsg>().Subscribe(DelAllSubjectMsgHandler);

            _eventAggregator.GetEvent<ClassGroupSessionVmMsgs.ClassGroupAddedMsg>().Subscribe(payload =>
            {
                _eventAggregator.GetEvent<ClassGroupAddedMsg>().Publish(payload);
                AddClassGroupModel(payload);
            });

            _eventAggregator.GetEvent<SearchVmMsgs.SelectCgmsMsg>().Subscribe(payload =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    AddClassGroupModelsAndReload(payload);
                });
            });

            _eventAggregator.GetEvent<SolveConflictVmMsgs.RemoveChoicedClassMsg>().Subscribe(payload =>
            {
                _dialogService.CloseDialog();
                RemoveChoosedClassMsgHandler(payload);
            });

            //eventAggregator.GetEvent<UpdateVmMsgs.UpdateSuccessMsg>().Subscribe(payload =>
            //{
            //    ClassGroupModels.Clear();
            //    ConflictModels.Clear();
            //    PlaceConflictFinderModels.Clear();
            //});

            // Click vào block thì đồng thời select class group model tương ứng.
            _eventAggregator.GetEvent<ScheduleBlockMsgs.SelectedMsg>().Subscribe(payload =>
            {
                if (payload.GetType() == typeof(SchoolClassBlock))
                {
                    var schoolClassBlock = (SchoolClassBlock)payload;
                    var result = ClassGroupModels.FirstOrDefault(cgm => cgm.ClassGroup.Name.Equals(schoolClassBlock.SchoolClassUnit.SchoolClass.ClassGroupName));
                    if (result != null)
                    {
                        SelectedClassGroupModel = result;
                    }
                }
                else
                {
                    return;
                }
            });
            #endregion

            SaveCommand = new DelegateCommand(OpenSaveDialog, () => ClassGroupModels.Count > 0);
            DeleteCommand = new DelegateCommand(OnDelete, () => _selectedClassGroupModel != null);
            DeleteAllCommand = new DelegateCommand(OnDeleteAll, () => ClassGroupModels.Count > 0);
            CopyCodeCommand = new DelegateCommand(OnCopyCode);
            SolveConflictCommand = new DelegateCommand(OnSolve);
            OpenShareStringWindowCommand = new DelegateCommand(OnOpenShareStringWindow);

            PlaceConflictFinderModels = new ObservableCollection<PlaceConflictFinderModel>();
            ConflictModels = new ObservableCollection<ConflictModel>();
            ClassGroupModels = new ObservableCollection<ClassGroupModel>();
        }

        /// <summary>
        /// Giải quyết xung đột
        /// </summary>
        private void OnSolve()
        {
            var solveConflictUc = new SolveConflictUC();
            var vm = new SolveConflictViewModel(SelectedConflictModel, _unitOfWork, _eventAggregator);
            solveConflictUc.DataContext = vm;
            _dialogService.OpenDialog(solveConflictUc);
        }

        /// <summary>
        /// Sao chép mã môn
        /// </summary>
        private void OnCopyCode()
        {
            if (SelectedClassGroupModel.RegisterCodes.Count > 0)
            {
                var registerCode = SelectedClassGroupModel.RegisterCodes[0];
                Clipboard.SetData(DataFormats.Text, registerCode);
                _snackbarMessageQueue.Enqueue($"Sao chép thành công {registerCode}");
            }
            else
            {
                _snackbarMessageQueue.Enqueue("Lớp này không có mã đăng ký");
            }
        }

        /// <summary>
        /// Xoá tất cả
        /// </summary>
        private void OnDeleteAll()
        {
            var actionData = new List<ClassGroupModel>();
            foreach (var classGroupModel in ClassGroupModels)
            {
                actionData.Add(classGroupModel.DeepClone());
            }

            ClassGroupModels.Clear();
            UpdateConflicts();
            SaveCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
            _eventAggregator.GetEvent<DelAllClassGroupChoiceMsg>().Publish(DBNull.Value);
            _snackbarMessageQueue.Enqueue("Đã bỏ chọn tất cả", "HOÀN TÁC", OnRestore, actionData);
        }

        /// <summary>
        /// Hoàn tác
        /// </summary>
        /// <param name="classGroupModels">Danh sách lớp hoàn tác</param>
        private void OnRestore(IEnumerable<ClassGroupModel> classGroupModels)
        {
            foreach (var classGroupModel in classGroupModels)
            {
                AddClassGroupModel(classGroupModel);
            }
            var payload = Tuple.Create(classGroupModels, ConflictModels, PlaceConflictFinderModels);
            _eventAggregator.GetEvent<UndoDelAllMsg>().Publish(payload);
        }

        private void OnDelete()
        {
            var actionData = _selectedClassGroupModel.DeepClone();
            _eventAggregator.GetEvent<DelClassGroupChoiceMsg>().Publish(_selectedClassGroupModel);

            ClassGroupModels.Remove(_selectedClassGroupModel);
            _snackbarMessageQueue.Enqueue(
                $"Đã xoá lớp {_selectedClassGroupModel.Name}",
                "HOÀN TÁC",
                (obj) => AddClassGroupModel(actionData),
                actionData
            );

            SaveCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            UpdateConflicts();
        }

        /// <summary>
        /// Mở Dialog lưu session
        /// </summary>
        /// <returns>Task</returns>
        private void OpenSaveDialog()
        {
            var saveSessionUc = new SaveSessionUC();
            var vm = (SaveSessionUCViewModel)saveSessionUc.DataContext;
            vm.ClassGroupModels = ClassGroupModels;
            _dialogService.OpenDialog(saveSessionUc);
        }

        private void OnOpenShareStringWindow()
        {
            var shareStringUc = new ShareStringUC();
            var vm = shareStringUc.DataContext as ShareStringViewModel;
            UpdateShareString();
            vm.ShareString = _shareString;
            _dialogService.OpenDialog(shareStringUc);
        }

        /// <summary>
        /// Kiểm tra xem một Class Group Model nào đó có tồn tại một
        /// phiên bản cùng Subject ClassGroupName nhưng khác tên khác không.
        /// </summary>
        /// <param name="classGroupModel">Một Class Group Model.</param>
        /// <returns>Trả về index của ClassGroupModel nếu nó có SubjectCode
        /// bằng với ClassGroupModel được truyền vào nếu không trả về -1.</returns>
        private int IsReallyHaveAnotherVersionInChoicedList(ClassGroupModel classGroupModel)
        {
            for (var i = 0; i < ClassGroupModels.Count; ++i)
            {
                if (ClassGroupModels[i].SubjectCode.Equals(classGroupModel.SubjectCode))
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
        private void UpdateConflictModelCollection(List<SchoolClassModel> schoolClassModels)
        {
            ConflictModels.Clear();
            for (var i = 0; i < schoolClassModels.Count; ++i)
            {
                for (var k = i + 1; k < schoolClassModels.Count; ++k)
                {
                    if (schoolClassModels[i].SchoolClass.ClassGroupName
                        .Equals(schoolClassModels[k].SchoolClass.ClassGroupName))
                    {
                        continue;
                    }

                    var lessonA = new Lesson(
                            schoolClassModels[i].StudyWeek,
                            schoolClassModels[i].Schedule,
                            schoolClassModels[i].DayPlaceMetaData,
                            schoolClassModels[i].SchoolClass.GetMetaDataMap(),
                            schoolClassModels[i].Phase,
                            schoolClassModels[i].SchoolClassName,
                            schoolClassModels[i].SchoolClass.ClassGroupName,
                            schoolClassModels[i].SubjectCode
                    );

                    var lessonB = new Lesson(
                            schoolClassModels[k].StudyWeek,
                            schoolClassModels[k].Schedule,
                            schoolClassModels[k].DayPlaceMetaData,
                            schoolClassModels[k].SchoolClass.GetMetaDataMap(),
                            schoolClassModels[k].Phase,
                            schoolClassModels[k].SchoolClassName,
                            schoolClassModels[k].SchoolClass.ClassGroupName,
                            schoolClassModels[k].SubjectCode
                    );

                    var conflict = new Conflict(lessonA, lessonB);
                    var conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                    {
                        var conflictModel = new ConflictModel(conflict);
                        ConflictModels.Add(conflictModel);
                    }
                }
            }
            _eventAggregator.GetEvent<ConflictCollChangedMsg>().Publish(ConflictModels);
        }

        /// <summary>
        /// Thực hiện bắt cặp tất cả các ClassGroupModel có 
        /// trong Collection để phát hiện các Conflict Place.
        /// </summary>
        private void UpdatePlaceConflictCollection(List<SchoolClassModel> schoolClassModels)
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
                        schoolClassModels[i].SchoolClass.GetMetaDataMap(),
                        schoolClassModels[i].Phase,
                        schoolClassModels[i].SchoolClassName,
                        schoolClassModels[i].SchoolClass.ClassGroupName,
                        schoolClassModels[i].SubjectCode
                    );

                    var lessonB = new Lesson(
                        schoolClassModels[k].StudyWeek,
                        schoolClassModels[k].Schedule,
                        schoolClassModels[k].DayPlaceMetaData,
                        schoolClassModels[k].SchoolClass.GetMetaDataMap(),
                        schoolClassModels[k].Phase,
                        schoolClassModels[k].SchoolClassName,
                        schoolClassModels[k].SchoolClass.ClassGroupName,
                        schoolClassModels[k].SubjectCode
                    );

                    var placeConflict = new PlaceConflictFinder(lessonA, lessonB);
                    var conflictPlace = placeConflict.GetPlaceConflict();
                    if (conflictPlace != null)
                    {
                        var placeConflictModel = new PlaceConflictFinderModel(placeConflict);
                        PlaceConflictFinderModels.Add(placeConflictModel);
                    }
                }
            }
            _eventAggregator.GetEvent<PlaceConflictCollChangedMsg>().Publish(PlaceConflictFinderModels);
        }

        private void AddClassGroupModel(ClassGroupModel classGroupModel)
        {
            if (classGroupModel != null)
            {
                var classGroupModelIndex = IsReallyHaveAnotherVersionInChoicedList(classGroupModel);
                if (classGroupModelIndex != -1)
                    ClassGroupModels[classGroupModelIndex] = classGroupModel;
                else
                    ClassGroupModels.Add(classGroupModel);
            }
            UpdateConflicts();
            SaveCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
        }

        private void AddClassGroupModelsAndReload(IEnumerable<ClassGroupModel> classGroupModels)
        {
            foreach (var classGroupModel in classGroupModels)
            {
                ClassGroupModels.Add(classGroupModel);
            }
            UpdateConflicts();
            SaveCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
        }

        private void UpdateShareString()
        {
            _shareString = _shareStringGenerator.GetShareString(ClassGroupModels);
        }

        private void UpdateConflicts()
        {
            var schoolClasses = new List<SchoolClassModel>();
            foreach (var classGroupModel in ClassGroupModels)
            {
                if (classGroupModel.IsSpecialClassGroup)
                {
                    schoolClasses.AddRange(classGroupModel.CurrentSchoolClassModels);
                }
                else
                {
                    schoolClasses.AddRange(classGroupModel.CurrentSchoolClassModels);
                    //schoolClassModels.AddRange(classGroupModel.ClassGroup.SchoolClasses);
                }
            }
            UpdateConflictModelCollection(schoolClasses);
            UpdatePlaceConflictCollection(schoolClasses);
        }

        #region Handlers | Xử lý các message được gửi đến

        /// <summary>
        /// Xử lý Remove Choiced Class Message
        /// </summary>
        private void RemoveChoosedClassMsgHandler(string className)
        {
            if (className == string.Empty || className == null)
            {
                _snackbarMessageQueue.Enqueue("Tên lớp cần bỏ chọn không hợp lệ");
                return;
            }
            ClassGroupModel actionData;
            for (var i = 0; i < ClassGroupModels.Count; i++)
            {
                if (ClassGroupModels[i].Name == className)
                {
                    actionData = ClassGroupModels[i].DeepClone();
                    _eventAggregator.GetEvent<DelClassGroupChoiceMsg>().Publish(ClassGroupModels[i]);
                    ClassGroupModels.RemoveAt(i);
                    _snackbarMessageQueue.Enqueue(
                        $"Đã xoá {className}",
                        "HOÀN TÁC",
                        obj => AddClassGroupModel(actionData),
                        actionData
                    );

                    SaveCommand.RaiseCanExecuteChanged();
                    DeleteAllCommand.RaiseCanExecuteChanged();
                    DeleteCommand.RaiseCanExecuteChanged();
                    UpdateConflicts();
                    break;
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện xoá môn học
        /// </summary>
        /// <param name="message">Thông tin sự kiện môn học đã xoá</param>
        private void DelSubjectMsgHandler(SubjectModel subjectModel)
        {
            foreach (var classGroupModel in ClassGroupModels)
            {
                if (classGroupModel.SubjectCode.Equals(subjectModel.SubjectCode))
                {
                    ClassGroupModels.Remove(classGroupModel);
                    break;
                }
            }
            UpdateConflicts();
            SaveCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Xử lý sự kiện xoá toàn bộ môn học
        /// </summary>
        /// <param name="message">Thông tin sự kiện môn học đã xoá</param>
        private void DelAllSubjectMsgHandler()
        {
            ClassGroupModels.Clear();
            UpdateConflicts();
            SaveCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
            _eventAggregator.GetEvent<DelAllClassGroupChoiceMsg>().Publish(DBNull.Value);
        }
        #endregion
    }
}
