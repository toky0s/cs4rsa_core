using cs4rsa.BasicData;
using cs4rsa.Database;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace cs4rsa.Dialogs.Implements
{
    class LoginDialogViewModel : DialogViewModelBase<LoginResult>
    {
        private ObservableCollection<StudentModel> _studentInfos = new ObservableCollection<StudentModel>();
        public ObservableCollection<StudentModel> StudentInfos
        {
            get
            {
                return _studentInfos;
            }
            set
            {
                _studentInfos = value;
            }
        }

        private StudentModel _selectedStudentInfo;
        public StudentModel SelectedStudentInfo
        {
            get
            {
                return _selectedStudentInfo;
            }
            set
            {
                _selectedStudentInfo = value;
            }
        }

        public RelayCommand LoginCommand { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }
        public Action<LoginResult> CloseDialogCallback { get; set; }

        public LoginDialogViewModel()
        {
            LoadStudentInfos();
            LoginCommand = new RelayCommand(OnReturnStudent, () => true);
            CloseDialogCommand = new RelayCommand(OnCloseDialog, () => true);
        }

        private void OnCloseDialog(object obj)
        {
            CloseDialogCallback.Invoke(null);
        }

        private void OnReturnStudent(object obj)
        {
            StudentModel studentModel = obj as StudentModel;
            LoginResult loginResult = new LoginResult() { StudentModel = studentModel };
            CloseDialogCallback.Invoke(loginResult);
        }

        private void LoadStudentInfos()
        {
            _studentInfos.Clear();
            List<StudentInfo> studentInfos = Cs4rsaDataView.GetStudentInfos();
            foreach (StudentInfo info in studentInfos)
            {
                Student student = new Student(info);
                StudentModel studentModel = new StudentModel(student);
                _studentInfos.Add(studentModel);
            }
        }
    }
}
