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

    public class ScheduleRow
    {
        public string Time { get; set; }
        public string[] DayAndClassGroups = new string[7];

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

    class StudyBlock
    {
        public ClassGroupModel ClassGroupModel { get; set; }
        public string ColorCodeLocation { get; set; }
        public string ColorClassGroup { get; set; }
    }

    class ScheduleTableViewModel : NotifyPropertyChangedBase,
        IMessageHandler<ChoicesAddChangedMessage>
    {
        private List<ClassGroupModel> classGroupModels = new List<ClassGroupModel>();

        private List<ClassGroupModel> Phase1 = new List<ClassGroupModel>();
        private List<ClassGroupModel> Phase2 = new List<ClassGroupModel>();

        public ObservableCollection<ScheduleRow> Phase1Schedule;
        public ObservableCollection<ScheduleRow> Phase2Schedule;

        public ScheduleTableViewModel()
        {
            MessageBus.Default.FromAny().Where<ChoicesAddChangedMessage>().Notify(this);
            Phase1Schedule = RenderDataTable();
            Phase2Schedule = RenderDataTable();
        }

        private DataTable RenderDataTable()
        {
            DataTable table = new DataTable();
            DataColumn time = new DataColumn("Times", typeof(string));
            DataColumn T2 = new DataColumn("T2", typeof(string));
            DataColumn T3 = new DataColumn("T3", typeof(string));
            DataColumn T4 = new DataColumn("T4", typeof(string));
            DataColumn T5 = new DataColumn("T5", typeof(string));
            DataColumn T6 = new DataColumn("T6", typeof(string));
            DataColumn T7 = new DataColumn("T7", typeof(string));
            DataColumn CN = new DataColumn("CN", typeof(string));

            table.Columns.Add(time);
            table.Columns.Add(T2);
            table.Columns.Add(T3);
            table.Columns.Add(T4);
            table.Columns.Add(T5);
            table.Columns.Add(T6);
            table.Columns.Add(T7);
            table.Columns.Add(CN);

            return table;
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
            PaintTimeStringToTable(Phase1, Phase1Schedule);
            PaintTimeStringToTable(Phase2, Phase2Schedule);
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
