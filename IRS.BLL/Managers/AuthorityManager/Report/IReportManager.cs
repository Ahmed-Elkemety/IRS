using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.AuthorityDto.Report;
using IRS.BLL.Managers.AccountManager.Auth;

namespace IRS.BLL.Managers.AuthorityManager.Report
{
    public interface IReportManager
    {
        Task<List<ReportDto>> GetReportsAsync(ReportFilterDto filter);
        Task<ReportDto?> GetByIdAsync(int id);
        Task<APPResult> UpdateReportStatusAsync(int reportId, UpdateReportStatusDto dto);
    }
}
