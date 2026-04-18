using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Database;
using IRS.DAL.Enums;
using IRS.DAL.Models;
using IRS.DAL.RepoDtos.ReportDto;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Coordinate = NetTopologySuite.Geometries.Coordinate;

namespace IRS.DAL.Repository.ReportRepo
{
    public class ReportRepository : IReportRepository
    {
        private readonly IRS_Context _context;

        public ReportRepository(IRS_Context context)
        {
            _context = context;
        }

        
        public async Task<bool> UpdateReportAsync(int reportId, string userId, EditReportRepoDto dto)
        {
            var report = await _context.Reports
                .Include(r => r.Citizen)
                .FirstOrDefaultAsync(r => r.Id == reportId && r.Citizen.UserId == userId);

            if (report == null)
                return false;

            report.Title = dto.Title;
            report.Description = dto.Description;
            report.CategoryId = dto.CategoryId;
            report.Periority = dto.Periority;

            var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            report.Location = geometryFactory.CreatePoint(new Coordinate(dto.Longitude, dto.Latitude));

            if (dto.Image != null)
            {
                using var ms = new MemoryStream();
                await dto.Image.CopyToAsync(ms);
                report.Image = ms.ToArray();
            }

            _context.Reports.Update(report);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ReportStatus> GetStatusAsync(int reportId)
        {
            var status = await _context.Reports
            .Where(r => r.Id == reportId)
            .Select(r => (ReportStatus?)r.Status)
            .FirstOrDefaultAsync();

            return status ?? throw new Exception("Report not found. Make sure the report ID is correct.");
        }

        public async Task<List<Report>> GetReportsAsync(ReportFilterRepoDto filter)
        {
            var query = _context.Reports
                .Include(r => r.Category)
                .Include(r => r.Citizen)
                .AsQueryable();

            // Filters
            if (filter.From.HasValue)
                query = query.Where(r => r.DateTime >= filter.From);

            if (filter.To.HasValue)
                query = query.Where(r => r.DateTime <= filter.To);

            if (filter.CategoryId.HasValue)
                query = query.Where(r => r.CategoryId == filter.CategoryId);

            if (filter.Status.HasValue)
                query = query.Where(r => r.Status == filter.Status);

            if (filter.Priority.HasValue)
                query = query.Where(r => r.Periority == filter.Priority);

            if (filter.MinConfidence.HasValue)
                query = query.Where(r => r.ConfidenceScore >= filter.MinConfidence);

            return await query.OrderByDescending(r => r.DateTime).ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports
                .Include(r => r.Category)
                .Include(r => r.Citizen)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> UpdateStatusAsync(Report report)
        {
            _context.Reports.Update(report);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
