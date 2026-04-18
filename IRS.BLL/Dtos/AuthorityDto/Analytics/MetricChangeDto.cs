using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.BLL.Dtos.AuthorityDto.Analytics
{
    public class MetricChangeDto
    {
        public double Value { get; set; }   
        public double Change { get; set; }
        public string ChangeType { get; set; }
    }
}
