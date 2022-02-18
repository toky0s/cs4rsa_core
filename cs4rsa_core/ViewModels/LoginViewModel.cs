using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogViews;
using cs4rsa_core.Dialogs.Implements;
using cs4rsa_core.Dialogs.MessageBoxService;
using cs4rsa_core.Messages;
using cs4rsa_core.ModelExtensions;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using LightMessageBus;
using LightMessageBus.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace cs4rsa_core.ViewModels
{
    public class LoginViewModel : ViewModelBase, IMessageHandler<ExitSessionInputMessage>
    {
        public ObservableCollection<Student> Students { get; set; }
        public Student SelectedStudent { get; set; }

        private string _sessionId;
        public string SessionId
        {
            get { return _sessionId; }
            set
            {
                _sessionId = value;
                OnPropertyChanged();
            }
        }

        #region Commands
        public AsyncRelayCommand FindCommand { get; set; }
        public AsyncRelayCommand DeleteCommand { get; set; }
        #endregion

        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageBox _cs4RsaMessageBox;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion
        public LoginViewModel(IMessageBox cs4rsaMessageBox, IUnitOfWork unitOfWork,
            ISnackbarMessageQueue snackbarMessageQueue)
        {
            _cs4RsaMessageBox = cs4rsaMessageBox;
            _unitOfWork = unitOfWork;
            _snackbarMessageQueue = snackbarMessageQueue;

            MessageBus.Default.FromAny().Where<ExitSessionInputMessage>().Notify(this);

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
            await LoadStudentInfos();

            _snackbarMessageQueue.Enqueue<Student>(message, "HOÀN TÁC", OnRestore, actionData);
        }

        private void OnRestore(Student obj)
        {
            _unitOfWork.Students.Add(obj);
            _unitOfWork.Complete();
            Students.Clear();
            IEnumerable<Student> students = _unitOfWork.Students.GetAll();
            foreach (Student student in students)
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
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).OpenDialog(sessionInputUC);
            await vm.Find();
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

        public async void Handle(ExitSessionInputMessage message)
        {
            await LoadStudentInfos();
        }
    }
}
