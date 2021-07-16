using cs4rsa.Crawler;
using cs4rsa.Database;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.MessageBoxService;
using System.ComponentModel;
using System.Windows;
using LightMessageBus;
using cs4rsa.Messages;

namespace cs4rsa.Dialogs.Implements
{
    class UpdateViewModel : DialogViewModelBase<UpdateResult>
    {
        public RelayCommand StartUpdateCommand { get; set; }
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
                RaisePropertyChanged();
            }
        }
        private IMessageBox _messageBox;

        public UpdateViewModel(IMessageBox messageBox)
        {
            _messageBox = messageBox;
            StartUpdateCommand = new RelayCommand(OnStartUpdate);
        }

        private void OnStartUpdate(object obj)
        {
            // đảm bảo ràng buộc toàn vẹn khi xoá, không thay đổi vị trí hai lệnh này.
            Cs4rsaDataEdit.DeleteDataInTableKeyword();
            Cs4rsaDataEdit.DeleteDataInTableDiscipline();
            // chia thread cập nhật dưới luồng
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
            CloseDialogWithResult(UpdateResult.Success);
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
