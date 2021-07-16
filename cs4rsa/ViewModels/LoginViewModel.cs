using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Database;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Dialogs.DialogViews;
using cs4rsa.Dialogs.Implements;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Messages;
using cs4rsa.Models;
using LightMessageBus;
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
            }
        }

        public RelayCommand FindCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }

        public LoginViewModel()
        {
            LoadStudentInfos();
            FindCommand = new RelayCommand(OnFind);
            DeleteCommand = new RelayCommand(OnDelete);
        }

        private void OnDelete(object obj)
        {
            StudentModel studentModel = obj as StudentModel;
            Cs4rsaDataEdit.DeleteStudent(studentModel);
            LoadStudentInfos();
            string message = $"Bạn vừa xoá {studentModel.StudentInfo.Name}";
            MessageBus.Default.Publish(new Cs4rsaSnackbarMessage(message));
        }

        private void OnFind(object obj)
        {
            Cs4rsaMessageBox messageBox = new Cs4rsaMessageBox();
            SessionInputViewModel vm = new SessionInputViewModel(_sessionId, messageBox);
            SessionInputWindow loginWindow = new SessionInputWindow();
            DialogService<StudentResult>.OpenDialog(vm, loginWindow, obj as Window);
            LoadStudentInfos();
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
