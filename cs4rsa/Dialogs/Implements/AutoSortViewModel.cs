using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Models;
using cs4rsa.Crawler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;

namespace cs4rsa.Dialogs.Implements
{
    public class AutoSortViewModel: DialogViewModelBase<AutoSortResult>
    {
        private List<ProgramSubjectModel> _programSubjectModels;
        public AutoSortViewModel(List<ProgramSubjectModel> programSubjectModels)
        {
            _programSubjectModels = programSubjectModels;
            Sort();
        }

        private void Sort()
        {
            List<string> courseIds = _programSubjectModels.Select(item => item.CourseId).ToList();
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync(courseIds);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<Subject> subjects = e.Result as List<Subject>;

        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> courseIds = e.Argument as List<string>;
            List<Subject> subjects = new List<Subject>();
            foreach (string courseId in courseIds)
            {
                SubjectCrawler subjectCrawler = new SubjectCrawler(courseId);
                Subject subject = subjectCrawler.ToSubject();
                subjects.Add(subject);
            }
            e.Result = subjects;
        }
    }
}
