using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.BLL.Dtos.AuthorityDto.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalReports { get; set; }
        public int Pending { get; set; }
        public int InProgress { get; set; }
        public int Resolved { get; set; }
        public int HighPriority { get; set; }

        public double TotalReportsChange { get; set; }
        public double PendingChange { get; set; }
        public double InProgressChange { get; set; }
        public double ResolvedChange { get; set; }
        public double HighPriorityChange { get; set; }
    }
}
