using CommunityToolkit.Mvvm.ComponentModel;

using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;

using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Cs4rsa.Models.AutoScheduling
{
    public partial class PlanTableModel : ObservableObject
    {
        private readonly IUnitOfWork _unitOfWork;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private int _totalUnit;

        [ObservableProperty]
        private ObservableCollection<PlanRecordModel> _planRecordModels;

        private PlanTableModel(PlanTable planTable, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            PlanRecordModels = new();
            Name = planTable.Name;
            planTable.PlanRecords.ForEach(planRecord => _totalUnit += planRecord.StudyUnit);
        }

        private async Task<PlanTableModel> RenderPlanRecordModel(PlanTable planTable)
        {
            foreach (PlanRecord record in planTable.PlanRecords)
            {
                PlanRecordModel planRecordModel = new()
                {
                    StudyUnit = record.StudyUnit,
                    SubjectCode = record.SubjectCode,
                    SubjectName = record.SubjectName,
                    Url = record.Url,
                    IsAvailable = await _unitOfWork.Keywords.ExistBySubjectCodeAsync(record.SubjectCode),
                    IsSelected = false,
                };
                PlanRecordModels.Add(planRecordModel);
            }
            return this;
        }

        public static async Task<PlanTableModel> Build(PlanTable planTable, IUnitOfWork unitOfWork)
        {
            PlanTableModel planTableModel = new(planTable, unitOfWork);
            return await planTableModel.RenderPlanRecordModel(planTable);
        }
    }
}
