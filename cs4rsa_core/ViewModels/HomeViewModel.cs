using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.CourseSearchSvc.Crawlers;
using Cs4rsa.Settings.Interfaces;
using Cs4rsa.Utils.Interfaces;

using System.Threading.Tasks;

namespace Cs4rsa.ViewModels
{
    internal partial class HomeViewModel : ViewModelBase, IScreenViewModel
    {
        [ObservableProperty]
        private string _currentYearInfo;

        [ObservableProperty]
        private string _currentSemesterInfo;

        [ObservableProperty]
        private bool _isNewSemester;

        [ObservableProperty]
        private CourseCrawler _courseCrawler;

        #region Commands
        public RelayCommand UpdateSubjectDatabaseCommand { get; set; }
        public RelayCommand GotoFormCommand { get; set; }
        public RelayCommand DonateCommand { get; set; }
        public RelayCommand GotoGitHubCommand { get; set; }
        public RelayCommand ManualCommand { get; set; }
        #endregion

        #region DI
        private readonly ISetting _setting;
        private readonly IOpenInBrowser _openInBrowser;
        #endregion

        public HomeViewModel(
            CourseCrawler courseCrawler,
            ISetting setting,
            IOpenInBrowser openInBrowser
        )
        {
            _courseCrawler = courseCrawler;
            _setting = setting;
            _openInBrowser = openInBrowser;

            Messenger.Register<UpdateVmMsgs.UpdateSuccessMsg>(this, (r, m) =>
            {
                LoadIsNewSemester();
            });

            _currentSemesterInfo = _courseCrawler.CurrentSemesterInfo;
            _currentYearInfo = _courseCrawler.CurrentYearInfo;

            UpdateSubjectDatabaseCommand = new RelayCommand(OnUpdate);
            GotoFormCommand = new RelayCommand(OnGotoForm);
            GotoGitHubCommand = new RelayCommand(OnGotoGithubCommand);
            ManualCommand = new RelayCommand(OnGotoManualCommand);
            DonateCommand = new RelayCommand(OnDonate);

            LoadIsNewSemester();
        }

        private void OnDonate()
        {
            OpenDialog(new DonateUC());
        }

        private void OnGotoManualCommand()
        {
            _openInBrowser.Open(VMConstants.LK_PROJECT);
        }

        private void OnGotoGithubCommand()
        {
            _openInBrowser.Open(VMConstants.LK_PROJECT_PAGE);
        }

        private void OnGotoForm()
        {
            _openInBrowser.Open(VMConstants.LK_PROJECT_GG_SHEET);
        }

        private void OnUpdate()
        {
            Messenger.Send(new HomeVmMsgs.UpdateSubjectDbMsg(null));
        }

        public void LoadIsNewSemester()
        {
            IsNewSemester =
                (
                    CourseCrawler.CurrentSemesterValue != null
                    && CourseCrawler.CurrentYearValue != null
                ) && (
                    _setting.CurrentSetting.CurrentSemesterValue != CourseCrawler.CurrentSemesterValue
                    || _setting.CurrentSetting.CurrentYearValue != CourseCrawler.CurrentYearValue
                );
        }

        public void InitData()
        {

        }

        public Task InitDataAsync()
        {
            return Task.FromResult<object>(null);
        }
    }
}
