using CwebizAPI.Businesses;
using CwebizAPI.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CwebizAPI.Controllers;

[Authorize(Roles = "Trial, Student, Admin")]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[EnableCors(Policies.CredizBlazorPolicy)]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly BuSchedule _buSchedule;

    public ScheduleController(BuSchedule buSchedule)
    {
        _buSchedule = buSchedule;
    }

    /// <summary>
    /// Trả về các mốc thời gian.
    /// </summary>
    /// <returns></returns>
    [HttpGet(template: "TimeSignatures", Name = "GetTimeSignatures")]
    public ActionResult<string[]> GetTimeSignatures()
    {
        return Ok(BuSchedule.TimeSignatures);
    }
}