using ConflictService.DataTypes;
using ConflictService.Models;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.DialogViews;
using cs4rsa_core.Dialogs.Implements;
using cs4rsa_core.Messages;
using cs4rsa_core.ModelExtensions;
using cs4rsa_core.Models;
using cs4rsa_core.Utils;
using LightMessageBus;
using LightMessageBus.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.Input;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace cs4rsa_core.ViewModels
{
    public class ChoicedSessionViewModel : ViewModelBase,
        IMessageHandler<ClassGroupAddedMessage>,
        IMessageHandler<DeleteSubjectMessage>,
        IMessageHandler<RemoveChoicedClassMessage>
    {
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

        #region Commands
        public AsyncRelayCommand SaveCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
        public RelayCommand CopyCodeCommand { get; set; }
        public RelayCommand SolveConflictCommand { get; set; }
        public RelayCommand OpenShareStringWindowCommand { get; set; }
        #endregion

        #region Services
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly ShareString _shareStringGenerator;
        #endregion
        public ChoicedSessionViewModel(ISnackbarMessageQueue snackbarMessageQueue, ShareString shareString)
        {
            _snackbarMessageQueue = snackbarMessageQueue;
            _shareStringGenerator = shareString;

            MessageBus.Default.FromAny().Where<ClassGroupAddedMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteSubjectMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<RemoveChoicedClassMessage>().Notify(this);

            SaveCommand = new AsyncRelayCommand(OpenSaveDialog, CanSave);
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, CanDeleteAll);
            CopyCodeCommand = new RelayCommand(OnCopyCode);
            SolveConflictCommand = new RelayCommand(OnSolve);
            OpenShareStringWindowCommand = new RelayCommand(OnOpenShareStringWindow);

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
                CloseDialogCallback = (r) => CloseD()
            };
            solveConflictUC.DataContext = vm;
            OpenD(solveConflictUC);
        }

        /// <summary>
        /// Sao chép mã môn
        /// </summary>
        private void OnCopyCode()
        {
            string registerCode = _selectedClassGroupModel.RegisterCode;
            Clipboard.SetData(DataFormats.Text, registerCode);
            string message = $"Đã copy mã của môn {_selectedClassGroupModel.SubjectCode} vào Clipboard";
            _snackbarMessageQueue.Enqueue(message);
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
            UpdateConflictModelCollection();
            UpdatePlaceConflictCollection();
            UpdateShareString();
            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            MessageBus.Default.Publish(new ChoicesChangedMessage(ClassGroupModels.ToList()));
            _snackbarMessageQueue.Enqueue("Đã bỏ chọn tất cả", "HOÀN TÁC", OnRestore, actionData);
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
            ClassGroupModels.Remove(_selectedClassGroupModel);
            _snackbarMessageQueue.Enqueue(message, "HOÀN TÁC", OnRestore, actionData);

            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            UpdateConflictModelCollection();
            UpdatePlaceConflictCollection();
            UpdateShareString();
            MessageBus.Default.Publish(new DeleteClassGroupChoiceMessage(ClassGroupModels.ToList()));
        }

        /// <summary>
        /// Mở Dialog lưu session
        /// </summary>
        /// <returns>Task</returns>
        private async Task OpenSaveDialog()
        {
            SaveSessionUC saveSessionUC = new();
            SaveSessionViewModel vm = saveSessionUC.DataContext as SaveSessionViewModel;
            vm.ClassGroupModels = ClassGroupModels.ToList();
            vm.CloseDialogCallback = CloseDialogAndHandleSaveResult;
            OpenD(saveSessionUC);
            await vm.LoadScheduleSessions();
        }

        private void OnOpenShareStringWindow()
        {
            ShareStringUC shareStringUC = new();
            ShareStringViewModel vm = shareStringUC.DataContext as ShareStringViewModel;
            vm.ShareString = _shareString;
            OpenD(shareStringUC);
        }

        private void CloseDialogAndHandleSaveResult(SaveResult result)
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            if (result != null)
            {
                string message = $"Đã lưu phiên hiện tại với tên {result.Name}";
                _snackbarMessageQueue.Enqueue(message);
            }
        }

        /// <summary>
        /// Kiểm tra xem một Class Group Model nào đó có tồn tại một
        /// phiên bản cùng Subject Code nhưng khác tên khác không.
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
        private void UpdateConflictModelCollection()
        {
            ConflictModels.Clear();
            List<SchoolClass> schoolClasses = new();
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
            }

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
                    if (conflictTime != null)
                    {
                        ConflictModel conflictModel = new(conflict);
                        ConflictModels.Add(conflictModel);
                    }
                }
            }
            MessageBus.Default.Publish(new ConflictCollectionChangeMessage(ConflictModels.ToList()));
        }

        /// <summary>
        /// Thực hiện bắt cặp tất cả các ClassGroupModel có 
        /// trong Collection để phát hiện các Conflict Place.
        /// </summary>
        private void UpdatePlaceConflictCollection()
        {
            PlaceConflictFinderModels.Clear();
            List<SchoolClass> schoolClasses = new();
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
            }

            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    PlaceConflictFinder placeConflict = new(schoolClasses[i], schoolClasses[k]);
                    ConflictPlace conflictPlace = placeConflict.GetPlaceConflict();
                    if (conflictPlace != null)
                    {
                        PlaceConflictFinderModel placeConflictModel = new(placeConflict);
                        PlaceConflictFinderModels.Add(placeConflictModel);
                    }
                }
            }
            MessageBus.Default.Publish(new PlaceConflictCollectionChangeMessage(PlaceConflictFinderModels.ToList()));
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

            UpdateConflictModelCollection();
            UpdatePlaceConflictCollection();

            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            MessageBus.Default.Publish(new ChoicesChangedMessage(ClassGroupModels.ToList()));
            UpdateShareString();
        }

        private void UpdateShareString()
        {
            List<ClassGroup> classGroups = ClassGroupModels.Select(classGroupModels => classGroupModels.ClassGroup).ToList();
            _shareString = _shareStringGenerator.GetShareString(classGroups);
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
        /// Xử lý message xoá lớp học đã chọn.
        /// </summary>
        /// <param name="message">Thông điệp xoá lớp học</param>
        public void Handle(RemoveChoicedClassMessage message)
        {
            string className = message.Source;
            if (className == string.Empty || className == null)
            {
                _snackbarMessageQueue.Enqueue("Tên lớp cần bỏ chọn không hợp lệ");
                return;
            }
            ClassGroupModel actionData;
            for (int i = 0; i < ClassGroupModels.Count; i++)
            {
                if (ClassGroupModels[i].Name == className)
                {
                    actionData = ClassGroupModels[i].DeepClone();
                    ClassGroupModels.RemoveAt(i);
                    string messageContent = $"Đã bỏ chọn lớp {className}";
                    _snackbarMessageQueue.Enqueue(messageContent, "HOÀN TÁC", OnRestore, actionData);

                    SaveCommand.NotifyCanExecuteChanged();
                    DeleteAllCommand.NotifyCanExecuteChanged();
                    DeleteCommand.NotifyCanExecuteChanged();
                    UpdateConflictModelCollection();
                    UpdatePlaceConflictCollection();
                    MessageBus.Default.Publish(new DeleteClassGroupChoiceMessage(ClassGroupModels.ToList()));
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện xoá môn học
        /// </summary>
        /// <param name="message">Thông tin sự kiện môn học đã xoá</param>
        public void Handle(DeleteSubjectMessage message)
        {
            SubjectModel subjectModel = message.Source;
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                if (classGroupModel.SubjectCode.Equals(subjectModel.SubjectCode))
                {
                    ClassGroupModels.Remove(classGroupModel);
                    break;
                }
            }
            UpdateConflictModelCollection();
            UpdatePlaceConflictCollection();
            SaveCommand.NotifyCanExecuteChanged();
            MessageBus.Default.Publish(new ChoicesChangedMessage(ClassGroupModels.ToList()));
            UpdateShareString();
        }

        /// <summary>
        /// Xử lý sự kiện thêm Nhóm Lớp
        /// </summary>
        /// <param name="message">Thông tin sự kiện thêm nhóm lớp</param>
        public void Handle(ClassGroupAddedMessage message)
        {
            AddClassGroupModelAndReload(message.Source);
        }
        #endregion
    }
}
