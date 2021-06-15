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
    class SaveDialogViewModel : DialogViewModelBase<SaveResult>
    {
        private IMessageBox _messageBox;
        private List<ClassGroupModel> _classGroupModels;

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
                RaisePropertyChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand SaveCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }
        
        public SaveDialogViewModel(IMessageBox messageBox, List<ClassGroupModel> classGroupModels)
        {
            SaveCommand = new RelayCommand(Save, ()=> true); 
            CancelCommand = new RelayCommand(Cancle, ()=> true);
            _messageBox = messageBox;
            _classGroupModels = classGroupModels;
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
            SaveResult a = new SaveResult
            {
                Name = null
            };
            CloseDialogWithResult(obj as Window, a);
        }

        private void Save(object obj)
        {
            if (name == null || name.Trim() == "")
            {
                _messageBox.ShowMessage("Tên không được để trống");
                return;
            }
            Cs4rsaDataEdit.AddSession(name, DateTime.Now.ToString(), _classGroupModels);
            SaveResult result = new SaveResult();
            result.Name = name;
            CloseDialogWithResult(obj as Window, result);
        }
    }
}
