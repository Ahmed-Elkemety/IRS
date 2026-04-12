using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.AuthorityDto.Dashboard;
using IRS.BLL.Managers.AccountManager.Auth;

namespace IRS.BLL.Managers.AuthorityManager.Dashboard
{
    public interface IDashboardService
    {
        Task<APPResult> GetSummaryAsync();
        Task<APPResult> GetIncidentVolumeAsync(int days);
        Task<APPResult> GetLatestReportsAsync();
        Task<APPResult> GetRecentActivityAsync();
    }
}
