using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa_core.Models
{
    /// <summary>
    /// Mỗi session sẽ có nhiều class group được chọn, class này đại diện cho các class group đó.
    /// </summary>
    class ScheduleSessionDetail
    {
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string ClassGroup { get; set; }
    }
}
