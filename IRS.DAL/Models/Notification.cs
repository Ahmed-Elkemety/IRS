using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Enums;

namespace IRS.DAL.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public  NotificationType NotificationType { get; set; }
        public bool IsRead { get; set; }

        public int CitizenId { get; set; }
        public Citizen Citizen { get; set; }

        public int ReportId { get; set; }
        public Report Report { get; set; }
    }
}
