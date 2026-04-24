using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.BLL.Dtos.CitizenDto.ReportDtos
{
    public class ReportHistoryDto
    {
        public int Id { get; set; }
        public string ReportCode { get; set; }   // INC-2023-001
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}
