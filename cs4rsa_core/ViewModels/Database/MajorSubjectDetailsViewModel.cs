using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.DTOs;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Messages.Publishers;
using Cs4rsa.Models.Database;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Cs4rsa.ViewModels.Database
{
    public partial class MajorSubjectDetailsViewModel : ViewModelBase
    {
        public ObservableCollection<DtoDbProgramSubject> DbProgramSubjects { get; set; }
        public ObservableCollection<PlanTable> PlanTables { get; set; }

        [ObservableProperty]
        private MajorSubjectModel _selectedMSubjectModel;

        private readonly IUnitOfWork _unitOfWork;

        public MajorSubjectDetailsViewModel(
            IUnitOfWork unitOfWork
        )
        {
            _unitOfWork = unitOfWork;

            DbProgramSubjects = new();
            PlanTables = new();

            Messenger.Register<DbVmMsgs.SelectMajorMsg>(this, (recipient, msg) =>
            {
                SelectedMSubjectModel = msg.Value;
                LoadStudentPlan();
                DbProgramSubjects.Clear();
                IEnumerable<DtoDbProgramSubject> result = _unitOfWork
                                                .ProgramSubjects
                                                .GetDbProgramSubjectsByCrrId(msg.Value.Curriculum.CurriculumId);
                foreach (DtoDbProgramSubject dtoDb in result)
                {
                    DbProgramSubjects.Add(dtoDb);
                }
            });
        }

        private void LoadStudentPlan()
        {
            string planPath = CredizText.PathPlanJsonFile(SelectedMSubjectModel.Curriculum.CurriculumId);
            if (File.Exists(planPath))
            {
                string json = File.ReadAllText(planPath);
                PlanTable[] planTables = JsonConvert.DeserializeObject<PlanTable[]>(json);
                PlanTables.Clear();
                for (int i = 0; i < planTables.Length; ++i)
                {
                    CalTotalCredit(ref planTables[i]);
                    planTables[i].PlanRecords.ForEach(pr => AddColorForPlanRecord(ref pr));
                    PlanTables.Add(planTables[i]);
                }
            }
            else
            {
                MessageBox.Show(
                    CredizText.DbMsg002(SelectedMSubjectModel.Curriculum.CurriculumId.ToString()),
                    ViewConstants.Screen03.MenuName,
                    MessageBoxButton.OK
                );
            }
        }

        private void AddColorForPlanRecord(ref PlanRecord planRecord)
        {
            planRecord.Color = _unitOfWork.Keywords.GetColorBySubjectCode(planRecord.SubjectCode);
        }

        private static void CalTotalCredit(ref PlanTable planTable)
        {
            int total = 0;
            foreach (PlanRecord pr in planTable.PlanRecords)
            {
                total += pr.StudyUnit;
            }
            planTable.TotalUnit = total;
        }
    }
}
