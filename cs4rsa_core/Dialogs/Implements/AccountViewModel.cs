using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Dialogs.MessageBoxService;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.ModelExtensions;

using MaterialDesignThemes.Wpf;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Cs4rsa.Dialogs.Implements
{
    internal sealed partial class AccountViewModel : ViewModelBase
    {
        #region Properties
        public ObservableCollection<Student> Students { get; set; }
        public Student SelectedStudent { get; set; }

        [ObservableProperty]
        private string _sessionId;
        #endregion

        #region Commands
        public AsyncRelayCommand FindCommand { get; set; }
        public AsyncRelayCommand DeleteCommand { get; set; }
        #endregion

        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageBox _cs4RsaMessageBox;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion
        public AccountViewModel(
            IMessageBox cs4rsaMessageBox,
            IUnitOfWork unitOfWork,
            ISnackbarMessageQueue snackbarMessageQueue
        )
        {
            _cs4RsaMessageBox = cs4rsaMessageBox;
            _unitOfWork = unitOfWork;
            _snackbarMessageQueue = snackbarMessageQueue;

            Messenger.Register<SessionInputVmMsgs.ExitSearchAccountMsg>(this, async (r, m) =>
            {
                await LoadStudent();
            });

            FindCommand = new AsyncRelayCommand(OnFind);
            DeleteCommand = new AsyncRelayCommand(OnDelete);

            Students = new();
        }

        private async Task OnDelete()
        {
            string name = SelectedStudent.Name;
            string message = $"Bạn vừa xoá {name}";
            Student actionData = SelectedStudent.DeepClone();

            _unitOfWork.Students.Remove(SelectedStudent);
            await _unitOfWork.CompleteAsync();
            await LoadStudent();

            _snackbarMessageQueue.Enqueue(message, VMConstants.SNBAC_RESTORE, async (obj) => await OnRestore(obj), actionData);
        }

        private async Task OnRestore(Student obj)
        {
            await _unitOfWork.Students.AddAsync(obj);
            await _unitOfWork.CompleteAsync();
            Students.Clear();
            IAsyncEnumerable<Student> students = _unitOfWork.Students.GetAll();
            await foreach (Student student in students)
            {
                Students.Add(student);
            }
        }

        private async Task OnFind()
        {
            SessionInputUC sessionInputUC = new();
            SessionInputViewModel vm = sessionInputUC.DataContext as SessionInputViewModel;
            vm.MessageBox = _cs4RsaMessageBox;
            vm.SessionId = _sessionId;
            OpenDialog(sessionInputUC);
            await vm.Find();
        }

        public async Task LoadStudent()
        {
            Students.Clear();
            IAsyncEnumerable<Student> students = _unitOfWork.Students.GetAll();
            await foreach (Student student in students)
            {
                Students.Add(student);
            }
        }
    }
}
