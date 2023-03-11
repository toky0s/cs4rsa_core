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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.Dialogs.Implements
{
    public sealed partial class AccountViewModel : DialogVmBase
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
            SnackbarMessageQueue.Enqueue(
                $"Bạn vừa xoá {name}",
                VmConstants.SnbRestore,
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
            try
            {
                PreventCloseDialog(true);
                await _unitOfWork.BeginTransAsync();
                SessionInputUC sessionInputUC = new();
                SessionInputViewModel vm = sessionInputUC.DataContext as SessionInputViewModel;
                vm.SessionId = SessionId;

                OpenDialog(sessionInputUC);
                await vm.Find();
                SessionId = string.Empty;
                await _unitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                MessageBox.Show(
                    $"Gặp lỗi không mong muốn: {e.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                PreventCloseDialog(false);
                CloseDialog();
            }
        }

        public async Task LoadStudent()
        {
            Students.Clear();
            IAsyncEnumerable<Student> students = _unitOfWork.Students.GetAllBySpecialStringNotNull();
            await foreach (Student student in students)
            {
                Students.Add(student);
            }
        }
    }
}
