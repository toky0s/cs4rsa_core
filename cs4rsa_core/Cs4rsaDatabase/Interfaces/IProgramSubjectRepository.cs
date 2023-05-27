using Cs4rsa.Cs4rsaDatabase.DTOs;
using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;

namespace Cs4rsa.Cs4rsaDatabase.Interfaces
{
    public interface IProgramSubjectRepository
    {
        bool ExistsByCourseId(string courseId);
        /// <summary>
        /// Lấy ra danh sách các ProgramSubject dựa theo Curriculum ID.
        /// </summary>
        /// <param name="crrId">Curriculum ID (mã ngành)</param>
        /// <returns>Danh sách các ProgramSubject</returns>
        List<DtoDbProgramSubject> GetDbProgramSubjectsByCrrId(int crrId);
        void Add(DbProgramSubject programSubject);
    }
}
