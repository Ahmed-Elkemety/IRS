using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.RepoDtos.AnalyticsDto;

namespace IRS.BLL.Dtos.AuthorityDto.Analytics
{
    public class AnalyticsDto
    {
        public MetricChangeDto TotalReports { get; set; }
        public MetricChangeDto ResolutionRate { get; set; }
        public MetricChangeDto AvgResponseTime { get; set; }
        public MetricChangeDto HighPriority { get; set; }

        public List<MonthlyCountDto> IncidentTrends { get; set; }
        public List<MonthlyCountDto> ResolutionComparison { get; set; }
        public List<CategoryStatsDto> Categories { get; set; }
    }
}
