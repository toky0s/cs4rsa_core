using Cs4rsa.Services.ConflictSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils.Models;

using System.Collections.Generic;

namespace Cs4rsa.Interfaces
{
    public enum ScheduleTableItemType
    {
        SchoolClass,
        TimeConflict,
        PlaceConflict,
    }

    public class ScheduleItemId
    {
        private readonly string _space;
        private readonly string _value;

        public string Value
        {
            get
            {
                return _space + _value;
            }
        }

        /// <summary>
        /// Space của một SchoolClassModel tương ứng với SubjectCode của nó.
        /// </summary>
        public string Space { get => _space; }

        public static ScheduleItemId Of(SchoolClassModel schoolClassModel)
        {
            return new ScheduleItemId(schoolClassModel.SubjectCode, schoolClassModel.SchoolClassName);
        }

        public static IEnumerable<ScheduleItemId> Of(ClassGroupModel classGroupModel)
        {
            foreach (SchoolClassModel schoolClassModel in classGroupModel.CurrentSchoolClassModels)
            {
                yield return Of(schoolClassModel);
            }
        }

        public static ScheduleItemId Of(ConflictModel conflictModel)
        {
            string space = conflictModel.FirstSchoolClass.SchoolClassName + conflictModel.SecondSchoolClass.SchoolClassName;
            string value = conflictModel.GetHashCode().ToString();
            return new ScheduleItemId(space, value);
        }

        public static ScheduleItemId Of(PlaceConflictFinderModel placeConflictFinderModel)
        {
            string space = placeConflictFinderModel.FirstSchoolClass.SchoolClassName + placeConflictFinderModel.SecondSchoolClass.SchoolClassName;
            string value = placeConflictFinderModel.GetHashCode().ToString();
            return new ScheduleItemId(space, value);
        }

        private ScheduleItemId(string space, string value)
        {
            _space = space;
            _value = value;
        }

        public bool IsSameSpace(ScheduleItemId other)
        {
            return other._space.Equals(_space);
        }

        private bool Equals(ScheduleItemId other)
        {
            return other != null && (_space + _value).Equals(other._space + other._value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not ScheduleItemId) return false;
            return Equals(obj as ScheduleItemId);
        }

        public static bool operator ==(ScheduleItemId scheduleItemId, ScheduleItemId other)
        {
            if (ReferenceEquals(null, scheduleItemId)|| ReferenceEquals(null, other)) return false;
            return scheduleItemId.Equals(other);
        }

        public static bool operator !=(ScheduleItemId scheduleItemId, ScheduleItemId other)
        {
            if (ReferenceEquals(null, scheduleItemId) || ReferenceEquals(null, other)) return true;
            return !scheduleItemId.Equals(other);
        }

        public override int GetHashCode()
        {
            return (_space + _value).GetHashCode();
        }
    }

    /// <summary>
    /// Buộc phải triển khai Interface này nếu muốn hiển thị một 
    /// khối thời gian trên ScheduleTable.
    /// </summary>
    public interface IScheduleTableItem
    {
        /// <summary>
        /// Trả về một danh sách khối thời gian sẽ được vẽ trên bảng mô phỏng.
        /// </summary>
        /// <returns>
        /// <see cref="IEnumerable{T}"/>
        /// </returns>
        IEnumerable<TimeBlock> GetBlocks();

        /// <summary>
        /// Implement phương thức này để xác định khối thời gian sẽ thuộc giai đoạn nào.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        ///     <item>Phase.First: Khối thời gian sẽ được vẽ trên bảng đầu tiên.</item>
        ///     <item>Phase.Second: Khối thời gian sẽ được vẽ trên bảng thứ hai.</item>
        ///     <item>Phase.All: Khối thời gian sẽ được vẽ trên cả hai bảng.</item>
        /// </list>
        /// </returns>
        Phase GetPhase();

        ScheduleTableItemType GetScheduleTableItemType();

        ScheduleItemId GetScheduleItemId();
    }
}
