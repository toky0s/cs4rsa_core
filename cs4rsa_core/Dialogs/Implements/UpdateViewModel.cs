using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.DataProviders;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Services.DisciplineCrawlerSvc.Crawlers;
using Cs4rsa.Settings.Interfaces;
using Cs4rsa.ViewModels;

using MaterialDesignThemes.Wpf;

using System.ComponentModel;
using System.Windows;

namespace Cs4rsa.Dialogs.Implements
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

        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly Cs4rsaDbContext _cs4rsaDbContext;
        private readonly ICourseCrawler _courseCrawler;
        private readonly ISetting _setting;
        private readonly DisciplineCrawler _disciplineCrawler;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion
        public UpdateViewModel(
            IUnitOfWork unitOfWork,
            ICourseCrawler courseCrawler,
            ISetting setting,
            ISnackbarMessageQueue snackbarMessageQueue,
            Cs4rsaDbContext cs4rsaDbContext,
            DisciplineCrawler disciplineCrawler
        )
        {
            _cs4rsaDbContext = cs4rsaDbContext;
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _setting = setting;
            _disciplineCrawler = disciplineCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;

            StartUpdateCommand = new RelayCommand(OnStartUpdate);
            CloseDialogCommand = new RelayCommand(CloseDialog, CanClose);
        }

        private bool CanClose()
        {
            return _progressValue == 0;
        }

        private void OnStartUpdate()
        {
            CloseDialogCommand.NotifyCanExecuteChanged();
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).IsCloseOnClickAway = false;
            _unitOfWork.Disciplines.RemoveRange(_cs4rsaDbContext.Disciplines);

            BackgroundWorker backgroundWorker = new()
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
            _setting.CurrentSetting.CurrentSemesterValue = _courseCrawler.GetCurrentSemesterValue();
            _setting.CurrentSetting.CurrentYearValue = _courseCrawler.GetCurrentYearValue();
            _setting.Save();
            CloseDialog();
            Messenger.Send(new UpdateVmMsgs.UpdateSuccessMsg(null));
            short result = (short)e.Result;
            string message = $"Hoàn tất cập nhật {result} môn";
            _snackbarMessageQueue.Enqueue(message);
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
