using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdtService.Commons
{
    public class BaseCells
    {
        public static readonly BaseCells STT = new(0, "STT", "SỐ THỨ TỰ");
        public static readonly BaseCells MSV = new(1, "MSV", "MASV", "MÃ SINH VIÊN", "MÃ SỐ SINH VIÊN");
        public static readonly BaseCells LAST_NAME = new(2, "HỌ VÀ", "HỌ");
        public static readonly BaseCells FIRST_NAME = new(3, "TÊN");
        public static readonly BaseCells CLASS = new(4, "LỚP", "LỚP SINH HOẠT");

        /// <summary>
        /// Trả về số lượng BaseCells đã được khởi tạo
        /// </summary>
        public static int Count;


        private readonly string[] _colNames;

        public readonly int BaseId;

        private BaseCells(int baseId, params string[] colNames)
        {
            ++Count;

            BaseId = baseId;
            _colNames = colNames;
        }

        public override bool Equals(object? obj)
        {
            return obj is BaseCells cells && BaseId == cells.BaseId;
        }

        /// <summary>
        /// So sánh matcher với một tên cột
        /// </summary>
        /// <param name="col">Tên cột</param>
        /// <returns>true nếu match, ngược lại trả về false.</returns>
        public bool Equals(string col)
        {
            return _colNames.Contains(col.ToUpper());
        }

        public override int GetHashCode()
        {
            return BaseId.GetHashCode() * _colNames.GetHashCode();
        }
    }
}
