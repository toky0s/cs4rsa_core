using ConflictService.DataTypes;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.DialogViews;
using cs4rsa_core.Dialogs.Implements;
using cs4rsa_core.Messages;
using cs4rsa_core.Models;
using LightMessageBus;
using LightMessageBus.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.Input;
using SubjectCrawlService1.DataTypes;
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
        IMessageHandler<RemoveAChoiceClassGroupMessage>
    {
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
        #endregion

        #region Services
        private readonly ImportSessionViewModel _importSessionViewModel;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion
        public ChoicedSessionViewModel(ImportSessionViewModel importSessionViewModel, 
            ISnackbarMessageQueue snackbarMessageQueue)
        {
            _importSessionViewModel = importSessionViewModel;
            _snackbarMessageQueue = snackbarMessageQueue;

            MessageBus.Default.FromAny().Where<ClassGroupAddedMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteSubjectMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<RemoveAChoiceClassGroupMessage>().Notify(this);

            SaveCommand = new AsyncRelayCommand(OpenSaveDialog, CanSave);
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, CanDeleteAll);
            CopyCodeCommand = new RelayCommand(OnCopyCode);
            SolveConflictCommand = new RelayCommand(OnSolve);

            PlaceConflictFinderModels = new();
            ConflictModels = new();
            ClassGroupModels = new();
        }

        /// <summary>
        /// Giải quyết xung đột
        /// </summary>
        /// <param name="obj"></param>
        private void OnSolve()
        {
            SolveConflictUC solveConflictUC = new();
            SolveConflictViewModel vm = new(_selectedConflictModel);
            vm.CloseDialogCallback = CloseDialogAndHandleSolveConflictResult;
            solveConflictUC.DataContext = vm;
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(solveConflictUC);
        }

        private void CloseDialogAndHandleSolveConflictResult(SolveConflictResult result)
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
        }

        private bool CanDelete()
        {
            return _selectedClassGroupModel != null;
        }

        private void OnCopyCode()
        {
            string registerCode = _selectedClassGroupModel.RegisterCode;
            Clipboard.SetData(DataFormats.Text, registerCode);
            string message = $"Đã copy mã của môn {_selectedClassGroupModel.SubjectCode} vào Clipboard";
            _snackbarMessageQueue.Enqueue(message);
        }

        private bool CanDeleteAll()
        {
            return ClassGroupModels.Count > 0;
        }

        private void OnDeleteAll()
        {
            ClassGroupModels.Clear();
            UpdateConflictModelCollection();
            UpdatePlaceConflictCollection();
            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            MessageBus.Default.Publish(new ChoicesChangedMessage(ClassGroupModels.ToList()));
        }

        private void OnDelete()
        {
            ClassGroupModels.Remove(_selectedClassGroupModel);
            SaveCommand.NotifyCanExecuteChanged();
            DeleteAllCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
            UpdateConflictModelCollection();
            UpdatePlaceConflictCollection();
            MessageBus.Default.Publish(new DeleteClassGroupChoiceMessage(ClassGroupModels.ToList()));
        }

        private bool CanSave()
        {
            return ClassGroupModels.Count > 0;
        }

        private readonly SaveSessionUC _saveSessionUC = new();
        private async Task OpenSaveDialog()
        {
            SaveSessionViewModel vm = _saveSessionUC.DataContext as SaveSessionViewModel;
            vm.ClassGroupModels = ClassGroupModels.ToList();
            vm.CloseDialogCallback = CloseDialogAndHandleSaveResult;
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(_saveSessionUC);
            await vm.LoadScheduleSessions();
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

        public void Handle(ClassGroupAddedMessage message)
        {
            ClassGroupModel classGroupModel = message.Source;

            if (classGroupModel != null)
            {
                // Nếu có sẵn một ClassGroupModel cùng SubjectModel với ClassGroupModel
                // được truyền vào => thay thế chúng.
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
                if (ClassGroupModels[i].SubjectCode.Equals(classGroupModel.SubjectCode))
                    return i;
            return -1;
        }

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
        }

        /// <summary>
        /// Thực hiện bắt cặp tất cả các ClassGroupModel có 
        /// trong Collection để phát hiện các Conflict Time.
        /// </summary>
        private void UpdateConflictModelCollection()
        {
            ConflictModels.Clear();
            List<SchoolClass> schoolClasses = new List<SchoolClass>();
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
            }

            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    if (schoolClasses[i].ClassGroupName.Equals(schoolClasses[k].ClassGroupName))
                        continue;
                    Conflict conflict = new Conflict(schoolClasses[i], schoolClasses[k]);
                    ConflictTime conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                    {
                        ConflictModel conflictModel = new ConflictModel(conflict);
                        ConflictModels.Add(conflictModel);
                    }
                }
            }
            MessageBus.Default.Publish(new ConflictCollectionChangeMessage(ConflictModels.ToList()));
        }

        private void UpdatePlaceConflictCollection()
        {
            PlaceConflictFinderModels.Clear();
            List<SchoolClass> schoolClasses = new List<SchoolClass>();
            foreach (ClassGroupModel classGroupModel in ClassGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
            }

            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i + 1; k < schoolClasses.Count; ++k)
                {
                    PlaceConflictFinder placeConflict = new PlaceConflictFinder(schoolClasses[i], schoolClasses[k]);
                    ConflictPlace conflictPlace = placeConflict.GetPlaceConflict();
                    if (conflictPlace != null)
                    {
                        PlaceConflictFinderModel placeConflictModel = new PlaceConflictFinderModel(placeConflict);
                        PlaceConflictFinderModels.Add(placeConflictModel);
                    }
                }
            }
            MessageBus.Default.Publish(new PlaceConflictCollectionChangeMessage(PlaceConflictFinderModels.ToList()));
        }

        public void Handle(RemoveAChoiceClassGroupMessage message)
        {
            string classGroupModelName = message.Source;
            for (int i = 0; i < ClassGroupModels.Count; i++)
            {
                if (ClassGroupModels[i].Name == classGroupModelName)
                {
                    ClassGroupModels.RemoveAt(i);
                    break;
                }
            }
            MessageBus.Default.Publish(new DeleteClassGroupChoiceMessage(ClassGroupModels.ToList()));
            string snackMessage = $"Đã bỏ chọn lớp {classGroupModelName}";
            _snackbarMessageQueue.Enqueue(snackMessage);
            UpdateConflictModelCollection();
            UpdatePlaceConflictCollection();
        }
    }
}
