using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Helpers
{
    /// <summary>
    /// Bộ phát hiện mã môn trong một chuỗi.
    /// </summary>
    public class Subjectidentifier
    {
        private static readonly Subjectidentifier instance = new Subjectidentifier();

        private Subjectidentifier() { }

        public static Subjectidentifier GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// Phương thức này trả về null khi không phát hiện mã môn trong chuỗi truyền vào.
        /// </summary>
        /// <param name="text">Chuỗi cần tìm</param>
        /// <returns>Mã môn</returns>
        public string GetSubjectCode(string text)
        {
            return null;
        }
    }
}
