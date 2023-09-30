using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Services.CourseSearchSvc.Crawlers;
using Cs4rsa.Utils.Interfaces;

namespace Cs4rsa.ViewModels
{
    public partial class HomeViewModel : ViewModelBase, IScreenViewModel
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
        public RelayCommand UpdateSubjectDbCommand { get; set; }
        public RelayCommand GotoFormCommand { get; set; }
        public RelayCommand DonateCommand { get; set; }
        public RelayCommand GotoGitHubCommand { get; set; }
        public RelayCommand ManualCommand { get; set; }
        #endregion

        private readonly IOpenInBrowser _openInBrowser;
        private readonly IUnitOfWork _unitOfWork;

        public HomeViewModel(
            CourseCrawler courseCrawler,
            IOpenInBrowser openInBrowser,
            IUnitOfWork unitOfWork
        )
        {
            _courseCrawler = courseCrawler;
            _openInBrowser = openInBrowser;
            _unitOfWork = unitOfWork;

            Messenger.Register<UpdateVmMsgs.UpdateSuccessMsg>(this, (r, m) =>
            {
                LoadIsNewSemester();
            });

            _currentSemesterInfo = _courseCrawler.CurrentSemesterInfo;
            _currentYearInfo = _courseCrawler.CurrentYearInfo;

            UpdateSubjectDbCommand = new RelayCommand(OnUpdate);
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
            _openInBrowser.Open(VmConstants.LinkProject);
        }

        private void OnGotoGithubCommand()
        {
            _openInBrowser.Open(VmConstants.LinkProjectPage);
        }

        private void OnGotoForm()
        {
            _openInBrowser.Open(VmConstants.LinkProjectGoogleSheet);
        }

        private void OnUpdate()
        {
            GotoScreen(4);
        }

        public void LoadIsNewSemester()
        {
            IsNewSemester =
                (
                    CourseCrawler.CurrentSemesterValue != null
                    && CourseCrawler.CurrentYearValue != null
                ) && (
                    _unitOfWork.Settings.GetBykey(VmConstants.StCurrentSemesterValue) != CourseCrawler.CurrentSemesterValue
                    || _unitOfWork.Settings.GetBykey(VmConstants.StCurrentYearValue) != CourseCrawler.CurrentYearValue
                );
        }

        public void InitData()
        {

        }
    }
}
