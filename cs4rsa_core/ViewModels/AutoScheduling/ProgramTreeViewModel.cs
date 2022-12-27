using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Models;
using Cs4rsa.Models.AutoScheduling;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.Interfaces;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.ViewModels.AutoScheduling
{
    internal sealed partial class ProgramTreeViewModel : ViewModelBase
    {
        private readonly MyProgramUC _myProgramUC;
        private readonly AccountUC _accountUC;

        public ObservableCollection<Student> Students { get; set; }
        public ObservableCollection<PlanTableModel> PlanTableModels { get; set; }
        public ObservableCollection<ProgramFolderModel> ProgramFolderModels { get; set; }

        [ObservableProperty]
        private bool _isFinding;

        [ObservableProperty]
        private bool _isUseCache;

        [ObservableProperty]
        private Student _selectedStudent;

        [ObservableProperty]
        private ProgramSubjectModel _selectedProSubject;

        public AsyncRelayCommand LoadProgramCommand { get; set; }
        public RelayCommand AccountCommand { get; set; }

        /// <summary>
        /// Hiển thị Dialog chứa chương trình học dự kiến
        /// của người dùng hiện tại
        /// </summary>
        public RelayCommand MyProgramCommand { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudentPlanCrawler _studentPlanCrawler;
        private readonly ColorGenerator _colorGenerator;

        public ProgramTreeViewModel(
            IUnitOfWork unitOfWork,
            IStudentPlanCrawler studentPlanCrawler,
            ColorGenerator colorGenerator
            )
        {
            _unitOfWork = unitOfWork;
            _studentPlanCrawler = studentPlanCrawler;
            _colorGenerator = colorGenerator;

            ProgramFolderModels = new();
            PlanTableModels = new();
            Students = new();
            IsUseCache = true;

            LoadProgramCommand = new AsyncRelayCommand(LoadProgramSubject, () => _selectedStudent != null);
            AccountCommand = new RelayCommand(() => OpenDialog(_accountUC));
            MyProgramCommand = new RelayCommand(() => OpenDialog(_myProgramUC), () => _selectedStudent != null && !_selectedStudent.StudentId.Equals("0"));

            _myProgramUC = new() { DataContext = this };
            _accountUC = new();

            Messenger.Register<SessionInputVmMsgs.ExitFindStudentMsg>(this, (r, m) =>
            {
                Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadStudents();
                });
            });

            Messenger.Register<AccountVmMsgs.DelStudentMsg>(this, (r, m) =>
            {
                Student student = Students.Where(student => student.StudentId.Equals(m.Value)).First();
                Students.Remove(student);
                ProgramFolderModels.Clear();
            });

            Messenger.Register<AccountVmMsgs.UndoDelStudentMsg>(this, (r, m) =>
            {
                Students.Add(m.Value);
                if (Students.Count == 1)
                {
                    SelectedStudent = Students[0];
                }
            });

            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await LoadStudents();
            });
        }

        partial void OnSelectedStudentChanged(Student value)
        {
            Application.Current.Dispatcher.InvokeAsync(async () => await LoadStudentPlan());
            LoadProgramCommand.NotifyCanExecuteChanged();
            MyProgramCommand.NotifyCanExecuteChanged();
        }

        private async Task LoadStudents()
        {
            IAsyncEnumerable<Student> students = _unitOfWork.Students.GetAll();
            await foreach (Student student in students)
            {
                Students.Add(student);
            }
            if (Students.Any())
            {
                SelectedStudent = Students.First();
            }
        }

        /// <summary>
        /// Load chương trình học dự kiến của sinh viên.
        /// Đánh giá các môn học có sẵn trong học kỳ hiện tại
        /// cùng với tình trạng học của môn học đó so với
        /// chương trình học (Đã qua, Đang học/chưa có điểm, Chưa học)
        /// </summary>
        private async Task LoadStudentPlan()
        {
            if (_selectedStudent == null) return;
            PlanTableModels.Clear();

            List<Task<PlanTableModel>> tasks = new();
            IEnumerable<PlanTable> planTables = await _studentPlanCrawler.GetPlanTables(_selectedStudent.CurriculumId);
            if (planTables == null) return;
            foreach (PlanTable planTable in planTables)
            {
                tasks.Add(PlanTableModel.Build(planTable, _unitOfWork));
            }
            PlanTableModel[] planTableModels = await Task.WhenAll(tasks);
            foreach (PlanTableModel planTableModel in planTableModels)
            {
                PlanTableModels.Add(planTableModel);
            }
        }

        private async Task LoadProgramSubject()
        {
            ProgramFolderModels.Clear();
            string programPath = Path.Combine(
                AppContext.BaseDirectory,
                IFolderManager.FD_STUDENT_PROGRAMS,
                _selectedStudent.StudentId,
                VMConstants.FN_STUDENT_PROGRAM);
            if (File.Exists(programPath))
            {
                string json = await File.ReadAllTextAsync(programPath);
                ProgramFolder[] programFolders = JsonConvert.DeserializeObject<ProgramFolder[]>(json);

                foreach (ProgramFolder programFolder in programFolders)
                {
                    ProgramFolderModel programFolderModel = await ProgramFolderModel.CreateAsync(
                        programFolder,
                        _colorGenerator,
                        _unitOfWork
                    );
                    ProgramFolderModels.Add(programFolderModel);
                }
            }
            else
            {
                MessageBox.Show(CredizText.AutoMsg001(VMConstants.FN_STUDENT_PROGRAM, _selectedStudent.Name), "Thông báo", MessageBoxButton.OK);
            }
        }
    }
}
