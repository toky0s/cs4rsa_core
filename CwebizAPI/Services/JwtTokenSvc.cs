/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using System.Text;
using CwebizAPI.Services.Interfaces;
using CwebizAPI.Services.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace CwebizAPI.Services;

/// <summary>
/// Service này chịu trách nhiệm tạo JWT token và xác thực.
/// </summary>
/// <remarks>
/// Created Date: 17/06/2023
/// Modified Date: 17/06/2023
/// Author: Truong A Xin
/// </remarks>
public class JwtTokenSvc : IJwtTokenSvc
{
    private readonly IConfiguration _configuration;
    // Thời gian hết hạn 30 phút
    private const int ExpiredTime = 30;
    private const string JwtRegisterSubject = "JwtForRegister:Subject";
    private const string JwtRegisterKey = "JwtForRegister:Key";
    private const string JwtRegisterIssuer = "JwtForRegister:Issuer";
    private const string JwtRegisterAudience = "JwtForRegister:Audience";

    public JwtTokenSvc(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    /// <summary>
    /// Tạo JWT cho việc đăng ký Student.
    /// </summary>
    /// <param name="registerToken">RegisterToken</param>
    /// <returns>JwtSecurityToken</returns>
    public JwtSecurityToken GetRegisterJwtSecurityToken(RegisterToken registerToken)
    {
        long iat = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
        long exp = (long)DateTime.UtcNow.AddMinutes(ExpiredTime).Subtract(DateTime.UnixEpoch).TotalSeconds;

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration[JwtRegisterKey]!));
        SigningCredentials signIn = new(key, SecurityAlgorithms.HmacSha256);

        JwtHeader jwtHeader = new(signIn);

        JwtPayload jwtPayload = new()
        {
            { JwtRegisteredClaimNames.Iss, _configuration[JwtRegisterIssuer] },
            { JwtRegisteredClaimNames.Aud, _configuration[JwtRegisterAudience] },
            { JwtRegisteredClaimNames.Sub, _configuration[JwtRegisterSubject] },
            { JwtRegisteredClaimNames.Iat, iat },
            { JwtRegisteredClaimNames.Exp, exp },
            { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
            { "StudentId", registerToken.StudentId!}
        };
        
        return new JwtSecurityToken(jwtHeader, jwtPayload);
    }

    /// <summary>
    /// Xác thực token.
    /// </summary>
    /// <param name="token">Chuỗi token.</param>
    /// <param name="validatedToken">Nếu validate thành công trả về validatedToken, ngược lại NULL</param>
    /// <param name="principal">Nếu validate thành công trả về principal, ngược lại NULL</param>
    /// <returns>Trả về true nếu validate thành công. Ngược lại trả về false.</returns>
    public bool ValidateToken(string token, out SecurityToken? validatedToken, out IPrincipal? principal)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();

        IPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
        if (claimsPrincipal.Identity is { IsAuthenticated: true })
        {
            principal = claimsPrincipal;
            return true;
        }
        validatedToken = default;
        principal = default;
        return false;
    }
    
    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[JwtRegisterKey]!)),
            ValidAlgorithms = new []{SecurityAlgorithms.HmacSha256},
            ValidIssuer = _configuration[JwtRegisterIssuer],
            ValidAudience = _configuration[JwtRegisterAudience],
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            RequireExpirationTime = true
        };
    }
}