using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Interfaces
{
    /// <summary>
    /// Implement của interface này thực hiện thao tác trên một dữ liệu
    /// nào đó của crawler vào database. Các crawler muốn save data
    /// phải implement interface này chứ không sử dụng trực tiếp
    /// Cs4rsaDataView hoặc Cs4rsaDataEdit.
    /// </summary>
    /// <typeparam name="T">Một BasicData hoặc một Model.</typeparam>
    public interface ISaver<T>
    {
        void Save(T obj);
    }
}
