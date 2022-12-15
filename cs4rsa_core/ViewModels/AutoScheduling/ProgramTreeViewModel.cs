using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Dialogs.DialogViews;
using Cs4rsa.Messages.Publishers.Dialogs;
using Cs4rsa.Models;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.Crawlers;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.Interfaces;
using Cs4rsa.Utils;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.ViewModels.AutoScheduling
{
    internal sealed partial class ProgramTreeViewModel : ViewModelBase
    {
        private readonly MyProgramUC _myProgramUC;
        private readonly AccountUC _accountUC;

        private ProgramDiagram _programDiagram;

        public ObservableCollection<Student> Students { get; set; }
        public ObservableCollection<PlanTable> PlanTables { get; set; }
        public ObservableCollection<ProgramFolderModel> ProgramFolderModels { get; set; }

        [ObservableProperty]
        private bool _isFinding;

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
        private readonly ProgramDiagramCrawler _programDiagramCrawler;
        private readonly ColorGenerator _colorGenerator;

        public ProgramTreeViewModel(
            IUnitOfWork unitOfWork,
            IStudentPlanCrawler studentPlanCrawler,
            ProgramDiagramCrawler programDiagramCrawler,
            ColorGenerator colorGenerator
            )
        {
            _unitOfWork = unitOfWork;
            _studentPlanCrawler = studentPlanCrawler;
            _programDiagramCrawler = programDiagramCrawler;
            _colorGenerator = colorGenerator;

            ProgramFolderModels = new();
            PlanTables = new();
            Students = new();

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
            if (!PlanTables.Any())
            {
                Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await LoadStudentPlan();
                });
            }
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

        private async Task LoadStudentPlan()
        {
            if (_selectedStudent.StudentId.Equals("0")) return;
            PlanTables.Clear();
            IEnumerable<PlanTable> planTables = await _studentPlanCrawler.GetPlanTables(_selectedStudent.CurriculumId);
            foreach (PlanTable planTable in planTables)
            {
                PlanTables.Add(planTable);
            }
        }

        private async Task LoadProgramSubject()
        {
            ProgramFolderModels.Clear();

            IsFinding = !IsFinding;
            ProgramFolder[] folders = await _programDiagramCrawler.ToProgramDiagram(
                string.Empty,
                _selectedStudent.SpecialString,
                _selectedStudent.StudentId
            );
            IsFinding = !IsFinding;

            _programDiagram = new(folders[0], folders[1], folders[2], folders[3], _unitOfWork);
            await Task.WhenAll(
                AddProgramFolder(folders[0]),
                AddProgramFolder(folders[1]),
                AddProgramFolder(folders[2]),
                AddProgramFolder(folders[3])
            );
        }

        private async Task AddProgramFolder(ProgramFolder programFolder)
        {
            ProgramFolderModel programFolderModel = await ProgramFolderModel.CreateAsync(programFolder, _colorGenerator, _unitOfWork);
            ProgramFolderModels.Add(programFolderModel);
        }
    }
}
