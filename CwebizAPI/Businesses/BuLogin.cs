/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.IdentityModel.Tokens.Jwt;
using CwebizAPI.Db.Interfaces;
using CwebizAPI.DTOs.Requests;
using CwebizAPI.DTOs.Responses;
using CwebizAPI.Services.Interfaces;
using CwebizAPI.Share.Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace CwebizAPI.Businesses;

/// <summary>
/// Business Login.
/// Tầng xử lý nghiệp vụ đăng nhập.
/// </summary>
/// <remarks>
/// Created Date: 24/06/2023
/// Modified Date: 24/06/2023
/// Author: Truong A Xin
/// </remarks>
public class BuLogin
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenSvc _jwtTokenSvc;
    public BuLogin(
        IUnitOfWork userRepository, 
        IJwtTokenSvc jwtTokenSvc)
    {
        _unitOfWork = userRepository;
        _jwtTokenSvc = jwtTokenSvc;
    }

    /// <summary>
    /// Xác thực đăng nhập.
    /// </summary>
    /// <param name="dtoRqLogin">Thông tin đăng nhập.</param>
    /// <returns>
    /// Nếu thành công trả về JWT.
    /// </returns>
    public async Task<IActionResult> Authenticate(DtoRqLogin dtoRqLogin)
    {
        // Kiểm tra người dùng tồn tại
        bool exist = await _unitOfWork.UserRepository!.ExistsByUserName(dtoRqLogin.Username!);
        if (!exist) return new ForbidResult();

        // Kiểm tra Password
        CwebizUser? cwebizUser = await _unitOfWork.UserRepository.GetUserByUserName(dtoRqLogin.Username!);
        bool matched = BCrypt.Net.BCrypt.Verify(
            dtoRqLogin.Password, 
            cwebizUser!.Password
        );
        if (!matched) return new ForbidResult();
        
        // Tạo JWT
        JwtSecurityToken jwtSecurityToken = _jwtTokenSvc.GetLoginJwtSecurityToken(cwebizUser);
        DtoRpLogin dtoRpLogin = new()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
        };
        return new OkObjectResult(dtoRpLogin);
    }
}
