using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.AuthorityDto.Analytics;

namespace IRS.BLL.Managers.AuthorityManager.Analytics
{
    public interface IAnalyticsManager
    {
        Task<AnalyticsDto> GetAnalyticsAsync();
    }
}
