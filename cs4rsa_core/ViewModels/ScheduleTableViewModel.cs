using ConflictService.Models;

using cs4rsa_core.BaseClasses;
using cs4rsa_core.Messages;
using cs4rsa_core.Models;
using cs4rsa_core.ViewModels.Interfaces;

using Cs4rsaCommon.Interfaces;

using LightMessageBus;
using LightMessageBus.Interfaces;

using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.DataTypes.Enums;
using SubjectCrawlService1.Models;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace cs4rsa_core.ViewModels
{
    class ScheduleTableViewModel : ViewModelBase, IScheduleTableViewModel,
        IMessageHandler<ChoicesChangedMessage>,
        IMessageHandler<ConflictCollectionChangeMessage>,
        IMessageHandler<PlaceConflictCollectionChangeMessage>,
        IMessageHandler<DeleteClassGroupChoiceMessage>
    {
        private List<ClassGroupModel> _classGroupModels;
        private List<ConflictModel> _conflictModels;
        private List<PlaceConflictFinderModel> _placeConflictFinderModels;

        public ObservableCollection<IScheduleTableItem> Phase1 { get; set; }
        public ObservableCollection<IScheduleTableItem> Phase2 { get; set; }

        public ScheduleTableViewModel()
        {
            MessageBus.Default.FromAny().Where<ChoicesChangedMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<ConflictCollectionChangeMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<PlaceConflictCollectionChangeMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteClassGroupChoiceMessage>().Notify(this);

            _classGroupModels = new();
            _conflictModels = new();
            _placeConflictFinderModels = new();
            Phase1 = new();
            Phase2 = new();
        }

        private void DivideSchoolClassesByPhases()
        {
            foreach (ClassGroupModel classGroupModel in _classGroupModels)
            {
                List<SchoolClassModel> schoolClassModels = classGroupModel
                    .ClassGroup
                    .SchoolClasses
                    .Select(sc => GetSchoolClassModelCallback(sc, classGroupModel.Color))
                    .ToList();
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
            SchoolClassModel schoolClassModel = new(schoolClass);
            schoolClassModel.Color = color;
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

        public void Handle(ChoicesChangedMessage message)
        {
            _classGroupModels = message.Source;
            ReloadSchedule();
        }

        public void Handle(ConflictCollectionChangeMessage message)
        {
            _conflictModels = message.Source;
            ReloadSchedule();
        }

        public void Handle(DeleteClassGroupChoiceMessage message)
        {
            _classGroupModels = message.Source;
            ReloadSchedule();
        }

        public void Handle(PlaceConflictCollectionChangeMessage message)
        {
            _placeConflictFinderModels = message.Source;
            ReloadSchedule();
        }
    }
}
