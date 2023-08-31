using CwebizAPI.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CwebizAPI.Controllers
{
    /// <summary>
    /// Controller xử lý lỗi.
    ///
    /// Common xử lý các lỗi NotFound, BadRequest.
    ///
    /// Created Date: 14/07/2023
    /// Modified Date: 14/07/2023
    /// Author: Truong A Xin
    /// </summary>
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)] // Ignore Swagger
    public class ErrorsController : ControllerBase
    {
        [Route("Error")]
        public IActionResult Error()
        {
            IExceptionHandlerFeature? context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            Exception? exception = context?.Error;

            return BadRequest(new ErrorResponse(exception.Message));
        }
    }
}
