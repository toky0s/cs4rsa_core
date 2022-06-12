namespace Cs4rsaCommon.Enums
{
    /**
     * Enum này xác định style hiển thị của một
     * Time Block ngay trên bộ mô phỏng lịch.
     */
    public class BlockType
    {
        public static readonly BlockType SchoolClass = new(1);
        public static readonly BlockType TimeConflict = new(2);
        public static readonly BlockType PlaceConflict = new(3);

        private int _value;

        private BlockType(int value)
        {
            _value = value;
        }

        public bool Equals(BlockType block)
        {
            return _value == block._value;
        }
    }
}
