using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRS.DAL.Enums
{
    public enum ReportStatus
    {
       verified = 1,
       sent = 2,
       AIClassified = 3,
       UnitDispatched = 4,
       Resolved = 5
    }
}
