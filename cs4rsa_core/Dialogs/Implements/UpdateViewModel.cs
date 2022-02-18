using CourseSearchService.Crawlers.Interfaces;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Messages;
using cs4rsa_core.Settings.Interfaces;
using cs4rsa_core.ViewModels;
using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Interfaces;
using DisciplineCrawlerService.Crawlers;
using LightMessageBus;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows;

namespace cs4rsa_core.Dialogs.Implements
{
    public class UpdateViewModel : ViewModelBase
    {
        public RelayCommand StartUpdateCommand { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }

        private short _progressValue;
        public short ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }

        public Action CloseDialogCallback { get; set; }

        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly Cs4rsaDbContext _cs4rsaDbContext;
        private readonly ICourseCrawler _courseCrawler;
        private readonly ISetting _setting;
        private readonly DisciplineCrawler _disciplineCrawler;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion
        public UpdateViewModel(IUnitOfWork unitOfWork, Cs4rsaDbContext cs4rsaDbContext,
            ICourseCrawler courseCrawler, ISetting setting, DisciplineCrawler disciplineCrawler,
            ISnackbarMessageQueue snackbarMessageQueue)
        {
            _cs4rsaDbContext = cs4rsaDbContext;
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _setting = setting;
            _disciplineCrawler = disciplineCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;

            StartUpdateCommand = new RelayCommand(OnStartUpdate);
            CloseDialogCommand = new RelayCommand(OnCloseDialog, CanClose);
        }

        private bool CanClose()
        {
            return _progressValue == 0;
        }

        private void OnCloseDialog()
        {
            CloseDialogCallback.Invoke();
        }

        private void OnStartUpdate()
        {
            CloseDialogCommand.NotifyCanExecuteChanged();
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).IsCloseOnClickAway = false;
            _unitOfWork.Disciplines.RemoveRange(_cs4rsaDbContext.Disciplines);

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
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).IsCloseOnClickAway = true;
            short result = (short)e.Result;

            _setting.CurrentSetting.CurrentSemesterValue = _courseCrawler.GetCurrentSemesterValue();
            _setting.CurrentSetting.CurrentYearValue = _courseCrawler.GetCurrentYearValue();
            _setting.Save();
            string message = $"Hoàn tất cập nhật {result} môn";
            MessageBus.Default.Publish(new UpdateSuccessMessage(null));
            _snackbarMessageQueue.Enqueue(message);
            CloseDialogCallback.Invoke();
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressValue = (short)e.ProgressPercentage;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker backgroundWorker = sender as BackgroundWorker;
            short numberOfSubjects = _disciplineCrawler.GetNumberOfSubjects();
            _disciplineCrawler.GetDisciplineAndKeyword(backgroundWorker, numberOfSubjects);
            e.Result = numberOfSubjects;
        }
    }
}
