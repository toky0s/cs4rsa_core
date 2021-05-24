using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using cs4rsa.Models;
using cs4rsa.Database;
using cs4rsa.BaseClasses;
using System.Data;
using System.Globalization;

namespace cs4rsa.Dialogs.SaveDialog
{
    class SaveDialogViewModel: NotifyPropertyChangedBase
    {
        private Cs4rsaDatabase _cs4RsaDatabase;
        public MyICommand SaveCommand { get; set; }

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
            }
        }

        private ObservableCollection<ClassGroupModel> _classGroupModels;
        public ObservableCollection<ClassGroupModel> ClassGroupModels
        {
            get
            {
                return _classGroupModels;
            }
        }

        private string _name = "";
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public SaveDialogViewModel(ObservableCollection<ClassGroupModel> classGroupModels)
        {
            _classGroupModels = classGroupModels;
            _cs4RsaDatabase = new Cs4rsaDatabase(Cs4rsaData.connectString);
            SaveCommand = new MyICommand(Save, CanSave);
            GetListCurrentSession();
        }

        private void GetListCurrentSession()
        {
            string sqlString = @"select * from session";
            DataTable table = _cs4RsaDatabase.GetDataTable(sqlString);
            foreach (DataRow row in table.Rows)
            {
                ScheduleSession scheduleSession = new ScheduleSession
                {
                    Name = row["name"].ToString(),
                    SaveDate = DateTime.ParseExact(row["save_date"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture)
                };
                _scheduleSessions.Add(scheduleSession);
            }
        }

        private bool CanSave()
        {
            return Name != "";
        }

        private void Save()
        {
            
        }
    }
}
