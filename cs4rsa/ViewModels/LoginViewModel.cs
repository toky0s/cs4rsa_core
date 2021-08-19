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
    public class LoginViewModel:  ViewModelBase
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
                OnPropertyChanged();
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
            SessionInputUC sessionInputUC = new SessionInputUC();
            SessionInputViewModel vm = sessionInputUC.DataContext as SessionInputViewModel;
            vm.MessageBox = messageBox;
            vm.SessionId = _sessionId;
            vm.CloseDialogCallback = CloseDialogAndHandleStudentResult;
            (App.Current.MainWindow.DataContext as MainViewModel).OpenDialog(sessionInputUC);
            vm.Find();
        }

        private void CloseDialogAndHandleStudentResult(StudentResult obj)
        {
            (App.Current.MainWindow.DataContext as MainViewModel).CloseDialog();
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
