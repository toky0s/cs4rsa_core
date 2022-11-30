using Cs4rsa.Services.ConflictSvc.DataTypes;
using Cs4rsa.Services.ConflictSvc.Models;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Models;
using Cs4rsa.Utils.Models;

using System.Collections.Generic;

namespace Cs4rsa.Utils.Interfaces
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

        public static ScheduleItemId FromSchoolClassModel(SchoolClassModel schoolClassModel)
        {
            return new ScheduleItemId(schoolClassModel.SubjectCode, schoolClassModel.SchoolClassName);
        }

        public static IEnumerable<ScheduleItemId> FromClassGroupModel(ClassGroupModel classGroupModel)
        {
            foreach (SchoolClassModel schoolClassModel in classGroupModel.CurrentSchoolClassModels)
            {
                yield return new ScheduleItemId(schoolClassModel.SubjectCode, schoolClassModel.SchoolClassName);
            }
        }

        public static ScheduleItemId FromTimeConflict(ConflictModel conflictModel, StudyTimeIntersect studyTimeIntersect)
        {
            string space = conflictModel.FirstSchoolClass.SchoolClassName + conflictModel.SecondSchoolClass.SchoolClassName;
            string value = studyTimeIntersect.ToString();
            return new ScheduleItemId(space, value);
        }

        public static ScheduleItemId FromPlaceConflict(PlaceConflictFinderModel placeConflictFinderModel, PlaceAdjacent placeAdjacent)
        {
            string space = placeConflictFinderModel.FirstSchoolClass.SchoolClassName + placeConflictFinderModel.SecondSchoolClass.SchoolClassName;
            string value = placeAdjacent.ToString();
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

        public bool Equals(ScheduleItemId other)
        {
            return other != null && (_space + _value).Equals(other._space + other._value);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ScheduleItemId);
        }

        public override int GetHashCode()
        {
            return (_space + _value).GetHashCode();
        }
    }

    /**
     * Mô tả:
     *      Buộc phải triển khai Interface này nếu muốn hiển thị một 
     *      khối thời gian trên ScheduleTable.
     */
    public interface IScheduleTableItem
    {
        /**
         * Mô tả:
         *      Trả về một danh sách khối thời gian sẽ được vẽ trên bảng mô phỏng.
         */
        IEnumerable<TimeBlock> GetBlocks();


        /// <summary>
        /// Mô tả:
        ///      Implement phương thức này để xác định khối thời gian sẽ thuộc giai đoạn nào.
        /// 
        /// 
        /// Trả về:
        ///      Phase.First:
        ///          Khối thời gian sẽ được vẽ trên bảng đầu tiên.
        ///          
        ///      Phase.Second:
        ///          Khối thời gian sẽ được vẽ trên bảng thứ hai.
        ///          
        ///      Phase.All:
        ///          Khối thời gian sẽ được vẽ trên cả hai bảng.
        /// </summary>
        Phase GetPhase();

        ScheduleTableItemType GetScheduleTableItemType();
    }
}
