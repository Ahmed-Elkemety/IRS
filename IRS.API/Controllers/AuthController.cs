using System.Security.Claims;
using IRS.BLL.Dtos.AccountDto.Forget_password;
using IRS.BLL.Dtos.AccountDto.User;
using IRS.BLL.Managers.AccountManager.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IRS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthUser _authUser;

        public AuthController(IAuthUser authUser)
        {
            _authUser = authUser ;
        }

        // ✅ تسجيل مستخدم جديد
        [HttpPost("register-citizen")]
        public async Task<IActionResult> RegisterCitizen([FromBody] RegisterCitizenDto dto)
        {

            var result = await _authUser.RegisterCitizenAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ✅ تأكيد البريد الإلكتروني
        [HttpPost("confirm-email-citizen")]
        public async Task<IActionResult> ConfirmEmailCitizen(ConfirmEmailDto dto)
        {
            var result = await _authUser.ConfirmEmailCitizenAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Message);
        }

        // ✅ تسجيل الدخول
        [HttpPost("login-citizen")]
        public async Task<IActionResult> LoginCitizen([FromBody] LoginDto dto)
        {
            var result = await _authUser.LoginCitizenAsync(dto);
            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpPost("register-authority")]
        public async Task<IActionResult> RegisterAuthority([FromBody] RegisterAuthorityDto dto)
        {

            var result = await _authUser.RegisterAuthorityAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ✅ تأكيد البريد الإلكتروني
        [HttpPost("confirm-email-authority")]
        public async Task<IActionResult> ConfirmEmailAuthority(ConfirmEmailDto dto)
        {
            var result = await _authUser.ConfirmEmailAuthorityAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Message);
        }

        // ✅ تسجيل الدخول
        [HttpPost("login-authority")]
        public async Task<IActionResult> LoginAuthority([FromBody] LoginDto dto)
        {
            var result = await _authUser.LoginAuthorityAsync(dto);
            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        // ✅ تحديث الـRefreshToken

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _authUser.RefreshTokenAsync(userId);
            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        // --------------------- OTP Routes ---------------------

        // ✅ إعادة إرسال OTP للتسجيل
        [HttpPost("otp/resend-registration")]
        public async Task<IActionResult> ResendRegistrationOtp([FromBody] RequestResetDto dto)
        {
            var result = await _authUser.ResendRegistrationOtpAsync(dto.Email);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }


        // ✅ طلب OTP لإعادة تعيين الباسورد
        [HttpPost("request")]
        public async Task<IActionResult> RequestOtp([FromBody] RequestResetDto dto)
        {
            var result = await _authUser.RequestPasswordResetAsync(dto.Email);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ✅ التحقق من OTP
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var result = await _authUser.VerifyOtpAsync(dto);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // ✅ إعادة تعيين كلمة المرور
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authUser.ResetPasswordAsync(dto);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        // ✅ إعادة إرسال OTP 
        [HttpPost("otp/resend-forgetPassword")]
        public async Task<IActionResult> ResendOtp([FromBody] RequestResetDto dto)
        {
            var result = await _authUser.ResendOtpAsync(dto.Email);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

    }
}
