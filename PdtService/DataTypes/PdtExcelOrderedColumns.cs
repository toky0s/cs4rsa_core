using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PdtService.Commons;

namespace PdtService.DataTypes
{
    public class PdtExcelOrderedColumns
    {
        private bool _isValid = false;
        public bool IsValid { get => _isValid; }

        private readonly int _rowIndex;
        public int RowIndex 
        {
            get => _rowIndex;
        }

        private readonly int[] _colIndexs;
        public int[] ColIndexs
        {
            get => _colIndexs;
        }

        public PdtExcelOrderedColumns(int rowIndex)
        {
            _rowIndex = rowIndex;
            _colIndexs = new int[BaseCells.Count];
        }

        public void SetColIndex(int value, BaseCells baseCells)
        {
            _colIndexs[baseCells.BaseId] = value;
            _isValid = true;
        }
    }
}
