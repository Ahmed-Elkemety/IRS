using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.CitizenDto.ReportDtos;
using IRS.BLL.Managers.AccountManager.Auth;
using IRS.DAL.Enums;

namespace IRS.BLL.Managers.CitizenAppManager.ReportManager
{
    public interface IReportService
    {
        Task CreateReportAsync(CreateReportDto dto, string userId);
        Task<APPResult> EditReportAsync(int reportId, EditReportDto dto, string userId);

        Task<ReportStatus> GetStatusAsync(int reportId, string userId);
        Task<HomeDto> GetHomeAsync(string userId);
        Task<List<ReportHistoryDto>> GetCitizenReportsAsync(string userId, ReportStatus? status);

    }
}
