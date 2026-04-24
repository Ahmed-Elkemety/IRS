using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.CitizenDto.ProfileDtos;
using IRS.DAL.Models;
using IRS.DAL.Repository.CitizenRepo;
using Microsoft.AspNetCore.Identity;

namespace IRS.BLL.Managers.CitizenAppManager.CitizenManager
{
    public class ProfileService : IProfileService
    {
        private readonly ICitizenRepository _repo;
        private readonly UserManager<User> _userManager;

        public ProfileService(ICitizenRepository repo, UserManager<User> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        public async Task<ProfileDto> GetProfileAsync(string userId)
        {
            var citizen = await _repo.GetByUserIdAsync(userId);

            if (citizen == null)
                throw new Exception("Citizen not found");

            return new ProfileDto
            {
                FullName = citizen.FullName,
                Email = citizen.User.Email,
                PhoneNumber = citizen.User.PhoneNumber,
                Image = citizen.Image != null
                    ? Convert.ToBase64String(citizen.Image)
                    : null
            };
        }

        public async Task UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var citizen = await _repo.GetByUserIdAsync(userId);

            if (citizen == null)
                throw new Exception("Citizen not found");

            citizen.FullName = dto.FullName;

            citizen.User.PhoneNumber = dto.PhoneNumber;

            if (dto.Image != null)
            {
                using var ms = new MemoryStream();
                await dto.Image.CopyToAsync(ms);
                citizen.Image = ms.ToArray();
            }

            await _repo.UpdateAsync(citizen);
        }
    }
}
