using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.CitizenDto;
using IRS.BLL.Managers.AccountManager.Auth;

namespace IRS.BLL.Managers.CitizenAppManager.ReportManager
{
    public interface IReportService
    {
        Task CreateReportAsync(CreateReportDto dto, string userId);
        Task<APPResult> EditReportAsync(int reportId, EditReportDto dto, string userId);

    }
}
