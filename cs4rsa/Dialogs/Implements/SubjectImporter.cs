using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Database;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace cs4rsa.Dialogs.Implements
{
    /// <summary>
    /// Bộ ra lệnh cho SearchViewModel thực hiện import các Subject được truyền vào.
    /// </summary>
    class SubjectImporter : DialogViewModelBase<ImportResult>
    {

        private ObservableCollection<SubjectInfoData> _subjectInfoDatas = new ObservableCollection<SubjectInfoData>();
        public ObservableCollection<SubjectInfoData> SubjectInfoDatas
        {
            get
            {
                return _subjectInfoDatas;
            }
            set
            {
                _subjectInfoDatas = value;
            }
        }

        private int _progress = 0;
        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        private BackgroundWorker _backgroundWorker;

        private List<SubjectCrawler> _subjectCrawlers;

        public List<SubjectCrawler> SubjectCrawlers
        {
            get { return _subjectCrawlers; }
            set { _subjectCrawlers = value; }
        }

        private SessionManagerResult _sessionManagerResult;

        public SessionManagerResult SessionManagerResult
        {
            get { return _sessionManagerResult; }
            set
            {
                _sessionManagerResult = value;
                foreach (SubjectInfoData item in _sessionManagerResult.SubjectInfoDatas)
                {
                    SubjectInfoDatas.Add(item);
                }

                List<string> _courseIds = _sessionManagerResult.SubjectInfoDatas
                                                .Select(item => Cs4rsaDataView.GetCourseId(item.SubjectCode))
                                                .ToList();
                _subjectCrawlers = _courseIds.Select(item => new SubjectCrawler(item)).ToList();
                Run(_subjectCrawlers);
            }
        }

        public Action<ImportResult, SessionManagerResult> CloseDialogCallback { get; set; }

        private void Run(List<SubjectCrawler> subjectCrawlers)
        {
            _backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _backgroundWorker.DoWork += BackgroundWorker_DoWork; ;
            _backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += WorkerComplete;
            _backgroundWorker.RunWorkerAsync(subjectCrawlers);
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress += e.ProgressPercentage;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker backgroundWorker = sender as BackgroundWorker;
            List<SubjectCrawler> subjectCrawlers = (List<SubjectCrawler>)e.Argument;
            int ProgressCompleteRange = 100 / subjectCrawlers.Count;
            List<SubjectModel> subjectModels = new List<SubjectModel>();
            foreach (SubjectCrawler crawler in subjectCrawlers)
            {
                backgroundWorker.ReportProgress(ProgressCompleteRange / 2);
                Subject subject = crawler.ToSubject();
                SubjectModel subjectModel = new SubjectModel(subject);
                subjectModels.Add(subjectModel);
                backgroundWorker.ReportProgress(ProgressCompleteRange / 2);
            }
            e.Result = subjectModels;
        }

        private void WorkerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Progress = 100;
            List<SubjectModel> subjectModels = e.Result as List<SubjectModel>;
            foreach (SubjectModel subject in subjectModels)
            {
                subject.Color = ColorGenerator.GetColor(subject.CourseId);
            }
            ImportResult importResult = new ImportResult() { SubjectModels = subjectModels };
            CloseDialogCallback.Invoke(importResult, _sessionManagerResult);
        }
    }
}
