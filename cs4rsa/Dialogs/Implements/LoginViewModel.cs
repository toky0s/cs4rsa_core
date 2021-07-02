using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Database;
using cs4rsa.Dialogs.DialogViews;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Enums;
using System.Windows;
using cs4rsa.Models;

namespace cs4rsa.Dialogs.Implements
{
    class LoginViewModel: DialogViewModelBase<LoginResult>
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

        public RelayCommand OpenSessionIdInput { get; set; }
        public RelayCommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            OpenSessionIdInput = new RelayCommand(OnOpenSessionIdInput, () => true);
            LoginCommand = new RelayCommand(OnReturnStudent, () => true);
            LoadStudentInfos();
        }

        private void OnReturnStudent(object obj)
        {
            if (_selectedStudentInfo != null)
            {
                LoginResult loginResult = new LoginResult() { StudentModel=_selectedStudentInfo };
                CloseDialogWithResult(obj as Window, loginResult);
            }
        }

        private void OnOpenSessionIdInput(object obj)
        {
            Cs4rsaMessageBox messageBox = new Cs4rsaMessageBox();
            SessionInputWindow sessionInputWindow = new SessionInputWindow();
            SessionInputViewModel vm = new SessionInputViewModel(messageBox);
            StudentResult result = DialogService<StudentResult>.OpenDialog(vm, sessionInputWindow, obj as Window);
            if (result != null)
            {
                StudentModel studentModel = new StudentModel(result.Student);
                if (!_studentInfos.Contains(studentModel))
                    _studentInfos.Add(studentModel);
                else
                {
                    int index = _studentInfos.IndexOf(studentModel);
                    _studentInfos[index] = studentModel;
                }
            }
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
