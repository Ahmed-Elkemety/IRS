using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Database;
using IRS.DAL.Enums;
using IRS.DAL.RepoDtos.AnalyticsDto;
using Microsoft.EntityFrameworkCore;

namespace IRS.DAL.Repository.Analytics
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly IRS_Context _context;

        public AnalyticsRepository(IRS_Context context)
        {
            _context = context;
        }

        public async Task<int> GetTotalReportsAsync()
        {
            return await _context.Reports.CountAsync();
        }

        public async Task<int> GetHighPriorityReportsAsync()
        {
            return await _context.Reports
                .CountAsync(r => r.Periority == ReportPeriority.High);
        }

        public async Task<double> GetResolutionRateAsync()
        {
            var total = await _context.Reports.CountAsync();
            var resolved = await _context.Reports
                .CountAsync(r => r.Status == ReportStatus.Resolved);

            if (total == 0) return 0;

            return (double)resolved / total * 100;
        }

        public async Task<double> GetAvgResponseTimeAsync()
        {
            return await _context.Reports
                .Where(r => r.AiTime != null)
                .AverageAsync(r => r.AiTime.Value.TotalMinutes);
        }

        public async Task<List<MonthlyCountDto>> GetReportsPerMonthAsync()
        {
            return await _context.Reports
                .GroupBy(r => r.DateTime.Month)
                .Select(g => new MonthlyCountDto
                {
                    Month = g.Key.ToString(),
                    Count = g.Count()
                }).ToListAsync();
        }

        public async Task<List<MonthlyCountDto>> GetResolvedReportsPerMonthAsync()
        {
            return await _context.Reports
                .Where(r => r.Status == ReportStatus.Resolved)
                .GroupBy(r => r.DateTime.Month)
                .Select(g => new MonthlyCountDto
                {
                    Month = g.Key.ToString(),
                    Count = g.Count()
                }).ToListAsync();
        }

        public async Task<List<CategoryStatsDto>> GetCategoryStatsAsync()
        {
            return await _context.Reports
                .Include(r => r.Category)
                .GroupBy(r => r.Category.Name)
                .Select(g => new CategoryStatsDto
                {
                    CategoryName = g.Key,
                    Count = g.Count()
                }).ToListAsync();
        }

        public async Task<int> GetReportsCountByMonthAsync(int year, int month)
        {
            return await _context.Reports
                .CountAsync(r => r.DateTime.Year == year && r.DateTime.Month == month);
        }

        public async Task<int> GetResolvedCountByMonthAsync(int year, int month)
        {
            return await _context.Reports
                .CountAsync(r => r.DateTime.Year == year &&
                                 r.DateTime.Month == month &&
                                 r.Status == ReportStatus.Resolved);
        }

        public async Task<int> GetHighPriorityByMonthAsync(int year, int month)
        {
            return await _context.Reports
                .CountAsync(r => r.DateTime.Year == year &&
                                 r.DateTime.Month == month &&
                                 r.Periority == ReportPeriority.High);
        }

        public async Task<double> GetAvgResponseTimeByMonthAsync(int year, int month)
        {
            return await _context.Reports
                .Where(r => r.DateTime.Year == year &&
                            r.DateTime.Month == month &&
                            r.AiTime != null)
                .AverageAsync(r => r.AiTime.Value.TotalMinutes);
        }
    }
}
