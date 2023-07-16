/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Businesses;
using CwebizAPI.Settings;
using CwebizAPI.Share.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CwebizAPI.Controllers;

/// <summary>
/// Login controller.
/// </summary>
/// <remarks>
/// Created Date: 15/07/2023
/// Modified Date: 15/07/2023
/// Author: Truong A Xin
/// </remarks>
[AllowAnonymous]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[EnableCors(Policies.CredizBlazorPolicy)]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly BuLogin _buLogin;
    
    public LoginController(BuLogin buLogin)
    {
        _buLogin = buLogin;
    }

    /// <summary>
    /// Đăng nhập.
    /// </summary>
    /// <param name="dtoRqLogin">Thông tin đăng nhập.</param>
    /// <returns>IActionResult</returns>
    /// <exception cref="BadHttpRequestException"></exception>
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] DtoRqLogin dtoRqLogin)
    {
        if (ModelState.IsValid)
        {
            return await _buLogin.Authenticate(dtoRqLogin);
        }

        throw new BadHttpRequestException("Thông tin đăng nhập không hợp lệ");
    }
}