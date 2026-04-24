using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.AuthorityDto.Report;
using IRS.BLL.Managers.AccountManager.Auth;
using IRS.DAL.Database;
using IRS.DAL.Enums;
using IRS.DAL.Models;
using IRS.DAL.Repository.ReportRepo;
using Microsoft.EntityFrameworkCore;

namespace IRS.BLL.Managers.AuthorityManager.Report
{
    public class ReportManager : IReportManager
    {
        private readonly IReportRepository _repo;
        private readonly IRS_Context _context;

        public ReportManager(IReportRepository repo, IRS_Context context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<List<ReportDto>> GetReportsAsync(ReportFilterDto filter)
        {
            var filterRepoDto = new DAL.RepoDtos.ReportDto.ReportFilterRepoDto
            {
                CategoryId = filter.CategoryId,
                Status = filter.Status,
                Priority = filter.Priority,
                From = filter.From,
                To = filter.To,
                MinConfidence = filter.MinConfidence
            };

            var reports = await _repo.GetReportsAsync(filterRepoDto);

            return reports.Select(r => new ReportDto
            {
                Id = r.Id,
                Category = r.Category.Name,
                Priority = r.Periority.ToString(),
                Status = r.Status.ToString(),
                Location = $"{r.Location.X}, {r.Location.Y}",
                DateTime = r.DateTime,
                ReporterName = r.Citizen.FullName,
                PredictedCategory = r.PredictedCategory,
                ConfidenceScore = r.ConfidenceScore
            }).ToList();
        }

        public async Task<ReportDto?> GetByIdAsync(int id)
        {
            var r = await _repo.GetByIdAsync(id);

            if (r == null) return null;

            return new ReportDto
            {
                Id = r.Id,
                Category = r.Category.Name,
                Priority = r.Periority.ToString(),
                Status = r.Status.ToString(),
                Location = $"{r.Location.X}, {r.Location.Y}",
                DateTime = r.DateTime,
                ReporterName = r.Citizen.FullName,
                PredictedCategory = r.PredictedCategory,
                ConfidenceScore = r.ConfidenceScore
            };
        }

        public async Task<APPResult> UpdateReportStatusAsync(int reportId, UpdateReportStatusDto dto)
        {
            var result = new APPResult();

            try
            {
                // ✅ Validate Status
                if (dto.Status != ReportStatus.Inprogress && dto.Status != ReportStatus.Resolved)
                {
                    result.IsSuccess = false;
                    result.Message = "Status must be Inprogress or Resolved.";
                    return result;
                }

                var report = await _repo.GetByIdAsync(reportId);

                if (report == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Report not found.";
                    return result;
                }

                // ✅ Update Status
                report.Status = dto.Status;

                var updated = await _repo.UpdateStatusAsync(report);

                if (!updated)
                {
                    result.IsSuccess = false;
                    result.Message = "Failed to update status.";
                    return result;
                }

                // ✅ Notification
                var notification = new Notification
                {
                    Title = "Report Status Updated",
                    Message = dto.Status == ReportStatus.Inprogress
                        ? "Your report is now in progress."
                        : "Your report has been resolved.",
                    NotificationType = NotificationType.ReportMessage,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    CitizenId = report.CitizenId,
                    ReportId = report.Id
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.Message = "Status updated successfully.";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "Error updating status.";
                result.Errors.Add(ex.Message);
            }

            return result;
        }
    }
}
