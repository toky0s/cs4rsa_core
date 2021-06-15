using cs4rsa.Database;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace cs4rsa.Dialogs.Implements
{
    class ImportDialogViewModel : DialogViewModelBase<SessionManagerResult>
    {
        private ObservableCollection<ScheduleSession> _scheduleSessions = new ObservableCollection<ScheduleSession>();
        public ObservableCollection<ScheduleSession> ScheduleSessions
        {
            get
            {
                return _scheduleSessions;
            }
            set
            {
                _scheduleSessions = value;
            }
        }

        private ObservableCollection<ScheduleSessionDetail> _scheduleSessionDetails = new ObservableCollection<ScheduleSessionDetail>();
        public ObservableCollection<ScheduleSessionDetail> ScheduleSessionDetails
        {
            get
            {
                return _scheduleSessionDetails;
            }
            set
            {
                _scheduleSessionDetails = value;
            }
        }

        private ScheduleSession _selectedScheduleSession;
        public ScheduleSession SelectedScheduleSession
        {
            get
            {
                return _selectedScheduleSession;
            }
            set
            {
                _selectedScheduleSession = value;
                RaisePropertyChanged();
                ImportCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                LoadScheduleSessionDetail(value);
            }
        }

        private void LoadScheduleSessionDetail(ScheduleSession value)
        {
            if (value != null)
            {
                ScheduleSessionDetails.Clear();
                var details = Cs4rsaDataView.GetSessionDetail(value.ID);
                foreach (ScheduleSessionDetail item in details)
                    ScheduleSessionDetails.Add(item);
            }
        }

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }

        private IMessageBox _messageBox;

        public ImportDialogViewModel(IMessageBox messageBox)
        {
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            ImportCommand = new RelayCommand(OnImport, CanImport);
            _messageBox = messageBox;
            LoadScheduleSession();
        }

        private bool CanImport()
        {
            if (_selectedScheduleSession != null)
                return true;
            return false;
        }

        private bool CanDelete()
        {
            if (_selectedScheduleSession != null)
                return true;
            return false;
        }

        private void LoadScheduleSession()
        {
            ScheduleSessions.Clear();
            var sessions = Cs4rsaDataView.GetScheduleSessions();
            foreach (ScheduleSession item in sessions)
                ScheduleSessions.Add(item);
        }

        private void OnImport(object obj)
        {
            List<SubjectInfoData> subjectInfoDatas = new List<SubjectInfoData>();
            foreach (ScheduleSessionDetail item in _scheduleSessionDetails)
            {
                SubjectInfoData data = new SubjectInfoData()
                {
                    SubjectCode = item.SubjectCode,
                    ClassGroup = item.ClassGroup
                };
                subjectInfoDatas.Add(data);
            }
            SessionManagerResult result = new SessionManagerResult(subjectInfoDatas);
            CloseDialogWithResult(obj as Window, result);
        }

        private void OnDelete(object obj)
        {
            string sessionName = _selectedScheduleSession.Name;
            MessageBoxResult result = _messageBox.ShowMessage($"Bạn có chắc muốn xoá phiên {sessionName}?",
                                    "Thông báo",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Cs4rsaDataEdit.DeleteSession(_selectedScheduleSession.ID);
                Reload();
            }
        }

        private void Reload()
        {
            ScheduleSessions.Clear();
            ScheduleSessionDetails.Clear();
            LoadScheduleSession();
        }
    }
}
