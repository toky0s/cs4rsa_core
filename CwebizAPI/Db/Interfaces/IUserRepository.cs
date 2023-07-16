/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Share.Database.Models;

namespace CwebizAPI.Db.Interfaces;

public interface IUserRepository : IDisposable
{
    /// <summary>
    /// Thêm User vào hệ thống.
    /// </summary>
    /// <param name="cwebizUser">CwebizUser</param>
    void Insert(CwebizUser cwebizUser);

    /// <summary>
    /// Kiểm tra tồn tại bằng Username.
    /// </summary>
    /// <param name="username">Username</param>
    /// <returns>Trả về True nếu tồn tại, ngược lại trả về False.</returns>
    Task<bool> ExistsByUserName(string username);

    /// <summary>
    /// Tìm người dùng bằng username.
    /// </summary>
    /// <param name="username">Username</param>
    /// <returns>CwebizUser</returns>
    Task<CwebizUser?> GetUserByUserName(string username);
    
    /// <summary>
    /// Tìm người dùng bằng ID.
    /// </summary>
    /// <param name="id">CwebizUser ID.</param>
    /// <returns>CwebizUser</returns>
    Task<CwebizUser?> GetUserById(int id);
}