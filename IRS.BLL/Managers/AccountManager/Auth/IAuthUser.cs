using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRS.BLL.Dtos.AccountDto.Forget_password;
using IRS.BLL.Dtos.AccountDto.User;
namespace IRS.BLL.Managers.AccountManager.Auth
{
    public interface IAuthUser
    {
        Task<APPResult> RegisterCitizenAsync(RegisterCitizenDto dto);
        Task<APPResult> ConfirmEmailCitizenAsync(ConfirmEmailDto dto);
        Task<APPResult> LoginCitizenAsync(LoginDto dto);

        Task<APPResult> RegisterAuthorityAsync(RegisterAuthorityDto dto);
        Task<APPResult> ConfirmEmailAuthorityAsync(ConfirmEmailDto dto);
        Task<APPResult> LoginAuthorityAsync(LoginDto dto);

        Task<APPResult> RefreshTokenAsync(string refreshToken);
        Task<APPResult> ResendRegistrationOtpAsync(string email);

        Task<APPResult> ChangePasswordAsync(string userId, ChangePasswordDto dto);

        Task<APPResult> RequestPasswordResetAsync(string email);
        Task<APPResult> VerifyOtpAsync(VerifyOtpDto dto);
        Task<APPResult> ResetPasswordAsync(ResetPasswordDto dto);
        Task<APPResult> ResendOtpAsync(string email);

    }
}
