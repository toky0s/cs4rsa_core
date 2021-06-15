using cs4rsa.Database;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace cs4rsa.Dialogs.Implements
{
    class ImportDialogViewModel : DialogViewModelBase<SessionManagerResult>
    {
        private ObservableCollection<ScheduleSession> _scheduleSessions =  new ObservableCollection<ScheduleSession>();
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
                LoadScheduleSessionDetail(value);
            }
        }

        private void LoadScheduleSessionDetail(ScheduleSession value)
        {
            var details = Cs4rsaDataView.GetSessionDetail(value.ID);
            foreach (ScheduleSessionDetail item in details)
                ScheduleSessionDetails.Add(item);
        }

        public RelayCommand DeleteCommand;
        public RelayCommand ImportCommand;

        public ImportDialogViewModel(IMessageBox messageBox)
        {
            DeleteCommand = new RelayCommand(OnDelete, () => true);
            ImportCommand = new RelayCommand(OnImport, () => true);
            LoadScheduleSession();
        }

        private void LoadScheduleSession()
        {
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
            Console.WriteLine("Xoa");
        }
    }
}
