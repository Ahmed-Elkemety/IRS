using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.BLL.Dtos.CitizenDto.ReportDtos
{
    public class HomeDto
    {
        public string FullName { get; set; }
        public byte[]? Image { get; set; }
        public int PendingReports { get; set; }

        public int TotalReports { get; set; }
        public int PendingCount { get; set; }
        public int InProgressCount { get; set; }
        public int ResolvedCount { get; set; }

        public int ReportsThisMonth { get; set; }
    }
}
