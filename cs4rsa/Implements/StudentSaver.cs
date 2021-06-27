using cs4rsa.BasicData;
using cs4rsa.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.Database;

namespace cs4rsa.Implements
{
    class StudentSaver : IStudentSaver
    {
        /// <summary>
        /// Lưu thông tin của một sinh viên vào database.
        /// Đã bao gồm Update trong trường hợp trùng mã sinh viên.
        /// </summary>
        /// <param name="studentInfo"></param>
        /// <returns>Trả về 1 nếu là thêm mới, 0 nếu là cập nhật.</returns>
        public int Save(StudentInfo studentInfo)
        {
            if (!Cs4rsaDataView.IsStudentExists(studentInfo.SpecialString))
            {
                Cs4rsaDataEdit.AddStudent(studentInfo);
                return 1;
            }
            else
            {
                Cs4rsaDataEdit.UpdateStudent(studentInfo);
                return 0;
            }
        }
    }
}
