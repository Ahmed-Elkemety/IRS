using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.DAL.Models;

namespace IRS.DAL.Repository.CitizenRepo
{
    public interface ICitizenRepository
    {
        Task<Citizen> GetByUserIdAsync(string userId);
        Task UpdateAsync(Citizen citizen);
    }
}
