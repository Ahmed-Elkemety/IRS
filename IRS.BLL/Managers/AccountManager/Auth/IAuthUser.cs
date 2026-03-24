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
        Task<APPResult> RegisterAsync(RegisterDto dto);
        Task<APPResult> LoginAsync(LoginDto dto);
        Task<APPResult> RefreshTokenAsync(string refreshToken);
        Task<APPResult> ConfirmEmailAsync(ConfirmEmailDto dto);
        Task<APPResult> ResendRegistrationOtpAsync(string email);

        Task<APPResult> RequestPasswordResetAsync(string email);
        Task<APPResult> VerifyOtpAsync(VerifyOtpDto dto);
        Task<APPResult> ResetPasswordAsync(ResetPasswordDto dto);
        Task<APPResult> ResendOtpAsync(string email);

    }
}
