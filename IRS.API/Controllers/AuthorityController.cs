using IRS.BLL.Managers.AuthorityManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IRS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "AUTHORITY")]
    public class AuthorityController : ControllerBase
    {
        private readonly IDashboardService _service;

        public AuthorityController(IDashboardService service)
        {
            _service = service;
        }

        // ============================
        // 1. Summary
        // ============================
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _service.GetSummaryAsync();

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ============================
        // 2. Incident Volume
        // ============================
        [HttpGet("incident-volume")]
        public async Task<IActionResult> GetIncidentVolume([FromQuery] int days = 7)
        {
            var result = await _service.GetIncidentVolumeAsync(days);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ============================
        // 3. Latest Reports
        // ============================
        [HttpGet("latest-reports")]
        public async Task<IActionResult> GetLatestReports()
        {
            var result = await _service.GetLatestReportsAsync();

            if (!result.IsSuccess)
                return NotFound(result); 

            return Ok(result);
        }

        // ============================
        // 4. Recent Activity
        // ============================
        [HttpGet("recent-activity")]
        public async Task<IActionResult> GetRecentActivity()
        {
            var result = await _service.GetRecentActivityAsync();

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

    }
}
