using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Database;
using System.Windows;
using System.Windows.Input;
using cs4rsa.Models;
using cs4rsa.BaseClasses;
using System.Collections.ObjectModel;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa;

namespace cs4rsa.Dialogs.Implements
{
    class SaveDialogViewModel : ViewModelBase
    {
        private List<ClassGroupModel> _classGroupModels;
        public List<ClassGroupModel> ClassGroupModels
        {
            get { return _classGroupModels; }
            set { _classGroupModels = value; }
        }

        private ObservableCollection<ScheduleSession> scheduleSessions = new ObservableCollection<ScheduleSession>();
        public ObservableCollection<ScheduleSession> ScheduleSessions
        {
            get
            {
                return scheduleSessions;
            }
            set
            {
                scheduleSessions = value;
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand SaveCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public Action<SaveResult> CloseDialogCallback { get; set; }

        public SaveDialogViewModel()
        {
            SaveCommand = new RelayCommand(Save, ()=> true); 
            CancelCommand = new RelayCommand(Cancle, ()=> true);
            LoadScheduleSessions();
        }

        private void LoadScheduleSessions()
        {
            var sessions = Cs4rsaDataView.GetScheduleSessions();
            foreach (ScheduleSession item in sessions)
                scheduleSessions.Add(item);
        }

        private void Cancle(object obj)
        {
            CloseDialogCallback.Invoke(null);
        }

        private void Save(object obj)
        {
            Cs4rsaDataEdit.AddSession(name, DateTime.Now.ToString(), _classGroupModels);
            SaveResult result = new SaveResult() { Name = name};
            CloseDialogCallback.Invoke(result);
        }
    }
}
