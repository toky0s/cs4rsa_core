using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Interfaces
{
    /// <summary>
    /// Tất cả các Crawler muốn lấy ra một Basic Data hay Model đều phải thông qua
    /// một implement của interface này, không thao tác trực tiếp trên Cs4rsaDataView hay Cs4rsaDataEdit.
    /// <para>Interface này sinh ra để sử dụng trong một concept chung</para> 
    /// <para>Một crawler cần một child object cho một object cha nào đó, mà child object
    /// đó được lấy từ database nếu có, hoặc lấy thông qua một crawler khác nhằm đảm bảo hiệu suất.</para>
    /// <para>Như các teacher trong các class group, hoặc các môn tiên quyết và song hành trong chương trình học.</para>
    /// </summary>
    /// <typeparam name="T">Là một Basic Data hoặc Model.</typeparam>
    public interface IGetter<T>
    {
        T Get(object[] parameters=null);
    }
}
