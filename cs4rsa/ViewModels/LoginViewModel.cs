using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Database;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.DialogViews;
using cs4rsa.Dialogs.Implements;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cs4rsa.ViewModels
{
    public class LoginViewModel:  NotifyPropertyChangedBase
    {
        private ObservableCollection<StudentModel> _students = new ObservableCollection<StudentModel>();
        public ObservableCollection<StudentModel> Students
        {
            get
            {
                return _students;
            }
            set
            {
                _students = value;
            }
        }

        private string _sessionId;
        public string SessionId
        {
            get { return _sessionId; }
            set
            {
                _sessionId = value;
                RaisePropertyChanged();
                FindCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand FindCommand { get; set; }

        public LoginViewModel()
        {
            LoadStudentInfos();
            FindCommand = new RelayCommand(OnFind, CanFind);
        }

        private void OnFind(object obj)
        {
            Cs4rsaMessageBox messageBox = new Cs4rsaMessageBox();
            SessionInputViewModel vm = new SessionInputViewModel(_sessionId, messageBox);
            SessionInputWindow loginWindow = new SessionInputWindow();
            DialogService<StudentResult>.OpenDialog(vm, loginWindow, obj as Window);
            LoadStudentInfos();
        }

        private bool CanFind()
        {
            return _sessionId != null || _sessionId != "";
        }

        private void LoadStudentInfos()
        {
            _students.Clear();
            List<StudentInfo> studentInfos = Cs4rsaDataView.GetStudentInfos();
            foreach (StudentInfo info in studentInfos)
            {
                Student student = new Student(info);
                StudentModel studentModel = new StudentModel(student);
                _students.Add(studentModel);
            }
        }
    }
}
