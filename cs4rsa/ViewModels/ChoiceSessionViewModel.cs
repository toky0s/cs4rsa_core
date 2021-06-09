﻿using cs4rsa.BaseClasses;
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

namespace cs4rsa.ViewModels
{
    class ChoiceSessionViewModel : NotifyPropertyChangedBase,
        IMessageHandler<ClassGroupAddedMessage>,
        IMessageHandler<DeleteSubjectMessage>
    {

        private bool _canSave;
        public bool CanSave
        {
            get
            {
                return _canSave;
            }
            set
            {
                _canSave = value;
                RaisePropertyChanged();
            }
        }

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

        private ObservableCollection<ConflictModel> conflictModels = new ObservableCollection<ConflictModel>();
        public ObservableCollection<ConflictModel> ConflictModels
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

        public ChoiceSessionViewModel()
        {
            MessageBus.Default.FromAny().Where<ClassGroupAddedMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteSubjectMessage>().Notify(this);
            SaveCommand = new RelayCommand(OpenSaveDialog, () => true);
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
            CanNewSaveChange();
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
            CanNewSaveChange();
            MessageBus.Default.Publish(new ChoicesChangedMessage(classGroupModels.ToList()));
        }

        /// <summary>
        /// Thực hiện bắt cặp tất cả các ClassGroupModel có 
        /// trong Collection để phát hiện các Conflict.
        /// </summary>
        /// <returns>Danh sách các Conflict</returns>
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
                        ConflictModel conflictModel = new ConflictModel(conflict, conflictTime);
                        conflictModels.Add(conflictModel);
                    }
                }
            }
        }

        /// <summary>
        /// Kiểm tra lại xem nút Lưu Mới có thể nhấn được hay không.
        /// </summary>
        private void CanNewSaveChange()
        {
            CanSave = classGroupModels.Count > 0;
        }

        public string GetShareString()
        {
            HomeCourseSearch homeCourseSearch = new HomeCourseSearch();
            string subjectCode = "";
            string registerCode = "";
            string cs4rsaHashCode = "";
            string replaceChar = "%";
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                subjectCode = classGroupModel.SubjectCode;
                registerCode = classGroupModel.RegisterCode;
                cs4rsaHashCode = cs4rsaHashCode + subjectCode + replaceChar + registerCode;
            }
            return StringHelper.SuperCleanString($"#cs4rsa!{homeCourseSearch.CurrentSemesterValue}!{classGroupModels.Count}!{cs4rsaHashCode}#cs4rsa");
        }
    }
}
