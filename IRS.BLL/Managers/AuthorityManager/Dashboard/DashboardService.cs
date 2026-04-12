using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.AuthorityDto.Dashboard;
using IRS.BLL.Managers.AccountManager.Auth;
using IRS.DAL.Database;
using IRS.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IRS.BLL.Managers.AuthorityManager.Dashboard
{
    public class DashboardService:IDashboardService
    {
        private readonly IRS_Context _context;

        public DashboardService(IRS_Context context)
        {
            context = context;
        }
        public async Task<APPResult> GetSummaryAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var currentStart = now.AddDays(-7);
                var previousStart = now.AddDays(-14);

                if (currentStart > now)
                {
                    return new APPResult
                    {
                        IsSuccess = false,
                        Message = "Invalid date range",
                        Errors = new List<string> { "Start date cannot be in the future" }
                    };
                }

                var currentReports = _context.Reports
                    .Where(r => r.DateTime >= currentStart);

                var previousReports = _context.Reports
                    .Where(r => r.DateTime >= previousStart && r.DateTime < currentStart);

                var totalCurrent = await currentReports.CountAsync();
                var totalPrevious = await previousReports.CountAsync();

                var result = new DashboardSummaryDto
                {
                    TotalReports = totalCurrent,
                    TotalReportsChange = CalculatePercentageChange(totalCurrent, totalPrevious)
                };

                return new APPResult
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new APPResult
                {
                    IsSuccess = false,
                    Message = "Something went wrong",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        private double CalculatePercentageChange(int current, int previous)
        {
            if (previous == 0)
                return current == 0 ? 0 : 100;

            return (double)(current - previous) / previous * 100;
        }

        public async Task<APPResult> GetIncidentVolumeAsync(int days)
        {
            if (days <= 0 || days > 365)
            {
                return new APPResult
                {
                    IsSuccess = false,
                    Message = "Invalid input",
                    Errors = new List<string> { "Days must be between 1 and 365" }
                };
            }

            var startDate = DateTime.UtcNow.AddDays(-days);

            var data = await _context.Reports
                .Where(r => r.DateTime >= startDate)
                .GroupBy(r => r.DateTime.Date)
                .Select(g => new IncidentVolumeDto
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return new APPResult
            {
                IsSuccess = true,
                Message = "Success",
                Data = data
            };
        }

        public async Task<APPResult> GetLatestReportsAsync()
        {


            var data =   _context.Reports
                .Include(r => r.Category)
                .OrderByDescending(r => r.DateTime)
                .Take(5)
                .Select(r => new LatestReportDto
                {
                    Id = r.Id,
                    Type = r.Category.Name,
                    Status = r.Status.ToString(),
                    ReportedTime = r.DateTime
                })
                .ToListAsync();
            if (data == null)
            {
                return new APPResult
                {
                    IsSuccess = false,
                    Message = "No data found",
                    Errors = new List<string> { "No reports available" }
                };
            }
            else
            {
                return new APPResult
                {
                    IsSuccess = true,
                    Message = "Done",
                    Data = data
                };
            }
        }
        public async Task<APPResult> GetRecentActivityAsync()
        {
            var activities = new List<ActivityDto>();

            var newReports = await _context.Reports
                .OrderByDescending(r => r.DateTime)
                .Take(3)
                .ToListAsync();

            var resolvedReports = await _context.Reports
                .Where(r => r.Status == ReportStatus.Resolved)
                .OrderByDescending(r => r.DateTime)
                .Take(2)
                .ToListAsync();

            activities.AddRange(newReports.Select(r => new ActivityDto
            {
                Message = $"New report #{r.Id}",
                Time = r.DateTime
            }));

            activities.AddRange(resolvedReports.Select(r => new ActivityDto
            {
                Message = $"Report #{r.Id} resolved",
                Time = r.DateTime
            }));

            var result = activities
                .OrderByDescending(a => a.Time)
                .ToList();

            return new APPResult
            {
                IsSuccess = true,
                Message = "Recent activity fetched successfully",
                Data = result
            };
        }

    }
}
