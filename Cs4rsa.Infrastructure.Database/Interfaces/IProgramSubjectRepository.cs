using System.Collections.Generic;
using Cs4rsa.Database.DTOs;
using Cs4rsa.Database.Models;

namespace Cs4rsa.Database.Interfaces
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
