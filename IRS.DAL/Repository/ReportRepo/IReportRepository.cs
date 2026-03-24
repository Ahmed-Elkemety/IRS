using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.RepoDtos;

namespace IRS.DAL.Repository.ReportRepo
{
    public interface IReportRepository
    {
        Task<bool> UpdateReportAsync(int reportId, string userId, EditReportRepoDto dto);

    }
}
