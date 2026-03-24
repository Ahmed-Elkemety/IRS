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
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {

            var result = await _authUser.RegisterAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ✅ تأكيد البريد الإلكتروني
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto dto)
        {
            var result = await _authUser.ConfirmEmailAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Message);
        }

        // ✅ تسجيل الدخول
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authUser.LoginAsync(dto);
            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        // ✅ تحديث الـRefreshToken

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {

            var result = await _authUser.RefreshTokenAsync(refreshToken);
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
