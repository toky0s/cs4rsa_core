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

        public LoginViewModel()
        {
            List<StudentInfo> studentInfos = Cs4rsaDataView.GetStudentInfos();
            foreach (StudentInfo info in studentInfos)
            {
                _studentInfos.Add(info);
            }
        }
    }
}
