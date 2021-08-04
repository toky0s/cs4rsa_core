using System.ComponentModel;
using System.Windows;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using cs4rsa.BaseClasses;
using cs4rsa.Models;
using cs4rsa.Messages;
using cs4rsa.Crawler;
using cs4rsa.Database;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.BasicData;
using LightMessageBus;

namespace cs4rsa.Dialogs.Implements
{
    public class UpdateViewModel : ViewModelBase
    {
        public RelayCommand StartUpdateCommand { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }

        private int _progressValue;
        public int ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }

        public Action CloseDialogCallback { get; set; }
        public UpdateViewModel()
        {
            StartUpdateCommand = new RelayCommand(OnStartUpdate);
            CloseDialogCommand = new RelayCommand(OnCloseDialog, CanClose);
        }

        private bool CanClose()
        {
            return _progressValue == 0;
        }

        private void OnCloseDialog(object obj)
        {
            CloseDialogCallback.Invoke();
        }

        private void OnStartUpdate(object obj)
        {
            CloseDialogCommand.RaiseCanExecuteChanged();
            // đảm bảo ràng buộc toàn vẹn khi xoá, không thay đổi vị trí hai lệnh này.
            Cs4rsaDataEdit.DeleteDataInTableKeyword();
            Cs4rsaDataEdit.DeleteDataInTableDiscipline();

            BackgroundWorker backgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true
            };
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressValue = 1000;
            int result = (int)e.Result;
            string message = $"Hoàn tất cập nhật {result} môn";
            MessageBus.Default.Publish(new Cs4rsaSnackbarMessage(message));
            CloseDialogCallback.Invoke();
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressValue = e.ProgressPercentage;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker backgroundWorker = sender as BackgroundWorker;
            DisciplineData disciplineData = new DisciplineData();
            int numberOfSubjects = DisciplineData.GetNumberOfSubjects();
            disciplineData.GetDisciplineAndKeywordDatabase(backgroundWorker, numberOfSubjects);
            e.Result = numberOfSubjects;
        }
    }
}
