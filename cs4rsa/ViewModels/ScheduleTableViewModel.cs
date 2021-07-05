using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Helpers;
using cs4rsa.Messages;
using cs4rsa.Models;
using cs4rsa.Models.Interfaces;
using cs4rsa.Settings;
using LightMessageBus;
using LightMessageBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace cs4rsa.ViewModels
{
    public class ScheduleRow
    {
        public ShortedTime Time { get; set; }
        public TimeBlock[] DayAndTimeBlock = new TimeBlock[7];
        public TimeBlock Sunday => DayAndTimeBlock[0];
        public TimeBlock Monday => DayAndTimeBlock[1];
        public TimeBlock Tuseday => DayAndTimeBlock[2];
        public TimeBlock Wednessday => DayAndTimeBlock[3];
        public TimeBlock Thursday => DayAndTimeBlock[4];
        public TimeBlock Friday => DayAndTimeBlock[5];
        public TimeBlock Saturday => DayAndTimeBlock[6];

        public ScheduleRow(ShortedTime time)
        {
            Time = time;
        }

        public void AddClassGroupModelToDayOfWeek(ClassGroupModel classGroupModel, DayOfWeek day)
        {
            int dayIndex = (int)day;
            DayAndTimeBlock[dayIndex] = new TimeBlock(classGroupModel);
        }

        public void AddStudyTimeIntersectToDayOfWeek(StudyTimeIntersect timeIntersect, DayOfWeek day)
        {
            int dayIndex = (int)day;
            DayAndTimeBlock[dayIndex] = new TimeBlock(timeIntersect);
        }
    }

    class ScheduleTableViewModel : NotifyPropertyChangedBase,
        IMessageHandler<ChoicesChangedMessage>,
        IMessageHandler<ConflictCollectionChangeMessage>,
        IMessageHandler<SettingChangeMessage>,
        IMessageHandler<DeleteClassGroupChoiceMessage>
    {
        private List<ClassGroupModel> classGroupModels = new List<ClassGroupModel>();
        private List<ConflictModel> _conflictModels = new List<ConflictModel>();
        private bool _settingIsDynamicSchedule;
        private bool _settingIsShowPlaceColor;

        private List<ClassGroupModel> Phase1 = new List<ClassGroupModel>();
        private List<ClassGroupModel> Phase2 = new List<ClassGroupModel>();

        private List<ConflictModel> ConflictPhase1 = new List<ConflictModel>();
        private List<ConflictModel> ConflictPhase2 = new List<ConflictModel>();

        public ObservableCollection<ScheduleRow> Schedule1 = new ObservableCollection<ScheduleRow>();
        public ObservableCollection<ScheduleRow> Schedule2 = new ObservableCollection<ScheduleRow>();

        public ScheduleTableViewModel()
        {
            // Load setting
            _settingIsDynamicSchedule = SettingReader.GetSetting(Setting.IsDynamicSchedule) == "1" ? true : false;
            _settingIsShowPlaceColor = SettingReader.GetSetting(Setting.IsShowPlaceColor) == "1" ? true : false;

            MessageBus.Default.FromAny().Where<ChoicesChangedMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteClassGroupChoiceMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<ConflictCollectionChangeMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<SettingChangeMessage>().Notify(this);
        }

        private void DeleteClassGroup(ClassGroupModel classGroupModel)
        {
            classGroupModels.Remove(classGroupModel);
            ReloadSchedule();
        }

        private void DivideClassGroupsByPhases()
        {
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                switch (classGroupModel.Phase)
                {
                    case Phase.FIRST:
                        Phase1.Add(classGroupModel);
                        break;
                    case Phase.SECOND:
                        Phase2.Add(classGroupModel);
                        break;
                    case Phase.ALL:
                        Phase1.Add(classGroupModel);
                        Phase2.Add(classGroupModel);
                        break;
                    case Phase.NON:
                        break;
                    default:
                        break;
                }
            }
        }

        private List<ShortedTime> GetShortedTimes(List<ClassGroupModel> classGroupModels)
        {
            ShortedTimeConverter converter = new ShortedTimeConverter();
            List<ShortedTime> shortedTimes = new List<ShortedTime>();
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                List<StudyTime> studyTimes = classGroupModel.Schedule.GetStudyTimes();
                foreach (StudyTime studyTime in studyTimes)
                {
                    ShortedTime shortedTimeStart = converter.Convert(studyTime.Start);
                    if (!shortedTimes.Contains(shortedTimeStart))
                        shortedTimes.Add(shortedTimeStart);

                    ShortedTime shortedTimeEnd = converter.Convert(studyTime.End);
                    if (!shortedTimes.Contains(shortedTimeEnd))
                        shortedTimes.Add(shortedTimeEnd);
                }
            }
            shortedTimes.Sort();
            return shortedTimes;
        }

        /// <summary>
        /// Main
        /// </summary>
        private void ReloadSchedule()
        {
            CleanPhase();
            CleanConflictPhase();
            CleanSchedules();
            DivideClassGroupsByPhases();
            Render(ref Schedule1, ref Phase1);
            Render(ref Schedule2, ref Phase2);
            DumpClassGroupModel(ref Schedule1, ref Phase1);
            DumpClassGroupModel(ref Schedule2, ref Phase2);
            DivideConflictByPhase();
            DumConflictModel(ref Schedule1, ref ConflictPhase1);
            DumConflictModel(ref Schedule2, ref ConflictPhase2);
        }

        private void DivideConflictByPhase()
        {
            foreach (ConflictModel conflict in _conflictModels)
            {
                if (conflict.GetPhase() == Phase.FIRST || conflict.GetPhase() == Phase.ALL)
                {
                    ConflictPhase1.Add(conflict);
                }
                else
                {
                    ConflictPhase2.Add(conflict);
                }
            }
        }


        /// <summary>
        /// Render ra các ScheduleRow có shorted Time trống.
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="classGroupModels"></param>
        private void Render(ref ObservableCollection<ScheduleRow> schedule, ref List<ClassGroupModel> classGroupModels)
        {
            // Dựa vào các shortedTime của các classGroups (render lịch động)
            List<ShortedTime> shortedTimes = GetShortedTimes(classGroupModels);
            // render schedule rows
            foreach (ShortedTime shortedTime in shortedTimes)
            {
                ScheduleRow scheduleRow = new ScheduleRow(shortedTime);
                schedule.Add(scheduleRow);
            }
        }

        private void DumpClassGroupModel(ref ObservableCollection<ScheduleRow> schedule, ref List<ClassGroupModel> classGroupModels)
        {
            foreach (ClassGroupModel cgm in classGroupModels)
            {
                AddClassGroup(ref schedule, cgm);
            }
        }

        private void DumConflictModel(ref ObservableCollection<ScheduleRow> schedule, ref List<ConflictModel> conflictModels)
        {
            foreach (ConflictModel conflictModel in conflictModels)
            {
                AddConflict(ref schedule, conflictModel);
            }
        }

        private void AddConflict(ref ObservableCollection<ScheduleRow> schedule, ConflictModel conflictModel)
        {
            ShortedTimeConverter converter = new ShortedTimeConverter();
            foreach (DayOfWeek day in conflictModel.ConflictTime.GetSchoolDays())
            {
                List<StudyTimeIntersect> timeIntersects = conflictModel.ConflictTime.GetStudyTimeIntersects(day);
                foreach (StudyTimeIntersect timeIntersect in timeIntersects)
                {
                    foreach (ScheduleRow scheduleRow in schedule)
                    {
                        if (scheduleRow.Time >= converter.Convert(timeIntersect.Start) &&
                            scheduleRow.Time <= converter.Convert(timeIntersect.End))
                        {
                            scheduleRow.AddStudyTimeIntersectToDayOfWeek(timeIntersect, day);
                        }
                    }
                }

            }
        }

        private void AddClassGroup(ref ObservableCollection<ScheduleRow> schedule, ClassGroupModel classGroupModel)
        {
            ShortedTimeConverter converter = new ShortedTimeConverter();
            foreach (DayOfWeek day in classGroupModel.Schedule.GetSchoolDays())
            {
                List<StudyTime> studyTimes = classGroupModel.Schedule.GetStudyTimesAtDay(day);
                foreach (StudyTime time in studyTimes)
                {
                    foreach (ScheduleRow scheduleRow in schedule)
                    {
                        if (scheduleRow.Time >= converter.Convert(time.Start) &&
                            scheduleRow.Time <= converter.Convert(time.End))
                        {
                            scheduleRow.AddClassGroupModelToDayOfWeek(classGroupModel, day);
                        }
                    }
                }
            }
        }

        private void CleanSchedules()
        {
            Schedule1.Clear();
            Schedule2.Clear();
        }

        private void CleanPhase()
        {
            Phase1.Clear();
            Phase2.Clear();
        }

        private void CleanConflictPhase()
        {
            ConflictPhase1.Clear();
            ConflictPhase2.Clear();
        }

        public void Handle(ChoicesChangedMessage message)
        {
            classGroupModels = message.Source;
            ReloadSchedule();
        }

        public void Handle(SettingChangeMessage message)
        {
            throw new NotImplementedException();
        }

        public void Handle(ConflictCollectionChangeMessage message)
        {
            _conflictModels = message.Source;
            ReloadSchedule();
        }

        public void Handle(DeleteClassGroupChoiceMessage message)
        {
            classGroupModels = message.Source;
            ReloadSchedule();
        }
    }
}
