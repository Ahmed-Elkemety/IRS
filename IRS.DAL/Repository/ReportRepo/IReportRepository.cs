using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Enums;
using IRS.DAL.Models;
using IRS.DAL.RepoDtos.ReportDto;

namespace IRS.DAL.Repository.ReportRepo
{
    public interface IReportRepository
    {
        Task<bool> UpdateReportAsync(int reportId, string userId, EditReportRepoDto dto);
        Task<ReportStatus> GetStatusAsync(int reportId);
        Task<List<Report>> GetReportsAsync(ReportFilterRepoDto filter);
        Task<Report?> GetByIdAsync(int id);
        Task<bool> UpdateStatusAsync(Report report);
        Task<Citizen> GetCitizenWithReportsAsync(string userId);
        Task<List<Report>> GetReportsByCitizenIdAsync(int citizenId, ReportStatus? status);

    }
}
