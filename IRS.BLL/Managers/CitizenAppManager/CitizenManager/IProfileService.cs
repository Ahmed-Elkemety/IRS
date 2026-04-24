using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.CitizenDto.ProfileDtos;

namespace IRS.BLL.Managers.CitizenAppManager.CitizenManager
{
    public interface IProfileService
    {
        Task<ProfileDto> GetProfileAsync(string userId);
        Task UpdateProfileAsync(string userId, UpdateProfileDto dto);
    }
}
