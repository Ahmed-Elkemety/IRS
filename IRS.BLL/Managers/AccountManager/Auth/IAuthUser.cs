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
        Task<AuthResult> RegisterAsync(RegisterDto dto);
        Task<AuthResult> LoginAsync(LoginDto dto);
        Task<AuthResult> RefreshTokenAsync(string refreshToken);
        Task<AuthResult> ConfirmEmailAsync(ConfirmEmailDto dto);
        Task<AuthResult> ResendRegistrationOtpAsync(string email);

        Task<AuthResult> RequestPasswordResetAsync(string email);
        Task<AuthResult> VerifyOtpAsync(VerifyOtpDto dto);
        Task<AuthResult> ResetPasswordAsync(ResetPasswordDto dto);
        Task<AuthResult> ResendOtpAsync(string email);

    }
}
