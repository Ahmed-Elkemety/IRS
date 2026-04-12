using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Enums;

namespace IRS.BLL.Dtos.AuthorityDto.Report
{
    public class ReportFilterDto
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int? CategoryId { get; set; }
        public ReportStatus? Status { get; set; }
        public ReportPeriority? Priority { get; set; }

        public double? MinConfidence { get; set; }
    }
}
