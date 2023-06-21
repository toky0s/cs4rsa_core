/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Db.Interfaces;
using CwebizAPI.Db.Repos;
using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Db
{
    /// <summary>
    /// UnitOfWork
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IDisciplineRepository? _disciplineRepository;
        public IDisciplineRepository DisciplineRepository 
        { 
            get
            {
                _disciplineRepository ??= new DisciplineRepository(DbContext);
                return _disciplineRepository;
            }
        }

        private IStudentRepository? _studentRepository;
        public IStudentRepository? StudentRepository
        {
            get
            {
                _studentRepository ??= new StudentRepository(DbContext);
                return _studentRepository;
            }
        }

        private IUserRepository? _userRepository;

        public IUserRepository? UserRepository
        {
            get
            {
                _userRepository ??= new UserRepository(DbContext);
                return _userRepository;
            }
        }

        public CwebizContext DbContext { get; } = new();

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task SaveChangeAsync()
        {
            await DbContext.SaveChangesAsync();
        }
    }
}
