using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Dialogs.Implements;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Messages.Publishers.UIs;
using Cs4rsa.Messages.States;
using Cs4rsa.ModelExtensions;
using Cs4rsa.Models;
using Cs4rsa.Services.ConflictSvc.DataTypes;
using Cs4rsa.Services.ConflictSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils;

using MaterialDesignThemes.Wpf;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using static Cs4rsa.Messages.Publishers.ChoosedVmMsgs;

namespace Cs4rsa.ViewModels.ManualScheduling
{
    public partial class ChoseViewModel : ViewModelBase
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

        [ObservableProperty]
        private ConflictModel _selectedConflictModel;

        public ObservableCollection<PlaceConflictFinderModel> PlaceConflictFinderModels { get; set; }
        #endregion

        #region Commands
        public RelayCommand OpenShareStringWindowCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
        public RelayCommand CopyCodeCommand { get; set; }
        public RelayCommand SolveConflictCommand { get; set; }
        #endregion

        #region DI
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly ShareString _shareStringGenerator;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        public ChoseViewModel(
            ISnackbarMessageQueue snackbarMessageQueue,
            ShareString shareString,
            IUnitOfWork unitOfWork
        )
        {
            _snackbarMessageQueue = snackbarMessageQueue;
            _shareStringGenerator = shareString;
            _unitOfWork = unitOfWork;

            #region Messengers
            Messenger.Register<SearchVmMsgs.DelSubjectMsg>(this, (r, m) =>
            {
                DelSubjectMsgHandler(m.Value);
            });

            Messenger.Register<SearchVmMsgs.DelAllSubjectMsg>(this, (r, m) =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    DelAllSubjectMsgHandler();
                });
            });

            Messenger.Register<ClassGroupSessionVmMsgs.ClassGroupAddedMsg>(this, (r, m) =>
            {
                Messenger.Send(new ClassGroupAddedMsg(m.Value));
                AddClassGroupModel(m.Value);
            });

            Messenger.Register<SearchVmMsgs.SelectCgmsMsg>(this, (r, m) =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    AddClassGroupModelsAndReload(m.Value);
                });
            });

            Messenger.Register<SolveConflictVmMsgs.RemoveChoicedClassMsg>(this, (r, m) =>
            {
                CloseDialog();
                RemoveChoosedClassMsgHandler(m.Value);
            });

            Messenger.Register<PhaseStoreMsgs.BetweenPointChangedMsg>(this, (r, m) =>
            {
                UpdateConflicts();
            });

            Messenger.Register<UpdateVmMsgs.UpdateSuccessMsg>(this, (r, m) =>
            {
                ClassGroupModels.Clear();
                ConflictModels.Clear();
                PlaceConflictFinderModels.Clear();
            });
            
            // Click vào block thì đồng thời select class group model tương ứng.
            Messenger.Register<ScheduleBlockMsgs.SelectedMsg>(this, (r, m) =>
            {
                if (m.Value is SchoolClassBlock @schoolClassBlock)
                {
                    ClassGroupModel result = ClassGroupModels
                        .Where(cgm => cgm.ClassGroup.Name.Equals(schoolClassBlock.SchoolClassUnit.SchoolClass.ClassGroupName))
                        .FirstOrDefault();
                    if (result != null)
                    {
                        SelectedClassGroupModel = result;
                    }
                }
            });
            #endregion

            SaveCommand = new RelayCommand(OpenSaveDialog, () => ClassGroupModels.Count > 0);
            DeleteCommand = new RelayCommand(OnDelete, () => _selectedClassGroupModel != null);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, () => ClassGroupModels.Count > 0);
            CopyCodeCommand = new RelayCommand(OnCopyCode);
            SolveConflictCommand = new RelayCommand(OnSolve);
            OpenShareStringWindowCommand = new(OnOpenShareStringWindow);

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
            SolveConflictViewModel vm = new(SelectedConflictModel, _unitOfWork);
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
                _snackbarMessageQueue.Enqueue(VmConstants.SnbCopySuccess + VmConstants.StrSpace + registerCode);
            }
            else
            {
                _snackbarMessageQueue.Enqueue(VmConstants.SnbNfRegisterCode);
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
            UpdateConflicts();
            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            Messenger.Send(new DelAllClassGroupChoiceMsg(DBNull.Value));
            _snackbarMessageQueue.Enqueue(VmConstants.SnbUnselectAll, VmConstants.SnbRestore, OnRestore, actionData);
        }

        /// <summary>
        /// Hoàn tác
        /// </summary>
        /// <param name="classGroupModels">Danh sách lớp hoàn tác</param>
        private void OnRestore(IEnumerable<ClassGroupModel> classGroupModels)
        {
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                AddClassGroupModel(classGroupModel);
            }
            Messenger.Send(new UndoDelAllMsg(classGroupModels));
        }

        private void OnDelete()
        {
            string message = CredizText.ManualMsg001(_selectedClassGroupModel.Name);
            ClassGroupModel actionData = _selectedClassGroupModel.DeepClone();
            Messenger.Send(new DelClassGroupChoiceMsg(_selectedClassGroupModel));

            ClassGroupModels.Remove(_selectedClassGroupModel);
            _snackbarMessageQueue.Enqueue(
                message,
                VmConstants.SnbRestore,
                (obj) => AddClassGroupModel(actionData),
                actionData
            );

            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            UpdateConflicts();
        }

        /// <summary>
        /// Mở Dialog lưu session
        /// </summary>
        /// <returns>Task</returns>
        private void OpenSaveDialog()
        {
            SaveSessionUC saveSessionUC = new();
            SaveSessionViewModel vm = saveSessionUC.DataContext as SaveSessionViewModel;
            vm.ClassGroupModels = ClassGroupModels;
            OpenDialog(saveSessionUC);
        }

        private void OnOpenShareStringWindow()
        {
            ShareStringUC shareStringUC = new();
            ShareStringViewModel vm = shareStringUC.DataContext as ShareStringViewModel;
            UpdateShareString();
            vm.ShareString = _shareString;
            OpenDialog(shareStringUC);
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
        private void UpdateConflictModelCollection(List<SchoolClassModel> schoolClassModels)
        {
            ConflictModels.Clear();
            for (int i = 0; i < schoolClassModels.Count; ++i)
            {
                for (int k = i + 1; k < schoolClassModels.Count; ++k)
                {
                    if (schoolClassModels[i].SchoolClass.ClassGroupName
                        .Equals(schoolClassModels[k].SchoolClass.ClassGroupName))
                    {
                        continue;
                    }
                    Conflict conflict = new(schoolClassModels[i], schoolClassModels[k]);
                    ConflictTime conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                    {
                        ConflictModel conflictModel = new(conflict);
                        ConflictModels.Add(conflictModel);
                    }
                }
            }
            Messenger.Send(new ConflictCollChangedMsg(ConflictModels));
        }

        /// <summary>
        /// Thực hiện bắt cặp tất cả các ClassGroupModel có 
        /// trong Collection để phát hiện các Conflict Place.
        /// </summary>
        private void UpdatePlaceConflictCollection(List<SchoolClassModel> schoolClasseModels)
        {
            PlaceConflictFinderModels.Clear();
            for (int i = 0; i < schoolClasseModels.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasseModels.Count; ++k)
                {
                    PlaceConflictFinder placeConflict = new(schoolClasseModels[i], schoolClasseModels[k]);
                    ConflictPlace conflictPlace = placeConflict.GetPlaceConflict();
                    if (conflictPlace != null)
                    {
                        PlaceConflictFinderModel placeConflictModel = new(placeConflict);
                        PlaceConflictFinderModels.Add(placeConflictModel);
                    }
                }
            }
            Messenger.Send(new PlaceConflictCollChangedMsg(PlaceConflictFinderModels));
        }

        private void AddClassGroupModel(ClassGroupModel classGroupModel)
        {
            if (classGroupModel != null)
            {
                int ClassGroupModelIndex = IsReallyHaveAnotherVersionInChoicedList(classGroupModel);
                if (ClassGroupModelIndex != -1)
                    ClassGroupModels[ClassGroupModelIndex] = classGroupModel;
                else
                    ClassGroupModels.Add(classGroupModel);
            }
            UpdateConflicts();
            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
        }

        private void AddClassGroupModelsAndReload(IEnumerable<ClassGroupModel> classGroupModels)
        {
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                ClassGroupModels.Add(classGroupModel);
            }
            UpdateConflicts();
            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
        }

        private void UpdateShareString()
        {
            _shareString = _shareStringGenerator.GetShareString(ClassGroupModels);
        }

        private void UpdateConflicts()
        {
            List<SchoolClassModel> schoolClasses = new();
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
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
                _snackbarMessageQueue.Enqueue(VmConstants.SnbInvalidUnselectSubjectName);
                return;
            }
            ClassGroupModel actionData;
            for (int i = 0; i < ClassGroupModels.Count; i++)
            {
                if (ClassGroupModels[i].Name == className)
                {
                    actionData = ClassGroupModels[i].DeepClone();
                    Messenger.Send(new DelClassGroupChoiceMsg(ClassGroupModels[i]));
                    ClassGroupModels.RemoveAt(i);
                    _snackbarMessageQueue.Enqueue(
                        CredizText.ManualMsg001(className),
                        VmConstants.SnbRestore,
                        (obj) => AddClassGroupModel(actionData),
                        actionData
                    );

                    SaveCommand.NotifyCanExecuteChanged();
                    DeleteAllCommand.NotifyCanExecuteChanged();
                    DeleteCommand.NotifyCanExecuteChanged();
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
            Messenger.Send(new DelAllClassGroupChoiceMsg(DBNull.Value));
        }
        #endregion
    }
}
