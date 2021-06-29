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
using System.Windows;

namespace cs4rsa.Dialogs.Implements
{
    class LoginViewModel: DialogViewModelBase<LoginResult>
    {
        private ObservableCollection<StudentInfo> _studentInfos = new ObservableCollection<StudentInfo>();
        public ObservableCollection<StudentInfo> StudentInfos
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

        private StudentInfo _selectedStudentInfo;
        public StudentInfo SelectedStudentInfo
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

        public LoginViewModel()
        {
            OpenSessionIdInput = new RelayCommand(OnOpenSessionIdInput, () => true);
            LoadStudentInfos();
        }

        private void OnOpenSessionIdInput(object obj)
        {
            Cs4rsaMessageBox messageBox = new Cs4rsaMessageBox();
            SessionInputWindow sessionInputWindow = new SessionInputWindow();
            SessionInputViewModel vm = new SessionInputViewModel(messageBox);
            DialogService<StudentResult>.OpenDialog(vm, sessionInputWindow, obj as Window);
        }

        private void LoadStudentInfos()
        {
            _studentInfos.Clear();
            List<StudentInfo> studentInfos = Cs4rsaDataView.GetStudentInfos();
            foreach (StudentInfo info in studentInfos)
            {
                _studentInfos.Add(info);
            }
        }
    }
}
