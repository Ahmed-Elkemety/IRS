using IRS.BLL.Managers.CitizenAppManager.ReportManager;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IRS.BLL.Dtos.CitizenDto.ReportDtos;
using IRS.DAL.Enums;
using IRS.BLL.Dtos.CitizenDto.ProfileDtos;
using IRS.BLL.Managers.CitizenAppManager.CitizenManager;

namespace IRS.API.Controllers
{
    [Authorize(Roles = "CITIZEN")]
    [Route("api/[controller]")]
    [ApiController]
    public class CitizenController : ControllerBase
    {
        private readonly IReportService _service;
        private readonly IProfileService _profile;

        public CitizenController(IReportService service ,IProfileService profile)
        {
            _service = service;
            _profile = profile;
        }

        [HttpPost("Create-report")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateReportDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                await _service.CreateReportAsync(dto, userId);

                return Ok(new
                {
                    message = "Report Created Successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }

        [HttpPut("Edit-Report/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditReport(int id, [FromForm] EditReportDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _service.EditReportAsync(id, dto, userId);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }


        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetStatus(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var status = await _service.GetStatusAsync(id , userId);
                return Ok(new { ReportId = id, Status = status.ToString() });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Report not found" });
            }
        }

        [HttpGet("home")]
        public async Task<IActionResult> GetHome()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _service.GetHomeAsync(userId);

            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetReports([FromQuery] ReportStatus? status)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _service.GetCitizenReportsAsync(userId, status);

            return Ok(result);
        }

        // 📌 Get Profile
        [HttpGet("View-Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _profile.GetProfileAsync(userId);

            return Ok(result);
        }

        // 📌 Update Profile
        [HttpPut("Update-Info")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _profile.UpdateProfileAsync(userId, dto);

            return Ok(new { message = "Profile updated successfully" });
        }
    }
}
