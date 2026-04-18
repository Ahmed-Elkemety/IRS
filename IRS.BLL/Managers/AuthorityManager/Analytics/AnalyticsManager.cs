using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.AuthorityDto.Analytics;
using IRS.DAL.Repository.Analytics;

namespace IRS.BLL.Managers.AuthorityManager.Analytics
{
    public class AnalyticsManager : IAnalyticsManager
    {
        private readonly IAnalyticsRepository _repo;

        public AnalyticsManager(IAnalyticsRepository repo)
        {
            _repo = repo;
        }

        public async Task<AnalyticsDto> GetAnalyticsAsync()
        {
            var now = DateTime.UtcNow;
            var currentMonth = now.Month;
            var currentYear = now.Year;

            var prevMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var prevYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            // 🔢 Total Reports
            var currentTotal = await _repo.GetReportsCountByMonthAsync(currentYear, currentMonth);
            var prevTotal = await _repo.GetReportsCountByMonthAsync(prevYear, prevMonth);

            double totalChange = prevTotal == 0 ? 0 : ((double)(currentTotal - prevTotal) / prevTotal) * 100;

            // 🔢 Resolution Rate
            var currentResolved = await _repo.GetResolvedCountByMonthAsync(currentYear, currentMonth);
            var prevResolved = await _repo.GetResolvedCountByMonthAsync(prevYear, prevMonth);

            double currentRate = currentTotal == 0 ? 0 : (double)currentResolved / currentTotal * 100;
            double prevRate = prevTotal == 0 ? 0 : (double)prevResolved / prevTotal * 100;

            double rateChange = currentRate - prevRate;

            // ⏱ Avg Time
            var currentAvg = await _repo.GetAvgResponseTimeByMonthAsync(currentYear, currentMonth);
            var prevAvg = await _repo.GetAvgResponseTimeByMonthAsync(prevYear, prevMonth);

            double avgChange = currentAvg - prevAvg;

            // 🔥 High Priority
            var currentHigh = await _repo.GetHighPriorityByMonthAsync(currentYear, currentMonth);
            var prevHigh = await _repo.GetHighPriorityByMonthAsync(prevYear, prevMonth);

            double highChange = currentHigh - prevHigh;

            return new AnalyticsDto
            {
                TotalReports = new MetricChangeDto
                {
                    Value = currentTotal,
                    Change = totalChange,
                    ChangeType = totalChange >= 0 ? "up" : "down"
                },
                ResolutionRate = new MetricChangeDto
                {
                    Value = currentRate,
                    Change = rateChange,
                    ChangeType = rateChange >= 0 ? "up" : "down"
                },
                AvgResponseTime = new MetricChangeDto
                {
                    Value = currentAvg,
                    Change = avgChange,
                    ChangeType = avgChange <= 0 ? "up" : "down"
                },
                HighPriority = new MetricChangeDto
                {
                    Value = currentHigh,
                    Change = highChange,
                    ChangeType = highChange >= 0 ? "up" : "down"
                },

                IncidentTrends = await _repo.GetReportsPerMonthAsync(),
                ResolutionComparison = await _repo.GetResolvedReportsPerMonthAsync(),
                Categories = await _repo.GetCategoryStatsAsync()
            };
        }
    }
}
