using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using cs4rsa_core.BaseClasses;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Messages.Publishers.Dialogs;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace cs4rsa_core.Dialogs.Implements
{
    public sealed class StudentInputViewModel : ViewModelBase
    {
        public ObservableCollection<Student> Students { get; set; }
        public Student SelectedStudent { get; set; }
        public RelayCommand LoginCommand { get; set; }

        #region DI
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        public StudentInputViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Students = new();
            LoginCommand = new RelayCommand(OnReturnStudent, () => true);
        }

        private void OnReturnStudent()
        {
            LoginResult loginResult = new() { Student = SelectedStudent };
            CloseDialog();
            Messenger.Send(new StudentInputVmMsgs.ExitLoginMsg(loginResult));
        }

        public async Task LoadStudentInfos()
        {
            Students.Clear();
            IEnumerable<Student> students = await _unitOfWork.Students.GetAllAsync();
            foreach (Student student in students)
            {
                Students.Add(student);
            }
        }
    }
}
