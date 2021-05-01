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

namespace cs4rsa.ViewModels
{
    class StudyBlock
    {
        public ClassGroupModel ClassGroupModel { get; set; }
        public string ColorCodeLocation { get; set; }
        public string ColorClassGroup { get; set; }
    }

    class ScheduleTableViewModel: NotifyPropertyChangedBase
    {
        private List<ClassGroupModel> classGroupModels = new List<ClassGroupModel>();

        private List<ClassGroupModel> Phase1 = new List<ClassGroupModel>();
        private List<ClassGroupModel> Phase2 = new List<ClassGroupModel>();

        public DataTable Phase1Schedule = new DataTable();
        public DataTable Phase2Schedule = new DataTable();

        public ScheduleTableViewModel()
        {
        }

        private void AddClassGroup(ClassGroupModel classGroupModel)
        {
            classGroupModels.Add(classGroupModel);
            ReloadSchedule();
        }

        private void DeleteClassGroup(ClassGroupModel classGroupModel)
        {
            classGroupModels.Remove(classGroupModel);
            ReloadSchedule();
        }

        private void DivideClassGroupsByPhases()
        {
            foreach(ClassGroupModel classGroupModel in classGroupModels)
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

        private List<string> GetTimeString(List<ClassGroupModel> classGroupModels)
        {
            return GetShortedTimes(classGroupModels)
                .Select(item => item.NewTime.ToString("HH:mm"))
                .ToList();
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
                    ShortedTime shortedTimeEnd = converter.Convert(studyTime.End);
                    shortedTimes.Add(shortedTimeStart);
                    shortedTimes.Add(shortedTimeEnd);
                }
            }
            return shortedTimes;
        }

        private void ReloadSchedule()
        {
            DivideClassGroupsByPhases();
        }

        private void AddStudyBlock()
        {

        }
    }
}
