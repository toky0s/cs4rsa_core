using System.ComponentModel;
using System;
using cs4rsa_core.BaseClasses;
using cs4rsa_core.Messages;
using LightMessageBus;
using Microsoft.Toolkit.Mvvm.Input;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.DataProviders;
using cs4rsa_core.Settings;
using CourseSearchService.Crawlers.Interfaces;
using DisciplineCrawlerService.Crawlers;
using cs4rsa_core.Settings.Interfaces;

namespace cs4rsa_core.Dialogs.Implements
{
    public class UpdateViewModel : ViewModelBase
    {
        public RelayCommand StartUpdateCommand { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }

        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }

        public Action CloseDialogCallback { get; set; }
        private readonly IDisciplineRepository _disciplineRepository;
        private readonly Cs4rsaDbContext _cs4rsaDbContext;
        private readonly ICourseCrawler _courseCrawler;
        private readonly ISetting _setting;
        private readonly DisciplineCrawler _disciplineCrawler;
        public UpdateViewModel(IDisciplineRepository disciplineRepository, Cs4rsaDbContext cs4rsaDbContext, 
            ICourseCrawler courseCrawler, ISetting setting, DisciplineCrawler disciplineCrawler)
        {
            _disciplineRepository = disciplineRepository;
            _cs4rsaDbContext = cs4rsaDbContext;
            _courseCrawler = courseCrawler;
            _setting = setting;
            _disciplineCrawler = disciplineCrawler;
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
            // đảm bảo ràng buộc toàn vẹn khi xoá, không thay đổi vị trí hai lệnh này.
            //Cs4rsaDataEdit.DeleteDataInTableKeyword();
            //Cs4rsaDataEdit.DeleteDataInTableDiscipline();
            _disciplineRepository.RemoveRange(_cs4rsaDbContext.Disciplines);


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

            _setting.CurrentSetting.CurrentSemesterValue = _courseCrawler.GetCurrentSemesterValue();
            _setting.CurrentSetting.CurrentYearValue = _courseCrawler.GetCurrentYearValue();
            _setting.Save();
            string message = $"Hoàn tất cập nhật {result} môn";
            MessageBus.Default.Publish<UpdateSuccessMessage>(new UpdateSuccessMessage(null));
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
            int numberOfSubjects = _disciplineCrawler.GetNumberOfSubjects();
            _disciplineCrawler.GetDisciplineAndKeyword(backgroundWorker, numberOfSubjects);
            e.Result = numberOfSubjects;
        }
    }
}
