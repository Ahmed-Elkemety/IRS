using IRS.BLL.Dtos.CitizenDto;
using IRS.BLL.Managers.CitizenAppManager.ReportManager;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace IRS.API.Controllers
{
    [Authorize(Roles = "CITIZEN")]
    [Route("api/[controller]")]
    [ApiController]
    public class CitizenController : ControllerBase
    {
        private readonly IReportService _service;

        public CitizenController(IReportService service)
        {
            _service = service;
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
    }
}
