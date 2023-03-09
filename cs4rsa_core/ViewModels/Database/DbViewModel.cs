using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.CourseSearchSvc.Crawlers;
using Cs4rsa.Services.DisciplineCrawlerSvc.Crawlers;
using Cs4rsa.Settings.Interfaces;
using Cs4rsa.Utils.Interfaces;

using MaterialDesignThemes.Wpf;

using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;


namespace Cs4rsa.ViewModels.Database
{
    internal partial class DbViewModel : ViewModelBase, IScreenViewModel
    {
        [ObservableProperty]
        private string _currentSemesterInf;

        [ObservableProperty]
        private string _currentYearInf;

        [ObservableProperty]
        private long _subjectQuantity;

        [ObservableProperty]
        private int _progressValue;

        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand StartUpdateCommand { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        private readonly CourseCrawler _courseCrawler;
        private readonly ISetting _setting;
        private readonly DisciplineCrawler _disciplineCrawler;
        private readonly IFolderManager _folderManager;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;

        public DbViewModel(
            IUnitOfWork unitOfWork,
            CourseCrawler courseCrawler,
            ISetting setting,
            ISnackbarMessageQueue snackbarMessageQueue,
            IFolderManager folderManager,
            DisciplineCrawler disciplineCrawler
        )
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _setting = setting;
            _disciplineCrawler = disciplineCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;
            _folderManager = folderManager;

            StartUpdateCommand = new RelayCommand(OnStartUpdate);
            RefreshCommand = new(OnRefresh);
        }


        /// <summary>
        /// Khởi tạo infor màn hình.
        /// </summary>
        private void LoadInf()
        {
            ProgressValue = 0;
            SubjectQuantity = _unitOfWork.Keywords.Count();
            CurrentSemesterInf = _courseCrawler.CurrentSemesterInfo;
            CurrentYearInf = _courseCrawler.CurrentYearInfo;
        }


        /// <summary>
        /// Cập nhật cơ sở dữ liệu môn học.
        /// 
        /// 1. Start Trans.
        /// 2. Remove Old Data.
        /// 3. Fetch and save new data.
        /// 4. Commit.
        /// </summary>
        private void OnStartUpdate()
        {
            PreventOperation(true);
            BackgroundWorker backgroundWorker = new()
            {
                WorkerReportsProgress = true,
            };
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Gửi Message Refesh tới những thằng fetch dữ liệu từ DB.
        /// </summary>
        private void OnRefresh()
        {
            Messenger.Send(new DbVmMsgs.RefreshMsg());
            LoadInf();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PreventOperation(false);
            if (e.Result is string @message)
            {
                MessageBox.Show(
                    @message
                    , ViewConstants.Screen05.MenuName
                    , MessageBoxButton.OK
                    , MessageBoxImage.Error);
            }
            else if (e.Result is int @result)
            {
                if (result == -1)
                {
                    MessageBox.Show(
                        CredizText.Common001("Cập nhật cơ sở dữ liệu")
                        , ViewConstants.Screen05.MenuName
                        , MessageBoxButton.OK
                        , MessageBoxImage.Error
                    );
                }
                else
                {
                    _setting.CurrentSetting.CurrentSemesterValue = _courseCrawler.CurrentSemesterValue;
                    _setting.CurrentSetting.CurrentYearValue = _courseCrawler.CurrentYearValue;
                    _setting.CurrentSetting.CurrentYear = _courseCrawler.CurrentYearInfo;
                    _setting.CurrentSetting.CurrentSemester = _courseCrawler.CurrentSemesterInfo;
                    _setting.Save();
                    _folderManager.DelAllInThisFolder(Path.Combine(AppContext.BaseDirectory, IFolderManager.FdHtmlCaches));

                    Messenger.Send(new UpdateVmMsgs.UpdateSuccessMsg());
                    Messenger.Send(new DbVmMsgs.RefreshMsg());
                    string msg = CredizText.DbMsg001(result);
                    _snackbarMessageQueue.Enqueue(msg);
                }
            }

            LoadInf();
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressValue = e.ProgressPercentage;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker bw = sender as BackgroundWorker;
                int numberOfSubjects = _disciplineCrawler.GetNumberOfSubjects();
                e.Result = _disciplineCrawler.GetDisciplineAndKeyword(bw, numberOfSubjects);
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
            }
        }

        public void InitData()
        {
            LoadInf();
        }

        public Task InitDataAsync()
        {
            return Task.FromResult<object>(null);
        }
    }
}
