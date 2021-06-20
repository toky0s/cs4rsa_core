using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Models.Enums
{
    /// <summary>
    /// Đại diện cho kiểu của một xung đột có thể là xung đột thời gian
    /// hoặc có thể là một xung đột vị trí.
    /// </summary>
    public enum ConflictType
    {
        Place,
        Time
    }
}
