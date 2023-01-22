using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.ModelExtensions;

using MaterialDesignThemes.Wpf;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Cs4rsa.Dialogs.Implements
{
    internal sealed partial class AccountViewModel : DialogVmBase
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

        private readonly IUnitOfWork _unitOfWork;

        [ObservableProperty]
        private ISnackbarMessageQueue _snackbarMessageQueue;
        public AccountViewModel(
            IUnitOfWork unitOfWork,
            ISnackbarMessageQueue snackbarMessageQueue
        ) : base()
        {
            _unitOfWork = unitOfWork;
            SnackbarMessageQueue = snackbarMessageQueue;

            FindCommand = new AsyncRelayCommand(OnFind);
            DeleteCommand = new AsyncRelayCommand(OnDelete);

            Students = new();

            Messenger.Register<SessionInputVmMsgs.ExitFindStudentMsg>(this, (r, m) =>
            {
                Students.Add(m.Value);
            });
        }

        private async Task OnDelete()
        {
            string name = SelectedStudent.Name;
            string id = SelectedStudent.StudentId;
            int index = Students.IndexOf(SelectedStudent);
            Student actionData = SelectedStudent.DeepClone();

            _unitOfWork.Students.Remove(SelectedStudent);
            await _unitOfWork.CompleteAsync();
            Students.RemoveAt(index);

            Messenger.Send(new AccountVmMsgs.DelStudentMsg(id));
            _snackbarMessageQueue.Enqueue(
                $"Bạn vừa xoá {name}",
                VMConstants.SNBAC_RESTORE,
                async (obj) => await OnRestore(obj),
                actionData
            );
        }

        private async Task OnRestore(Student obj)
        {
            await _unitOfWork.Students.AddAsync(obj);
            await _unitOfWork.CompleteAsync();
            Students.Add(obj);
            Messenger.Send(new AccountVmMsgs.UndoDelStudentMsg(obj));
        }

        private async Task OnFind()
        {
            SessionInputUC sessionInputUC = new();
            SessionInputViewModel vm = sessionInputUC.DataContext as SessionInputViewModel;
            vm.SessionId = _sessionId;

            OpenDialog(sessionInputUC);
            await vm.Find();
            CloseDialog();
            SessionId = string.Empty;
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
