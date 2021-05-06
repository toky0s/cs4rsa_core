using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Models;
using cs4rsa.Helpers;
using cs4rsa.Messages;
using LightMessageBus;
using LightMessageBus.Interfaces;

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
        private string[] DayAndClassGroups = new string[7];
        public string Sunday => DayAndClassGroups[0];
        public string Monday => DayAndClassGroups[1];
        public string Tuseday => DayAndClassGroups[2];
        public string Wednessday => DayAndClassGroups[3];
        public string Thursday => DayAndClassGroups[4];
        public string Friday => DayAndClassGroups[5];
        public string Saturday => DayAndClassGroups[6];

        public ScheduleRow(ShortedTime time)
        {
            Time = time;
        }

        public void AddClassGroupModel(ClassGroupModel classGroupModel)
        {
            Schedule schedule = classGroupModel.Schedule;
            
            foreach(DayOfWeek day in schedule.GetSchoolDays())
            {
                int dayIndex = (int)day;
                DayAndClassGroups[dayIndex] = classGroupModel.Name;
            }
        }
    }

    class ScheduleTableViewModel : NotifyPropertyChangedBase,
        IMessageHandler<ChoicesAddChangedMessage>
    {
        private List<ClassGroupModel> classGroupModels = new List<ClassGroupModel>();

        private List<ClassGroupModel> Phase1 = new List<ClassGroupModel>();
        private List<ClassGroupModel> Phase2 = new List<ClassGroupModel>();

        public ObservableCollection<ScheduleRow> Phase1Schedule = new ObservableCollection<ScheduleRow>();
        public ObservableCollection<ScheduleRow> Phase2Schedule = new ObservableCollection<ScheduleRow>();

        public ScheduleTableViewModel()
        {
            MessageBus.Default.FromAny().Where<ChoicesAddChangedMessage>().Notify(this);
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

        private void PaintTimeStringToTable(List<ClassGroupModel> source, DataTable destination)
        {
            List<string> timeString = GetTimeString(source);
            foreach (string time in timeString)
            {
                DataRow row = destination.NewRow();
                row[0] = time;
                destination.Rows.Add(row);
            }
        }

        private List<string> GetTimeString(List<ClassGroupModel> classGroupModels)
        {
            List<string> timeStrings = new List<string>();
            foreach (ShortedTime item in GetShortedTimes(classGroupModels))
            {
                string timeString = item.NewTime.ToString("HH:mm");
                timeStrings.Add(timeString);
            }
            return timeStrings;
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
            Render(ref Phase1Schedule, ref Phase1);
            Render(ref Phase2Schedule, ref Phase2);
            DumpClassGroupModel(ref Phase1Schedule, ref Phase1);
            DumpClassGroupModel(ref Phase2Schedule, ref Phase2);
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
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                AddClassGroup(ref schedule, classGroupModel);
            }
        }

        private void AddClassGroup(ref ObservableCollection<ScheduleRow> schedule, ClassGroupModel classGroupModel)
        {
            ShortedTimeConverter converter = new ShortedTimeConverter();
            List<StudyTime> studyTimes = classGroupModel.Schedule.GetStudyTimes();
            foreach (ScheduleRow scheduleRow in schedule)
            {
                if (converter.ToShortedTime(studyTimes).Contains(scheduleRow.Time))
                {
                    scheduleRow.AddClassGroupModel(classGroupModel);
                }
            }
        }

        public void Handle(ChoicesAddChangedMessage message)
        {
            classGroupModels = message.Source;
            ReloadSchedule();
        }

        private void CleanSchedules()
        {
            Phase1Schedule.Clear();
            Phase2Schedule.Clear();
        }

        private void CleanPhase()
        {
            Phase1.Clear();
            Phase2.Clear();
        }
    }
}
