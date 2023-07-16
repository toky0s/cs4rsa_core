/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using CwebizAPI.Services.Tokens;
using CwebizAPI.Share.Database.Models;
using Microsoft.IdentityModel.Tokens;

namespace CwebizAPI.Services.Interfaces;

/// <summary>
/// JSON Web Token Service
/// </summary>
/// <remarks>
/// Created Date: 21/06/2023
/// Modified Date: 21/06/2023
/// Author: Truong A Xin
/// </remarks>
public interface IJwtTokenSvc
{
    /// <summary>
    /// Lấy ra JwtSecurityToken
    /// </summary>
    /// <param name="registerToken">RegisterToken</param>
    /// <returns>JwtSecurityToken</returns>
    JwtSecurityToken GetRegisterJwtSecurityToken(RegisterToken registerToken);
    
    /// <summary>
    /// Lấy ra JwtSecurityToken cho Login
    /// </summary>
    /// <param name="cwebizUser">Thông tin người dùng.</param>
    /// <returns>JwtSecurityToken</returns>
    JwtSecurityToken GetLoginJwtSecurityToken(CwebizUser cwebizUser);
    
    /// <summary>
    /// Xác thực token.
    /// </summary>
    /// <param name="token">Chuỗi token.</param>
    /// <param name="validatedToken">Nếu validate thành công trả về validatedToken, ngược lại NULL</param>
    /// <param name="principal">Nếu validate thành công trả về principal, ngược lại NULL</param>
    /// <returns>Trả về true nếu validate thành công. Ngược lại trả về false.</returns>
    bool ValidateToken(string token, out SecurityToken? validatedToken, out IPrincipal? principal);
}