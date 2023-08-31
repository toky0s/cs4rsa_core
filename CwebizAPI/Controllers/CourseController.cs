/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Businesses;
using CwebizAPI.DTOs.Responses;
using CwebizAPI.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CwebizAPI.Controllers;

/// <summary>
/// Course Controller
/// 
/// Created Date: 02/08/2023
/// Modified Date: 02/08/2023
/// Author: Truong A Xin
/// </summary>
[Authorize(Roles = "Trial, Student, Admin")]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[EnableCors(Policies.CredizBlazorPolicy)]
[ApiController]
public class CourseController : ControllerBase
{
    private readonly BuCourse _buCourse;
    public CourseController(BuCourse buCourse)
    {
        _buCourse = buCourse;
    }

    /// <summary>
    /// Lấy ra thông tin Course mới nhất.
    /// </summary>
    /// <returns>DTO Response Course.</returns>
    [HttpGet(template: "LatestCourse",Name = "GetLatestCourse")]
    public async Task<ActionResult<DtoRpCourse>> GetLatestCourse()
    {
        return await _buCourse.GetLatestCourse();
    }
}