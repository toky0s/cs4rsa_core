using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Messages;
using cs4rsa.Models;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.Implements;
using cs4rsa.Helpers;
using cs4rsa.Crawler;
using LightMessageBus;
using LightMessageBus.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Windows;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Dialogs.DialogViews;
using cs4rsa.Models.Interfaces;
using System.Collections.Generic;

namespace cs4rsa.ViewModels
{
    class ChoiceSessionViewModel : ViewModelBase,
        IMessageHandler<ClassGroupAddedMessage>,
        IMessageHandler<DeleteSubjectMessage>,
        IMessageHandler<RemoveAChoiceClassGroupMessage>
    {
        private ObservableCollection<ClassGroupModel> _classGroupModels = new ObservableCollection<ClassGroupModel>();
        public ObservableCollection<ClassGroupModel> ClassGroupModels
        {
            get
            {
                return _classGroupModels;
            }
            set
            {
                _classGroupModels = value;
            }
        }

        private ClassGroupModel _selectedClassGroupModel;
        public ClassGroupModel SelectedClassGroupModel
        {
            get
            {
                return _selectedClassGroupModel;
            }
            set
            {
                _selectedClassGroupModel = value;
                OnPropertyChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<ConflictModel> _conflictModels = new ObservableCollection<ConflictModel>();
        public ObservableCollection<ConflictModel> ConflictModels
        {
            get
            {
                return _conflictModels;
            }
            set
            {
                _conflictModels = value;
            }
        }

        private ConflictModel _selectedConflictModel;
        public ConflictModel SelectedConflictModel
        {
            get { return _selectedConflictModel; }
            set { _selectedConflictModel = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PlaceConflictFinderModel> _placeConflictFinderModels = new ObservableCollection<PlaceConflictFinderModel>();
        public ObservableCollection<PlaceConflictFinderModel> PlaceConflictFinderModels
        {
            get
            {
                return _placeConflictFinderModels;
            }
            set
            {
                _placeConflictFinderModels = value;
            }
        }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteAllCommand { get; set; }
        public RelayCommand CopyCodeCommand { get; set; }
        public RelayCommand SolveConflictCommand { get; set; }

        public ChoiceSessionViewModel()
        {
            MessageBus.Default.FromAny().Where<ClassGroupAddedMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteSubjectMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<RemoveAChoiceClassGroupMessage>().Notify(this);
            SaveCommand = new RelayCommand(OpenSaveDialog, CanSave);
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            DeleteAllCommand = new RelayCommand(OnDeleteAll, CanDeleteAll);
            CopyCodeCommand = new RelayCommand(OnCopyCode);
            SolveConflictCommand = new RelayCommand(OnSolve);
        }


        /// <summary>
        /// Giải quyết xung đột
        /// </summary>
        /// <param name="obj"></param>
        private void OnSolve(object obj)
        {
            SolveConflictUC solveConflictUC = new SolveConflictUC();
            SolveConflictViewModel vm = new SolveConflictViewModel(_selectedConflictModel);
            vm.CloseDialogCallback = CloseDialogAndHandleSolveConflictResult;
            solveConflictUC.DataContext = vm;
            (App.Current.MainWindow.DataContext as MainViewModel).OpenDialog(solveConflictUC);
        }

        private void CloseDialogAndHandleSolveConflictResult(SolveConflictResult result)
        {
            (App.Current.MainWindow.DataContext as MainViewModel).CloseDialog();
        }

        private bool CanDelete()
        {
            return _selectedClassGroupModel != null;
        }

        private void OnCopyCode(object obj)
        {
            string registerCode = _selectedClassGroupModel.RegisterCode;
            Clipboard.SetData(DataFormats.Text, registerCode);
            string message = $"Đã copy mã của môn {_selectedClassGroupModel.SubjectCode} vào Clipboard";
            MessageBus.Default.Publish(new Cs4rsaSnackbarMessage(message));
        }

        private bool CanDeleteAll()
        {
            return _classGroupModels.Count > 0;
        }

        private void OnDeleteAll(object obj)
        {
            _classGroupModels.Clear();
            UpdateConflictModelCollection();
            UpdatePlaceConflictCollection();
            SaveCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
            MessageBus.Default.Publish(new ChoicesChangedMessage(_classGroupModels.ToList()));
        }

        private void OnDelete(object obj)
        {
            _classGroupModels.Remove(_selectedClassGroupModel);
            SaveCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            UpdateConflictModelCollection();
            MessageBus.Default.Publish(new DeleteClassGroupChoiceMessage(_classGroupModels.ToList()));
        }

        private bool CanSave()
        {
            return _classGroupModels.Count > 0;
        }

        private void OpenSaveDialog(object parameter)
        {
            SaveSessionUC saveSessionUC = new SaveSessionUC();
            SaveDialogViewModel vm = saveSessionUC.DataContext as SaveDialogViewModel;
            vm.ClassGroupModels = _classGroupModels.ToList();
            vm.CloseDialogCallback = CloseDialogAndHandleSaveResult;
            (App.Current.MainWindow.DataContext as MainViewModel).OpenDialog(saveSessionUC);
        }

        private void CloseDialogAndHandleSaveResult(SaveResult result)
        {
            (App.Current.MainWindow.DataContext as MainViewModel).CloseDialog();
            if (result != null)
            {
                string message = $"Đã lưu phiên hiện tại với tên {result.Name}";
                MessageBus.Default.Publish(new Cs4rsaSnackbarMessage(message));
            }
        }

        public void Handle(ClassGroupAddedMessage message)
        {
            ClassGroupModel classGroupModel = message.Source;
            
            if (classGroupModel != null)
            {
                // Nếu có sẵn một ClassGroupModel cùng SubjectModel với ClassGroupModel được truyền vào
                // thay thế chúng.
                int ClassGroupModelIndex = IsReallyHaveAnotherVersionInChoicedList(classGroupModel);
                if (ClassGroupModelIndex != -1)
                    _classGroupModels[ClassGroupModelIndex] = classGroupModel;
                else
                    _classGroupModels.Add(classGroupModel);
            }

            UpdateConflictModelCollection();
            UpdatePlaceConflictCollection();

            SaveCommand.RaiseCanExecuteChanged();
            DeleteAllCommand.RaiseCanExecuteChanged();
            MessageBus.Default.Publish(new ChoicesChangedMessage(_classGroupModels.ToList()));
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
            for (int i = 0; i < _classGroupModels.Count; ++i)
                if (_classGroupModels[i].SubjectCode.Equals(classGroupModel.SubjectCode))
                    return i;
            return -1;
        }

        public void Handle(DeleteSubjectMessage message)
        {
            SubjectModel subjectModel = message.Source;
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                if (classGroupModel.SubjectCode.Equals(subjectModel.SubjectCode))
                {
                    _classGroupModels.Remove(classGroupModel);
                    break;
                }
            }
            UpdateConflictModelCollection();
            UpdatePlaceConflictCollection();
            SaveCommand.RaiseCanExecuteChanged();
            MessageBus.Default.Publish(new ChoicesChangedMessage(_classGroupModels.ToList()));
        }

        /// <summary>
        /// Thực hiện bắt cặp tất cả các ClassGroupModel có 
        /// trong Collection để phát hiện các Conflict Time.
        /// </summary>
        private void UpdateConflictModelCollection()
        {
            _conflictModels.Clear();
            List<SchoolClass> schoolClasses = new List<SchoolClass>();
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                schoolClasses.AddRange(classGroupModel.ClassGroup.SchoolClasses);
            }

            for (int i = 0; i < schoolClasses.Count; ++i)
            {
                for (int k = i+1; k < schoolClasses.Count; ++k)
                {
                    if (schoolClasses[i].ClassGroupName.Equals(schoolClasses[k].ClassGroupName))
                        continue;
                    Conflict conflict = new Conflict(schoolClasses[i], schoolClasses[k]);
                    ConflictTime conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                    {
                        ConflictModel conflictModel = new ConflictModel(conflict);
                        _conflictModels.Add(conflictModel);
                    }
                }
            }
            MessageBus.Default.Publish(new ConflictCollectionChangeMessage(_conflictModels.ToList()));
        }

        private void UpdatePlaceConflictCollection()
        {
            _placeConflictFinderModels.Clear();
            List<SchoolClass> schoolClasses = new List<SchoolClass>();
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
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
                        _placeConflictFinderModels.Add(placeConflictModel);
                    }
                }
            }
            MessageBus.Default.Publish(new PlaceConflictCollectionChangeMessage(_placeConflictFinderModels.ToList()));
        }

        public void Handle(RemoveAChoiceClassGroupMessage message)
        {
            string classGroupModelName = message.Source;
            for (int i = 0; i < _classGroupModels.Count; i++)
            {
                if (_classGroupModels[i].Name == classGroupModelName)
                {
                    _classGroupModels.RemoveAt(i);
                    break;
                }
            }
            MessageBus.Default.Publish<DeleteClassGroupChoiceMessage>(new DeleteClassGroupChoiceMessage(_classGroupModels.ToList()));
            string snackMessage = $"Đã bỏ chọn lớp {classGroupModelName}";
            MessageBus.Default.Publish<Cs4rsaSnackbarMessage>(new Cs4rsaSnackbarMessage(snackMessage));
            UpdateConflictModelCollection();
            UpdatePlaceConflictCollection();
        }
    }
}
