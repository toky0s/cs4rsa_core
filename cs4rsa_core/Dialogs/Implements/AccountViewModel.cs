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
using CommunityToolkit.Mvvm.ComponentModel;

namespace Cs4rsa.Dialogs.Implements
{
    public sealed partial class AccountViewModel : DialogVmBase
    {
        #region Properties

        [ObservableProperty] private ISnackbarMessageQueue _snackbarMessageQueue;
        [ObservableProperty] private string _sessionId;
        public ObservableCollection<Student> Students { get; }
        public Student SelectedStudent { get; set; }

        #endregion

        #region Commands
        public AsyncRelayCommand FindCommand { get; }
        public RelayCommand DeleteCommand { get; }
        #endregion

        private readonly IUnitOfWork _unitOfWork;

        public AccountViewModel(
            IUnitOfWork unitOfWork,
            ISnackbarMessageQueue snackbarMessageQueue
        )
        {
            _unitOfWork = unitOfWork;
            SnackbarMessageQueue = snackbarMessageQueue;

            FindCommand = new AsyncRelayCommand(OnFind);
            DeleteCommand = new(OnDelete);

            Students = new();

            Messenger.Register<SessionInputVmMsgs.ExitFindStudentMsg>(this, (r, m) =>
            {
                Students.Add(m.Value);
            });
        }

        private void OnDelete()
        {
            string name = SelectedStudent.Name;
            string id = SelectedStudent.StudentId;
            int index = Students.IndexOf(SelectedStudent);
            Student actionData = SelectedStudent.DeepClone();

            _unitOfWork.Students.Remove(SelectedStudent);
            Students.RemoveAt(index);

            Messenger.Send(new AccountVmMsgs.DelStudentMsg(id));
            SnackbarMessageQueue.Enqueue(
                $"Bạn vừa xoá {name}",
                VmConstants.SnbRestore,
                obj => OnRestore(obj),
                actionData
            );
        }

        private void OnRestore(Student obj)
        {
            _unitOfWork.Students.Add(obj);
            Students.Add(obj);
            Messenger.Send(new AccountVmMsgs.UndoDelStudentMsg(obj));
        }

        private async Task OnFind()
        {
            try
            {
                PreventCloseDialog(true);
                SessionInputUC sessionInputUc = new();
                SessionInputViewModel vm = (SessionInputViewModel)sessionInputUc.DataContext;
                vm.SessionId = SessionId;

                OpenDialog(sessionInputUc);
                await vm.Find();
                SessionId = string.Empty;
            }
            catch (Exception e)
            {
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

        public void LoadStudent()
        {
            Students.Clear();
            IEnumerable<Student> students = _unitOfWork.Students.GetAllBySpecialStringNotNull();
            foreach (Student student in students)
            {
                Students.Add(student);
            }
        }
    }
}
