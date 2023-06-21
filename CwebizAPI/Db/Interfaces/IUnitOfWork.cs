/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Db.Interfaces
{
    /// <summary>
    /// IUnitOfWork
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public interface IUnitOfWork
    {
        public IDisciplineRepository? DisciplineRepository { get; }
        public IStudentRepository? StudentRepository { get; }
        public IUserRepository? UserRepository { get; }
        CwebizContext DbContext { get; }
        Task SaveChangeAsync();
    }
}
