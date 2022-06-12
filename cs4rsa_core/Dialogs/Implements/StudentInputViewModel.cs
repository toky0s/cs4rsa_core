using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.DialogServices;
using cs4rsa_core.Messages;
using cs4rsa_core.ViewModels;

using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;

using LightMessageBus;

using Microsoft.Toolkit.Mvvm.Input;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace cs4rsa_core.Dialogs.Implements
{
    public sealed class StudentInputViewModel : DialogViewModelBase<LoginResult>
    {
        public ObservableCollection<Student> Students { get; set; }
        public Student SelectedStudent { get; set; }
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }
        private readonly IUnitOfWork _unitOfWork;
        public StudentInputViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Students = new ObservableCollection<Student>();
            LoginCommand = new RelayCommand(OnReturnStudent, () => true);
            CloseDialogCommand = new RelayCommand(OnCloseDialog, () => true);
        }

        private void OnCloseDialog()
        {
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
        }

        private void OnReturnStudent()
        {
            LoginResult loginResult = new() { Student = SelectedStudent };
            (Application.Current.MainWindow.DataContext as MainWindowViewModel).CloseDialog();
            MessageBus.Default.Publish(new ExitLoginMessage(loginResult));
        }

        public async Task LoadStudentInfos()
        {
            Students.Clear();
            IEnumerable<Student> students = await _unitOfWork.Students.GetAllAsync();
            students.ToList().ForEach(student => Students.Add(student));
        }
    }
}
