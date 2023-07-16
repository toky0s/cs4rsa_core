/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Db.Interfaces;
using CwebizAPI.Share.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CwebizAPI.Db.Repos;

/// <summary>
/// User Repository
/// </summary>
/// <remarks>
/// Created Date: 21/06/2023
/// Modified Date: 21/06/2023
///                24/06/2023 : GetUserByUserName 
/// Author: Truong A Xin
/// </remarks>
public sealed class UserRepository : IUserRepository
{
    private readonly CwebizContext _cwebizDbContext;
    
    public UserRepository(CwebizContext cwebizDbContext)
    {
        _cwebizDbContext = cwebizDbContext;
    }
    
    public void Insert(CwebizUser cwebizUser)
    {
        _cwebizDbContext.CwebizUsers?.Add(cwebizUser);
    }
    
    public async Task<bool> ExistsByUserName(string username)
    {
        return await _cwebizDbContext.CwebizUsers.AnyAsync(u => u.Username.Equals(username));
    }
    
    public async Task<CwebizUser?> GetUserByUserName(string username)
    {
        return await (
            from u in _cwebizDbContext.CwebizUsers
            where u.Username.Equals(username)
            select u
        ).FirstOrDefaultAsync();
    }
    
    public async Task<CwebizUser?> GetUserById(int id)
    {
        return await (
            from u in _cwebizDbContext.CwebizUsers
            where u.Id == id
            select u
        ).FirstOrDefaultAsync();
    }

    public void Dispose()
    {
        _cwebizDbContext.Dispose();
    }
}