/*
 * Copyright 2023 CS4RSA.Cwebiz
 */

using CwebizAPI.Businesses;
using CwebizAPI.DTOs.Requests;
using CwebizAPI.DTOs.Responses;
using CwebizAPI.Settings;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CwebizAPI.Controllers
{
    /// <summary>
    /// Register Controller.
    /// </summary>
    /// <remarks>
    /// Created Date: 21/06/2023
    /// Modified Date: 21/06/2023
    /// Author: Truong A Xin
    /// </remarks>
    [Route("api/v1/[controller]/Student")]
    [Produces("application/json")]
    [EnableCors(Policies.CredizBlazorPolicy)]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly BuRegister _buRegister;
        
        public RegisterController(BuRegister buRegister)
        {
            _buRegister = buRegister;
        }
        
        /// <summary>
        /// Đăng ký tài khoản cho một sinh viên DTU.
        /// </summary>
        /// <remarks>
        /// - 200:
        ///     Đăng ký thành công khi cào được thông tin sinh viên
        ///     (thông qua ASP.NET_SessionID) có Email match với Email
        ///     trong register information.
        /// </remarks>
        /// <param name="registerInformation">Thông tin đăng ký tài khoản.</param>
        /// <returns>
        /// Thông tin đăng ký bao gồm một Jwt Token xác thực cho việc
        /// tạo tài khoản và liên kết tài khoản.
        /// </returns>
        [HttpPost("AddStudent", Name = "Register account for a students")]
        public async Task<IActionResult> RegisterStudent(
            [FromBody] DtoRqRegister registerInformation
        )
        {
            if (!ModelState.IsValid) throw new BadHttpRequestException("Thông tin đăng ký không hợp lệ");
            return Ok(await _buRegister.Register(registerInformation));
        }

        /// <summary>
        /// Đăng ký tài khoản người dùng và liên kết tài khoản với
        /// thông tin sinh viên đã đăng ký trước đó.
        /// </summary>
        /// <returns>DtoRqLinkAccount</returns>
        [HttpPost("CreateAndLink", Name = "Create account and link to the registered student")]
        public async Task<IActionResult> CreateAccountAndLinkStudent(
            [FromBody] DtoRqLinkAccount accountInformation)
        {
            if (!ModelState.IsValid) return BadRequest("Thông tin tài khoản không hợp lệ");
            try
            {
                DtoRpLinkStudent response = await _buRegister.CreateAndLinkAccount(accountInformation);
                return Created(Request.Path.ToString(), response);
            }
            catch (BadHttpRequestException exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
