using PdtService.DataTypes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdtService.Interfaces
{
    /// <summary>
    /// Trình phần tích Excel lấy ra thông tin sinh viên có trong
    /// các file danh sách thi.
    /// </summary>
    internal interface IPdtExcelParser
    {
        IEnumerable<StudentRecord> Get(string excelFilePath);
    }
}
