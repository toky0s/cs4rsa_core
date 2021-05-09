using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Helpers;
using cs4rsa.Messages;
using cs4rsa.Models;
using LightMessageBus;
using LightMessageBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace cs4rsa.ViewModels
{
    /// <summary>
    /// Đại diện cho một ô trong ScheduleRow bao gồm các thuộc tính về giao diện.
    /// </summary>
    public class TimeBlock
    {
        public string BackgroundColor { get; set; }
        public string Name { get; set; }
    }


    public class ScheduleRow
    {
        public ShortedTime Time { get; set; }
        private ClassGroupModel[] DayAndClassGroups = new ClassGroupModel[7];
        public ClassGroupModel Sunday => DayAndClassGroups[0];
        public ClassGroupModel Monday => DayAndClassGroups[1];
        public ClassGroupModel Tuseday => DayAndClassGroups[2];
        public ClassGroupModel Wednessday => DayAndClassGroups[3];
        public ClassGroupModel Thursday => DayAndClassGroups[4];
        public ClassGroupModel Friday => DayAndClassGroups[5];
        public ClassGroupModel Saturday => DayAndClassGroups[6];

        public ScheduleRow(ShortedTime time)
        {
            Time = time;
        }

        public void AddClassGroupModelToDayOfWeek(ClassGroupModel classGroupModel, DayOfWeek day)
        {
            int dayIndex = (int)day;
            DayAndClassGroups[dayIndex] = classGroupModel;
        }
    }

    class ScheduleTableViewModel : NotifyPropertyChangedBase,
        IMessageHandler<ChoicesChangedMessage>
    {
        private List<ClassGroupModel> classGroupModels = new List<ClassGroupModel>();

        private List<ClassGroupModel> Phase1 = new List<ClassGroupModel>();
        private List<ClassGroupModel> Phase2 = new List<ClassGroupModel>();

        public ObservableCollection<ScheduleRow> Schedule1 = new ObservableCollection<ScheduleRow>();
        public ObservableCollection<ScheduleRow> Schedule2 = new ObservableCollection<ScheduleRow>();

        public ScheduleTableViewModel()
        {
            MessageBus.Default.FromAny().Where<ChoicesChangedMessage>().Notify(this);
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

        private void ReloadSchedule()
        {
            CleanPhase();
            CleanSchedules();
            DivideClassGroupsByPhases();
            Render(ref Schedule1, ref Phase1);
            Render(ref Schedule2, ref Phase2);
            DumpClassGroupModel(ref Schedule1, ref Phase1);
            DumpClassGroupModel(ref Schedule2, ref Phase2);
        }

        private void Render(ref ObservableCollection<ScheduleRow> schedule, ref List<ClassGroupModel> classGroupModels)
        {
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
                            scheduleRow.AddClassGroupModelToDayOfWeek(classGroupModel, day);
                    }
                }
            }
        }

        public void Handle(ChoicesChangedMessage message)
        {
            classGroupModels = message.Source;
            ReloadSchedule();
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
    }
}
