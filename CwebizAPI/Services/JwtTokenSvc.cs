/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using CwebizAPI.Services.Interfaces;
using CwebizAPI.Services.Tokens;
using CwebizAPI.Share.Database.Models;
using Microsoft.IdentityModel.Tokens;

namespace CwebizAPI.Services;

/// <summary>
/// Service này chịu trách nhiệm tạo JWT token và xác thực.
/// </summary>
/// <remarks>
/// Created Date: 17/06/2023
/// Modified Date:
/// 17/06/2023: Init
/// 24/06/2023: GetLoginJwtSecurityToken
/// Author: Truong A Xin
/// </remarks>
public class JwtTokenSvc : IJwtTokenSvc
{
    private readonly IConfiguration _configuration;

    #region Register Configs
    
    // Thời gian hết hạn 30 phút
    private const int RegisterExpiredTime = 30;
    private const string JwtRegisterSubject = "JwtForRegister:Subject";
    private const string JwtRegisterKey = "JwtForRegister:Key";
    private const string JwtRegisterIssuer = "JwtForRegister:Issuer";
    private const string JwtRegisterAudience = "JwtForRegister:Audience";
    
    #endregion

    #region Login Configs

    // Thời gian hết hạn 7 ngày
    private const int LoginExpiredTime = 7;
    private const string JwtRequestKey = "JwtForRequest:Key";
    private const string JwtRequestIssuer = "JwtForRequest:Issuer";
    private const string JwtRequestAudience = "JwtForRequest:Audience";

    #endregion
    
    public JwtTokenSvc(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public JwtSecurityToken GetRegisterJwtSecurityToken(RegisterToken registerToken)
    {
        long iat = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
        long exp = (long)DateTime.UtcNow.AddMinutes(RegisterExpiredTime).Subtract(DateTime.UnixEpoch).TotalSeconds;

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
            { "StudentId", registerToken.StudentId! }
        };

        return new JwtSecurityToken(jwtHeader, jwtPayload);
    }

    /// <summary>
    /// Hàm sinh Token cho Login.
    /// </summary>
    /// <param name="cwebizUser"></param>
    /// <returns>JwtSecurityToken</returns>
    public JwtSecurityToken GetLoginJwtSecurityToken(CwebizUser cwebizUser)
    {
        long iat = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
        long exp = (long)DateTime.UtcNow.AddDays(LoginExpiredTime).Subtract(DateTime.UnixEpoch).TotalSeconds;

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration[JwtRequestKey]!));
        SigningCredentials signIn = new(key, SecurityAlgorithms.HmacSha256);
        JwtHeader jwtHeader = new(signIn);

        JwtPayload jwtPayload = new()
        {
            { JwtRegisteredClaimNames.Iss, _configuration[JwtRequestIssuer] },
            { JwtRegisteredClaimNames.Aud, _configuration[JwtRequestAudience] },
            { JwtRegisteredClaimNames.Iat, iat },
            { JwtRegisteredClaimNames.Exp, exp },
            { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
        };
        
        jwtPayload.AddClaim(new Claim(ClaimTypes.Name, cwebizUser.Username));
        jwtPayload.AddClaim(new Claim(ClaimTypes.NameIdentifier, cwebizUser.Id.ToString()));
        
        return new JwtSecurityToken(jwtHeader, jwtPayload);
    }

    public bool ValidateToken(string token, out SecurityToken? validatedToken, out IPrincipal? principal)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        TokenValidationParameters validationParameters = GetValidationParameters();

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