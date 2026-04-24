using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.CitizenDto.ReportDtos;
using IRS.BLL.Managers.AccountManager.Auth;
using IRS.DAL.Database;
using IRS.DAL.Enums;
using IRS.DAL.Models;
using IRS.DAL.RepoDtos;
using IRS.DAL.Repository.ReportRepo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Coordinate = NetTopologySuite.Geometries.Coordinate;

namespace IRS.BLL.Managers.CitizenAppManager.ReportManager
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepo;
        private readonly IRS_Context _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<User> _userManager;

        public ReportService(IReportRepository reportRepo, IRS_Context Context , IWebHostEnvironment env, UserManager<User> userManager)
        {
            _reportRepo = reportRepo;
            _context = Context;
            _env = env;
            _userManager = userManager;
        }

        public async Task CreateReportAsync(CreateReportDto dto, string userId)
        {
            // 1. Get Citizen
            var citizen = await _context.Citizens.FirstOrDefaultAsync(c => c.UserId == userId);
            if (citizen == null)
                throw new Exception("Citizen not found. Make sure the user exists in the database.");

            // 2. Validate Category
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                throw new Exception("Invalid category. Make sure the CategoryId exists.");

            // 3. Validate Latitude / Longitude
            if (dto.Latitude < -90 || dto.Latitude > 90)
                throw new Exception("Invalid Latitude. Must be between -90 and 90.");
            if (dto.Longitude < -180 || dto.Longitude > 180)
                throw new Exception("Invalid Longitude. Must be between -180 and 180.");

            // 4. Handle Image (optional)
            byte[] imageBytes = null;
            if (dto.Image != null)
            {
                var allowedTypes = new[] { "image/jpeg", "image/png" };
                if (!allowedTypes.Contains(dto.Image.ContentType))
                    throw new Exception("Only JPG and PNG images are allowed.");

                if (dto.Image.Length > 2 * 1024 * 1024)
                    throw new Exception("Image size must be <= 2MB.");

                using var ms = new MemoryStream();
                await dto.Image.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            // 5. Create Location
            var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
            var location = geometryFactory.CreatePoint(new Coordinate(dto.Longitude, dto.Latitude));

            // 6. Create Report
            var report = new Report
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Location = (Point)location,
                CitizenId = citizen.Id,
                Image = imageBytes,
                DateTime = DateTime.UtcNow,
                Status = IRS.DAL.Enums.ReportStatus.Pinding,
                Periority = dto.Periority
            };

            // 7. Save to Database with try/catch for logging
            try
            {
                await _context.Reports.AddAsync(report);

                var notification = new Notification
                {
                    Title = "Report Created",
                    Message = "Your report has been created successfully.",
                    NotificationType = NotificationType.ReportMessage,
                    IsRead = false,
                    CitizenId = citizen.Id,
                    Report = report
                };

                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine("Error saving report: " + ex);
                throw new Exception("An error occurred while saving the report. Check server logs for details.");
            }
        }

        public async Task<APPResult> EditReportAsync(int reportId, EditReportDto dto, string userId)
        {
            var result = new APPResult();

            try
            {
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
                if (!categoryExists)
                {
                    result.IsSuccess = false;
                    result.Message = "Invalid CategoryId.";
                    return result;
                }

                if (dto.Latitude < -90 || dto.Latitude > 90 || dto.Longitude < -180 || dto.Longitude > 180)
                {
                    result.IsSuccess = false;
                    result.Message = "Invalid Latitude or Longitude.";
                    return result;
                }

                byte[]? imageBytes = null;
                if (dto.Image != null)
                {
                    var allowedTypes = new[] { "image/jpeg", "image/png" };
                    if (!allowedTypes.Contains(dto.Image.ContentType))
                    {
                        result.IsSuccess = false;
                        result.Message = "Only JPG and PNG images are allowed.";
                        return result;
                    }

                    if (dto.Image.Length > 2 * 1024 * 1024)
                    {
                        result.IsSuccess = false;
                        result.Message = "Image size must be <= 2MB.";
                        return result;
                    }

                    using var ms = new MemoryStream();
                    await dto.Image.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                var dto1 = new DAL.RepoDtos.ReportDto.EditReportRepoDto
                {
                    CategoryId = dto.CategoryId,
                    Description = dto.Description,
                    Image = dto.Image,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    Title = dto.Title,
                    Periority = dto.Periority
                    
                };
                var updated = await _reportRepo.UpdateReportAsync(reportId, userId, dto1);
                if (!updated)
                {
                    result.IsSuccess = false;
                    result.Message = "Report not found or you don't have permission.";
                    return result;
                }
                var report = await _context.Reports.FirstOrDefaultAsync(r => r.Id == reportId);
                if (report == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Report not found.";
                    return result;
                }

                var notification = new Notification
                {
                    Title = "Report Updated",
                    Message = "Your report has been updated successfully.",
                    NotificationType = NotificationType.ReportMessage,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    CitizenId = report.CitizenId,
                    ReportId = report.Id
                };

                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = "Report updated successfully.";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Error updating report.";
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        public async Task<ReportStatus> GetStatusAsync(int reportId, string userId)
        {
            var citizen = await _context.Citizens.FirstOrDefaultAsync(c => c.UserId == userId);
            if (citizen == null)
                throw new Exception("Citizen not found. Make sure the user exists in the database.");

            var status = await _reportRepo.GetStatusAsync(reportId);

            if (status == null)
                throw new KeyNotFoundException();

            return status;
        }

        public async Task<HomeDto> GetHomeAsync(string userId)
        {
            var citizen = await _reportRepo.GetCitizenWithReportsAsync(userId);

            if (citizen == null)
                throw new Exception("Citizen not found");

            var reports = citizen.Reports ?? new List<Report>();

            var now = DateTime.UtcNow;

            var dto = new HomeDto
            {
                FullName = citizen.FullName,
                Image = citizen.Image,

                PendingReports = reports.Count(r => r.Status == ReportStatus.Pinding),

                TotalReports = reports.Count(),

                PendingCount = reports.Count(r => r.Status == ReportStatus.Pinding),
                InProgressCount = reports.Count(r => r.Status == ReportStatus.Inprogress),
                ResolvedCount = reports.Count(r => r.Status == ReportStatus.Resolved),

                ReportsThisMonth = reports.Count(r =>
                    r.DateTime.Month == now.Month &&
                    r.DateTime.Year == now.Year)
            };

            return dto;
        }

        public async Task<List<ReportHistoryDto>> GetCitizenReportsAsync(string userId, ReportStatus? status)
        {
            var user = await _userManager.Users
                .Include(u => u.Citizen)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Citizen == null)
                throw new Exception("Citizen not found");

            var reports = await _reportRepo.GetReportsByCitizenIdAsync(user.Citizen.Id, status);

            return reports.Select(r => new ReportHistoryDto
            {
                Id = r.Id,
                ReportCode = $"INC-{r.DateTime.Year}-{r.Id:D3}",
                Title = r.Title,
                CategoryName = r.Category.Name,
                Date = r.DateTime,
                Status = MapStatus(r.Status),
            }).ToList();
        }

        private string MapStatus(ReportStatus status)
        {
            return status switch
            {
                ReportStatus.Pinding => "Pending",
                ReportStatus.Inprogress => "In Progress",
                ReportStatus.Resolved => "Solved",
                _ => "Unknown"
            };
        }

    }

}
