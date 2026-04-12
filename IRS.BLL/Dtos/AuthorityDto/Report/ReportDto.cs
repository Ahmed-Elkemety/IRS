using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.BLL.Dtos.AuthorityDto.Report
{
    public class ReportDto
    {
        public int Id { get; set; }
        public string ReportId ;

        public string Category { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }

        public string Location { get; set; }
        public DateTime DateTime { get; set; }

        public string ReporterName { get; set; }
        public DateTime CreatedAt { get; set; }


        public string? PredictedCategory { get; set; }
        public double? ConfidenceScore { get; set; }
    }
}
