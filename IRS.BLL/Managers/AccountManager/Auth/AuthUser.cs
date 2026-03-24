using SAFQA.BLL.Managers.AccountManager.Email_Sender;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using IRS.BLL.Help;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IRS.DAL.Database;
using IRS.DAL.Models;
using IRS.BLL.Dtos.AccountDto.User;
using IRS.BLL.Dtos.AccountDto.Forget_password;




namespace IRS.BLL.Managers.AccountManager.Auth
{
    public class AuthUser : IAuthUser
    {
        #region  Dependency Injection , UserManagement & SignInManager in Identity , IConfiguration To Access To App Settings 
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IRS_Context _context;

        public AuthUser(UserManager<User> userManager
            , SignInManager<User> signInManager
            , IConfiguration configuration
            , IEmailSender emailSender
            , IRS_Context context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _context = context;
        }
        #endregion

        #region  check By Email , Create User Object , Assign Password To This Email , Add Role To User By Identity
        public async Task<APPResult> RegisterAsync(RegisterDto dto)
        {
            // التأكد من أن الإيميل مش موجود بالفعل
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return new APPResult
                {
                    IsSuccess = false,
                    Errors = new() { "Email already in use" }
                };

            var passwordValidator = new PasswordValidator<User>();
            var fakeUser = new User { UserName = dto.Email, Email = dto.Email };
            var passwordResult = await passwordValidator.ValidateAsync(_userManager, fakeUser, dto.Password);

            // التأكد لو في pending user موجود
            var pendingUser = await _context.pendingCitizenRegistrations
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (pendingUser != null)
            {
                // إذا صلاحية OTP انتهت نقدر نعيد إرسال OTP
                var lastOtp = await _context.otps
                    .Where(o => o.Email == dto.Email && !o.IsUsed)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                if (lastOtp != null && lastOtp.Expiry > DateTime.UtcNow)
                {
                    return new APPResult
                    {
                        IsSuccess = false,
                        Errors = new() { "OTP already sent. Please check your email." }
                    };
                }
                else
                {
                    _context.pendingCitizenRegistrations.Remove(pendingUser);
                    await _context.SaveChangesAsync();
                }
            }

            // توليد OTP
            var otpCode = Helper.GenerateOtp();
            var otpSecret = _configuration["Security:OtpSecret"];
            var otpHash = Helper.HashOtp(otpCode, otpSecret);

            int otpExpiryMinutes = 5; // default
            var otpExpiryConfig = _configuration["Security:OtpExpiryMinutes"];
            if (!string.IsNullOrEmpty(otpExpiryConfig))
                otpExpiryMinutes = int.Parse(otpExpiryConfig);

            // إنشاء PendingUserRegistration بدون OTP
            var pending = new PendingCitizenRegistration
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = dto.Password,
                NationalId = dto.NationalId,
                Image = dto.Image,
                CreatedAt = DateTime.UtcNow
            };

            _context.pendingCitizenRegistrations.Add(pending);
            await _context.SaveChangesAsync();

            // إنشاء سجل OTP منفصل
            var otpEntity = new Otp
            {
                Email = dto.Email,
                Code = otpHash,
                Expiry = DateTime.UtcNow.AddMinutes(otpExpiryMinutes),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.otps.Add(otpEntity);
            await _context.SaveChangesAsync();

            // إرسال OTP للمستخدم
            await _emailSender.SendOtpEmailAsync(dto.Email, otpCode);

            return new APPResult
            {
                IsSuccess = true,
                Message = "OTP sent to your email"
            };
        }
        #endregion

        #region Confirm Email and Create User
        public async Task<APPResult> ConfirmEmailAsync(ConfirmEmailDto dto)
        {
            // استرجاع pending user من جدول PendingCitizenRegistration
            var pending = await _context.pendingCitizenRegistrations
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (pending == null)
                return new APPResult
                {
                    IsSuccess = false,
                    Errors = new() { "Pending registration not found" }
                };

            // استرجاع الـOTP من جدول الـOTP
            var otp = await _context.otps
                .Where(o => o.Email == dto.Email &&  !o.IsUsed && o.Expiry > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otp == null)
                return new APPResult
                {
                    IsSuccess = false,
                    Errors = new() { "Invalid or expired OTP" }
                };

            // تحقق من الكود
            var otpHash = Helper.HashOtp(dto.Otp, _configuration["Security:OtpSecret"]);
            if (otpHash != otp.Code)
                return new APPResult
                {
                    IsSuccess = false,
                    Errors = new() { "Invalid OTP" }
                };

            // إنشاء الـUser النهائي
            var user = new User
            {
                UserName = pending.Email,
                Email = pending.Email,
                PasswordHash = pending.Password,
            };

            var result = await _userManager.CreateAsync(user, pending.Password);
            if (!result.Succeeded)
                return new APPResult
                {
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };

            await _userManager.AddToRoleAsync(user, "CITIZEN");

            // إنشاء الـCitizen وربطه بالـUser
            var citizen = new Citizen
            {
                UserId = user.Id,
                User = user,
                FullName = pending.FullName,
                NationalId = pending.NationalId,
                Image = pending.Image,
                IsDeleted = false
            };

            _context.Citizens.Add(citizen);

            otp.IsUsed = true;

            _context.pendingCitizenRegistrations.Remove(pending);

            await _context.SaveChangesAsync();

            return new APPResult
            {
                IsSuccess = true,
                Message = "Email confirmed and account created successfully"
            };
        }
        #endregion

