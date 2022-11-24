﻿using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Dialogs.Implements;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Messages.States;
using Cs4rsa.ModelExtensions;
using Cs4rsa.Services.ConflictSvc.DataTypes;
using Cs4rsa.Services.ConflictSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;

using MaterialDesignThemes.Wpf;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.ViewModels
{
    public class ChoosedSessionViewModel : ViewModelBase
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
                _selectedClassGroupModel = value;
                OnPropertyChanged();
                DeleteCommand.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollection<ConflictModel> ConflictModels { get; set; }

        private ConflictModel _selectedConflictModel;
        public ConflictModel SelectedConflictModel
        {
            get { return _selectedConflictModel; }
            set { _selectedConflictModel = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PlaceConflictFinderModel> PlaceConflictFinderModels { get; set; }
        #endregion

        #region Commands
        public AsyncRelayCommand SaveCommand { get; set; }
        public AsyncRelayCommand OpenShareStringWindowCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
        public RelayCommand CopyCodeCommand { get; set; }
        public RelayCommand SolveConflictCommand { get; set; }
        #endregion

        #region DI
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly ShareString _shareStringGenerator;
        private readonly PhaseStore _phaseStore;
        #endregion

        public ChoosedSessionViewModel(
            ISnackbarMessageQueue snackbarMessageQueue,
            ShareString shareString,
            PhaseStore phaseStore
        )
        {
            _snackbarMessageQueue = snackbarMessageQueue;
            _shareStringGenerator = shareString;
            _phaseStore = phaseStore;

            #region WeakReferenceMessengers
            WeakReferenceMessenger.Default.Register<SearchVmMsgs.DelSubjectMsg>(this, (r, m) =>
            {
                DelSubjectMsgHandler(m.Value);
                _phaseStore.RemoveSchoolClassBySubjectCode(m.Value.SubjectCode);
            });

            WeakReferenceMessenger.Default.Register<SearchVmMsgs.DelAllSubjectMsg>(this, (r, m) =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    DelAllSubjectMsgHandler();
                    _phaseStore.RemoveAllSchoolClass();
                });
            });

            WeakReferenceMessenger.Default.Register<ClassGroupSessionVmMsgs.ClassGroupAddedMsg>(this, (r, m) =>
            {
                AddClassGroupModelAndReload(m.Value);
                _phaseStore.AddClassGroup(m.Value);
            });

            WeakReferenceMessenger.Default.Register<SearchVmMsgs.SelectClassGroupModelsMsg>(this, (r, m) =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    AddClassGroupModelsAndReload(m.Value);
                });
            });

            WeakReferenceMessenger.Default.Register<SolveConflictVmMsgs.RemoveChoicedClassMsg>(this, (r, m) =>
            {
                RemoveChoosedClassMsgHandler(m.Value);
            });

            WeakReferenceMessenger.Default.Register<PhaseStoreMsgs.BetweenPointChangedMsg>(this, (r, m) =>
            {
                UpdateConflicts();
            });
            #endregion

            SaveCommand = new AsyncRelayCommand(OpenSaveDialog, CanSave);
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, CanDeleteAll);
            CopyCodeCommand = new RelayCommand(OnCopyCode);
            SolveConflictCommand = new RelayCommand(OnSolve);
            OpenShareStringWindowCommand = new AsyncRelayCommand(OnOpenShareStringWindow);

            PlaceConflictFinderModels = new();
            ConflictModels = new();
            ClassGroupModels = new();
        }

        /// <summary>
        /// Giải quyết xung đột
        /// </summary>
        private void OnSolve()
        {
            SolveConflictUC solveConflictUC = new();
            SolveConflictViewModel vm = new(_selectedConflictModel)
            {
                CloseDialogCallback = (r) => CloseDialog()
            };
            solveConflictUC.DataContext = vm;
            OpenDialog(solveConflictUC);
        }

        /// <summary>
        /// Sao chép mã môn
        /// </summary>
        private void OnCopyCode()
        {
            if (SelectedClassGroupModel.RegisterCodes.Count > 0)
            {
                string registerCode = SelectedClassGroupModel.RegisterCodes[0];
                Clipboard.SetData(DataFormats.Text, registerCode);
            }
            else
            {
                _snackbarMessageQueue.Enqueue(VMConstants.SNB_NF_REGISTER_CODE);
            }
        }

        /// <summary>
        /// Xoá tất cả
        /// </summary>
        private void OnDeleteAll()
        {
            List<ClassGroupModel> actionData = new();
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                actionData.Add(classGroupModel.DeepClone());
            }

            ClassGroupModels.Clear();
            _phaseStore.RemoveAllSchoolClass();
            UpdateConflicts();
            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            Messenger.Send(new ChoicedSessionVmMsgs.ChoiceChangedMsg(ClassGroupModels));
            _snackbarMessageQueue.Enqueue(VMConstants.SNB_UNSELECT_ALL, VMConstants.SNBAC_RESTORE, OnRestore, actionData);
        }

        /// <summary>
        /// Hoàn tác
        /// </summary>
        /// <param name="obj">Danh sách lớp hoàn tác</param>
        private void OnRestore(List<ClassGroupModel> obj)
        {
            foreach (ClassGroupModel classGroupModel in obj)
            {
                AddClassGroupModelAndReload(classGroupModel);
            }
        }

        private void OnRestore(ClassGroupModel obj)
        {
            AddClassGroupModelAndReload(obj);
        }

        private void OnDelete()
        {
            string message = $"Đã bỏ chọn lớp {_selectedClassGroupModel.Name}";
            ClassGroupModel actionData = _selectedClassGroupModel.DeepClone();
            _phaseStore.RemoveSchoolClassBySubjectCode(_selectedClassGroupModel.SubjectCode);
            ClassGroupModels.Remove(_selectedClassGroupModel);
            _snackbarMessageQueue.Enqueue(message, VMConstants.SNBAC_RESTORE, OnRestore, actionData);

            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            UpdateConflicts();
            Messenger.Send(new ChoicedSessionVmMsgs.DelClassGroupChoiceMsg(ClassGroupModels));
        }

        /// <summary>
        /// Mở Dialog lưu session
        /// </summary>
        /// <returns>Task</returns>
        private async Task OpenSaveDialog()
        {
            SaveSessionUC saveSessionUC = new();
            SaveSessionViewModel vm = saveSessionUC.DataContext as SaveSessionViewModel;
            vm.ClassGroupModels = ClassGroupModels;
            vm.CloseDialogCallback = CloseDialogAndHandleSaveResult;
            OpenDialog(saveSessionUC);
            await vm.LoadScheduleSessions();
        }

        private async Task OnOpenShareStringWindow()
        {
            ShareStringUC shareStringUC = new();
            ShareStringViewModel vm = shareStringUC.DataContext as ShareStringViewModel;
            await UpdateShareString();
            vm.ShareString = _shareString;
            OpenDialog(shareStringUC);
        }

        private void CloseDialogAndHandleSaveResult(SaveResult result)
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseModal();
            if (result != null)
            {
                string message = $"Đã lưu phiên hiện tại với tên {result.Name}";
                _snackbarMessageQueue.Enqueue(message);
            }
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
            for (int i = 0; i < ClassGroupModels.Count; ++i)
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
        private void UpdateConflictModelCollection(List<SchoolClass> schoolClasses)
        {
            ConflictModels.Clear();
            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    if (schoolClasses[i].ClassGroupName.Equals(schoolClasses[k].ClassGroupName))
                    {
                        continue;
                    }
                    Conflict conflict = new(schoolClasses[i], schoolClasses[k]);
                    ConflictTime conflictTime = conflict.GetConflictTime();
                    if (!conflictTime.Equals(ConflictTime.NullInstance))
                    {
                        ConflictModel conflictModel = new(conflict);
                        ConflictModels.Add(conflictModel);
                    }
                }
            }
            Messenger.Send(new ChoicedSessionVmMsgs.ConflictCollChangedMsg(ConflictModels));
        }

        /// <summary>
        /// Thực hiện bắt cặp tất cả các ClassGroupModel có 
        /// trong Collection để phát hiện các Conflict Place.
        /// </summary>
        private void UpdatePlaceConflictCollection(List<SchoolClass> schoolClasses)
        {
            PlaceConflictFinderModels.Clear();
            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    PlaceConflictFinder placeConflict = new(schoolClasses[i], schoolClasses[k]);
                    ConflictPlace conflictPlace = placeConflict.GetPlaceConflict();
                    if (!conflictPlace.Equals(ConflictPlace.NullInstance))
                    {
                        PlaceConflictFinderModel placeConflictModel = new(placeConflict);
                        PlaceConflictFinderModels.Add(placeConflictModel);
                    }
                }
            }
            Messenger.Send(new ChoicedSessionVmMsgs.PlaceConflictCollChangedMsg(PlaceConflictFinderModels));
        }

        private void AddClassGroupModelAndReload(ClassGroupModel classGroupModel)
        {
            if (classGroupModel != null)
            {
                int ClassGroupModelIndex = IsReallyHaveAnotherVersionInChoicedList(classGroupModel);
                if (ClassGroupModelIndex != -1)
                    ClassGroupModels[ClassGroupModelIndex] = classGroupModel;
                else
                    ClassGroupModels.Add(classGroupModel);
            }

            _phaseStore.AddClassGroup(classGroupModel);
            UpdateConflicts();

            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            Messenger.Send(new ChoicedSessionVmMsgs.ChoiceChangedMsg(ClassGroupModels));
        }

        private void AddClassGroupModelsAndReload(IEnumerable<ClassGroupModel> classGroupModels)
        {
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                ClassGroupModels.Add(classGroupModel);
                _phaseStore.AddClassGroup(classGroupModel);
            }

            UpdateConflicts();

            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            Messenger.Send(new ChoicedSessionVmMsgs.ChoiceChangedMsg(ClassGroupModels));
        }

        private async Task UpdateShareString()
        {
            _shareString = await _shareStringGenerator.GetShareString(ClassGroupModels);
        }

        private void UpdateConflicts()
        {
            List<SchoolClass> schoolClasses = new();
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                if (classGroupModel.IsSpecialClassGroup)
                {
                    schoolClasses.AddRange(classGroupModel.CurrentSchoolClassModels.Select(scm => scm.SchoolClass));
                }
                else
                {
                    schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
                }
            }
            UpdateConflictModelCollection(schoolClasses);
            UpdatePlaceConflictCollection(schoolClasses);
        }

        #region Điều kiện thực thi command
        private bool CanSave()
        {
            return ClassGroupModels.Count > 0;
        }

        private bool CanDeleteAll()
        {
            return ClassGroupModels.Count > 0;
        }

        private bool CanDelete()
        {
            return _selectedClassGroupModel != null;
        }
        #endregion

        #region Handlers | Xử lý các message được gửi đến

        /// <summary>
        /// Xử lý Remove Choiced Class Message
        /// </summary>
        private void RemoveChoosedClassMsgHandler(string className)
        {
            if (className == string.Empty || className == null)
            {
                _snackbarMessageQueue.Enqueue(VMConstants.SNB_INVALID_UNSELECT_SUBJECT_NAME);
                return;
            }
            ClassGroupModel actionData;
            for (int i = 0; i < ClassGroupModels.Count; i++)
            {
                if (ClassGroupModels[i].Name == className)
                {
                    actionData = ClassGroupModels[i].DeepClone();
                    _phaseStore.RemoveSchoolClassBySubjectCode(ClassGroupModels[i].SubjectCode);
                    ClassGroupModels.RemoveAt(i);
                    string messageContent = $"Đã bỏ chọn lớp {className}";
                    _snackbarMessageQueue.Enqueue(messageContent, VMConstants.SNBAC_RESTORE, OnRestore, actionData);

                    SaveCommand.NotifyCanExecuteChanged();
                    DeleteAllCommand.NotifyCanExecuteChanged();
                    DeleteCommand.NotifyCanExecuteChanged();
                    UpdateConflicts();
                    Messenger.Send(new ChoicedSessionVmMsgs.DelClassGroupChoiceMsg(ClassGroupModels));
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện xoá môn học
        /// </summary>
        /// <param name="message">Thông tin sự kiện môn học đã xoá</param>
        private void DelSubjectMsgHandler(SubjectModel subjectModel)
        {
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                if (classGroupModel.SubjectCode.Equals(subjectModel.SubjectCode))
                {
                    ClassGroupModels.Remove(classGroupModel);
                    break;
                }
            }
            UpdateConflicts();
            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            Messenger.Send(new ChoicedSessionVmMsgs.ChoiceChangedMsg(ClassGroupModels));
        }

        /// <summary>
        /// Xử lý sự kiện xoá toàn bộ môn học
        /// </summary>
        /// <param name="message">Thông tin sự kiện môn học đã xoá</param>
        private void DelAllSubjectMsgHandler()
        {
            ClassGroupModels.Clear();
            UpdateConflicts();
            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            Messenger.Send(new ChoicedSessionVmMsgs.ChoiceChangedMsg(ClassGroupModels));
        }
        #endregion
    }
}