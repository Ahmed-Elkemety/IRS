using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Enums;
using Microsoft.AspNetCore.Http;

namespace IRS.DAL.RepoDtos
{
    public class EditReportRepoDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public ReportPeriority Periority { get; set; }

        public int CategoryId { get; set; }

        public IFormFile Image { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
