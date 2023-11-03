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
/// SubjectController
/// </summary>
/// <remarks>
/// Created Date: 25/07/2023
/// Modified Date: 25/07/2023
/// Author: Truong A Xin
/// </remarks>
[Authorize(Roles = "Trial, Student, Admin")]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[EnableCors(Policies.CredizBlazorPolicy)]
[ApiController]
public class SubjectController : ControllerBase
{
    private readonly BuSubject _buSubject;
    public SubjectController(BuSubject buSubject)
    {
        _buSubject = buSubject;
    }

    /// <summary>
    /// Lấy thông tin môn học bằng Discipline Name và Keyword
    /// CS 100: Giới thiệu khoa học máy tính.
    /// </summary>
    /// <param name="discipline">Discipline Name, CS</param>
    /// <param name="keyword">Keyword, 100</param>
    /// <returns>Thông tin môn học.</returns>
    [HttpGet("Crawl/{Discipline}/{Keyword}", Name = "GetSubjectByDisciplineKeyword")]
    public async Task<IActionResult> GetSubject(
        [FromRoute(Name = "Discipline")] string discipline,
        [FromRoute(Name = "Keyword")] string keyword)
    {
        DtoRpSubject? subject = await _buSubject.GetSubjectByDisciplineKeyword(discipline, keyword);
        if (subject is null) return NotFound();
        return Ok(subject);
    }
    
    /// <summary>
    /// Lấy thông tin môn học bằng Course ID
    /// </summary>
    /// <param name="courseId">Course ID</param>
    /// <returns>Thông tin môn học.</returns>
    [HttpGet("Crawl/{CourseId}", Name = "CrawlSubjectByCourseId")]
    public async Task<IActionResult> GetSubject(
        [FromRoute(Name = "CourseId")] string courseId)
    {
        DtoRpSubject? subject = await _buSubject.GetSubjectByCourseId(courseId);
        if (subject is null) return NotFound();
        return Ok(subject);
    }
}