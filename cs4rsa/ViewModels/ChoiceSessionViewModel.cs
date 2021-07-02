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

namespace cs4rsa.ViewModels
{
    class ChoiceSessionViewModel : NotifyPropertyChangedBase,
        IMessageHandler<ClassGroupAddedMessage>,
        IMessageHandler<DeleteSubjectMessage>
    {
        private ObservableCollection<ClassGroupModel> classGroupModels = new ObservableCollection<ClassGroupModel>();
        public ObservableCollection<ClassGroupModel> ClassGroupModels
        {
            get
            {
                return classGroupModels;
            }
            set
            {
                classGroupModels = value;
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
            }
        }

        private ObservableCollection<IConflictModel> conflictModels = new ObservableCollection<IConflictModel>();
        public ObservableCollection<IConflictModel> ConflictModels
        {
            get
            {
                return conflictModels;
            }
            set
            {
                conflictModels = value;
            }
        }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }

        public ChoiceSessionViewModel()
        {
            MessageBus.Default.FromAny().Where<ClassGroupAddedMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteSubjectMessage>().Notify(this);
            SaveCommand = new RelayCommand(OpenSaveDialog, CanSave);
            DeleteCommand = new RelayCommand(OnDelete);
        }

        private void OnDelete(object obj)
        {
            classGroupModels.Remove(_selectedClassGroupModel);
            UpdateConflictModelCollection();
            MessageBus.Default.Publish(new DeleteClassGroupChoiceMessage(classGroupModels.ToList()));
        }

        private bool CanSave()
        {
            return classGroupModels.Count > 0;
        }

        private void OpenSaveDialog(object parameter)
        {
            Cs4rsaMessageBox errorMessageBox = new Cs4rsaMessageBox();
            SaveDialogViewModel saveDialogViewModel = new SaveDialogViewModel(errorMessageBox, classGroupModels.ToList());
            SaveDialogWindow dialogWindow = new SaveDialogWindow();
            SaveResult result = DialogService<SaveResult>.OpenDialog(saveDialogViewModel, dialogWindow, parameter as Window);
        }

        public void Handle(ClassGroupAddedMessage message)
        {
            ClassGroupModel classGroupModel = message.Source;
            
            if (classGroupModel != null)
            {
                int ClassGroupModelIndex = IsReallyHaveAnotherVersionInChoicedList(classGroupModel);
                if (ClassGroupModelIndex != -1)
                    classGroupModels[ClassGroupModelIndex] = classGroupModel;
                else
                    classGroupModels.Add(classGroupModel);
            }
            UpdateConflictModelCollection();
            SaveCommand.RaiseCanExecuteChanged();
            MessageBus.Default.Publish(new ChoicesChangedMessage(classGroupModels.ToList()));
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
            for (int i = 0; i < classGroupModels.Count; ++i)
                if (classGroupModels[i].SubjectCode.Equals(classGroupModel.SubjectCode))
                    return i;
            return -1;
        }

        public void Handle(DeleteSubjectMessage message)
        {
            SubjectModel subjectModel = message.Source;
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                if (classGroupModel.SubjectCode.Equals(subjectModel.SubjectCode))
                {
                    classGroupModels.Remove(classGroupModel);
                    break;
                }
            }
            UpdateConflictModelCollection();
            SaveCommand.RaiseCanExecuteChanged();
            MessageBus.Default.Publish(new ChoicesChangedMessage(classGroupModels.ToList()));
        }

        /// <summary>
        /// Thực hiện bắt cặp tất cả các ClassGroupModel có 
        /// trong Collection để phát hiện các Conflict.
        /// </summary>
        private void UpdateConflictModelCollection()
        {
            conflictModels.Clear();
            for (int i = 0; i < classGroupModels.Count; ++i)
            {
                for (int k = i+1; k < classGroupModels.Count; ++k)
                {
                    Conflict conflict = new Conflict(classGroupModels[i], classGroupModels[k]);
                    ConflictTime conflictTime = conflict.GetConflictTime();
                    if (conflictTime != null)
                    {
                        IConflictModel conflictModel = new ConflictModel(conflict);
                        conflictModels.Add(conflictModel);
                    }
                    PlaceConflictFinder placeConflict = new PlaceConflictFinder(classGroupModels[i], classGroupModels[k]);
                    ConflictPlace conflictPlace = placeConflict.GetPlaceConflict();
                    if (conflictPlace != null)
                    {
                        IConflictModel placeConflictModel = new PlaceConflictFinderModel(placeConflict);
                        conflictModels.Add(placeConflictModel);
                    }
                }
            }
            MessageBus.Default.Publish(new ConflictCollectionChangeMessage(conflictModels.ToList()));
        }
    }
}
