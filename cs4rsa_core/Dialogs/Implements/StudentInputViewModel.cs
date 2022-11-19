using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Messages.Publishers.Dialogs;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Cs4rsa.Dialogs.Implements
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
