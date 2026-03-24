using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Enums;
using NetTopologySuite.Geometries;

namespace IRS.DAL.Models
{
    public class Report
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public Point Location { get; set; }
        public DateTime DateTime { get; set; }
        public ReportStatus Status { get; set; }
        public string Description { get; set; }
        public byte[]? Image { get; set; }
        public TimeSpan? AiTime { get; set; }

        public string? PredictedCategory { get; set; }
        public DateTime? ReportSubmit { get; set; }
        public double? ConfidenceScore { get; set; }

        // FK
        public int? AuthorityId { get; set; }
        public Authority Authority { get; set; }

        public int CitizenId { get; set; }
        public Citizen Citizen { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // Relations
        public ICollection<Notification>? Notifications { get; set; }
    }
}
