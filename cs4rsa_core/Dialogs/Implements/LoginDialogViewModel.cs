using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.DialogServices;
using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Models;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace cs4rsa_core.Dialogs.Implements
{
    class LoginDialogViewModel : DialogViewModelBase<LoginResult>
    {
        public ObservableCollection<Student> StudentInfos { get; set; }
        public Student SelectedStudentInfo { get; set; }

        public RelayCommand LoginCommand { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }
        public Action<LoginResult> CloseDialogCallback { get; set; }

        private Cs4rsaDbContext _cs4rsaDbContext;

        public LoginDialogViewModel(Cs4rsaDbContext cs4rsaDbContext)
        {
            _cs4rsaDbContext = cs4rsaDbContext;
            LoadStudentInfos();
            StudentInfos = new ObservableCollection<Student>();
            LoginCommand = new RelayCommand(OnReturnStudent, () => true);
            CloseDialogCommand = new RelayCommand(OnCloseDialog, () => true);
        }

        private void OnCloseDialog()
        {
            CloseDialogCallback.Invoke(null);
        }

        private void OnReturnStudent()
        {
            Console.WriteLine("Return Student");
            //Student studentModel = obj as Student;
            //LoginResult loginResult = new LoginResult() { Student = studentModel };
            //CloseDialogCallback.Invoke(loginResult);
        }

        private void LoadStudentInfos()
        {
            StudentInfos.Clear();
            List<Student> students = _cs4rsaDbContext.Students.ToList();
            students.ForEach(st => StudentInfos.Add(st));
        }
    }
}
