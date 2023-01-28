using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Services.DisciplineCrawlerSvc.Crawlers;
using Cs4rsa.Settings.Interfaces;
using Cs4rsa.Utils.Interfaces;

using MaterialDesignThemes.Wpf;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.ViewModels.Database
{
    internal partial class DbViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _currentSemesterInf;

        [ObservableProperty]
        private string _currentYearInf;

        [ObservableProperty]
        private long _subjectQuantity;

        [ObservableProperty]
        private int _progressValue;

        [ObservableProperty]
        private Discipline _sltDiscipline;

        public ObservableCollection<Discipline> Disciplines { get; set; }
        public ObservableCollection<Keyword> Keywords { get; set; }

        public RelayCommand StartUpdateCommand { get; set; }
        public AsyncRelayCommand<int> ViewCacheCommand { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        private readonly ISetting _setting;
        private readonly IOpenInBrowser _openInBrowser;
        private readonly DisciplineCrawler _disciplineCrawler;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;

        public DbViewModel(
            IUnitOfWork unitOfWork,
            ICourseCrawler courseCrawler,
            ISetting setting,
            IOpenInBrowser openInBrowser,
            ISnackbarMessageQueue snackbarMessageQueue,
            DisciplineCrawler disciplineCrawler
        )
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
            _setting = setting;
            _openInBrowser = openInBrowser;
            _disciplineCrawler = disciplineCrawler;
            _snackbarMessageQueue = snackbarMessageQueue;

            Disciplines = new();
            Keywords = new();

            StartUpdateCommand = new RelayCommand(OnStartUpdate);
            ViewCacheCommand = new(OnViewCache);

            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await Task.WhenAll(LoadInf(), LoadDisciplines());
            });
        }

        partial void OnSltDisciplineChanged(Discipline value)
        {
            if (value != null)
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Keywords.Clear();
                    foreach (Keyword kw in value.Keywords)
                    {
                        Keywords.Add(kw);
                    }
                });
            }
        }

        /// <summary>
        /// Khởi tạo infor màn hình.
        /// </summary>
        private async Task LoadInf()
        {
            Keywords.Clear();
            ProgressValue = 0;
            CurrentSemesterInf = _courseCrawler.GetCurrentSemesterInfo();
            CurrentYearInf = _courseCrawler.GetCurrentYearInfo();
            SubjectQuantity = await _unitOfWork.Keywords.Count();
        }

        private async Task LoadDisciplines()
        {
            IAsyncEnumerable<Discipline> disciplines = _unitOfWork.Disciplines.GetAll();
            await foreach (Discipline dcl in disciplines)
            {
                Disciplines.Add(dcl);
            }
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
        /// Tạo và mở file Cache trong Browser.
        /// </summary>
        /// <param name="courseId">Course ID</param>
        private async Task OnViewCache(int courseId)
        {
            string filePath = CredizText.PathHtmlCacheFile(courseId);
            if (!File.Exists(filePath))
            {
                Keyword kw = Keywords.Where(kw => kw.CourseId == courseId).First();
                await File.WriteAllTextAsync(filePath, kw.Cache);
            }
            _openInBrowser.Open(filePath);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PreventOperation(false);
            if (e.Result is string @message)
            {
                MessageBox.Show(@message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e.Result is int @result)
            {
                if (result == -1)
                {
                    MessageBox.Show(
                        CredizText.Common001("Cập nhật cơ sở dữ liệu")
                        , "Lỗi"
                        , MessageBoxButton.OK
                        , MessageBoxImage.Error
                    );
                }
                else
                {
                    _setting.CurrentSetting.CurrentSemesterValue = _courseCrawler.GetCurrentSemesterValue();
                    _setting.CurrentSetting.CurrentYearValue = _courseCrawler.GetCurrentYearValue();
                    _setting.CurrentSetting.CurrentYear = _courseCrawler.GetCurrentYearInfo();
                    _setting.CurrentSetting.CurrentSemester = _courseCrawler.GetCurrentSemesterInfo();
                    _setting.Save();

                    Messenger.Send(new UpdateVmMsgs.UpdateSuccessMsg());
                    string msg = CredizText.DbMsg001((int)e.Result);
                    _snackbarMessageQueue.Enqueue(msg);
                }
            }

            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await Task.WhenAll(LoadInf(), LoadDisciplines());
            });
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
    }
}
