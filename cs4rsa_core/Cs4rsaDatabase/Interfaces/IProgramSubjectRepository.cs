﻿using cs4rsa_core.Cs4rsaDatabase.Models;

using System.Threading.Tasks;

namespace cs4rsa_core.Cs4rsaDatabase.Interfaces
{
    public interface IProgramSubjectRepository : IGenericRepository<ProgramSubject>
    {
        Task<ProgramSubject> GetByCourseIdAsync(string CourseId);

        Task<ProgramSubject> GetBySubjectCode(string subjectCode);
    }
}