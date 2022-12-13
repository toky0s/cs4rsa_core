using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Dialogs.DialogViews;
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

            MyProgramCommand = new RelayCommand(() => OpenDialog(_myProgramUC), () => _selectedStudent != null && !_selectedStudent.StudentId.Equals("0"));
            _myProgramUC = new() { DataContext = this };

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
            IEnumerable<Student> students = await _unitOfWork.Students.GetAllAsync();
            foreach (Student student in students)
            {
                Students.Add(student);
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
            IsFinding = true;
            ProgramFolderModels.Clear();

            ProgramFolder[] folders = await _programDiagramCrawler.ToProgramDiagram(
                string.Empty,
                _selectedStudent.SpecialString,
                _selectedStudent.StudentId
            );

            foreach (ProgramFolder folder in folders)
            {
                await AddProgramFolder(folder);
            }

            IsFinding = false;
        }

        private async Task AddProgramFolder(ProgramFolder programFolder)
        {
            ProgramFolderModel programFolderModel = await ProgramFolderModel.CreateAsync(programFolder, _colorGenerator, _unitOfWork);
            ProgramFolderModels.Add(programFolderModel);
        }
    }
}
