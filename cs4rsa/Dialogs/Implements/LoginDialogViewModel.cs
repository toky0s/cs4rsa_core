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

        public LoginDialogViewModel()
        {
            LoginCommand = new RelayCommand(OnReturnStudent, () => true);
            LoadStudentInfos();
        }

        private void OnReturnStudent(object obj)
        {
            if (_selectedStudentInfo != null)
            {
                LoginResult loginResult = new LoginResult() { StudentModel = _selectedStudentInfo };
                CloseDialogWithResult(obj as Window, loginResult);
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
