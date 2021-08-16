using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace cs4rsa.Dialogs.Implements
{
    public class AutoSortSubjectLoadVM : ViewModelBase
    {
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

        private List<ProgramSubjectModel> _programSubjectModels = new List<ProgramSubjectModel>();
        public List<ProgramSubjectModel> ProgramSubjectModels {
            get
            {
                return _programSubjectModels;
            }
            set
            {
                _programSubjectModels = value;
            }
        }

        public Action<List<SubjectModel>> CloseDialogCallback { get; set; }

        private bool _isRemoveClassGroupInvalid;

        public bool IsRemoveClassGroupInvalid
        {
            get { return _isRemoveClassGroupInvalid; }
            set { _isRemoveClassGroupInvalid = value; }
        }

        public void Download()
        {
            List<string> courseIds = _programSubjectModels.Select(item => item.CourseId).ToList();
            BackgroundWorker backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            Tuple<List<string>, bool> tuplePara = Tuple.Create(courseIds, _isRemoveClassGroupInvalid);
            backgroundWorker.RunWorkerAsync(tuplePara);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CloseDialogCallback.Invoke(e.Result as List<SubjectModel>);
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressValue = e.ProgressPercentage;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int progress = 0;
            BackgroundWorker worker = sender as BackgroundWorker;
            Tuple<List<string>, bool> para = e.Argument as Tuple<List<string>, bool>;

            List<string> courseIds = para.Item1 as List<string>;
            bool isRemoveClassGroupInvalid = (bool)para.Item2;

            List<Subject> subjects = new List<Subject>();
            progress = 10;
            worker.ReportProgress(progress);
            foreach (string courseId in courseIds)
            {
                SubjectCrawler subjectCrawler = new SubjectCrawler(courseId);
                Subject subject = subjectCrawler.ToSubject();
                subjects.Add(subject);
                progress += (100 - 10) / courseId.Count();
                worker.ReportProgress(progress);
            }
            List<SubjectModel> subjectModels = subjects.Select(item => new SubjectModel(item)).ToList();
            e.Result = subjectModels;
            worker.ReportProgress(100);
        }
    }
}
