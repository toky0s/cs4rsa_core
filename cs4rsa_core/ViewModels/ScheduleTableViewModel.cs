using cs4rsa_core.BaseClasses;
using cs4rsa_core.Messages.Publishers;
using cs4rsa_core.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using cs4rsa_core.Commons.Interfaces;
using cs4rsa_core.Services.SubjectCrawlerSvc.Models;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;
using cs4rsa_core.Services.ConflictSvc.Models;
using System.Windows;
using System.Threading.Tasks;

namespace cs4rsa_core.ViewModels
{
    class ScheduleTableViewModel : ViewModelBase, IScheduleTableViewModel
    {
        private IEnumerable<ClassGroupModel> _classGroupModels;
        private IEnumerable<ConflictModel> _conflictModels;
        private IEnumerable<PlaceConflictFinderModel> _placeConflictFinderModels;

        public ObservableCollection<IScheduleTableItem> Phase1 { get; set; }
        public ObservableCollection<IScheduleTableItem> Phase2 { get; set; }

        public ScheduleTableViewModel()
        {
            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.ChoiceChangedMsg>(this, (r, m) =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _classGroupModels = m.Value;
                    ReloadSchedule();
                    return Task.CompletedTask;
                });
            });

            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.ConflictCollChangedMsg>(this, (r, m) =>
            {
                _conflictModels = m.Value;
                ReloadSchedule();
            });

            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.PlaceConflictCollChangedMsg>(this, (r, m) =>
            {
                _placeConflictFinderModels = m.Value;
                ReloadSchedule();
            });

            WeakReferenceMessenger.Default.Register<ChoicedSessionVmMsgs.DelClassGroupChoiceMsg>(this, (r, m) =>
            {
                _classGroupModels = m.Value;
                ReloadSchedule();
            });

            _classGroupModels = new List<ClassGroupModel>();
            _conflictModels = new List<ConflictModel>();
            _placeConflictFinderModels = new List<PlaceConflictFinderModel>();

            Phase1 = new();
            Phase2 = new();
        }

        private void DivideSchoolClassesByPhases()
        {
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                IEnumerable<SchoolClassModel> schoolClassModels;
                if (classGroupModel.IsSpecialClassGroup)
                {
                    schoolClassModels = classGroupModel
                    .CurrentSchoolClassModels
                    .Select(sc => GetSchoolClassModelCallback(sc.SchoolClass, classGroupModel.Color));
                } 
                else
                {
                    schoolClassModels = classGroupModel
                    .ClassGroup
                    .SchoolClasses
                    .Select(sc => GetSchoolClassModelCallback(sc, classGroupModel.Color));
                }

                foreach (SchoolClassModel schoolClassModel in schoolClassModels)
                {
                    switch (schoolClassModel.StudyWeek.GetPhase())
                    {
                        case Phase.First:
                            Phase1.Add(schoolClassModel);
                            break;
                        case Phase.Second:
                            Phase2.Add(schoolClassModel);
                            break;
                        case Phase.All:
                            Phase1.Add(schoolClassModel);
                            Phase2.Add(schoolClassModel);
                            break;
                        case Phase.Non:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void DivideConflictByPhase()
        {
            foreach (ConflictModel conflict in _conflictModels)
            {
                if (conflict.GetPhase() == Phase.First || conflict.GetPhase() == Phase.All)
                {
                    Phase1.Add(conflict);
                }
                else
                {
                    Phase2.Add(conflict);
                }
            }
        }

        private void DividePlaceConflictByPhase()
        {
            foreach (PlaceConflictFinderModel conflict in _placeConflictFinderModels)
            {
                if (conflict.GetPhase() == Phase.First || conflict.GetPhase() == Phase.All)
                {
                    Phase1.Add(conflict);
                }
                else
                {
                    Phase2.Add(conflict);
                }
            }
        }

        private static SchoolClassModel GetSchoolClassModelCallback(SchoolClass schoolClass, string color)
        {
            SchoolClassModel schoolClassModel = new(schoolClass)
            {
                Color = color
            };
            return schoolClassModel;
        }

        private void ReloadSchedule()
        {
            CleanPhase();
            DivideSchoolClassesByPhases();
            DividePlaceConflictByPhase();
            DivideConflictByPhase();
        }

        private void CleanPhase()
        {
            Phase1.Clear();
            Phase2.Clear();
        }
    }
}
