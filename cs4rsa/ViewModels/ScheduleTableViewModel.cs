﻿using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Helpers;
using cs4rsa.Messages;
using cs4rsa.Models;
using LightMessageBus;
using LightMessageBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace cs4rsa.ViewModels
{
    public class ScheduleRow: NotifyPropertyChangedBase 
    {
        private ShortedTime _time;
        public ShortedTime Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                RaisePropertyChanged();
            }
        }

        private TimeBlock _sunday;
        private TimeBlock _monday;
        private TimeBlock _tuseday;
        private TimeBlock _wednessday;
        private TimeBlock _thursday;
        private TimeBlock _friday;
        private TimeBlock _saturday;

        public TimeBlock Sunday
        {
            get
            {
                return _sunday;
            }
            set
            {
                _sunday = value;
                RaisePropertyChanged();
            }
        }
        public TimeBlock Monday
        {
            get
            {
                return _monday;
            }
            set
            {
                _monday = value;
                RaisePropertyChanged();
            }
        }
        public TimeBlock Tuseday
        {
            get
            {
                return _tuseday;
            }
            set
            {
                _tuseday = value;
                RaisePropertyChanged();
            }
        }
        public TimeBlock Wednessday
        {
            get
            {
                return _wednessday;
            }
            set
            {
                _wednessday = value;
                RaisePropertyChanged();
            }
        }
        public TimeBlock Thursday
        {
            get
            {
                return _thursday;
            }
            set
            {
                _thursday = value;
                RaisePropertyChanged();
            }
        }
        public TimeBlock Friday
        {
            get
            {
                return _friday;
            }
            set
            {
                _friday = value;
                RaisePropertyChanged();
            }
        }
        public TimeBlock Saturday
        {
            get
            {
                return _saturday;
            }
            set
            {
                _saturday = value;
                RaisePropertyChanged();
            }
        }

        public ScheduleRow(ShortedTime time)
        {
            Time = time;
        }

        public void Add(ClassGroupModel classGroupModel, DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    Monday = new TimeBlock(classGroupModel);
                    break;
                case DayOfWeek.Tuesday:
                    Tuseday = new TimeBlock(classGroupModel);
                    break;
                case DayOfWeek.Wednesday:
                    Wednessday = new TimeBlock(classGroupModel);
                    break;
                case DayOfWeek.Thursday:
                    Thursday = new TimeBlock(classGroupModel);
                    break;
                case DayOfWeek.Friday:
                    Friday = new TimeBlock(classGroupModel);
                    break;
                case DayOfWeek.Saturday:
                    Saturday = new TimeBlock(classGroupModel);
                    break;
                case DayOfWeek.Sunday:
                    Sunday = new TimeBlock(classGroupModel);
                    break;
            }
        }

        public void Add(StudyTimeIntersect timeIntersect, DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    Monday = new TimeBlock(timeIntersect);
                    break;
                case DayOfWeek.Tuesday:
                    Tuseday = new TimeBlock(timeIntersect);
                    break;
                case DayOfWeek.Wednesday:
                    Wednessday = new TimeBlock(timeIntersect);
                    break;
                case DayOfWeek.Thursday:
                    Thursday = new TimeBlock(timeIntersect);
                    break;
                case DayOfWeek.Friday:
                    Friday = new TimeBlock(timeIntersect);
                    break;
                case DayOfWeek.Saturday:
                    Saturday = new TimeBlock(timeIntersect);
                    break;
                case DayOfWeek.Sunday:
                    Sunday = new TimeBlock(timeIntersect);
                    break;
            }
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
            _settingIsDynamicSchedule = false;

            MessageBus.Default.FromAny().Where<ChoicesChangedMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<DeleteClassGroupChoiceMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<ConflictCollectionChangeMessage>().Notify(this);
            MessageBus.Default.FromAny().Where<SettingChangeMessage>().Notify(this);

            // render tĩnh
            if (_settingIsDynamicSchedule == false)
            {
                CleanSchedules();
                RenderStatic(ref Schedule1);
                RenderStatic(ref Schedule2);
            }
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
            ShortedTimeConverter converter = ShortedTimeConverter.GetInstance();
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
        /// Mỗi khi có sự thay đổi về các class group được người dùng lựa chọn
        /// hay các xung đột được sinh ra thì phương thức này sẽ được tự động gọi lại.
        /// Việc gọi lại cũng sẽ phụ thuộc vào Setting là động hay tĩnh nhằm đảm bảo hiệu suất cho ứng dụng.
        /// </summary>
        private void ReloadSchedule()
        {
            if (_settingIsDynamicSchedule)
            {
                CleanPhase();
                CleanConflictPhase();
                CleanSchedules();
                DivideClassGroupsByPhases();
                RenderDynamic(ref Schedule1, ref Phase1);
                RenderDynamic(ref Schedule2, ref Phase2);
                DumpClassGroupModel(ref Schedule1, ref Phase1);
                DumpClassGroupModel(ref Schedule2, ref Phase2);
                DivideConflictByPhase();
                DumConflictModel(ref Schedule1, ref ConflictPhase1);
                DumConflictModel(ref Schedule2, ref ConflictPhase2);
            }
            else
            {
                CleanPhase();
                CleanConflictPhase();
                DivideClassGroupsByPhases();
                CleanStaticSchedule(ref Schedule1);
                CleanStaticSchedule(ref Schedule2);
                DumpClassGroupModel(ref Schedule1, ref Phase1);
                DumpClassGroupModel(ref Schedule2, ref Phase2);
                DivideConflictByPhase();
                DumConflictModel(ref Schedule1, ref ConflictPhase1);
                DumConflictModel(ref Schedule2, ref ConflictPhase2);
            }
        }


        /// <summary>
        /// Làm sạch tất cả các hiển thị trên mô phỏng,
        /// giữ lại các ScheduleRow nhằm tối đa hiệu năng và trải nghiệm người dùng.
        /// </summary>
        private void CleanStaticSchedule(ref ObservableCollection<ScheduleRow> schedule)
        {
            foreach(ScheduleRow row in schedule)
            {
                row.Monday = null;
                row.Tuseday = null;
                row.Wednessday = null;
                row.Thursday = null;
                row.Friday = null;
                row.Saturday = null;
                row.Sunday = null;
            }
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
        /// Render ra các ScheduleRow đi kèm với ShortedTime. Phương thức này
        /// render động dựa theo số lượng ClassGroup truyền vào cho nó.
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="classGroupModels"></param>
        private void RenderDynamic(ref ObservableCollection<ScheduleRow> schedule, ref List<ClassGroupModel> classGroupModels)
        {
            List<ShortedTime> shortedTimes = GetShortedTimes(classGroupModels);
            foreach (ShortedTime shortedTime in shortedTimes)
            {
                ScheduleRow scheduleRow = new ScheduleRow(shortedTime);
                schedule.Add(scheduleRow);
            }
        }

        /// <summary>
        /// Phương thức này như tên gọi, nó sẽ render ra Schedule tĩnh với các mốc
        /// thời gian học tiêu chuẩn. Phương thức này chỉ chạy hai lần trong mỗi vòng đời
        /// chỉ để render ra hay bảng.
        /// </summary>
        /// <param name="schedule"></param>
        private void RenderStatic(ref ObservableCollection<ScheduleRow> schedule)
        {
            List<ShortedTime> shortedTimes = GetStandardTimesAsShortedTime();
            foreach (ShortedTime shortedTime in shortedTimes)
            {
                ScheduleRow scheduleRow = new ScheduleRow(shortedTime);
                schedule.Add(scheduleRow);
            }
        }

        /// <summary>
        /// Lấy ra danh sách tất cả các mốc thồi gian có thể có và chuyển
        /// chúng thành các ShortedTime cố định.
        /// </summary>
        /// <returns></returns>
        private List<ShortedTime> GetStandardTimesAsShortedTime()
        {
            List<DateTime> dateTimes = new List<DateTime>()
            {
                TimeConverter.GetDateTimeFromString("07:00"),
                TimeConverter.GetDateTimeFromString("09:00"),
                TimeConverter.GetDateTimeFromString("09:15"),
                TimeConverter.GetDateTimeFromString("10:15"),
                TimeConverter.GetDateTimeFromString("11:15"),
                TimeConverter.GetDateTimeFromString("13:00"),
                TimeConverter.GetDateTimeFromString("14:00"),
                TimeConverter.GetDateTimeFromString("15:00"),
                TimeConverter.GetDateTimeFromString("15:15"),
                TimeConverter.GetDateTimeFromString("16:15"),
                TimeConverter.GetDateTimeFromString("17:15"),
                TimeConverter.GetDateTimeFromString("17:45"),
                TimeConverter.GetDateTimeFromString("18:45"),
                TimeConverter.GetDateTimeFromString("21:00"),
            };
            ShortedTimeConverter shortedTimeConverter = ShortedTimeConverter.GetInstance();
            return dateTimes.Select(time => shortedTimeConverter.Convert(time)).ToList();
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
            ShortedTimeConverter converter = ShortedTimeConverter.GetInstance();
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
                            scheduleRow.Add(timeIntersect, day);
                        }
                    }
                }

            }
        }

        private void AddClassGroup(ref ObservableCollection<ScheduleRow> schedule, ClassGroupModel classGroupModel)
        {
            ShortedTimeConverter converter = ShortedTimeConverter.GetInstance();
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
                            scheduleRow.Add(classGroupModel, day);
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
