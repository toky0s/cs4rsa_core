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
    public class ProSubjectLoadViewModel: DialogViewModelBase<ProSubjectLoadResult>
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

        public ProSubjectLoadViewModel(string specialString)
        {
            _specialString = specialString;
            Load(specialString);
        }


        private void Load(string specialString)
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync(specialString);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgramDiagram programDiagram = e.Result as ProgramDiagram;
            ProSubjectLoadResult result = new ProSubjectLoadResult(programDiagram);
            CloseDialogWithResult(result);
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
