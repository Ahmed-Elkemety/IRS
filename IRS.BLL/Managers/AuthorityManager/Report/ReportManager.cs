using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.AuthorityDto.Report;
using IRS.DAL.Repository.ReportRepo;

namespace IRS.BLL.Managers.AuthorityManager.Report
{
    public class ReportManager : IReportManager
    {
        private readonly IReportRepository _repo;

        public ReportManager(IReportRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<ReportDto>> GetReportsAsync(ReportFilterDto filter)
        {
            var filterRepoDto = new DAL.RepoDtos.ReportFilterRepoDto
            {
                CategoryId = filter.CategoryId,
                Status = filter.Status,
                Priority = filter.Priority,
                From = filter.From,
                To = filter.To,
                MinConfidence = filter.MinConfidence
            };

            var reports = await _repo.GetReportsAsync(filterRepoDto);

            return reports.Select(r => new ReportDto
            {
                Id = r.Id,
                Category = r.Category.Name,
                Priority = r.Periority.ToString(),
                Status = r.Status.ToString(),
                Location = $"{r.Location.X}, {r.Location.Y}",
                DateTime = r.DateTime,
                ReporterName = r.Citizen.FullName,
                PredictedCategory = r.PredictedCategory,
                ConfidenceScore = r.ConfidenceScore
            }).ToList();
        }

        public async Task<ReportDto?> GetByIdAsync(int id)
        {
            var r = await _repo.GetByIdAsync(id);

            if (r == null) return null;

            return new ReportDto
            {
                Id = r.Id,
                Category = r.Category.Name,
                Priority = r.Periority.ToString(),
                Status = r.Status.ToString(),
                Location = $"{r.Location.X}, {r.Location.Y}",
                DateTime = r.DateTime,
                ReporterName = r.Citizen.FullName,
                PredictedCategory = r.PredictedCategory,
                ConfidenceScore = r.ConfidenceScore
            };
        }
    }
}
