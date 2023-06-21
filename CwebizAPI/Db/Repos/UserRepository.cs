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
/// Author: Truong A Xin
/// </remarks>
public sealed class UserRepository : IUserRepository
{
    private readonly CwebizContext _cwebizDbContext;
    
    public UserRepository(CwebizContext cwebizDbContext)
    {
        _cwebizDbContext = cwebizDbContext;
    }
    
    /// <summary>
    /// Thêm mới người dùng.
    /// </summary>
    /// <param name="cwebizUser">Thông tin người dùng.</param>
    public void Insert(CwebizUser cwebizUser)
    {
        _cwebizDbContext.CwebizUsers?.Add(cwebizUser);
    }

    /// <summary>
    /// Kiếm tra tồn tại bằng Username.
    /// </summary>
    /// <param name="username">Username</param>
    /// <returns>Trả về True nếu tồn tại, ngược lại trả về False.</returns>
    public async Task<bool> ExistsByUserName(string username)
    {
        return await _cwebizDbContext.CwebizUsers.AnyAsync(u => u.Username.Equals(username));
    }

    public void Dispose()
    {
        _cwebizDbContext.Dispose();
    }
}