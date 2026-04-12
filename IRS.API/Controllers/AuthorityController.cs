using IRS.BLL.Dtos.AuthorityDto.Report;
using IRS.BLL.Managers.AccountManager.Auth;
using IRS.BLL.Managers.AuthorityManager.Dashboard;
using IRS.BLL.Managers.AuthorityManager.Report;
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
        private readonly IReportManager _report;

        public AuthorityController(IDashboardService service , IReportManager report)
        {
            _service = service;
            _report = report;
        }

        // ============================
        //  Summary
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
        //  Incident Volume
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
        //  Latest Reports
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
        //  Recent Activity
        // ============================
        [HttpGet("recent-activity")]
        public async Task<IActionResult> GetRecentActivity()
        {
            var result = await _service.GetRecentActivityAsync();

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        //================================================================

        [HttpGet("GetReports")]
        public async Task<IActionResult> GetReports([FromQuery] ReportFilterDto filter)
        {
            var result = await _report.GetReportsAsync(filter);

            return Ok(new APPResult
            {
                IsSuccess = true,
                Message = "Reports fetched successfully",
                Data = result
            });
        }

        
        [HttpGet("GetReport/{id}")]
        public async Task<IActionResult> GetReport(int id)
        {
            var report = await _report.GetByIdAsync(id);

            if (report == null)
            {
                return NotFound(new APPResult
                {
                    IsSuccess = false,
                    Message = "Report not found"
                });
            }

            return Ok(new APPResult
            {
                IsSuccess = true,
                Data = report
            });
        }

    }
}
