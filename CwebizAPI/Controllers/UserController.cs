/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using System.Security.Claims;
using CwebizAPI.Businesses;
using CwebizAPI.DTOs.Responses;
using CwebizAPI.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CwebizAPI.Controllers;

/// <summary>
/// UserController
/// </summary>
/// <remarks>
/// Created Date: 21/06/2023
/// Modified Date: 21/06/2023
/// Author: Truong A Xin
/// </remarks>
[Authorize(Roles = "Trial, Student, Admin")]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[EnableCors(Policies.CredizBlazorPolicy)]
[ApiController]
public class UserController : ControllerBase
{
    private readonly BuUser _buUser;

    public UserController(BuUser buUser)
    {
        _buUser = buUser;
    }

    /// <summary>
    /// Lấy thông tin người dùng hiện tại.
    /// </summary>
    /// <returns>IActionResult</returns>
    [HttpGet("GetMe", Name = "GetUserInfo")]
    public async Task<IActionResult> GetUser()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userId, out int id)) return BadRequest(new ErrorResponse("Thông tin không hợp lệ"));
        DtoRpUser dtoRpUser = await _buUser.GetUser(id);
        return Ok(dtoRpUser);
    }
}