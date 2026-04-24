using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Database;
using IRS.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace IRS.DAL.Repository.CitizenRepo
{
    public class CitizenRepository : ICitizenRepository
    {
        private readonly IRS_Context _context;

        public CitizenRepository(IRS_Context context)
        {
            _context = context;
        }

        public async Task<Citizen> GetByUserIdAsync(string userId)
        {
            return await _context.Citizens
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task UpdateAsync(Citizen citizen)
        {
            _context.Citizens.Update(citizen);
            await _context.SaveChangesAsync();
        }
    }
}
