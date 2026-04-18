using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.RepoDtos.AnalyticsDto;

namespace IRS.DAL.Repository.Analytics
{
    public interface IAnalyticsRepository
    {
        Task<int> GetTotalReportsAsync();
        Task<int> GetHighPriorityReportsAsync();
        Task<double> GetResolutionRateAsync();
        Task<double> GetAvgResponseTimeAsync();

        Task<List<MonthlyCountDto>> GetReportsPerMonthAsync();
        Task<List<MonthlyCountDto>> GetResolvedReportsPerMonthAsync();

        Task<List<CategoryStatsDto>> GetCategoryStatsAsync();

        Task<int> GetReportsCountByMonthAsync(int year, int month);
        Task<int> GetResolvedCountByMonthAsync(int year, int month);
        Task<int> GetHighPriorityByMonthAsync(int year, int month);
        Task<double> GetAvgResponseTimeByMonthAsync(int year, int month);
    }
}
