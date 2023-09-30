/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Businesses;
using CwebizAPI.DTOs.Responses;
using CwebizAPI.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CwebizAPI.Controllers
{
    /// <summary>
    /// Discipline Controller.
    /// </summary>
    [Authorize(Roles = "Trial, Student, Admin")]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [EnableCors(Policies.CredizBlazorPolicy)]
    [ApiController]
    public class DisciplineController : ControllerBase
    {
        private readonly BuDiscipline _buDiscipline;
        public DisciplineController(BuDiscipline buDiscipline)
        {
            _buDiscipline = buDiscipline;
        }
        
        /// <summary>
        /// Lấy ra danh sách các Discipline.
        /// </summary>
        /// <remarks>
        /// Danh sách các Discipline được trả về dựa theo thông tin mã ngành
        /// của sinh viên.
        /// </remarks>
        /// <returns>Danh sách các Discipline.</returns>
        [HttpGet("Disciplines", Name = "GetListDiscipline")]
        public async Task<ActionResult<IEnumerable<DtoRpDiscipline>>> Get()
        {
            return Ok(await _buDiscipline.GetAllDtoDiscipline());
        }

        /// <summary>
        /// Lấy danh sách các Keyword dựa theo Discipline ID.
        /// </summary>
        /// <remarks>
        /// Phân quyền:
        ///     Người dùng ẩn danh:
        ///         - Giới hạn một số Discipline ngẫu nhiên khi dùng thử.
        ///         - 403 Forbidden:
        ///             Truy cập vào tài nguyên không được phép.
        ///             
        ///     Sinh viên:
        ///         - Dựa theo mã ngành và các môn học tương ứng sẽ được cấp quyền
        ///         truy cập vào các Discipline và Keyword tương ứng.
        ///         - 401 Unauthorized:
        ///             Các Discipline không được phép truy cập.
        ///             Các Keyword liên quan đến các Discipline không được phép truy cập.
        ///         - 404 Not Found:
        ///             Truy cập vào các Discipline và Keyword không tồn tại.
        ///         - 200 Success:
        ///             Các Discipline và Keyword liên quan được phép truy cập.
        ///             
        ///     Quản trị:
        ///         - Tất cả các quyền.
        /// </remarks>
        /// <param name="disciplineId">Mã ngành</param>
        /// <returns>Danh sách mã môn liên quan</returns>
        [HttpGet("Keywords/{DisciplineId:int}", Name = "GetKeywordsByDisciplineId")]
        public async Task<ActionResult<List<DtoRpKeyword>>> GetKeywordsByDisciplineId(
            [FromRoute(Name = "DisciplineId")] int disciplineId)
        {
            // Phân quyền đoạn này.
            return Ok(await _buDiscipline.GetKeywordsByDisciplineId(disciplineId));
        }
    }
}
