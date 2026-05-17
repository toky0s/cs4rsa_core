using System;

namespace Cs4rsa.Module.ManuallySchedule.Models
{
    public class TimeConflictContext {
        public ClassGroupModel ClassGroupModel_A { get; }
        public ClassGroupModel ClassGroupModel_B { get; }
        public TimeConflictContext(ClassGroupModel a, ClassGroupModel b)
        {
            ClassGroupModel_A = a;
            ClassGroupModel_B = b;
        }
    }


    public enum WarningType
    {
        TimeConflict,
        PlaceConflict,
        EmptySeat
    }

    public class WarningModel
    {
        public string WarningTitle { get; set; }
        public string Description { get; set; }
        public WarningType WarningType { get; }

        private object _context;

        public object Context
        {
            get { return _context; }
            set
            {
                if (WarningType == WarningType.TimeConflict && typeof(TimeConflictContext) == value.GetType())
                {
                    _context = value;
                }
                else
                {
                    throw new ArgumentException($"Context must be of type {typeof(TimeConflictContext)}");
                }
            }
        }

        public WarningModel(WarningType type, string description, object context)
        {
            if (context.GetType() == typeof(TimeConflictContext))
            {
                _context = context;
            }
            else
            {
                throw new ArgumentException($"Context must be of type {typeof(TimeConflictContext)}");
            }
            WarningType = type;
            WarningTitle = ConvertTypeToTitle(type);
            Description = description;
        }

        public bool TryGetContext(out TimeConflictContext timeConflictContext)
        {
            if (WarningType == WarningType.TimeConflict && _context is TimeConflictContext context)
            {
                timeConflictContext = context;
                return true;
            }
            else
            {
                //throw new InvalidOperationException("Context is not of type TimeConflictContext");
                timeConflictContext = null;
                return false;
            }
        }

        private string ConvertTypeToTitle(WarningType type)
        {
            switch (type)
            {
                case WarningType.TimeConflict:
                    return "Trùng lịch";
                case WarningType.PlaceConflict:
                    return "Vị trí xa";
                case WarningType.EmptySeat:
                    return "Hết chỗ";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
