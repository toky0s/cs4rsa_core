using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Utils;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using System;
using System.Windows;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ShareStringUCViewModel : BindableBase, IDialogAware
    {
        private string _shareString;
        public string ShareString
        {
            get => _shareString;
            set => SetProperty(ref _shareString, value);
        }

        #region Commands
        public DelegateCommand CopyCommand { get; set; }

        public string Title => "Share Your Schedule";
        #endregion

        public event Action<IDialogResult> RequestClose;
        private readonly IShareStringService _shareStringService;
        private readonly ILogger<ShareStringUCViewModel> _logger;

        public ShareStringUCViewModel(
            IShareStringService shareStringService,
            ILogger<ShareStringUCViewModel> logger
        )
        {
            _logger = logger;
            _shareStringService = shareStringService;
            CopyCommand = new DelegateCommand(OnCopy, CanCopy);
        }

        private bool CanCopy()
        {
            return !string.IsNullOrWhiteSpace(ShareString);
        }

        private void OnCopy()
        {
            if (_shareString == null) return;
            Clipboard.SetData(DataFormats.Text, _shareString);
            _logger.LogInformation("User copied share string successfully");
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            _logger.LogInformation("Dialog share string closed");
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var userSubjects = parameters.GetValue<UserSubject[]>("UserSubjects");
            if (userSubjects.Length == 0)
            {
                _logger.LogWarning("No user subject found to generate share string");
                ShareString = string.Empty;
                return;
            }
            ShareString = _shareStringService.GetShareString(userSubjects);
            CopyCommand.RaiseCanExecuteChanged();
        }
    }
}