        #region  Search By Email , Check Password To This Email , Generate Token
        public async Task<APPResult> LoginAsync(LoginDto dto)
        {
            //  نبحث في جدول Users
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                //  لو مش موجود، نشوف جدول PendingUserRegistrations
                var pendingUser = await _context.pendingCitizenRegistrations
                    .FirstOrDefaultAsync(p => p.Email == dto.Email);

                if (pendingUser != null)
                {
                    // Email مسجل لكن لم يتم التحقق من OTP
                    return new APPResult
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Email registered but OTP not verified. Please verify your email." }
                    };
                }

                // Email مش موجود نهائيًا
                return new APPResult
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid login attempt" }
                };
            }

            //  لو المستخدم موجود في Users، نتحقق من كلمة السر
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                return new APPResult
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid login attempt" }
                };
            }


            // 4️⃣ Generate Tokens
            var token = await GenerateTokensAsync(user);

            return new APPResult
            {
                IsSuccess = true,
                UserId = user.Id,
                Token = token.Token,
                RefreshToken = token.RefreshToken
            };
        }
        #endregion

        #region Search User. Token That == Argument Token , Check For This UsrToken ,Generate Token By Refresh Token & Add ExpiryDate
        public async Task<APPResult> RefreshTokenAsync(string refreshToken)
        {
            var tokenhash = refreshToken.Hash();
            var storedToken = await _context.refreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r =>
                    r.Token == tokenhash &&
                    !r.IsRevoked);

            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
                return new APPResult { IsSuccess = false, Message = "Invalid or expired refresh token" };

            storedToken.IsRevoked = true;

            var tokens = await GenerateTokensAsync(storedToken.User);

            await _context.SaveChangesAsync();

            return new APPResult
            {
                IsSuccess = true,
                UserId = storedToken.User.Id,
                Token = tokens.Token,
                RefreshToken = tokens.RefreshToken
            };
        }
        #endregion

        #region Generate Tokens
        private async Task<(string Token, string RefreshToken)> GenerateTokensAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("SecurityStamp", user.SecurityStamp),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshTokenHash = refreshToken.Hash();

            _context.refreshTokens.Add(new RefreshToken
            {
                Token = refreshTokenHash,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            await _context.SaveChangesAsync();

            return (new JwtSecurityTokenHandler().WriteToken(token), refreshToken);
        }
        #endregion

        public async Task<APPResult> ResendRegistrationOtpAsync(string email)
        {
            // نجيب ال pending user
            var pendingUser = await _context.pendingCitizenRegistrations
                .FirstOrDefaultAsync(u => u.Email == email);

            if (pendingUser == null)
                return new APPResult
                {
                    IsSuccess = false,
                    Message = "Pending user not found"
                };

            // توليد OTP جديد
            var code = Helper.GenerateOtp();
            var hash = Helper.HashOtp(code, _configuration["Security:OtpSecret"]);

            int otpExpiryMinutes = int.Parse(_configuration["Security:OtpExpiryMinutes"] ?? "5");

            // إنشاء سجل OTP جديد في جدول الـOtp
            var otpEntity = new Otp
            {
                Email = pendingUser.Email,
                Code = hash,
                Expiry = DateTime.UtcNow.AddMinutes(otpExpiryMinutes),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.otps.Add(otpEntity);
            await _context.SaveChangesAsync();

            // ارسال الايميل
            await _emailSender.SendEmailAsync(
                pendingUser.Email,
                "Your Registration OTP",
                $"<h2>Your OTP is: {code}</h2>");

            return new APPResult
            {
                IsSuccess = true,
                Message = "Registration OTP resent successfully"
            };
        }
        public async Task<APPResult> RequestPasswordResetAsync(string email)
        {
            // التأكد من وجود المستخدم
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new APPResult
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

            // Cleanup أي OTPs قديمة لهذا الغرض
            var oldOtps = await _context.otps
                .Where(o => o.Email == user.Email && !o.IsUsed)
                .ToListAsync();

            _context.otps.RemoveRange(oldOtps);
            await _context.SaveChangesAsync();

            // توليد OTP + hash
            var code = Helper.GenerateOtp();
            var hashed = Helper.HashOtp(code, _configuration["Security:OtpSecret"]);

            int otpExpiryMinutes = int.Parse(_configuration["Security:OtpExpiryMinutes"] ?? "10");

            // حفظ OTP جديد
            var otpEntity = new Otp
            {
                Email = user.Email,
                Code = hashed, 
                Expiry = DateTime.UtcNow.AddMinutes(otpExpiryMinutes),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.otps.Add(otpEntity);
            await _context.SaveChangesAsync();

            // إرسال الايميل
            var body = $"<h2>Your OTP code is: {code}</h2><p>It expires in {otpExpiryMinutes} minutes.</p>";
            await _emailSender.SendEmailAsync(user.Email, "Password Reset Code", body);

            return new APPResult
            {
                IsSuccess = true,
                Message = "OTP sent to your email"
            };
        }

        public async Task<APPResult> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return new APPResult
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

            // حذف OTPs القديمة الخاصة بهذا الهدف
            await CleanupOtpsAsync(user.Id);
            await _context.SaveChangesAsync();

            // الحصول على أحدث OTP صالح
            var record = await _context.otps
                .Where(x =>
                    x.Email == user.Email &&
                    !x.IsUsed &&
                    x.Expiry > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (record == null)
                return new APPResult
                {
                    IsSuccess = false,
                    Message = "Invalid or expired OTP"
                };

            // تحقق من عدد المحاولات
            int maxAttempts = int.Parse(_configuration["Security:MaxOtpAttempts"] ?? "3");

            if (record.FailedAttempts >= maxAttempts)
                return new APPResult
                {
                    IsSuccess = false,
                    Message = "Too many attempts"
                };

            // تحقق من الكود
            var hashedInput = Helper.HashOtp(dto.Code, _configuration["Security:OtpSecret"]);

            if (record.Code != hashedInput)
            {
                record.FailedAttempts++;
                await _context.SaveChangesAsync();

                return new APPResult
                {
                    IsSuccess = false,
                    Message = "Invalid OTP"
                };
            }

            // نجاح التحقق
            record.IsUsed = true;
            await _context.SaveChangesAsync();

            // إنشاء Session Token (أو أي Token مناسب)
            var sessionToken = Helper.GenerateSessionToken();

            return new APPResult
            {
                IsSuccess = true,
                Message = "OTP verified successfully",
                Token = sessionToken
            };
        }

        public async Task<APPResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new APPResult { IsSuccess = false, Message = "User not found" };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (!result.Succeeded)
                return new APPResult
                {
                    IsSuccess = false,
                    Message = "Reset failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };

            await _emailSender.SendEmailAsync(user.Email, "Password Reset Successful", "<p>Your password has been changed successfully.</p>");

            return new APPResult { IsSuccess = true, Message = "Password reset successfully" };
        }

        private async Task CleanupOtpsAsync(string email)
        {
            var expiredOtps = await _context.otps
                .Where(x => x.Email == email && (x.IsUsed || x.Expiry <= DateTime.UtcNow))
                .ToListAsync();

            if (expiredOtps.Any())
            {
                _context.otps.RemoveRange(expiredOtps);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<APPResult> ResendOtpAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return new APPResult { IsSuccess = false, Message = "User not found" };

            // آخر OTP
            var lastOtp = await _context.otps
                .Where(x => x.Email == user.Email)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            // cooldown
            int cooldown = int.Parse(_configuration["Security:OtpCooldownSeconds"] ?? "60");

            if (lastOtp != null && lastOtp.CreatedAt > DateTime.UtcNow.AddSeconds(-cooldown))
            {
                return new APPResult
                {
                    IsSuccess = false,
                    Message = "Please wait before requesting another OTP"
                };
            }

            // limit إعادة الإرسال لكل ساعة
            int resendLimit = int.Parse(_configuration["Security:MaxResendPerHour"] ?? "5");

            var resendCount = await _context.otps
                .CountAsync(x =>
                    x.Email == user.Email &&
                    x.CreatedAt > DateTime.UtcNow.AddHours(-1));

            if (resendCount >= resendLimit)
            {
                return new APPResult
                {
                    IsSuccess = false,
                    Message = "Too many requests, try again later"
                };
            }

            // حذف OTPs القديمة الخاصة بهذا الـPurpose
            var oldOtps = await _context.otps
                .Where(x => x.Email == user.Email)
                .ToListAsync();

            if (oldOtps.Any())
            {
                _context.otps.RemoveRange(oldOtps);
                await _context.SaveChangesAsync();
            }

            // توليد OTP جديد
            var code = Helper.GenerateOtp();
            var hash = Helper.HashOtp(code, _configuration["Security:OtpSecret"]);

            int otpExpiryMinutes = int.Parse(_configuration["Security:OtpExpiryMinutes"] ?? "5");

            var otp = new Otp
            {
                Email = user.Email,
                Code = hash,
                Expiry = DateTime.UtcNow.AddMinutes(otpExpiryMinutes),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.otps.Add(otp);
            await _context.SaveChangesAsync();

            // إرسال الايميل
            await _emailSender.SendEmailAsync(
                user.Email,
                "Your OTP Code",
                $"<h2>Your OTP is: {code}</h2>");

            return new APPResult
            {
                IsSuccess = true,
                Message = "OTP resent successfully"
            };
        }
    }
}
