using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Dialogs.Implements
{
    public class ProSubjectLoadViewModel: ViewModelBase
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

        private string _specialString;
        public string SpecialString
        {
            get { return _specialString; }
            set { _specialString = value; }
        }

        public Action<ProSubjectLoadResult> CloseDialogCallback { get; set; }

        public ProSubjectLoadViewModel()
        {
        }


        /// <summary>
        /// Tải danh sách các môn thuộc chương trình học của sinh viên.
        /// </summary>
        public void Load()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync(_specialString);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgramDiagram programDiagram = e.Result as ProgramDiagram;
            ProSubjectLoadResult result = new ProSubjectLoadResult(programDiagram);
            CloseDialogCallback.Invoke(result);
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker backgroundWorker = sender as BackgroundWorker;
            string specialString = (string)e.Argument;
            backgroundWorker.ReportProgress(20);
            ProgramDiagramCrawler programDiagramCrawler = new ProgramDiagramCrawler(null, specialString, backgroundWorker);
            ProgramDiagram result = programDiagramCrawler.ToProgramDiagram();
            e.Result = result;
            backgroundWorker.ReportProgress(100);
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressValue = e.ProgressPercentage;
        }
    }
}
