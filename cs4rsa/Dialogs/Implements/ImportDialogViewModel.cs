using cs4rsa.Database;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Models;
using System;
using System.Collections.ObjectModel;

namespace cs4rsa.Dialogs.Implements
{
    class ImportDialogViewModel : DialogViewModelBase<SessionManagerResult>
    {
        private ObservableCollection<ScheduleSession> _scheduleSessions;
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

        private ObservableCollection<ScheduleSessionDetail> _scheduleSessionDetails;
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
                LoadScheduleSessionDetail(value);
            }
        }

        private void LoadScheduleSessionDetail(ScheduleSession value)
        {
            throw new NotImplementedException();
        }

        public RelayCommand DeleteCommand;
        public RelayCommand ImportCommand;

        public ImportDialogViewModel()
        {
            DeleteCommand = new RelayCommand(OnDelete, () => true);
            ImportCommand = new RelayCommand(OnImport, () => true);
            LoadScheduleSession();
        }
        public ImportDialogViewModel(IMessageBox messageBox)
        {

        }

        private void LoadScheduleSession()
        {
            var sessions = Cs4rsaDataView.GetScheduleSessions();
            foreach (ScheduleSession item in sessions)
                ScheduleSessions.Add(item);
        }

        private void OnImport(object obj)
        {
            Console.WriteLine("Nhap");
        }

        private void OnDelete(object obj)
        {
            Console.WriteLine("Xoa");
        }
    }
}
