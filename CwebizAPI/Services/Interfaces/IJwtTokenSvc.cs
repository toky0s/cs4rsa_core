/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using CwebizAPI.Services.Tokens;
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
    /// Thực hiện xác thực Token.
    /// </summary>
    /// <param name="token">Thông tin token.</param>
    /// <param name="validatedToken">SecurityToken</param>
    /// <param name="principal">IPrincipal</param>
    /// <returns>Trả về True nếu xác thực thành công, ngược lại trả về False.</returns>
    bool ValidateToken(string token, out SecurityToken? validatedToken, out IPrincipal? principal);
}