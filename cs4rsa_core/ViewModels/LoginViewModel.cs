using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogViews;
using cs4rsa_core.Dialogs.Implements;
using cs4rsa_core.Dialogs.MessageBoxService;
using cs4rsa_core.Messages;
using cs4rsa_core.Messages.Publishers.Dialogs;
using cs4rsa_core.ModelExtensions;
using MaterialDesignThemes.Wpf;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using cs4rsa_core.Cs4rsaDatabase.Interfaces;
using cs4rsa_core.Cs4rsaDatabase.Models;

namespace cs4rsa_core.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region Properties
        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnPropertyChanged(); }
        }
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
        #endregion

        #region Commands
        public AsyncRelayCommand FindCommand { get; set; }
        public AsyncRelayCommand DeleteCommand { get; set; }
        public RelayCommand ExpandedCommand { get; set; }
        #endregion

        #region Services
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessageBox _cs4RsaMessageBox;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion
        public LoginViewModel(
            IMessageBox cs4rsaMessageBox, 
            IUnitOfWork unitOfWork,
            ISnackbarMessageQueue snackbarMessageQueue
        )
        {
            _cs4RsaMessageBox = cs4rsaMessageBox;
            _unitOfWork = unitOfWork;
            _snackbarMessageQueue = snackbarMessageQueue;

            WeakReferenceMessenger.Default.Register<SessionInputVmMsgs.ExitSearchAccountMsg>(this, async (r, m) =>
            {
                await LoadStudentInfos();
            });

            FindCommand = new AsyncRelayCommand(OnFind);
            DeleteCommand = new AsyncRelayCommand(OnDelete);
            ExpandedCommand = new RelayCommand(OnExpanded);

            Students = new();
        }

        private void OnExpanded()
        {
            IsExpanded = !IsExpanded;
        }

        private async Task OnDelete()
        {
            string name = SelectedStudent.Name;
            string message = $"Bạn vừa xoá {name}";
            Student actionData = SelectedStudent.DeepClone();

            _unitOfWork.Students.Remove(SelectedStudent);
            await _unitOfWork.CompleteAsync();
            await LoadStudentInfos();

            _snackbarMessageQueue.Enqueue(message, "HOÀN TÁC", async (obj) => await OnRestore(obj), actionData);
        }

        private async Task OnRestore(Student obj)
        {
            await _unitOfWork.BeginTransAsync();
            await _unitOfWork.Students.AddAsync(obj);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitAsync();
            Students.Clear();
            IEnumerable<Student> students = await _unitOfWork.Students.GetAllAsync();
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
            OpenDialog(sessionInputUC);
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
    }
}
